using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int ProjId { get; set; }

        [ForeignKey(nameof(ProjId))]
        public virtual tbl_Projects Project { get; set; } = null!;

        public string? OldData { get; set; }

        public string? NewData { get; set; }

        public string? ChangedBy { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; }
    }
}
