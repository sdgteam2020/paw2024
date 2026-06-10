using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.DAL;
using Microsoft.EntityFrameworkCore;

namespace swas.BAL.Repository
{
    public class
           UnitPermissionRepository
           : IUnitPermissionRepository
    {
        private readonly
            ApplicationDbContext
            _context;

        public
        UnitPermissionRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UnitPermissionDTO>
        GetUnitPermissions(int unitId)
        {
            var unit =
                await _context
                .tbl_mUnitBranch
                .FirstOrDefaultAsync(x =>
                    x.unitid == unitId);

            var unitClaims =
                await _context
                .UnitClaims
                .Where(x =>
                    x.UnitId == unitId
                    && x.IsActive)
                .Select(x =>
                    x.PermissionId)
                .ToListAsync();

            var permissions =
                await _context
                .PermissionMaster
                .Where(x =>
                    x.IsActive)
                .OrderBy(x =>
                    x.DisplayOrder)
                .ToListAsync();

            return new UnitPermissionDTO
            {
                UnitId = unit.unitid,

                UnitName = unit.UnitName,

                Permissions =
                    permissions.Select(x =>
                    new UnitPermissionCheckboxDTO
                    {
                        PermissionId =
                            x.Id,

                        PermissionKey =
                            x.PermissionKey,

                        DisplayName =
                            x.DisplayName,

                        IsSelected =
                            unitClaims
                            .Contains(x.Id)
                    }).ToList()
            };
        }

        public async Task
    <(bool success,
    string message)>
    SaveUnitPermissions(
    UnitPermissionDTO model)
        {
            using var transaction =
                await _context
                .Database
                .BeginTransactionAsync();

            try
            {
                var existingClaims =
                    await _context
                    .UnitClaims
                    .Where(x =>
                        x.UnitId ==
                        model.UnitId)
                    .ToListAsync();

                _context.UnitClaims
                    .RemoveRange(
                        existingClaims);

                var selectedPermissions =
                    model.Permissions
                    .Where(x =>
                        x.IsSelected)
                    .ToList();

                foreach (var item
                    in selectedPermissions)
                {
                    _context.UnitClaims
                        .Add(new UnitClaims
                        {
                            UnitId =
                                model.UnitId,

                            ClaimType =
                                "Permission",

                            PermissionId =
                                item.PermissionId,

                            IsActive =
                                true,

                            CreatedDate =
                                DateTime.Now
                        });
                }

                await _context
                    .SaveChangesAsync();

                await transaction
                    .CommitAsync();

                return (
                    true,
                    "Permissions saved successfully"
                );
            }
            catch (Exception ex)
            {
                await transaction
                    .RollbackAsync();

                return (
                    false,
                    ex.Message
                );
            }
        }
    }
    }
