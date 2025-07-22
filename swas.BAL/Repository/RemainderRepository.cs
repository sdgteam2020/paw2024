using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using swas.UI.Helpers;
using static swas.DAL.Models.LegacyHistory;



namespace swas.BAL.Repository
{
    public class RemainderRepository : IRemainder
    {
        private readonly ApplicationDbContext _dbContext;
        
        public RemainderRepository(ApplicationDbContext context)
        {
            _dbContext = context;


        }
        public async Task<int> AddRemainder(int psmid, int fromUnitId, int toUnitId, string remarks, string userDetails)
        {
            var tblRemainder = new trnRemainder
            {
                Projid = psmid,
                FromUnitId = fromUnitId,
                Tounitid = toUnitId,
                Remarks = remarks,
                UserDetails = userDetails,
                SendDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsRemainder = true,
                IsRead = false
            };

            _dbContext.TrnRemainders.Add(tblRemainder);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<RemainderDisplayDto>> GetAllAsync()
        {
            try
            {
                var ret = await (from rem in _dbContext.TrnRemainders
                                 join proj in _dbContext.Projects
                                     on rem.Projid equals proj.ProjId
                                 join fromUnit in _dbContext.tbl_mUnitBranch
                                     on rem.FromUnitId equals fromUnit.unitid into fromUnitJoin
                                 from fromUnit in fromUnitJoin.DefaultIfEmpty()
                                 join toUnit in _dbContext.tbl_mUnitBranch
                                     on rem.Tounitid equals toUnit.unitid into toUnitJoin
                                 from toUnit in toUnitJoin.DefaultIfEmpty()
                                 select new
                                 {
                                     rem.ReadDate,
                                     rem.RemainderId,
                                     rem.Remarks,
                                     rem.Projid,
                                     ProjName = proj.ProjName,
                                     Sponsor = proj.Sponsor,
                                     Domain = proj.Sponsor, // Adjust if there's a specific Domain field
                                     FromUnitName = fromUnit != null ? fromUnit.UnitName : "N/A",
                                     ToUnitName = toUnit != null ? toUnit.UnitName : "N/A",
                                     rem.UserDetails,
                                     rem.SendDate
                                 })
                  .GroupBy(x => x.ProjName)
                  .Select(g => new RemainderDisplayDto
                  {
                      ReadOn =g.First().ReadDate?? "No Date",
                     
                      projid = g.First().Projid,
                      ProjName = g.Key,
                      Sponsor = g.First().Sponsor,
                      Domain = g.First().UserDetails,
                      Remarks = g.OrderByDescending(x => x.SendDate).First().Remarks, // Latest Remarks
                   
                      FromUnit = g.OrderByDescending(x => x.SendDate).First().FromUnitName,
                      ToUnit = g.OrderByDescending(x => x.SendDate).First().ToUnitName,
                      SentOn = g.Max(x => x.SendDate) // Latest SendDate
                  })
                  .OrderByDescending(x => x.SentOn)
                  .ToListAsync();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }




        public async Task<List<RemainderDisplayDto>> ProjectRemainderMovHistory(int? ProjectId)
        {
            try
            {
                if (!ProjectId.HasValue)
                {
                    throw new ArgumentNullException(nameof(ProjectId), "ProjectId cannot be null.");
                }

                var history = await (from Rm in _dbContext.TrnRemainders
                                     join p in _dbContext.Projects on Rm.Projid equals p.ProjId
                                     join u in _dbContext.tbl_mUnitBranch on Rm.FromUnitId equals u.unitid
                                     join f in _dbContext.tbl_mUnitBranch on Rm.Tounitid equals f.unitid
                                     where Rm.Projid == ProjectId
                                     orderby Rm.SendDate descending
                                     select new RemainderDisplayDto
                                     {
                                         
                                         projid = Rm.Projid,
                                         ProjName = p.ProjName ?? "No Project",
                                         SentOn = Rm.SendDate?? "No Date",
                                         Remarks = Rm.Remarks ?? "No Remarks",
                                         userDetails = Rm.UserDetails ?? "No User",
                                         ReadOn = Rm.ReadDate??"No ReadDate",
                                         FromUnit = u.UnitName ?? "No Unit",
                                         ToUnit = f.UnitName ?? "No Unit",
                                         Sponsor = p.Sponsor ?? "No Unit",
                                     }).ToListAsync();

                return history;
            }
            catch (Exception ex)
            {
              
                throw;
            }
        }

        public Task<int> GetProjectById(string? ProjName)
        {
            throw new NotImplementedException();
        }
    }
}
