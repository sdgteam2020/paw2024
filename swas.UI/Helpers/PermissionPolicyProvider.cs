using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace swas.UI.Helpers
{
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(
            string policyName)
        {
            if (policyName.StartsWith("Permission:",
                StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName.Substring("Permission:".Length);

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return policy;
            }

            return await base.GetPolicyAsync(policyName);
        }
    }
}
