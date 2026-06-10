using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class UnitPermissionDTO
    {
        public int UnitId { get; set; }

        public string UnitName { get; set; }

        public List<UnitPermissionCheckboxDTO>
            Permissions
        { get; set; }
            = new();
    }

    public class UnitPermissionCheckboxDTO
    {
        public int PermissionId { get; set; }

        public string PermissionKey { get; set; }

        public string DisplayName { get; set; }

        public bool IsSelected { get; set; }
    }
}
