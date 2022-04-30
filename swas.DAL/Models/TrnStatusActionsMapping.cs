using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class TrnStatusActionsMapping
    {
        [Key]
        public int StatusActionsMappingId { get; set; }

        [Required(ErrorMessage = "StatusId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid StatusId")]
        [ForeignKey("tbl_mStatus")]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "ActionsId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid ActionsId")]
        [ForeignKey("tbl_mActions")]
        public int ActionsId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
