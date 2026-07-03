using DotNetEnv;

namespace swas.UI.NewFolder
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;

                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

                var connectSrc =
                    "connect-src 'self' " +
                    "https://192.168.10.92 " +
                    "https://dgisapp.army.mil:55102";

                if (_env.IsDevelopment())
                {
                    connectSrc +=
                        " http://localhost:* " +
                        " https://localhost:* " +
                        " ws://localhost:* " +
                        " wss://localhost:*";
                }

                headers["Content-Security-Policy"] =
                    "default-src 'self'; " +
                    "script-src 'self'; " +
                    "style-src 'self'; " +
                    "img-src 'self' data: blob:; " +
                    "font-src 'self' data:; " +
                    connectSrc + "; " +
                    "frame-ancestors 'none'; " +
                    "base-uri 'self'; " +
                    "form-action 'self';";

                headers["X-XSS-Protection"] = "0";

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
