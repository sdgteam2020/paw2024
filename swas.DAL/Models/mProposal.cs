using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class mProposal
    {
        [Key]
        public int ProposalId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProposalType { get; set; } = string.Empty;




        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
