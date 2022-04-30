
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace ASPNetCoreIdentityCustomFields.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int UserIntId { get; set; }

        [Required]
        public string? RoleName { get; set; }

        [StringLength(20, ErrorMessage = "Domain IAM cannot exceed 20 characters")]
        public string? domain_iam { get; set; }

        [StringLength(100)]
        public string? description_iam { get; set; }

        [StringLength(50)]
        public string? RoleName_IAM { get; set; }

        [Required]
        public int unitid { get; set; }

        [StringLength(20, ErrorMessage = "Appointment cannot exceed 20 characters")]
        public string? appointment { get; set; }

        [StringLength(20, ErrorMessage = "IC No cannot exceed 20 characters")]
        public string? Icno { get; set; }

        public int? Rank { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Officer Name cannot exceed 20 characters")]
        public string? Offr_Name { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Telephone cannot exceed 20 characters")]
        public string? Tele_Army { get; set; }

        public bool? Flag { get; set; }

        public DateTime? CreatedDate { get; set; }


    }
}