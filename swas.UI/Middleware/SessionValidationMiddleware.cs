    using Microsoft.AspNetCore.Authorization;
namespace swas.UI.Middleware
{

    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(
            RequestDelegate next,
            ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

       public async Task InvokeAsync(HttpContext context)
{
    var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

    // Allow root default route: / => Home/Index
    if (path == "/" || path == "/home" || path == "/home/index")
    {
        await _next(context);
        return;
    }

    // Allow static files
    if (path.StartsWith("/css") ||
        path.StartsWith("/js") ||
        path.StartsWith("/lib") ||
        path.StartsWith("/images") ||
        path.StartsWith("/uploads") ||
        path.StartsWith("/favicon.ico"))
    {
        await _next(context);
        return;
    }

    // Allow login / logout / identity pages
    if (path.StartsWith("/home/login") ||
        path.StartsWith("/home/checklogin") ||
        path.StartsWith("/home/logout") ||
        path.StartsWith("/identity/account/login") ||
        path.StartsWith("/identity/account/logout") ||
        path.StartsWith("/identity/account/accessdenied"))
    {
        await _next(context);
        return;
    }

    var endpoint = context.GetEndpoint();

    // Allow [AllowAnonymous]
    if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
    {
        await _next(context);
        return;
    }

    var userSession = context.Session.GetString("User");

    if (string.IsNullOrWhiteSpace(userSession))
    {
        _logger.LogWarning(
            "Session validation failed. Path: {Path}",
            context.Request.Path
        );

        if (IsAjaxRequest(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Session expired. Please login again.",
                redirectUrl = "/Home/Index"
            });

            return;
        }

        context.Response.Redirect("/Home/Index");
        return;
    }

    await _next(context);
}
        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Accept"].ToString().Contains("application/json");
        }
    }
}
