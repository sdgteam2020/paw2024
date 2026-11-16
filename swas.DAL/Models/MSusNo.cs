using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class MSusNo
    {
        [Key]
        public int SusNoId { get; set; }

        [Required]
        [StringLength(50)]
        public string SusNo { get; set; }

        public bool IsActive { get; set; } = true;

        public int? UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
