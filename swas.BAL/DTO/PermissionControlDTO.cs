using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class PermissionControlDTO
    {
        public string PermissionFor { get; set; } = string.Empty;
        public string TargetId { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;

        public List<PermissionControlCheckboxDTO> Permissions { get; set; } = new();
    }

    public class PermissionControlCheckboxDTO
    {
        public int PermissionId { get; set; }

        public string PermissionKey { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool IsSelected { get; set; }
    }

    public class PermissionControlSaveDTO
    {
        public string PermissionFor { get; set; } = string.Empty;

        public string TargetId { get; set; } = string.Empty;

        public List<PermissionControlCheckboxDTO> Permissions { get; set; } = new();
    }

    public static class PermissionForType
    {
        public const string Role = "Role";
        public const string User = "User";
        public const string Unit = "Unit";
    }
}
