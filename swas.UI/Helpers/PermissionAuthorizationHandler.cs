using Microsoft.AspNetCore.Authorization;

namespace swas.UI.Helpers
{
    public class PermissionAuthorizationHandler
       : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User == null ||
                context.User.Identity == null ||
                !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            bool hasPermission = context.User.Claims.Any(x =>
                x.Type == "Permission" &&
                x.Value.Equals(
                    requirement.Permission,
                    StringComparison.OrdinalIgnoreCase));

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
