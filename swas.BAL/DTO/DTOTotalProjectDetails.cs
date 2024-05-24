using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOTotalProjectDetails
    {
        public int ProjId { get; set; }
        public string? ProjName { get; set; }
        public DateTime? InitiatedDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Remark { get; set; }
        public string? Hostedon { get; set; }
        public string? Apptype { get; set; }
        public string? stakeHolder { get; set; }

    }
}
