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
    }



}
