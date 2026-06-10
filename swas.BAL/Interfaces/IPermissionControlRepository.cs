using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using swas.BAL.DTO;

namespace swas.BAL.Interfaces
{
    public interface IPermissionControlRepository
    {
        Task<List<SelectListItem>> GetTargetsAsync(string permissionFor);

        Task<(bool success, string message, PermissionControlDTO? data)>
            GetPermissionsAsync(string permissionFor, string targetId);

        Task<(bool success, string message)>
            SavePermissionsAsync(PermissionControlSaveDTO model);
    }
}
