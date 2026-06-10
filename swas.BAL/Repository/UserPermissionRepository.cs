using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;

namespace swas.BAL.Repository
{
    public class
       UserPermissionRepository
       : IUserPermissionRepository
    {
        private readonly
 UserManager<ApplicationUser>
 _userManager;

        private readonly
            ApplicationDbContext
            _context;

        
       public UserPermissionRepository(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<(bool success, string message, UserPermissionDTO? data)>
     GetUserPermissions(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return (false, "User is required.", null);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return (false, "User not found.", null);
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var userPermissionClaims = claims
                .Where(x => x.Type == "Permission")
                .Select(x => x.Value)
                .ToHashSet();

            var permissions = await _context.PermissionMaster
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var data = new UserPermissionDTO
            {
                UserId = user.Id,
                UserName = user.UserName,

                Permissions = permissions.Select(x => new UserPermissionCheckboxDTO
                {
                    PermissionId = x.Id,
                    PermissionKey = x.PermissionKey,
                    DisplayName = x.DisplayName,
                    IsSelected = userPermissionClaims.Contains(x.PermissionKey)
                }).ToList()
            };

            return (true, "Permissions loaded successfully.", data);
        }

        public async Task<(bool success, string message)> SaveUserPermissions(UserPermissionDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return (false, "User not found.");
            }

            var permissionKeys = model.Permissions
                .Where(x => x.IsSelected && !string.IsNullOrWhiteSpace(x.PermissionKey))
                .Select(x => x.PermissionKey.Trim())
                .Distinct()
                .ToList();

            var oldClaims = await _userManager.GetClaimsAsync(user);

            var oldPermissionClaims = oldClaims
                .Where(x => x.Type == "Permission")
                .ToList();

            foreach (var claim in oldPermissionClaims)
            {
                var removeResult = await _userManager.RemoveClaimAsync(user, claim);

                if (!removeResult.Succeeded)
                {
                    return (false, "Failed to remove old permissions.");
                }
            }

            foreach (var permissionKey in permissionKeys)
            {
                var addResult = await _userManager.AddClaimAsync(
                    user,
                    new Claim("Permission", permissionKey)
                );

                if (!addResult.Succeeded)
                {
                    return (false, $"Failed to add permission: {permissionKey}");
                }
            }

            return (true, "Permissions saved successfully.");
        }
    }
}
