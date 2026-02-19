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
    public class StakeHolderRepository : IStakeHolderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StakeHolderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> AddStakeHolderAsync(tbl_mStakeHolder stakeHolder)
        {
            _dbContext.mStakeHolder.Add(stakeHolder);
            await _dbContext.SaveChangesAsync();
            return stakeHolder.StakeHolderId;
        }
        public async Task<tbl_mStakeHolder> GetStakeHolderByIdAsync(int stakeHolderId)
        {
            return await _dbContext.mStakeHolder.FindAsync(stakeHolderId);
        }
        public async Task<List<tbl_mStakeHolder>> GetAllStakeHoldersAsync()
        {
            return await _dbContext.mStakeHolder.ToListAsync();
        }
        public async Task<bool> UpdateStakeHolderAsync(tbl_mStakeHolder stakeHolder)
        {
            _dbContext.Entry(stakeHolder).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteStakeHolderAsync(int stakeHolderId)
        {
            var stakeHolder = await _dbContext.mStakeHolder.FindAsync(stakeHolderId);
            if (stakeHolder == null)
                return false;

            _dbContext.mStakeHolder.Remove(stakeHolder);
            await _dbContext.SaveChangesAsync();
            return true;
        }


    }


}
