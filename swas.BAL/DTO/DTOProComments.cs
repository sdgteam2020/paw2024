using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOProComments
    {
        public int StkCommentId { get; set; }
        public int ProjId { get; set; }
        public string? ProjectName { get; set; }
        public int? PsmId { get; set; }
        public string? Stakeholder { get; set; }
        public int? UnitId { get; set; }
        public int? StkStatusId { get; set; }
        public string? Status { get; set; }
        public string Attpath { get; set; }
        public string ActFileName { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
    }
}
