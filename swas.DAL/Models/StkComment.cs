using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace swas.DAL.Models
{


    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StkComment
    {
        [Key]
        public int StkCommentId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid PsmId")]
        public int? PsmId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Project Id")]
        public int? ProjId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid StakeHolder")]
        public int? StakeHolderId { get; set; }

        [Range(1, int.MaxValue)]
        public int? ActionsId { get; set; }

        [Required(ErrorMessage = "Comments are required")]
        [StringLength(501, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!@#()_\-]*$", ErrorMessage = "Invalid characters in comments")]
        public string? Comments { get; set; }

        [Range(1, int.MaxValue)]
        public int? StkStatusId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int? EditDeleteBy { get; set; }

        public DateTime? EditDeleteDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? UpdatedByUserId { get; set; }

        public DateTime DateTimeOfUpdate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,:;@#()_\-]*$", ErrorMessage = "Invalid characters in UserDetails")]
        public string? UserDetails { get; set; }

        // File related fields
        [StringLength(255)]
        public string? ActFileName { get; set; }

        [StringLength(500)]
        public string? Attpath { get; set; }

        [StringLength(255)]
        public string? AttDesc { get; set; }

        // NotMapped (UI only)
        [NotMapped]
        public string? Remarks { get; set; }

        [NotMapped]
        public tbl_AttHistory? AttHisAdd { get; set; }

        public StkComment()
        {
            ProjDetl = new List<tbl_Projects>();
        }

        [NotMapped]
        public List<tbl_Projects>? ProjDetl { get; set; }
    }
}
