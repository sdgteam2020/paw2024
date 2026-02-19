using System;
using System.ComponentModel.DataAnnotations;

namespace swas.DAL.Models
{
    public class mCorps
    {
        [Key]
        public int corpsid { get; set; }

        [Required]
        [MaxLength(50)]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9\-_\/\.]+$", ErrorMessage = "Invalid characters.")]
        public string? corpsname { get; set; }

        public int? comdid { get; set; }

        
        public bool? IsDeleted { get; set; }

    
        public bool? IsActive { get; set; }

        public int? EditDeleteBy { get; set; }

        public DateTime? EditDeleteDate { get; set; }

        public int? UpdatedByUserId { get; set; }

        public DateTime? DateTimeOfUpdate { get; set; }
    }
}
