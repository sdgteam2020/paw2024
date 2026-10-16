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
            var path = context.Request.Path;

            // Public routes
            if (path == "/" ||
                path.StartsWithSegments("/Home/Index") ||
                path.StartsWithSegments("/Home/Error") ||
                path.StartsWithSegments("/Home/Login") ||
                path.StartsWithSegments("/Home/CheckLogin") ||
                path.StartsWithSegments("/Home/Logout") ||
                path.StartsWithSegments("/Identity/Account/Login") ||
                path.StartsWithSegments("/Identity/Account/Logout") ||
                path.StartsWithSegments("/Identity/Account/AccessDenied"))
            {
                await _next(context);
                return;
            }

            // Static resource routes
            if (path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/scripts") ||
                path.StartsWithSegments("/lib") ||
                path.StartsWithSegments("/images") ||
                path.StartsWithSegments("/uploads") ||
                path.StartsWithSegments("/fonts") ||
                path.StartsWithSegments("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            // Allow endpoints explicitly marked [AllowAnonymous]
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // Check application session
            var userSession = context.Session.GetString("User");

            if (string.IsNullOrWhiteSpace(userSession))
            {
                _logger.LogWarning(
                    "Session validation failed. Path={Path}, Authenticated={Authenticated}",
                    path,
                    context.User.Identity?.IsAuthenticated
                );

                if (IsAjaxRequest(context.Request))
                {
                    context.Response.StatusCode =
                        StatusCodes.Status401Unauthorized;

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
                   request.Headers["Accept"].ToString()
                       .Contains("application/json",
                           StringComparison.OrdinalIgnoreCase);
        }
    }

}
