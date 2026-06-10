using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.DAL;
using System.Security.Claims;

namespace swas.UI.Services
{
    public sealed class UnitClaimsTransformation : IClaimsTransformation
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UnitClaimsTransformation> _logger;
        private readonly IMemoryCache _cache;

        private const string PermissionClaimType = "Permission";
        private const string ClaimsLoadedMarker = "UnitClaimsLoaded";

        public UnitClaimsTransformation(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UnitClaimsTransformation> logger,
            IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal?.Identity is not ClaimsIdentity identity ||
                !identity.IsAuthenticated)
            {
                return principal!;
            }

            if (identity.HasClaim(x => x.Type == ClaimsLoadedMarker))
            {
                return principal;
            }

            try
            {
                var userId = _userManager.GetUserId(principal);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    AddMarker(identity);
                    return principal;
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("Claims transformation skipped. User not found.");
                    AddMarker(identity);
                    return principal;
                }

                if (user.unitid <= 0)
                {
                    _logger.LogWarning(
                        "Claims transformation skipped. Invalid UnitId for user {UserId}",
                        userId);

                    AddMarker(identity);
                    return principal;
                }

                var cacheKey = $"unit-permissions:{user.unitid}";

                var permissions = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(2);

                    return await _context.UnitClaims
                        .AsNoTracking()
                        .Where(x =>
                            x.UnitId == user.unitid &&
                            x.IsActive &&
                            x.PermissionMaster != null &&
                            x.PermissionMaster.IsActive &&
                            x.PermissionMaster.PermissionKey != null &&
                            x.PermissionMaster.PermissionKey != "")
                        .Select(x => x.PermissionMaster.PermissionKey!)
                        .Distinct()
                        .ToListAsync();
                });

                if (permissions == null || permissions.Count == 0)
                {
                    AddMarker(identity);
                    return principal;
                }

                var existingPermissions = identity.Claims
                    .Where(x => x.Type == PermissionClaimType)
                    .Select(x => x.Value)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var permission in permissions)
                {
                    if (!existingPermissions.Contains(permission))
                    {
                        identity.AddClaim(
                            new Claim(PermissionClaimType, permission));
                    }
                }

                AddMarker(identity);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred during unit claims transformation.");

                return principal;
            }
        }

        private static void AddMarker(ClaimsIdentity identity)
        {
            if (!identity.HasClaim(x => x.Type == ClaimsLoadedMarker))
            {
                identity.AddClaim(
                    new Claim(ClaimsLoadedMarker, "true"));
            }
        }
    }
}
