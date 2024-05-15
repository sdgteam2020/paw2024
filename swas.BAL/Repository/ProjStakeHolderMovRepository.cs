using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components.Routing;
using ASPNetCoreIdentityCustomFields.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace swas.BAL.Repository
{

    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class ProjStakeHolderMovRepository : GenericRepositoryDL<tbl_ProjStakeHolderMov>, IProjStakeHolderMovRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;

        public ProjStakeHolderMovRepository(ApplicationDbContext dbContext, IDataProtectionProvider DataProtector, IHttpContextAccessor httpContextAccessor):base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");

        }


        public async Task<List<DTOProjectMovHistory>> ProjectMovHistory(int ProjectId)
        {
            var query = await (from a in _dbContext.Projects
                        join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                        join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid
                        join tounit in _dbContext.tbl_mUnitBranch on b.ToUnitId equals tounit.unitid
                        join fromunit in _dbContext.tbl_mUnitBranch on b.FromUnitId equals fromunit.unitid
                        join ststus in _dbContext.mStatus on b.StatusId equals ststus.StatusId
                        join stge in _dbContext.mStages on ststus.StageId equals stge.StagesId
                        join act in _dbContext.mActions on b.ActionId equals act.ActionsId
                       
                        where b.ProjId == ProjectId
                        orderby b.TimeStamp descending
                         select new DTOProjectMovHistory
                        {
                            PsmId=b.PsmId,
                            Stages=stge.Stages,
                            Status=ststus.Status,
                            Actions=act.Actions,
                            FromUnitName=fromunit.UnitName,
                            ToUnitName=tounit.UnitName,
                            FromUser="",
                            ToUser="",
                            Date=b.TimeStamp,
                            Remarks=b.Remarks,
                            UndoRemarks=b.UndoRemarks
                         }).ToListAsync();



            return query;   
        }
        public int GetLastRecProjectMov(int ProjectId)
        {
            try
            {
               // var query = _context.ProjStakeHolderMov.Where(i => i.ProjId == ProjectId && i.PsmId < (_context.ProjStakeHolderMov.Max(p => p.PsmId))).Max(p => p.PsmId);
                var query = _context.ProjStakeHolderMov.Where(i => i.ProjId == ProjectId && i.IsActive==true && i.StatusId!=5).Max(p => p.PsmId);
                return query;

            }
            catch (Exception ex) { return 0; }
          
        }
       
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 15,16 & 31 Jul 23
        // Reviewed Date : 27 Aug 23, 02,03, 09 & 10 Oct 23  ---   Flags Error Rectified
        //  Data Transfer Error Rectified...   Get Request error by remove antifurg....  carried out
        //  outside user access error rectified
        //   model validation removed















        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 17 Nov 23





        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<tbl_ProjStakeHolderMov> GetProjStakeHolderMovByIdAsync(int psmId)
        {
            return await _dbContext.ProjStakeHolderMov.FindAsync(psmId);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<List<tbl_ProjStakeHolderMov>> GetAllProjStakeHolderMovAsync()
        {
            return await _dbContext.ProjStakeHolderMov.ToListAsync();
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<bool> UpdateProjStakeHolderMovAsync(tbl_ProjStakeHolderMov projStakeHolderMov)
        {
            _dbContext.Entry(projStakeHolderMov).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<bool> DeleteProjStakeHolderMovAsync(int psmId)
        {
            var projStakeHolderMov = await _dbContext.ProjStakeHolderMov.FindAsync(psmId);
            if (projStakeHolderMov == null)
                return false;

            _dbContext.ProjStakeHolderMov.Remove(projStakeHolderMov);
            await _dbContext.SaveChangesAsync();
            return true;
        }


      

     
        // added by Sub Maj Sanal on 21 Nov 23
        public async Task<int> ValStatusAsync(int? ProjId)
        {
            int? maxStatusId = _dbContext.ProjStakeHolderMov
    .Where(p => p.ProjId == ProjId)
    .Select(p => (int?)p.StatusId)
    .Max();
           
            int result = maxStatusId ?? 0;

            return result;
        }

        public async Task<int> AddProjStakeHolderMovAsync(tbl_ProjStakeHolderMov projmove)
        {
             _dbContext.ProjStakeHolderMov.Add(projmove);
             return   await _dbContext.SaveChangesAsync();

        }



        public Task<int> CountinboxAsync(int stkhol)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddProStkMovBlogAsync(Projmove psmove)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetlaststageId(int? ProjId)
        {
            int? maxStatusId = _dbContext.ProjStakeHolderMov
           .Where(p => p.ProjId == ProjId)
            .Select(p => (int?)p.StatusId)
           .Max();

            int result = maxStatusId ?? 0;
                
            return result;
        }

        public Task<int> ReturnDuplProjMovAsync(Projmove psmove)
        {
            throw new NotImplementedException();
        }

        public Task<int> RetWithObsnMovAsync(Projmove psmove)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjLogView>> GetProjLogviewAsync(string startDate, string endDate)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateUndoProjectMov(int ProjectId, int PsmId)
        {
            throw new NotImplementedException();
        }
    }
}
