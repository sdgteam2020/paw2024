using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;

namespace swas.BAL.Repository
{
    public class
   RolePermissionRepository
   : IRolePermissionRepository
    {
        private readonly
            RoleManager<IdentityRole>
            _roleManager;

        private readonly
            ApplicationDbContext
            _context;

        public
        RolePermissionRepository(
            RoleManager<IdentityRole>
            roleManager,
            ApplicationDbContext
            context)
        {
            _roleManager =
                roleManager;

            _context =
                context;
        }

        public async Task
            <RolePermissionDTO>
            GetRolePermissions(
            string roleId)
        {
            var role =
                await _roleManager
                .FindByIdAsync(roleId);

            var claims =
                await _roleManager
                .GetClaimsAsync(role);

            var permissions =
                await _context
                .PermissionMaster
                .Where(x =>
                    x.IsActive)
                .OrderBy(x =>
                    x.DisplayOrder)
                .ToListAsync();

            return new
                RolePermissionDTO
            {
                RoleId =
                    role.Id,

                RoleName =
                    role.Name,

                Permissions =
                    permissions
                    .Select(x =>
                    new
                    PermissionCheckboxDTO
                    {
                        PermissionId =
                            x.Id,

                        PermissionKey =
                            x.PermissionKey,

                        DisplayName =
                            x.DisplayName,

                        IsSelected =
                            claims.Any(c =>
                            c.Type
                            ==
                            "Permission"
                            &&
                            c.Value
                            ==
                            x.PermissionKey)
                    })
                    .ToList()
            };
        }

        public async Task<(bool success, string message)> SaveRolePermissions(RolePermissionDTO model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                return (false, "Role not found.");
            }

            var permissionKeys = model.Permissions
                .Where(x => x.IsSelected && !string.IsNullOrWhiteSpace(x.PermissionKey))
                .Select(x => x.PermissionKey.Trim())
                .Distinct()
                .ToList();

            var oldClaims = await _roleManager.GetClaimsAsync(role);

            var oldPermissionClaims = oldClaims
                .Where(x => x.Type == "Permission")
                .ToList();

            foreach (var claim in oldPermissionClaims)
            {
                var removeResult = await _roleManager.RemoveClaimAsync(role, claim);

                if (!removeResult.Succeeded)
                {
                    return (false, "Failed to remove old role permissions.");
                }
            }

            foreach (var permissionKey in permissionKeys)
            {
                var addResult = await _roleManager.AddClaimAsync(
                    role,
                    new Claim("Permission", permissionKey)
                );

                if (!addResult.Succeeded)
                {
                    return (false, $"Failed to add permission: {permissionKey}");
                }
            }

            return (true, "Role permissions saved successfully.");
        }
    }
}
