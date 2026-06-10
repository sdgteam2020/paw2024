using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class RolePermissionDTO
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public List<PermissionCheckboxDTO>
            Permissions
        { get; set; }
            = new();
    }

    public class PermissionCheckboxDTO
    {
        public int PermissionId { get; set; }

        public string PermissionKey { get; set; }

        public string DisplayName { get; set; }

        public bool IsSelected { get; set; }
    }
}
