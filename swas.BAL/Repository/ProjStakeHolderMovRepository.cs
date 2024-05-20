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
using System.Xml.Linq;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

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


        public async Task<List<DTOProjectMovHistory>> ProjectMovHistory(int? ProjectId)
        {
           

            var query = await (from a in _dbContext.Projects
                        join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                        join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid
                        join tounit in _dbContext.tbl_mUnitBranch on b.ToUnitId equals tounit.unitid
                        join fromunit in _dbContext.tbl_mUnitBranch on b.FromUnitId equals fromunit.unitid
                        join actmap in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actmap.StatusActionsMappingId
                        join ststus in _dbContext.mStatus on actmap.StatusId equals ststus.StatusId
                        join stge in _dbContext.mStages on ststus.StageId equals stge.StagesId
                        join act in _dbContext.mActions on actmap.ActionsId equals act.ActionsId
                       
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
                            UndoRemarks=b.UndoRemarks,
                            IsComment=b.IsComment,
                            AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.PsmId),
                            UserDetails=b.UserDetails,
                            

                         }).ToListAsync();



            return query;   
        }
        public int GetLastRecProjectMov(int ProjectId)
        {
            try
            {
               // var query = _context.ProjStakeHolderMov.Where(i => i.ProjId == ProjectId && i.PsmId < (_context.ProjStakeHolderMov.Max(p => p.PsmId))).Max(p => p.PsmId);
                var query = _context.ProjStakeHolderMov.Where(i => i.ProjId == ProjectId && i.IsActive==true).Max(p => p.PsmId);
                return query;

            }
            catch (Exception ex) { return 0; }
          
        }
        public async Task<DTODashboard> DashboardCount(int UserId)
        {
            DTODashboard db=new DTODashboard();
           

            var query = await (from mov in _dbContext.ProjStakeHolderMov
                               join proj in _dbContext.Projects on mov.ProjId equals proj.ProjId
                               join actmap in _dbContext.TrnStatusActionsMapping on mov.StatusActionsMappingId equals actmap.StatusActionsMappingId
                               join ststus in _dbContext.mStatus on actmap.StatusId equals ststus.StatusId
                               join stge in _dbContext.mStages on ststus.StageId equals stge.StagesId
                               //join act in _dbContext.mActions on actmap.ActionsId equals act.ActionsId
                               where 
                               //mov.ToUnitId == UserId &&
                               mov.IsComplete == false /*&& mov.ToUnitId == 1 && mov.StatusId != 5*/
                               && ststus.IsDashboard == true
                               orderby stge.StagesId ascending
                               group mov by new { ststus.StatusId, QStages = stge.Stages, QStagesId= stge.StagesId, 
                                   QStatus=ststus.Status,
                                
                                   QIsComplete = mov.IsComplete,QprojId= proj.ProjId
                                  
                               } into gr  //,QActionId= actmap.ActionsId

                               select new DTODashboardCount
                               {

                                   StatusId = gr.Key.StatusId,
                                   Stages = gr.Key.QStages,
                                   StagesId = gr.Key.QStagesId,
                                   Status = gr.Key.QStatus,
                                   IsComplete=gr.Key.QIsComplete,
                                   //ActionId = gr.Key.QActionId,
                                   Tot = gr.Count(),
                                 
                               }).ToListAsync();

            db.DTODashboardCountlst=(query);

            var query11 = await (from mov in _dbContext.ProjStakeHolderMov
                               join proj in _dbContext.Projects on mov.ProjId equals proj.ProjId
                               join actmap in _dbContext.TrnStatusActionsMapping on mov.StatusActionsMappingId equals actmap.StatusActionsMappingId
                               join ststus in _dbContext.mStatus on actmap.StatusId equals ststus.StatusId
                               join stge in _dbContext.mStages on ststus.StageId equals stge.StagesId
                               //join act in _dbContext.mActions on actmap.ActionsId equals act.ActionsId
                               where 
                              // mov.FromUnitId == UserId && 
                               mov.IsComplete == true /*&& mov.ToUnitId == 1 && mov.StatusId != 5*/
                               && ststus.IsDashboard == true
                         orderby stge.StagesId ascending
                               group mov by new
                               {
                                   ststus.StatusId,
                                   QStages = stge.Stages,
                                   QStagesId = stge.StagesId,
                                   QStatus = ststus.Status,
                                   
                                   QIsComplete = mov.IsComplete,
                                   QprojId = proj.ProjId

                               } into gr  //,QActionId= actmap.ActionsId

                               select new DTODashboardCount
                               {

                                   StatusId = gr.Key.StatusId,
                                   Stages = gr.Key.QStages,
                                   StagesId = gr.Key.QStagesId,
                                   Status = gr.Key.QStatus,
                                   IsComplete = gr.Key.QIsComplete,
                                   //ActionId = gr.Key.QActionId,
                                   
                                   Tot = gr.Count()
                               }).ToListAsync();

            
            db.DTODashboardCountlst.AddRange(query11);
            db.DTODashboardCountlst= db.DTODashboardCountlst.OrderBy(x => x.StagesId).OrderBy(x => x.StatusId).ToList();

            var query1 = await (from ststus in _dbContext.mStatus
                               join stge in _dbContext.mStages on ststus.StageId equals stge.StagesId
                               where ststus.IsDashboard == true
                                orderby stge.StagesId ascending
                                select new DTODashboardHeader
                               {
                                   StageId= stge.StagesId,
                                   StatusId=ststus.StatusId,
                                   Status=ststus.Status,
                                   Stages= stge.Stages,
                                   Icons= ststus.Icon

                                }).ToListAsync();
            db.DTODashboardHeaderlst = query1;

            //var query2 = await (from actmap in _dbContext.TrnStatusActionsMapping
            //                    join ststus in _dbContext.mStatus on actmap.StatusId equals ststus.StatusId
            //                    join act in _dbContext.mActions on actmap.ActionsId equals act.ActionsId
            //                    orderby actmap.ActionsId ascending
            //                    select new DTODashboardAction
            //                    {
            //                        StatusId= ststus.StatusId,
            //                        ActionId= act.ActionsId,
            //                        Action=act.Actions
            //                    }).ToListAsync();
            //db.DTODashboardActionlst = query2;
            return db;
        }
        //public async Task<int> IsReadInbox(int psmId)
        //{

        //    var _person = new tbl_ProjStakeHolderMov() {PsmId=psmId, IsRead=true};


        //    _context.ProjStakeHolderMov.Attach(_person);
        //    _context.Entry(_person).Property(X => X.PsmId).IsModified = true;
        //    _context.SaveChanges();
        //    return 1;
        //}
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
    .Select(p => (int?)p.StatusActionsMappingId)
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
            .Select(p => (int?)p.StatusActionsMappingId)
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
