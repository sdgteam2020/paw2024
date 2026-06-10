using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.DAL.Models;

namespace swas.BAL.Interfaces
{
    public interface IPermissionMasterRepository
    {
        List<PermissionMaster> GetAll();

        PermissionMaster GetById(int id);

        Task<(bool success, string message)>
            Save(PermissionMaster model,
            string username);

        Task<(bool success, string message)>
            Delete(int id, string username);

        bool IsDuplicate(
            string permissionKey,
            int id);
    }
}
