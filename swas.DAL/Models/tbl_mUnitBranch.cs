using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace swas.DAL.Models
{

    public class tbl_mUnitBranch
    {
        [Key]
        [Display(Name = "Unit ID")]
        public int unitid { get; set; }

        [Required(ErrorMessage = "Unit Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Unit Name must be between 2 and 200 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s._-]+$", ErrorMessage = "Invalid characters in Unit Name")]
        [Display(Name = "Unit Name")]
        public string? unitname { get; set; }

        [Required(ErrorMessage = "Company ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Company ID")]
        [Display(Name = "Company ID")]
        public int comdid { get; set; }

        [Required(ErrorMessage = "Unit SUS Number is required")]
        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9\s._/-]+$", ErrorMessage = "Invalid characters in SUS Number")]
        [Display(Name = "Unit SUS Number")]
        public string unitSusNo { get; set; }

        [Required(ErrorMessage = "Area Location is required")]
        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9\s,._-]+$", ErrorMessage = "Invalid characters in Area Location")]
        [Display(Name = "Area Location")]
        public string area_loc { get; set; }

        [Required(ErrorMessage = "Type ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Type ID")]
        [Display(Name = "Type ID")]
        [ForeignKey("Types")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Corps ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Corps ID")]
        [Display(Name = "Corps ID")]
        public int CorpsId { get; set; }

        [Required]
        [Display(Name = "Status")]
        public bool status { get; set; }

        [Required(ErrorMessage = "Updated By is required")]
        [StringLength(200, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9\s._-]+$", ErrorMessage = "Invalid characters in Updated By")]
        [Display(Name = "Updated By")]
        public string updatedby { get; set; }

        [Required(ErrorMessage = "Updated Date is required")]
        [Display(Name = "Updated Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Comment Required")]
        public bool? CommentRequired { get; set; } = false;
    }
}
