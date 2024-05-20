using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOProjectMovHistory
    {
        public int PsmId { get; set; }
        public string? Stages { get; set; }
        public string? Status { get; set; }
        public string? Actions { get; set; }
        public string? FromUnitName { get; set; }
        public string? ToUnitName { get; set; }
        public string? FromUser { get; set; }
        public string? ToUser { get; set; }
        public DateTime? Date { get; set; }
        public string? Remarks { get; set; }
        public string? UndoRemarks { get; set; }
        public bool IsComment { get; set; }
    }
}
