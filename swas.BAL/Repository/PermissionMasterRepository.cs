using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.DAL;
using Microsoft.EntityFrameworkCore;

namespace swas.BAL.Repository
{
    public class PermissionMasterRepository
         : IPermissionMasterRepository
    {
        private readonly ApplicationDbContext
            _context;

        private readonly ILogger
            <PermissionMasterRepository>
            _logger;

        public PermissionMasterRepository(
            ApplicationDbContext context,
            ILogger
            <PermissionMasterRepository>
            logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<PermissionMaster>
            GetAll()
        {
            try
            {
                return _context
                    .PermissionMaster
                    .OrderBy(x =>
                    x.DisplayOrder)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching permissions");

                return new List
                    <PermissionMaster>();
            }
        }

        public PermissionMaster
            GetById(int id)
        {
            return _context
                .PermissionMaster
                .FirstOrDefault(
                x => x.Id == id);
        }

        public bool IsDuplicate(
            string permissionKey,
            int id)
        {
            return _context
                .PermissionMaster
                .Any(x =>
                    x.PermissionKey
                    .ToLower()
                    ==
                    permissionKey
                    .ToLower()
                    &&
                    x.Id != id);
        }

        public async Task
            <(bool success,
            string message)>
            Save(
            PermissionMaster model,
            string username)
        {
            try
            {
                if (IsDuplicate(
                    model.PermissionKey,
                    model.Id))
                {
                    return (
                        false,
                        "Permission Key already exists"
                    );
                }

                if (model.Id == 0)
                {
                    model.CreatedDate =
                        DateTime.Now;

                    

                    model.IsActive =
                        true;

                    _context
                        .PermissionMaster
                        .Add(model);
                }
                else
                {
                    var db =
                        await _context
                        .PermissionMaster
                        .FirstOrDefaultAsync(
                            x =>
                            x.Id
                            ==
                            model.Id);

                    if (db == null)
                    {
                        return (
                            false,
                            "Record not found");
                    }

                    db.PermissionKey =
                        model.PermissionKey;

                    db.DisplayName =
                        model.DisplayName;

                    db.ModuleName =
                        model.ModuleName;

                    db.PermissionType =
                        model.PermissionType;

                    db.DisplayOrder =
                        model.DisplayOrder;

                    db.IsActive =
                        model.IsActive;

                  
                }

                await _context
                    .SaveChangesAsync();

                return (
                    true,
                    "Saved Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error saving permission");

                return (
                    false,
                    "Something went wrong");
            }
        }

        public async Task
            <(bool success,
            string message)>
            Delete(
            int id,
            string username)
        {
            try
            {
                var data =
                    await _context
                    .PermissionMaster
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

                if (data == null)
                {
                    return (
                        false,
                        "Record not found");
                }

                data.IsActive = false;

             

                await _context
                    .SaveChangesAsync();

                return (
                    true,
                    "Deleted Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Delete error");

                return (
                    false,
                    "Something went wrong");
            }
        }
    }
}
