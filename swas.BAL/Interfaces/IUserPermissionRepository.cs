using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.DTO;

namespace swas.BAL.Interfaces
{
    public interface
         IUserPermissionRepository
    {
        Task<(bool success, string message, UserPermissionDTO? data)>
     GetUserPermissions(string userId);

        Task<(bool success,
            string message)>
            SaveUserPermissions(
            UserPermissionDTO model);
    }
}
