using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.DTO;
using swas.DAL.Models;

namespace swas.BAL.Interfaces
{
    public interface IUnitPermissionRepository
    {
        Task<UnitPermissionDTO>
            GetUnitPermissions(
            int unitId);

        Task<(bool success,
            string message)>
            SaveUnitPermissions(
            UnitPermissionDTO model);
    }
}
