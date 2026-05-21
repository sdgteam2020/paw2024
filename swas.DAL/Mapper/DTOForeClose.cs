using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOForeClose
    {
        public int projid { get; set; }
        [NotMapped]
        public string? EncyID { get; set; }
        public string? ProjName { get; set; }

        public string? Sponsor { get; set; }

        public string? Approved_By { get; set; }

        public bool? IsProcess { get; set; }

        public bool? IsSubmited { get; set; }

        public string? UserDetails { get; set; }

        public bool? IsComment { get; set; }

        public bool? IsComplete { get; set; }
        public DateTime? TimeStamp { get; set; }

        public string? Stages { get; set; }

        public string? Status { get; set; }

        public string? Actions { get; set; }
    }
}
