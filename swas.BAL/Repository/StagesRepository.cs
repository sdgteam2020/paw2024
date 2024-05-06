using Microsoft.EntityFrameworkCore;
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

    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class StagesRepository : IStagesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StagesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<int> AddStageAsync(tbl_mStages stage)
        {
            _dbContext.mStages.Add(stage);
            await _dbContext.SaveChangesAsync();
            return stage.StagesId;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<tbl_mStages> GetStageByIdAsync(int stageId)
        {
            return await _dbContext.mStages.FindAsync(stageId);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<List<tbl_mStages>> GetAllStagesAsync()
        {
            return await _dbContext.mStages.ToListAsync();
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<bool> UpdateStageAsync(tbl_mStages stage)
        {
            _dbContext.Entry(stage).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        public async Task<bool> DeleteStageAsync(int stageId)
        {
            var stage = await _dbContext.mStages.FindAsync(stageId);
            if (stage == null)
                return false;

            _dbContext.mStages.Remove(stage);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        ///Created by : Ajay
        ///Reviewed Date : 24 Aug 23

        public List<tbl_mStages> GetAllStages()
        {
            return _dbContext.mStages.ToList();
        }

    }



}
