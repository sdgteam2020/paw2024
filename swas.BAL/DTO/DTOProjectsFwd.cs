using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOProjectsFwd
    {
        public int ProjId { get; set; }
        public int PsmIds { get; set; }
        public string? ProjName { get; set; }
        public int StakeHolderId { get; set; }
        public string? StakeHolder { get; set; }
        public string? Remarks { get; set; }
        public string? Status { get; set; }
        public int FromUnitId { get; set; }
        public string? FromUnitName { get; set; }
        public int ToUnitId { get; set; }
        public string? ToUnitName { get; set; }
        public string? Action { get; set; }
        public int StageId { get; set; }
        public int TotalDays { get; set; }
        public string? EncyID { get; set; }
        public string? EncyPsmID { get; set; }
        public bool IsProcess { get; set; }
        public int undopsmId { get; set; }
    }
}
