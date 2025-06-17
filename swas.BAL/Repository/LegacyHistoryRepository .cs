using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static swas.DAL.Models.LegacyHistory;
namespace swas.BAL.Repository
{
    public class LegacyHistoryRepository : ILegacyHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public LegacyHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(LegacyHistory history)
        {
            _context.LegacyHistory.Add(history);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<LegacyHistory>> GetHistoryByProjectIdAsync(int projectId)
        {
            try
            {
                var history = await (from LH in _context.LegacyHistory
                                     join p in _context.Projects on LH.ProjectId equals p.ProjId
                                     join u in _context.tbl_mUnitBranch on LH.UnitId equals u.unitid
                                     join f in _context.tbl_mUnitBranch on LH.FromUnit equals f.unitid
                                     where LH.ProjectId == projectId
                                     orderby LH.ActionDate descending
                                     select new LegacyHistory
                                     {
                                         HistoryId = LH.HistoryId,
                                         ProjectId = LH.ProjectId,
                                         ActionType = LH.ActionType,
                                         ActionDate = LH.ActionDate,
                                         Remarks = LH.Remarks,
                                         Userdetails = LH.Userdetails,
                                         ProjectName = p.ProjName,
                                         ToUnitName = u.UnitName,
                                         FromunitName = f.UnitName,


                                     }).ToListAsync();
                foreach (var item in history)
                {
                    item.ActionTypeText = Enum.GetName(typeof(ActionTypeEnum), item.ActionType); // Convert enum value to string
                }

                return history;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return null;
            }
        }
    }
}
