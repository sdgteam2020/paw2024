using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class tbl_ProjStakeHolderCcMov
    {
        [Key]
        public int PsmCcId { get; set; }

        [Required(ErrorMessage = "PsmId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid PsmId")]
        [ForeignKey("tbl_ProjStakeHolderMov")]
        public int PsmId { get; set; }

        [Required(ErrorMessage = "Project Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Project Id")]
        [ForeignKey("tbl_Projects")]
        public int ProjId { get; set; }

        [Required(ErrorMessage = "To CC Unit is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Unit Id")]
        public int ToCcUnitId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public bool IsRead { get; set; } = false;

        [Required(ErrorMessage = "Read Date is required")]
        public DateTime ReadDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,:;@#()_\-]*$", ErrorMessage = "Invalid characters in UserDetails")]
        public string? UserDetails { get; set; }
    }
}
