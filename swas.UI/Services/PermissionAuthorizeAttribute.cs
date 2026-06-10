using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using static Dapper.SqlMapper;

namespace swas.UI.Services
{
    public class PermissionAuthorizeAttribute
        : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public PermissionAuthorizeAttribute(
            string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(
            AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Account",
                        null);

                return;
            }

            if (!user.HasClaim(
                "Permission",
                _permission))
            {
                context.Result = new RedirectToActionResult(
    "AccessDenied",   // Action
    "Account",        // Controller
    new { area = "Identity" } // Area
);
            }
        }
    }
}
