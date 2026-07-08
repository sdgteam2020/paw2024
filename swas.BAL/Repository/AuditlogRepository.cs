using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Repository
{
    public class AuditlogRepository: IAuditlogRepository
    {
        private readonly ApplicationDbContext _context;
        public AuditlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AuditLogViewModel>> GetAllGetAuditLogHistory(int ProjId)
        {
            var auditDtos = await _context.Database
         .SqlQuery<AuditLogDTO>(
             $"EXEC USP_GetAuditLogHistory @ProjId={ProjId}")
         .ToListAsync();
           
            var result = auditDtos.Select(x =>
            {
                var vm = new AuditLogViewModel
                {
                    AuditLogId = x.Id,
                    ProjectId = x.ProjId,
                    ProjectName = x.ProjName,
                    ChangedBy = x.ChangedBy,
                    ChangedAt = TimeZoneInfo.ConvertTimeFromUtc(x.ChangedAt,TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
                };
                if (x.IsStageChanged)
                {
                    vm.Changes.Add(new AuditChangeItem
                    {
                        FieldName = "Stage",
                        OldValue = x.OldStageName,
                        NewValue = x.NewStageName
                    });
                }
                if (x.IsStatusChanged)
                {
                    vm.Changes.Add(new AuditChangeItem
                    {
                        FieldName = "Sub Stage",
                        OldValue = x.OldStatusName,
                        NewValue = x.NewStatusName
                    });
                }

                
                if (x.IsActionChanged)
                {
                    vm.Changes.Add(new AuditChangeItem
                    {
                        FieldName = "Action",
                        OldValue = x.OldActionName,
                        NewValue = x.NewActionName
                    });
                }


                if (x.IsUnitChanged)
                {
                    vm.Changes.Add(new AuditChangeItem
                    {
                        FieldName = "Fwd To",
                        OldValue = x.OldUnitName,
                        NewValue = x.NewUnitName
                    });
                }

                if (x.IsRemarksChanged)
                {
                    vm.Changes.Add(new AuditChangeItem
                    {
                        FieldName = "Remarks",
                        OldValue = x.OldRemarks,
                        NewValue = x.NewRemarks
                    });
                }

                if (x.IsTimeStampChanged)
                {
                    vm.Changes.Add(new AuditChangeItem
                    {
                        FieldName = "Time Stamp",
                        OldValue = x.OldTimeStamp?.ToString("dd-MMM-yyyy HH:mm"),
                        NewValue = x.NewTimeStamp?.ToString("dd-MMM-yyyy HH:mm")
                    });
                }

                return vm;
            }).ToList();

            return result;
        }
    }
}
