using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static swas.DAL.Models.LegacyHistory;
namespace swas.BAL.DTO
{
    public class DTOProjectMovHistory
    {
        public List<DTOProjectMovHistorycmd> DTOProjectMovHistorycmdlst { get; set; }
        public List<DTOProjectMovHistorypsm> DTOProjectMovHistorypsmlst { get; set; }
        public List<DTOProjectCCHistory> DTOProjectCCHistorylst { get; set; }
    }
    public class DTOProjectMovHistorypsm
    {
        public int PsmId { get; set; }
        public string? Stages { get; set; }
        public string? Status { get; set; }
        public int StatusId { get; set; }
        public string? Actions { get; set; }
        public string? FromUnitName { get; set; }
        public string? ToUnitName { get; set; }
        public string? FromUser { get; set; }
        public string? ToUser { get; set; }
        public DateTime? Date { get; set; }
        public string? Remarks { get; set; }
        public string? UndoRemarks { get; set; }
        public bool IsComment { get; set; }
        public int? AttCnt { get; set; }
        public string? UserDetails { get; set; }
        public bool? IsPulledBack { get; set; }
        public int StatusActionsMappingId { get; set; }
        public bool IsCc { get; set; }
        public string CcUnits { get; set; }
        public int StageId { get; set; }
        public int ActionsId { get; set; }
        public int FromUnitId { get; set; }
        public int StakeHolderId { get; set; }
        public ActionTypeEnum? LatestActionType { get; set; }
    }
    public class DTOProjectMovHistorycmd
    {
        public int PsmId { get; set; }
        public string? Comments { get; set; }
        public string? Status { get; set; }
        public string? UserDetails { get; set; }
        public DateTime? DateTimeOfUpdate { get; set; }
    }
    public class DTOProjectCCHistory
    {
        public int PsmId { get; set; }
        public string? UnitName { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadDate { get; set; }
        public string? UserDetails { get; set; }
    }
}
