using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.DAL.Models
{
    [Table("tbl_project_foreclose")]
    public class ProjectForeclose
    {
        [Key]
        public int ForecloseId { get; set; }

        public int Psmid { get; set; }

        public int ClosedByUserId { get; set; }

        public DateTime ClosedDate { get; set; } = DateTime.Now;

        public string? CloseRemarks { get; set; }

        public bool IsPresentClosed { get; set; } = true;

        public bool IsOpened { get; set; } = false;

        public int? OpenedByUserId { get; set; }

        public DateTime? OpenedDate { get; set; }

        public string? OpenRemarks { get; set; }
    }
}
