using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swas.DAL.Models
{
	public class tbl_mStages
	{
        [Key]
        public int StagesId { get; set; }

        [Required(ErrorMessage = "Stage name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Stage must be between 2 and 200 characters")]
        [Column(TypeName = "varchar(200)")]
        [RegularExpression(@"^[a-zA-Z0-9\s._-]+$", ErrorMessage = "Invalid characters in Stage name")]
        [Display(Name = "Stage")]
        public string Stages { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid user")]
        [Display(Name = "Edit/Delete By")]
        public int? EditDeleteBy { get; set; }

        [Display(Name = "Edit/Delete Date")]
        public DateTime? EditDeleteDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid user")]
        public int UpdatedByUserId { get; set; }

        [Required]
        [Display(Name = "Date of Update")]
        public DateTime DateTimeOfUpdate { get; set; } = DateTime.UtcNow;

        // Fixed naming + non-nullable
        public bool? InitiaalID { get; set; } = false;
		public bool? FininshID { get; set; } = false;
	}


}
