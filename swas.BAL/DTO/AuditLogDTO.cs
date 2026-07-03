using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.DTO
{
    public class AuditLogDTO
    {
        public long Id { get; set; }

        public int ProjId { get; set; }
        public string? ProjName { get; set; }

        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }

        public int? OldMappingId { get; set; }
        public int? NewMappingId { get; set; }

        public int? OldStatusId { get; set; }
        public string? OldStatusName { get; set; }

        public int? NewStatusId { get; set; }
        public string? NewStatusName { get; set; }

        public bool IsStatusChanged { get; set; }

        public int? OldStageId { get; set; }
        public string? OldStageName { get; set; }

        public int? NewStageId { get; set; }
        public string? NewStageName { get; set; }

        public bool IsStageChanged { get; set; }

        public int? OldActionId { get; set; }
        public string? OldActionName { get; set; }

        public int? NewActionId { get; set; }
        public string? NewActionName { get; set; }

        public bool IsActionChanged { get; set; }

        public string? OldRemarks { get; set; }
        public string? NewRemarks { get; set; }

        public bool IsRemarksChanged { get; set; }

        public DateTime? OldTimeStamp { get; set; }
        public DateTime? NewTimeStamp { get; set; }

        public bool IsTimeStampChanged { get; set; }

        public int? OldToUnitId { get; set; }
        public string? OldUnitName { get; set; }

        public int? NewToUnitId { get; set; }
        public string? NewUnitName { get; set; }

        public bool IsUnitChanged { get; set; }
    }
    public class AuditLogViewModel
    {
        public long AuditLogId { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; }

        public List<AuditChangeItem> Changes { get; set; } = new();
    }

    public class AuditChangeItem
    {
        public string FieldName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}
