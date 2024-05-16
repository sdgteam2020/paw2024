using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Repository
{

    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class StatusRepository : IStatusRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StatusRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        public async Task<int> AddStatusAsync(tbl_mStatus status)
        {
            _dbContext.mStatus.Add(status);
            await _dbContext.SaveChangesAsync();
            return status.StatusId;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        public async Task<tbl_mStatus> GetStatusByIdAsync(int statusId)
        {
            return await _dbContext.mStatus.FindAsync(statusId);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        public async Task<List<tbl_mStatus>> GetAllStatusAsync()
        {
            return await _dbContext.mStatus.ToListAsync();
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        public async Task<bool> UpdateStatusAsync(tbl_mStatus status)
        {
            _dbContext.Entry(status).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        public async Task<bool> DeleteStatusAsync(int statusId)
        {
            var status = await _dbContext.mStatus.FindAsync(statusId);
            if (status == null)
                return false;

            _dbContext.mStatus.Remove(status);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<DTODDLComman>> GetAllByStages_takeHolder(int ParentId, int UnitId, bool IsOwnProj)
        {
           // var retisdte=await _dbContext.mUnitBranch.Where()
            List<DTODDLComman> lst=new List<DTODDLComman>();
                var ret = (from Status in _dbContext.mStatus
                           join Stages in _dbContext.mStages on Status.StageId equals Stages.StagesId
                           join StatusMapping in _dbContext.TrnUnitStatusMapping on Status.StatusId equals StatusMapping.StatusId
                           where StatusMapping.UnitId == UnitId && Status.StageId == ParentId
                           select new DTODDLComman
                           {
                               Name = Status.Status,
                               Id = Status.StatusId,
                           }
                    ).ToListAsync();
                  lst.AddRange(ret.Result);
            if(IsOwnProj)
            {
                var ret1 = (from Status in _dbContext.mStatus
                           join Stages in _dbContext.mStages on Status.StageId equals Stages.StagesId
                           join StatusMapping in _dbContext.TrnUnitStatusMapping on Status.StatusId equals StatusMapping.StatusId
                           //join re1 in ret.Result on ret1.Id equals Status.StatusId
                            where StatusMapping.UnitId == 0 && Status.StageId == ParentId
                           select new DTODDLComman
                           {
                               Name = Status.Status,
                               Id = Status.StatusId,
                           }
                  ).ToListAsync();

                lst.AddRange(ret1.Result);
              //var ss=  lst.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.Select(e => e.Id).ToList());
            }
            
            return lst;
        }

        public async Task<List<DTODDLComman>> GetAll()
        {
            
            var ret1 =await (from Status in _dbContext.mStatus
                        select new DTODDLComman
                        {
                            Name = Status.Status,
                            Id = Status.StatusId,
                        }).ToListAsync();
           
            return ret1;
        }
    }



}
