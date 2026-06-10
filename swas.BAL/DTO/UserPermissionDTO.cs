using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class UserPermissionDTO
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public List<UserPermissionCheckboxDTO>
            Permissions
        { get; set; }
            = new();
    }

    public class UserPermissionCheckboxDTO
    {
        public int PermissionId { get; set; }

        public string PermissionKey { get; set; }

        public string DisplayName { get; set; }

        public bool IsSelected { get; set; }
    }
}
