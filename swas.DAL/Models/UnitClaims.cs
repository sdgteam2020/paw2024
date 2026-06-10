using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    [Table("UnitClaims")]
    public class UnitClaims
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UnitId { get; set; }

        [Required]
        [StringLength(100)]
        public string ClaimType { get; set; }

        public int PermissionId { get; set; }

        public bool IsActive { get; set; } = true;

     

        public DateTime CreatedDate { get; set; }

     

        // FK Navigation
        [ForeignKey("PermissionId")]
        public virtual PermissionMaster
            PermissionMaster
        { get; set; }

        // FK to tbl_munitbranch
        [ForeignKey("UnitId")]
        public virtual tbl_mUnitBranch
            UnitBranch
        { get; set; }
    }
}
