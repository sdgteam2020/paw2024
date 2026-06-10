using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.DTO;

namespace swas.BAL.Interfaces
{
    public interface
        IRolePermissionRepository
    {
        Task<RolePermissionDTO>
            GetRolePermissions(
            string roleId);

        Task<(bool success,
            string message)>
            SaveRolePermissions(
            RolePermissionDTO model);
    }
}
