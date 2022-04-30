using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swas.DAL.Models
{
    public class tbl_mStatus
    {
        [Key]
        [Display(Name = "StatusId")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Stage is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Stage")]
        [ForeignKey("tbl_mStages")]
        public int StageId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Status must be between 2 and 200 characters")]
        [Column(TypeName = "varchar(200)")]
        [RegularExpression(@"^[a-zA-Z0-9\s._-]+$", ErrorMessage = "Invalid characters in Status")]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid user")]
        [Display(Name = "Edit/Delete By")]
        public int? EditDeleteBy { get; set; }

        [Display(Name = "Edit/Delete Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EditDeleteDate { get; set; }

        [Required(ErrorMessage = "UpdatedByUserId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid user")]
        public int UpdatedByUserId { get; set; }

        [Required(ErrorMessage = "Date of Update is required")]
        [Display(Name = "Date of Update")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateTimeOfUpdate { get; set; } = DateTime.UtcNow;

        [Required]
        public bool? InitiaalID { get; set; } = false;

        [Required]
        public bool? FininshID { get; set; } = false;

        [Range(0, int.MaxValue, ErrorMessage = "TotalProj cannot be negative")]
        public int? TotalProj { get; set; }

        [Required(ErrorMessage = "Sequence is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid sequence")]
        public int Statseq { get; set; }

        public bool IsDashboard { get; set; } = false;

        [StringLength(100, ErrorMessage = "Icon name too long")]
        [RegularExpression(@"^[a-zA-Z0-9\s._-]*$", ErrorMessage = "Invalid characters in Icon")]
        public string? Icon { get; set; }
    }


}
