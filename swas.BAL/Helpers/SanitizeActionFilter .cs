using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace swas.BAL.Helpers
{
    public class SanitizeActionFilter : IActionFilter
    {
        private const int MaxDepth = 8;
        private const int MaxFindings = 20;

        private sealed record Finding(string Field, string Reason);

        private const RegexOptions Opts =
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100);

        private static Regex R(string pattern) => new(pattern, Opts, Timeout);

        // (reason shown to user, pattern)
        private static readonly (string Reason, Regex Pattern)[] Rules =
        {
            ("script tag",                R(@"<\s*/?\s*script\b")),
            ("javascript:/vbscript: URL", R(@"\b(java|vb)script\s*:")),
            ("data:text/html URL",        R(@"data\s*:\s*text/html")),
            ("HTML tag with event handler", R(@"<[^>]*\bon[a-z]+\s*=")),
            ("event handler attribute",   R(@"(?:^|[""'\s/])on(?:error|load|click|dblclick|mouse\w+|key\w+|focus\w*|blur|change|submit|toggle|input|pointer\w+|animation\w+|begin|end)\s*=")),
            ("dangerous HTML tag",        R(@"<\s*(iframe|object|embed|svg|math|meta|link|base|form|style)\b")),
            ("CSS expression()",          R(@"expression\s*\(")),
            ("SQL injection pattern",     R(@"\bunion\b\s+(all\s+)?select\b|;\s*(drop|truncate|delete|insert|update|alter|shutdown|exec)\b|'\s*or\s+'?\d+'?\s*=\s*'?\d+|\bxp_cmdshell\b")),
            ("SQL injection pattern", R(@"\bunion\b\s+(all\s+)?select\b")),
            ("SQL injection pattern", R(@";\s*(drop|truncate|delete|insert|update|alter|create|shutdown|exec|execute|waitfor|declare)\b")),
            ("SQL injection pattern", R(@"['""]\s*(or|and)\s+['""(]?\w+['""]?\s*(=|like|<|>)\s*['""(]?\w+")),
            ("SQL injection pattern", R(@"['""]\s*(--|#|/\*)")),
            ("SQL injection pattern", R(@"\b(or|and)\s+\d+\s*=\s*\d+")),
            ("SQL injection pattern", R(@"\bwaitfor\s+delay\b")),
            ("SQL injection pattern", R(@"\bexec(ute)?\s*\(|\bexec(ute)?\s+(sp_|xp_)")),
            ("SQL injection pattern", R(@"\b(xp_cmdshell|sp_executesql|sp_oacreate|openrowset|information_schema|sysobjects|syscolumns)\b|@@version"))
        };

        private static readonly HashSet<string> SkipFields =
            new(StringComparer.OrdinalIgnoreCase) { "__RequestVerificationToken" };

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            // POST, PUT, PATCH, DELETE
            if (HttpMethods.IsGet(request.Method) ||
                HttpMethods.IsHead(request.Method) ||
                HttpMethods.IsOptions(request.Method) ||
                HttpMethods.IsTrace(request.Method))
                return;

            var findings = new List<Finding>();

            // 1) Query string
            foreach (var kv in request.Query)
                foreach (var v in kv.Value)
                    CheckString(v, kv.Key, findings);

            // 2) Raw form (normal form post + ajax FormData / serialize())
            if (request.HasFormContentType)
            {
                try
                {
                    foreach (var kv in request.Form)
                    {
                        if (SkipFields.Contains(kv.Key)) continue;
                        CheckString(kv.Key, "(field name)", findings);
                        foreach (var v in kv.Value)
                            CheckString(v, kv.Key, findings);
                    }
                }
                catch (InvalidDataException) { /* malformed form, framework handles it */ }
            }
            else
            {
                // 3) JSON / other bodies: inspect the bound action arguments
                var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
                foreach (var (name, arg) in context.ActionArguments)
                    Inspect(arg, name, findings, visited, 0);
            }

            if (findings.Count == 0)
                return;

            var logger = context.HttpContext.RequestServices
                .GetService<ILoggerFactory>()?.CreateLogger<SanitizeActionFilter>();
            logger?.LogWarning(
                "Blocked suspicious input on {Method} {Path}. Fields: {Fields}",
                request.Method, request.Path,
                string.Join(", ", findings.Select(f => $"{f.Field} ({f.Reason})")));

            RejectRequest(context, findings);
        }

        // ---------- detection ----------

        private static void CheckString(string? input, string field, List<Finding> findings)
        {
            if (findings.Count >= MaxFindings) return;
            var reason = Detect(input);
            if (reason != null) findings.Add(new Finding(field, reason));
        }

        private static string? Detect(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            // check raw + decoded (catches %3Cscript%3E, &lt;script&gt;, double encoding)
            foreach (var candidate in Variants(input))
            {
                foreach (var (reason, pattern) in Rules)
                {
                    try
                    {
                        if (pattern.IsMatch(candidate)) return reason;
                    }
                    catch (RegexMatchTimeoutException)
                    {
                        return "suspicious input (too complex to scan)";
                    }
                }
            }
            return null;
        }

        private static IEnumerable<string> Variants(string input)
        {
            yield return input;

            string current = input;
            for (int i = 0; i < 2; i++)
            {
                string next = current;
                try { next = Uri.UnescapeDataString(next); } catch { }
                next = WebUtility.HtmlDecode(next).Replace("\0", "");
                if (next == current) yield break;
                current = next;
                yield return current;
            }
        }

        private static void Inspect(
            object? value, string path, List<Finding> findings,
            HashSet<object> visited, int depth)
        {
            if (value == null || depth > MaxDepth || findings.Count >= MaxFindings)
                return;

            if (value is IFormFile || value is IFormFileCollection ||
                value is IFormCollection || value is Stream ||
                value is byte[] || value is CancellationToken)
                return;

            if (value is string s)
            {
                if (!SkipFields.Contains(path)) CheckString(s, path, findings);
                return;
            }

            Type type = value.GetType();
            if (type.IsPrimitive || type.IsEnum || type.IsValueType) return;

            if (!visited.Add(value)) return;

            if (value is IDictionary dict)
            {
                foreach (DictionaryEntry e in dict)
                {
                    string key = e.Key?.ToString() ?? "";
                    CheckString(key, $"{path}[key]", findings);
                    Inspect(e.Value, $"{path}[{key}]", findings, visited, depth + 1);
                }
                return;
            }

            if (value is IEnumerable list)
            {
                int i = 0;
                foreach (var item in list)
                    Inspect(item, $"{path}[{i++}]", findings, visited, depth + 1);
                return;
            }

            // skip framework types (Uri, CultureInfo, etc.)
            if (type.Namespace is { } ns &&
                (ns.StartsWith("System") || ns.StartsWith("Microsoft")))
                return;

            foreach (PropertyInfo p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead || p.GetIndexParameters().Length > 0) continue;

                object? v;
                try { v = p.GetValue(value); }
                catch { continue; }

                Inspect(v, $"{path}.{p.Name}", findings, visited, depth + 1);
            }
        }

        // ---------- response ----------

        private static string SafeName(string field)
        {
            // field names come from the client, so strip anything that isn't a plain identifier
            var clean = Regex.Replace(field, @"[^A-Za-z0-9_.\[\]\-() ]", "");
            return clean.Length > 60 ? clean[..60] + "…" : clean;
        }

        private static void RejectRequest(ActionExecutingContext context, List<Finding> findings)
        {
            var request = context.HttpContext.Request;

            var distinct = findings
                .GroupBy(f => (f.Field, f.Reason))
                .Select(g => g.First())
                .ToList();

            string details = string.Join("; ",
                distinct.Select(f => $"'{SafeName(f.Field)}' contains {f.Reason}"));

            string message =
                $"Invalid input detected. Please change the value in: {details}.";

            bool isAjax = request.Headers.XRequestedWith == "XMLHttpRequest";
            bool acceptsJson = request.Headers.Accept.ToString()
                .Contains("application/json", StringComparison.OrdinalIgnoreCase);
            bool sendsJson = (request.ContentType ?? "")
                .Contains("application/json", StringComparison.OrdinalIgnoreCase);

            if (isAjax || acceptsJson || sendsJson)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    success = false,
                    message,
                    fields = distinct.Select(f => new
                    {
                        field = SafeName(f.Field),
                        reason = f.Reason
                    })
                });
                return;
            }

            var tempData = context.HttpContext.RequestServices
                .GetRequiredService<ITempDataDictionaryFactory>()
                .GetTempData(context.HttpContext);

            tempData["FailureMessage"] = message;
            tempData["VulnerableFields"] =
                string.Join(", ", distinct.Select(f => SafeName(f.Field)));

            // go back to the page the user came from, else Home/Index
            string? referer = request.Headers.Referer.ToString();
            if (Uri.TryCreate(referer, UriKind.Absolute, out var uri) &&
                string.Equals(uri.Host, request.Host.Host, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectResult(uri.PathAndQuery);
                return;
            }

            context.Result = new RedirectToActionResult("Index", "Home", null);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}