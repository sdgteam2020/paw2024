using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.DAL;
using Microsoft.EntityFrameworkCore;
using swas.UI.Helpers;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Microsoft.Extensions.Logging;

namespace swas.BAL.Repository
{
    public class PermissionControlRepository : IPermissionControlRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        
        private const string ClaimType = "Permission";

        public PermissionControlRepository(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IMemoryCache cache)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _cache = cache;
        }

        public async Task<List<SelectListItem>> GetTargetsAsync(string permissionFor)
        {
            if (permissionFor == PermissionForType.Role)
            {
                return await _roleManager.Roles
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Name
                    })
                    .ToListAsync();
            }

            if (permissionFor == PermissionForType.User)
            {
                return await _userManager.Users
                    .AsNoTracking()
                    .OrderBy(x => x.UserName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.UserName
                    })
                    .ToListAsync();
            }

            if (permissionFor == PermissionForType.Unit)
            {
                return await _context.tbl_mUnitBranch
                    .AsNoTracking()
                    .OrderBy(x => x.UnitName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.unitid.ToString(),
                        Text = x.UnitName
                    })
                    .ToListAsync();
            }

            return new List<SelectListItem>();
        }

        public async Task<(bool success, string message, PermissionControlDTO? data)>
      GetPermissionsAsync(string permissionFor, string targetId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(permissionFor))
                    return (false, "Permission type is required.", null);

                if (string.IsNullOrWhiteSpace(targetId))
                    return (false, "Target is required.", null);

                var permissions = await _context.PermissionMaster
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new
                    {
                        x.Id,
                        x.PermissionKey,
                        x.DisplayName
                    })
                    .ToListAsync();

                var selectedKeys = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

                string targetName = string.Empty;

                // ================= ROLE =================
                if (permissionFor == PermissionForType.Role)
                {
                    var role = await _roleManager.FindByIdAsync(targetId);

                    if (role == null)
                        return (false, "Role not found.", null);

                    targetName = role.Name ?? string.Empty;

                    await AddRolePermissionsAsync(role, selectedKeys);
                }

                // ================= UNIT =================
                else if (permissionFor == PermissionForType.Unit)
                {
                    if (!int.TryParse(targetId, out int unitId) || unitId <= 0)
                        return (false, "Invalid unit selected.", null);

                    var unit = await _context.tbl_mUnitBranch
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.unitid == unitId);

                    if (unit == null)
                        return (false, "Unit not found.", null);

                    targetName = unit.UnitName ?? string.Empty;

                    // Determine role name safely
                    string roleName = unit.TypeId == 1
                        ? RoleConstants.Dte
                        : RoleConstants.Unit;

                    var role = await _roleManager.FindByNameAsync(roleName);

                    if (role != null)
                    {
                        await AddRolePermissionsAsync(role, selectedKeys);
                    }

                    // Add Unit Permissions
                    var unitPermissions = await _context.UnitClaims
                        .AsNoTracking()
                        .Where(x =>
                            x.UnitId == unitId &&
                            x.IsActive &&
                            x.PermissionMaster != null)
                        .Select(x => x.PermissionMaster.PermissionKey)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToListAsync();

                    selectedKeys.UnionWith(unitPermissions!);
                }

                // ================= USER =================
                else if (permissionFor == PermissionForType.User)
                {
                    var user = await _userManager.FindByIdAsync(targetId);

                    if (user == null)
                        return (false, "User not found.", null);

                    targetName = user.UserName ?? string.Empty;

                    // Add Role Permissions
                    var roleNames = await _userManager.GetRolesAsync(user);

                    foreach (var roleName in roleNames)
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);

                        if (role != null)
                        {
                            await AddRolePermissionsAsync(
                                role,
                                selectedKeys);
                        }
                    }

                    // Add Unit Permissions
                    if (user.unitid > 0)
                    {
                        var unitPermissions = await _context.UnitClaims
                            .AsNoTracking()
                            .Where(x =>
                                x.UnitId == user.unitid &&
                                x.IsActive &&
                                x.PermissionMaster != null)
                            .Select(x => x.PermissionMaster.PermissionKey)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToListAsync();

                        selectedKeys.UnionWith(unitPermissions!);
                    }

                    // Add User Direct Permissions
                    var userClaims =
                        await _userManager.GetClaimsAsync(user);

                    selectedKeys.UnionWith(
                        userClaims
                            .Where(x => x.Type == ClaimType)
                            .Select(x => x.Value)
                    );
                }
                else
                {
                    return (false, "Invalid permission type.", null);
                }

                var data = new PermissionControlDTO
                {
                    PermissionFor = permissionFor,
                    TargetId = targetId,
                    TargetName = targetName,
                    Permissions = permissions
                        .Select(x => new PermissionControlCheckboxDTO
                        {
                            PermissionId = x.Id,
                            PermissionKey = x.PermissionKey ?? string.Empty,
                            DisplayName =
                                x.DisplayName ??
                                x.PermissionKey ??
                                string.Empty,
                            IsSelected = selectedKeys.Contains(
                                x.PermissionKey ?? string.Empty)
                        })
                        .ToList()
                };

                return (
                    true,
                    "Permissions loaded successfully.",
                    data
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    ex.Message +
                    " Error loading permissions. PermissionFor: {PermissionFor}, TargetId: {TargetId}",
                    permissionFor,
                    targetId);

                return (
                    false,
                    "Something went wrong while loading permissions.",
                    null
                );
            }
        }

        private async Task AddRolePermissionsAsync(
    IdentityRole role,
    HashSet<string> selectedKeys)
        {
            var roleClaims =
                await _roleManager.GetClaimsAsync(role);

            selectedKeys.UnionWith(
                roleClaims
                    .Where(x => x.Type == ClaimType)
                    .Select(x => x.Value)
            );
        }

        public async Task<(bool success, string message)>
            SavePermissionsAsync(PermissionControlSaveDTO model)
        {
            if (model == null)
                return (false, "Invalid request.");

            if (string.IsNullOrWhiteSpace(model.PermissionFor))
                return (false, "Permission type is required.");

            if (string.IsNullOrWhiteSpace(model.TargetId))
                return (false, "Target is required.");

            model.Permissions = model.Permissions
                .Where(x => x.PermissionId > 0)
                .GroupBy(x => x.PermissionId)
                .Select(x => x.First())
                .ToList();

            if (model.PermissionFor == PermissionForType.Role)
                return await SaveRolePermissionsAsync(model);

            if (model.PermissionFor == PermissionForType.User)
                return await SaveUserPermissionsAsync(model);

            if (model.PermissionFor == PermissionForType.Unit)
                return await SaveUnitPermissionsAsync(model);

            return (false, "Invalid permission type.");
        }

        private async Task<(bool success, string message)>
            SaveRolePermissionsAsync(PermissionControlSaveDTO model)
        {
            var role = await _roleManager.FindByIdAsync(model.TargetId);

            if (role == null)
                return (false, "Role not found.");

            var validPermissionKeys = await GetValidPermissionKeysAsync(model.Permissions);

            var existingClaims = await _roleManager.GetClaimsAsync(role);

            var existingPermissionClaims = existingClaims
                .Where(x => x.Type == ClaimType)
                .ToList();

            foreach (var claim in existingPermissionClaims)
            {
                if (!validPermissionKeys.Contains(claim.Value))
                {
                    var result = await _roleManager.RemoveClaimAsync(role, claim);

                    if (!result.Succeeded)
                        return (false, "Failed to remove old role permission.");
                }
            }

            var existingKeys = existingPermissionClaims
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var permissionKey in validPermissionKeys)
            {
                if (!existingKeys.Contains(permissionKey))
                {
                    var result = await _roleManager.AddClaimAsync(
                        role,
                        new Claim(ClaimType, permissionKey));

                    if (!result.Succeeded)
                        return (false, $"Failed to add permission: {permissionKey}");
                }
            }

            return (true, "Role permissions saved successfully.");
        }

        private async Task<(bool success, string message)>
            SaveUserPermissionsAsync(PermissionControlSaveDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.TargetId);

            if (user == null)
                return (false, "User not found.");

            var validPermissionKeys = await GetValidPermissionKeysAsync(model.Permissions);

            var existingClaims = await _userManager.GetClaimsAsync(user);

            var existingPermissionClaims = existingClaims
                .Where(x => x.Type == ClaimType)
                .ToList();

            foreach (var claim in existingPermissionClaims)
            {
                if (!validPermissionKeys.Contains(claim.Value))
                {
                    var result = await _userManager.RemoveClaimAsync(user, claim);

                    if (!result.Succeeded)
                        return (false, "Failed to remove old user permission.");
                }
            }

            var existingKeys = existingPermissionClaims
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var permissionKey in validPermissionKeys)
            {
                if (!existingKeys.Contains(permissionKey))
                {
                    var result = await _userManager.AddClaimAsync(
                        user,
                        new Claim(ClaimType, permissionKey));

                    if (!result.Succeeded)
                        return (false, $"Failed to add permission: {permissionKey}");
                }
            }

            return (true, "User permissions saved successfully.");
        }
            
        private async Task<(bool success, string message)>
            SaveUnitPermissionsAsync(PermissionControlSaveDTO model)
        {
            if (!int.TryParse(model.TargetId, out int unitId) || unitId <= 0)
                return (false, "Invalid unit selected.");

            var unitExists = await _context.tbl_mUnitBranch
                .AsNoTracking()
                .AnyAsync(x => x.unitid == unitId);

            if (!unitExists)
                return (false, "Unit not found.");

            var selectedPermissionIds = model.Permissions
                .Where(x => x.IsSelected && x.PermissionId > 0)
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            var validPermissionIds = await _context.PermissionMaster
                .AsNoTracking()
                .Where(x => x.IsActive && selectedPermissionIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingClaims = await _context.UnitClaims
                    .Where(x => x.UnitId == unitId)
                    .ToListAsync();

                _context.UnitClaims.RemoveRange(existingClaims);

                foreach (var permissionId in validPermissionIds)
                {
                    _context.UnitClaims.Add(new UnitClaims
                    {
                        UnitId = unitId,
                        ClaimType = ClaimType,
                        PermissionId = permissionId,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _cache.Remove($"unit-permissions:{unitId}");

                return (true, "Unit permissions saved successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Unable to save unit permissions.");
            }
        }

        private async Task<HashSet<string>> GetValidPermissionKeysAsync(
            List<PermissionControlCheckboxDTO> permissions)
        {
            var selectedKeys = permissions
                .Where(x => x.IsSelected && !string.IsNullOrWhiteSpace(x.PermissionKey))
                .Select(x => x.PermissionKey.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var validKeys = await _context.PermissionMaster
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    selectedKeys.Contains(x.PermissionKey))
                .Select(x => x.PermissionKey)
                .ToListAsync();

            return validKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }
}
