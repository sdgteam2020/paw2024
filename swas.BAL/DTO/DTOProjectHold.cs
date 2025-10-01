using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class DTOProjectHold
    {
        public int PsmId { get; set; }
        public string? Fromunit { get; set; }
        public int FromunitId { get; set; }
        public int TounitId { get; set; }
        public string? Tounit { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? TimeStampfrom { get; set; }
        public DateTime? TimeStampTo { get; set; }
        public DateTime? DateTimeOfUpdate { get; set; }
        public DateTime? LatestCommentDate { get; set; }
        public DateTime? Approveddate { get; set; }
        public DateTime? FirstActionDate { get; set; }
        public DateTime? RejectedDt { get; set; }

        public string? Status { get; set; }
        public int StatusId { get; set; }
        public int ApprovedStatusId { get; set; }
        public string? Action { get; set; }
        public string? UndoRemarks { get; set; }    
        public bool IsComment { get; set; }
        public bool IsComplete { get; set; }
        public int IsApproved { get; set; }
        public string? StkStauts { get; set; }
        public string? FirstStkStatus { get; set; }
    }
}
