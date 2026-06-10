using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace swas.DAL.Models
{
    [Table("PermissionMaster")]
    public class PermissionMaster
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Permission Key is required")]
        [StringLength(100)]
        public string PermissionKey { get; set; }

        [Required(ErrorMessage = "Display Name is required")]
        [StringLength(200)]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Module Name is required")]
        [StringLength(100)]
        public string ModuleName { get; set; }

        [Required(ErrorMessage = "Permission Type is required")]
        [StringLength(50)]
        public string PermissionType { get; set; }

        [Range(1, 9999,
            ErrorMessage = "Display Order required")]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

     

        public DateTime CreatedDate { get; set; }



        // Navigation Property
        [ValidateNever]
        public virtual ICollection<UnitClaims>
            UnitClaims
        { get; set; }
    }
}
