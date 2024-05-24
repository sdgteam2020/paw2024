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
using swas.BAL.Utility;

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
                var query = _context.ProjStakeHolderMov.Where(i => i.ProjId == ProjectId && i.IsActive==true && i.IsComment==false).Max(p => p.PsmId);
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
                               mov.IsComplete == false
                               && mov.IsActive == true
                               /*&& mov.ToUnitId == 1 && mov.StatusId != 5*/
                               && ststus.IsDashboard == true
                               && proj.IsSubmited == true   
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
                               mov.IsComplete == true
                                && mov.IsActive == true/*&& mov.ToUnitId == 1 && mov.StatusId != 5*/
                               && ststus.IsDashboard == true
                               && proj.IsSubmited == true
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
            TotalProject totalProject = new TotalProject(); 
            var count = (from result1 in _dbContext.Projects
                         where result1.IsActive == true 
                         select result1).Count();
            totalProject.Total = count;
            db.TotalProjectCount = totalProject;

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

        public async Task<List<ProjLogView>> GetProjLogviewAsync(string startDate, string endDate)
        {
            List<ProjLogView> plvew;

            try
            {
                var result = from b in _dbContext.ProjStakeHolderMov
                             join c in _dbContext.Projects on b.ProjId equals c.ProjId
                             join a in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals a.StatusActionsMappingId
                             
                             join e in _dbContext.mStatus on a.StatusId equals e.StatusId
                             join d in _dbContext.mStages on e.StageId equals d.StagesId
                             join f in _dbContext.mActions on a.ActionsId equals f.ActionsId
                             join g in _dbContext.tbl_mUnitBranch on b.ToUnitId equals g.unitid
                             join h in _dbContext.tbl_mUnitBranch on b.FromUnitId equals h.unitid
                             join j in _dbContext.Comment on b.PsmId equals j.PsmId into commentGroup
                             from j in commentGroup.DefaultIfEmpty()
                             join k in _dbContext.tbl_mUnitBranch on c.StakeHolderId equals k.unitid
                             where b.EditDeleteDate >= DateTime.Parse(startDate) &&
                                   b.EditDeleteDate <= DateTime.Parse(endDate) 
                             orderby b.ProjId, b.EditDeleteDate
                             select new
                             {
                                 b.PsmId,
                                 b.ProjId,
                                 c.ProjName,
                                 k.UnitName,
                                 d.Stages,
                                 e.Status,
                                 f.Actions,
                                 b.TimeStamp,
                                 FwdBy = h.UnitName,
                                 FwdTo = g.UnitName,
                                 j.Comment,
                                 AttDocu = string.Join(", ", _dbContext.AttHistory
                                                            .Where(a => a.PsmId == b.PsmId)
                                                            .Select(a => $"Desc: {a.Reamarks} : Docu: {a.ActFileName}")),
                                 Comments = string.Join(", ", _dbContext.StkComment
                                                            .Where(sc => sc.PsmId == b.PsmId)
                                                            .Select(sc => $"{k.UnitName} Desc: {sc.Comments} : Docu: {sc.ActFileName}")),
                                 b.Remarks,
                                 b.UpdatedByUserId,
                                 EncyId = _dataProtector.Protect(b.PsmId.ToString())
                             };

                // Assuming ProjLogView has the necessary properties
                var projLogViews = result.ToList().Select(x => new ProjLogView
                {
                    PsmId = x.PsmId,
                    ProjId = x.ProjId,
                    ProjName = x.ProjName,
                    UnitName = x.UnitName,
                    Stages = x.Stages,
                    Status = x.Status,
                    Actions = x.Actions,
                    TimeStamp = x.TimeStamp,
                    FwdBy = x.FwdBy,
                    FwdTo = x.FwdTo,
                    Comment = x.Comment,
                    AttDocu = x.AttDocu,
                    Comments = x.Comments,
                    AddRemarks = x.Remarks,
                    ActionByUser = (int)x.UpdatedByUserId,
                    EncyId = x.EncyId
                }).ToList();

                return projLogViews;
            }
            catch (Exception ex)
            {


                plvew = new List<ProjLogView>();
            }

            return plvew;
        }

        public Task<int> UpdateUndoProjectMov(int ProjectId, int PsmId)
        {
            throw new NotImplementedException();
        }

     
    }
}
