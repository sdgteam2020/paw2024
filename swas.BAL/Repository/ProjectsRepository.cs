using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System.Net.Mail;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Utility;
using static Grpc.Core.Metadata;
using Grpc.Core;
using System.Diagnostics;
using System.Threading;
using swas.UI.Helpers;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace swas.BAL.Repository
{

    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbContext _DBContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;
        private readonly IProjStakeHolderMovRepository _psmRepository;
        public ProjectsRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext DBContext, IDataProtectionProvider dataProtector, IProjStakeHolderMovRepository psmRepository)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _DBContext = DBContext;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
            _psmRepository = psmRepository;
        }
        public async Task<DTOProjectWiseStatus> GetProjectWiseStatus()
        {
            DTOProjectWiseStatus lst = new DTOProjectWiseStatus();
            var status = await (from stg in _dbContext.mStages
                                join sts in _dbContext.mStatus on stg.StagesId equals sts.StageId
                                where sts.IsDashboard==true
                                && sts.StatusId!=2 && sts.StatusId != 3 && sts.StatusId != 22
                                orderby sts.StageId, sts.Statseq
                                select new StatusProject
                                {
                                StatusId = sts.StatusId,
                                StageName=stg.Stages,
                                Status=sts.Status
                                }
                               ).ToListAsync();

            lst.StatusProjectlst = status;

            var movent = await (from proj in _dbContext.Projects
                                join mov in _dbContext.ProjStakeHolderMov on proj.ProjId equals mov.ProjId
                                select new MovProject
                                {

                                    ProjId = proj.ProjId,
                                    ProjName = proj.ProjName,
                                    TimeStamp = mov.TimeStamp,
                                    StatusId = (mov.StatusActionsMappingId == 1) ? 1 ://New Projects
                               // (mov.StatusActionsMappingId == 9) ? 2 ://Obsn
                               // (mov.StatusActionsMappingId == 113) ? 3 ://Obsn Rectified
                                (mov.StatusActionsMappingId == 48) ? 20 ://Auto Committee
                                (mov.StatusActionsMappingId == 53) ? 21 ://IPA Stage
                                //(mov.StatusActionsMappingId == 60) ? 22 ://Closed
                                (mov.StatusActionsMappingId == 68) ? 25 ://ACG (Lab Test)
                                (mov.StatusActionsMappingId == 73) ? 26 ://AHCC (IAM Integ)
                                (mov.StatusActionsMappingId == 78) ? 27 ://ACG (Remote Test)
                                (mov.StatusActionsMappingId == 83) ? 28 ://MI-11 Clearance
                                (mov.StatusActionsMappingId == 88) ? 29 ://Whitelisting Completed
                                (mov.StatusActionsMappingId == 26 && mov.IsComplete == true) ? 6 ://ASDC Vetting
                                (mov.StatusActionsMappingId == 31 && mov.IsComplete == true) ? 7 :// ACG Vetting
                                (mov.StatusActionsMappingId == 37 && mov.IsComplete == true) ? 11 : 0//AHCC Vetting

                                    //  StatusId = mov.StatusActionsMappingId==1? "Yes" : "No";
                                  
                                }).ToListAsync();
            lst.MovProjectlst = movent;

            return lst;
        }
        public async Task<List<DTOProjectsFwd>> GetDashboardApproved(int StatuId, int statusActionsMappingId)
        {
            List<DTOProjectsFwd> lst = new List<DTOProjectsFwd>();
            if (statusActionsMappingId == 26 || statusActionsMappingId == 31 || statusActionsMappingId == 37)
            {
                lst = await (from a in _dbContext.Projects
                             join mov in _dbContext.ProjStakeHolderMov on a.ProjId equals mov.ProjId
                             join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                             from stackcs in cs1.DefaultIfEmpty()
                             let datetime = (from mov1 in _dbContext.ProjStakeHolderMov
                                             join pro1 in _dbContext.Projects on mov1.ProjId equals pro1.ProjId
                                             where pro1.IsProcess == true && mov1.StatusActionsMappingId == statusActionsMappingId
                                            && pro1.ProjId == a.ProjId
                                             orderby mov1.PsmId
                                             select mov1.TimeStamp).FirstOrDefault()
                             where a.IsProcess == true && mov.StatusActionsMappingId == statusActionsMappingId
                             && mov.IsComplete==true
                             group mov by new
                             {
                                 a.ProjId,
                                 a.ProjName,
                                 a.StakeHolderId,
                                 stackcs.UnitName,
                                 datetime
                             } into gr
                             select new DTOProjectsFwd
                             {
                                 ProjId = gr.Key.ProjId,
                                 ProjName = gr.Key.ProjName,
                                 StakeHolderId = gr.Key.StakeHolderId,
                                 StakeHolder = gr.Key.UnitName,
                                 TimeStamp = gr.Key.datetime
                             }).ToListAsync();
            }
            else if (statusActionsMappingId == 1)
            {
                 lst = await (from a in _dbContext.Projects
                              join mov in _dbContext.ProjStakeHolderMov on a.ProjId equals mov.ProjId
                              join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                                  from stackcs in cs1.DefaultIfEmpty()
                              let datetime = (from mov1 in _dbContext.ProjStakeHolderMov
                                             join pro1 in _dbContext.Projects on mov1.ProjId equals pro1.ProjId
                                             where pro1.IsProcess == true && mov1.StatusActionsMappingId == statusActionsMappingId
                                            && pro1.ProjId==a.ProjId
                                              orderby mov1.PsmId
                                             select mov1.TimeStamp).FirstOrDefault()

                              where a.IsProcess==true && mov.StatusActionsMappingId == statusActionsMappingId
                              group mov by new
                              {
                                  a.ProjId,
                                  a.ProjName,
                                  a.StakeHolderId,
                                  stackcs.UnitName,
                                  datetime
                              } into gr
                              select new DTOProjectsFwd
                              {
                                  ProjId = gr.Key.ProjId,
                                  ProjName = gr.Key.ProjName,
                                  StakeHolderId = gr.Key.StakeHolderId,
                                  StakeHolder = gr.Key.UnitName,
                                  TimeStamp= gr.Key.datetime
                              }).ToListAsync();
            }
            else if (statusActionsMappingId==9|| statusActionsMappingId==15|| statusActionsMappingId==48|| 
                statusActionsMappingId==53|| statusActionsMappingId==60|| statusActionsMappingId==68||
                statusActionsMappingId==73|| statusActionsMappingId==78|| statusActionsMappingId==83|| 
                statusActionsMappingId==88)
            {
                lst = await (from a in _dbContext.Projects
                             join mov in _dbContext.ProjStakeHolderMov on a.ProjId equals mov.ProjId
                             join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                             from stackcs in cs1.DefaultIfEmpty()
                             let datetime = (from mov1 in _dbContext.ProjStakeHolderMov
                                             join pro1 in _dbContext.Projects on mov1.ProjId equals pro1.ProjId
                                             where pro1.IsProcess == true && mov1.StatusActionsMappingId == statusActionsMappingId
                                            && pro1.ProjId == a.ProjId
                                             orderby mov1.PsmId
                                             select mov1.TimeStamp).FirstOrDefault()
                             where a.IsProcess == true && mov.StatusActionsMappingId==statusActionsMappingId
                             group mov by new
                             {
                                 a.ProjId,
                                 a.ProjName,
                                 a.StakeHolderId,
                                 stackcs.UnitName,
                                 datetime
                             } into gr
                             select new DTOProjectsFwd
                             {
                                 ProjId = gr.Key.ProjId,
                                 ProjName = gr.Key.ProjName,
                                 StakeHolderId = gr.Key.StakeHolderId,
                                 StakeHolder = gr.Key.UnitName,
                                 TimeStamp = gr.Key.datetime
                             }).ToListAsync();
            }
           

            return lst;
        }
        public async Task<List<DTOProjectsFwd>> GetDashboardStatusDetails(int StatuId, int UnitId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            List<DTOProjectsFwd> lst = new List<DTOProjectsFwd>();

            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;

                var query = await (from a in _dbContext.Projects
                                   join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                                   join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                                   from stackcs in cs1.DefaultIfEmpty()
                                   join actm in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actm.StatusActionsMappingId
                                   join d in _dbContext.mStatus on actm.StatusId equals d.StatusId
                                   join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId

                                   join c in _dbContext.tbl_mUnitBranch on b.ToUnitId equals c.unitid into cs
                                   from toUnit in cs.DefaultIfEmpty()

                                   join g in _dbContext.tbl_mUnitBranch on b.FromUnitId equals g.unitid into cg
                                   from fromUnits in cg.DefaultIfEmpty()

                                   join j in _dbContext.mStages on d.StageId equals j.StagesId into js
                                   from eWithStages in js.DefaultIfEmpty()


                                   join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                                   from eWithComment in fs.DefaultIfEmpty()

                                   let StkStatusId =
                                  (from cr1 in _dbContext.StkComment
                                   join Stdkst in _dbContext.StkStatus on cr1.StkStatusId equals Stdkst.StkStatusId
                                   where cr1.StakeHolderId == b.ToUnitId && cr1.PsmId == b.PsmId
                                   orderby cr1.StkCommentId descending
                                   select cr1.StkStatusId
                                  ).FirstOrDefault()

                                  


                                   where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true //&& b.IsComplete == false
                                    //&& b.ToUnitId == Logins.unitid 
                                    && actm.StatusId == StatuId

                                   orderby a.ProjName, b.DateTimeOfUpdate descending

                                   select new DTOProjectsFwd
                                   {
                                       ProjId = a.ProjId,
                                       PsmIds = b.PsmId,
                                       ProjName = a.ProjName,
                                       StakeHolderId = a.StakeHolderId,
                                       StakeHolder = stackcs.UnitName,
                                       //Remarks= b != null ? b.Remarks : null,
                                       Status = d.Status,
                                       FromUnitId = b.FromUnitId,
                                       FromUnitName = fromUnits.UnitName,
                                       ToUnitId = b.ToUnitId,
                                       ToUnitName = toUnit.UnitName,
                                       Action = k.Actions,
                                       TotalDays = 0,
                                       StageId = eWithStages.StagesId,
                                       EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                       EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                       IsProcess = a.IsProcess,
                                       IsRead = b.IsRead,
                                       IsComplete = b.IsComplete,
                                       StkStatusId = Convert.ToInt32(StkStatusId),
                                       DateTimeOfUpdate= _dbContext.ProjStakeHolderMov.Where(i=>i.ProjId==a.ProjId).Select(x => x.DateTimeOfUpdate).Max()
                                   }).ToListAsync();

                lst = query;

                //var queryfrom = await (from a in _dbContext.Projects
                //                       join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                //                       join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                //                       from stackcs in cs1.DefaultIfEmpty()
                //                       join actm in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actm.StatusActionsMappingId
                //                       join d in _dbContext.mStatus on actm.StatusId equals d.StatusId
                //                       join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId

                //                       join c in _dbContext.tbl_mUnitBranch on b.ToUnitId equals c.unitid into cs
                //                       from toUnit in cs.DefaultIfEmpty()

                //                       join g in _dbContext.tbl_mUnitBranch on b.FromUnitId equals g.unitid into cg
                //                       from fromUnits in cg.DefaultIfEmpty()

                //                       join j in _dbContext.mStages on d.StageId equals j.StagesId into js
                //                       from eWithStages in js.DefaultIfEmpty()


                //                       join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                //                       from eWithComment in fs.DefaultIfEmpty()

                //                       where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true && b.IsComplete == true
                //                       //&& b.FromUnitId == Logins.unitid 
                //                       && actm.StatusId == StatuId
                //                       orderby a.ProjName, b.DateTimeOfUpdate descending

                //                       select new DTOProjectsFwd
                //                       {
                //                           ProjId = a.ProjId,
                //                           PsmIds = b.PsmId,
                //                           ProjName = a.ProjName,
                //                           StakeHolderId = a.StakeHolderId,
                //                           StakeHolder = stackcs.UnitName,
                //                           //Remarks= b != null ? b.Remarks : null,
                //                           Status = d.Status,
                //                           FromUnitId = b.FromUnitId,
                //                           FromUnitName = fromUnits.UnitName,
                //                           ToUnitId = b.ToUnitId,
                //                           ToUnitName = toUnit.UnitName,
                //                           Action = k.Actions,
                //                           TotalDays = 0,
                //                           StageId = eWithStages.StagesId,
                //                           EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                //                           EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                //                           IsProcess = a.IsProcess,
                //                           IsRead = b.IsRead,
                //                           IsComplete = b.IsComplete,
                //                           DateTimeOfUpdate = b.DateTimeOfUpdate
                //                       }).ToListAsync();

                //lst.AddRange(queryfrom);
            var RETT= lst.OrderByDescending(i => i.DateTimeOfUpdate).ToList();

                return RETT;/*.OrderBy(i => i.DateTimeOfUpdate).ToList();*/
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> ProjectNameExists(tbl_Projects project)
        {
            var ret =await _dbContext.Projects.AnyAsync(i => i.ProjName.ToUpper() == project.ProjName.ToUpper() && i.ProjId != project.ProjId);
            return ret;
        }
        public async Task<int> AddProjectAsync(tbl_Projects project)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");


            tbl_Comment cmt = new tbl_Comment();
            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync();

            ProjIDRes PjIR = new ProjIDRes();
            PjIR = swas.BAL.Utility.ExtensionMethods.FirstSecond(project.ProjName, project.ProjId, 0);

            if (PjIR.PorjPin == null)
            {

                project.ProjCode = "Error";

            }
            else
            {
                project.ProjCode = PjIR.PorjPin;

            }

            tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();


            psmove.ProjId = project.ProjId;
            psmove.StatusActionsMappingId = 1; //21  //ajayupdate
            //psmove.ActionId = 1;
            psmove.Remarks = project.InitialRemark;
            psmove.FromUnitId = Logins.unitid ?? 0;
            psmove.ToUnitId = 1; //  
                                 //psmove.TostackholderDt = DateTime.Now;  
            psmove.UserDetails = Helper1.LoginDetails(Logins);
            psmove.UpdatedByUserId = Logins.unitid; // change with userid
            psmove.DateTimeOfUpdate = project.InitiatedDate;
            psmove.IsActive = true;

            psmove.EditDeleteDate = project.InitiatedDate;
            psmove.EditDeleteBy = Logins.unitid;
            psmove.TimeStamp = project.InitiatedDate;
            psmove.IsComplete = false;
            psmove.IsComment = false;
            _dbContext.ProjStakeHolderMov.Add(psmove);
            await _dbContext.SaveChangesAsync();

            var projectToUpdate = await _dbContext.Projects.FirstOrDefaultAsync(a => a.ProjId == project.ProjId);
            if (projectToUpdate != null)
            {
                projectToUpdate.CurrentPslmId = psmove.PsmId;
                await _dbContext.SaveChangesAsync();
            }

            cmt.EditDeleteDate = project.InitiatedDate;
            cmt.IsDeleted = false;
            cmt.IsActive = true;
            cmt.DateTimeOfUpdate = project.InitiatedDate;
            cmt.Comment = project.InitialRemark;
            cmt.PsmId = psmove.PsmId;
            cmt.UpdatedByUserId = Logins.UserIntId;
            cmt.EditDeleteBy = 0;
            _dbContext.Comment.Add(cmt);
            await _dbContext.SaveChangesAsync();
            if (project.UploadedFile != null)
            {
                tbl_AttHistory atthis = new tbl_AttHistory();
                atthis.AttPath = project.UploadedFile;
                atthis.ActFileName = project.ActFileName;
                atthis.PsmId = psmove.PsmId;
                atthis.UpdatedByUserId = Logins.unitid;
                atthis.DateTimeOfUpdate = project.InitiatedDate;
                atthis.IsDeleted = false;
                atthis.IsActive = true;
                atthis.EditDeleteBy = Logins.unitid;
                atthis.ActionId = 1;
                atthis.TimeStamp = project.InitiatedDate;
                atthis.Reamarks = psmove.Remarks ?? "File Attached";

                _dbContext.AttHistory.Add(atthis);
                await _dbContext.SaveChangesAsync();
            }
            project.CurrentPslmId = psmove.PsmId;

            return project.ProjId;
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                var project = await _dbContext.Projects.FindAsync(projectId);
                if (project == null)
                    return false;

                _dbContext.Projects.Remove(project);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<tbl_Projects>> GetActComplettemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                var querys = from a in _dbContext.ProjStakeHolderMov
                             join b in _dbContext.Projects on a.ProjId equals b.ProjId
                             join c in _dbContext.tbl_mUnitBranch on a.ToUnitId equals c.unitid into currentStakeHolder
                             from current in currentStakeHolder.DefaultIfEmpty()
                             join d in _dbContext.tbl_mUnitBranch on a.ToUnitId equals d.unitid into toStakeHolder
                             from to in toStakeHolder.DefaultIfEmpty()
                             join e in _dbContext.tbl_mUnitBranch on a.FromUnitId equals e.unitid into fromStakeHolder
                             from fromStake in fromStakeHolder.DefaultIfEmpty()
                             join l in _dbContext.tbl_mUnitBranch on b.StakeHolderId equals l.unitid into stkhol
                             from stk in stkhol.DefaultIfEmpty()


                             join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                             from eWithComment in fs.DefaultIfEmpty()
                             join actm in _dbContext.TrnStatusActionsMapping on a.StatusActionsMappingId equals actm.StatusActionsMappingId
                             join h in _dbContext.mStatus on actm.StatusId equals h.StatusId into hs
                             from eWithStatus in hs.DefaultIfEmpty()
                             join j in _dbContext.mStages on eWithStatus.StageId equals j.StagesId into js
                             from eWithStages in js.DefaultIfEmpty()
                             join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId into ks
                             from eWithAction in ks.DefaultIfEmpty()
                             where b.IsSubmited == true
                             && actm.StatusActionsMappingId == 29 && a.IsComplete == false
                             // where a.ActionId == dft.ActionId
                             select new tbl_Projects

                             {
                                 ProjId = b.ProjId,
                                 PsmIds = a.PsmId,
                                 ProjName = b.ProjName,
                                 StakeHolderId = b.StakeHolderId,
                                 CurrentPslmId = b.CurrentPslmId,
                                 InitiatedDate = b.InitiatedDate,
                                 CompletionDate = b.CompletionDate,
                                 IsWhitelisted = b.IsWhitelisted,
                                 InitialRemark = b.InitialRemark,
                                 EditDeleteBy = a.EditDeleteBy,
                                 EditDeleteDate = a.TimeStamp,
                                 UpdatedByUserId = a.UpdatedByUserId,
                                 DateTimeOfUpdate = a.DateTimeOfUpdate,
                                 ProjCode = b.ProjCode,
                                 RecdFmUser = fromStake != null ? fromStake.UnitName : null,
                                 FwdtoUser = to != null ? to.UnitName : null,
                                 FwdBy = current != null ? current.UnitName : null,
                                 AdRemarks = a.Remarks,
                                 Comments = eWithComment.Comment,

                                 Status = eWithStatus.Status,
                                 Stages = eWithStages.Stages,
                                 Action = eWithAction.Actions,
                                 StakeHolder = stk.UnitName,

                                 AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                 HostTypeID = b.HostTypeID,
                                 EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString())



                             };


                var projectsWithDetails = await querys.ToListAsync();
                return projectsWithDetails;


            }
            else
            {
                return null;
            }
        }

        public async Task<List<tbl_Projects>> GetActDraftItemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;
            string username = Logins.UserName;

            if (Logins != null)
            {
                var querys = from a in _dbContext.ProjStakeHolderMov
                             join b in _dbContext.Projects on a.ProjId equals b.ProjId
                             join c in _dbContext.tbl_mUnitBranch on a.ToUnitId equals c.unitid into currentStakeHolder
                             from current in currentStakeHolder.DefaultIfEmpty()
                             join m in _DBContext.tbl_mUnitBranch on b.StakeHolderId equals m.unitid into stakehold
                             from stk in stakehold.DefaultIfEmpty()
                             join d in _dbContext.tbl_mUnitBranch on a.ToUnitId equals d.unitid into toStakeHolder
                             from to in toStakeHolder.DefaultIfEmpty()
                             join e in _dbContext.tbl_mUnitBranch on a.FromUnitId equals e.unitid into fromStakeHolder
                             from fromStake in fromStakeHolder.DefaultIfEmpty()
                             join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                             from eWithComment in fs.DefaultIfEmpty()
                             join actm in _dbContext.TrnStatusActionsMapping on a.StatusActionsMappingId equals actm.StatusActionsMappingId
                             join h in _dbContext.mStatus on actm.StatusId equals h.StatusId into hs
                             from eWithStatus in hs.DefaultIfEmpty()
                             join j in _dbContext.mStages on eWithStatus.StageId equals j.StagesId into js
                             from eWithStages in js.DefaultIfEmpty()
                             join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId into ks
                             from eWithAction in ks.DefaultIfEmpty()
                             where a.FromUnitId == Logins.unitid && b.IsSubmited == false && a.IsComplete == false

                             select new tbl_Projects

                             {
                                 ProjId = b.ProjId,
                                 PsmIds = a.PsmId,
                                 ProjName = b.ProjName,
                                 StakeHolderId = b.StakeHolderId,
                                 CurrentPslmId = b.CurrentPslmId,
                                 InitiatedDate = b.InitiatedDate,
                                 CompletionDate = b.CompletionDate,
                                 IsWhitelisted = b.IsWhitelisted,
                                 InitialRemark = b.InitialRemark,
                                 EditDeleteBy = a.EditDeleteBy,
                                 EditDeleteDate = a.TimeStamp,
                                 UpdatedByUserId = a.UpdatedByUserId,
                                 DateTimeOfUpdate = a.DateTimeOfUpdate,
                                 ProjCode = b.ProjCode,
                                 RecdFmUser = fromStake != null ? fromStake.UnitName : null,
                                 FwdtoUser = to != null ? to.UnitName : null,
                                 FwdBy = current != null ? current.UnitName : null,
                                 AdRemarks = a.Remarks,
                                 Comments = eWithComment.Comment,
                                 // FwdtoDate = a.TostackholderDt,
                                 Status = eWithStatus.Status,
                                 Stages = eWithStages.Stages,
                                 StakeHolder = stk.UnitName,
                                 Action = eWithAction.Actions,
                                 TotalDays = 15,
                                 //ActionCde = a.ActionCde,
                                 AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                 HostTypeID = b.HostTypeID,
                                 EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString()),

                                 EncyPsmID = _dataProtector.Protect(a.PsmId.ToString()),

                             };

                var projectsWithDetails = await querys.ToListAsync();

                return projectsWithDetails;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<DTOProjectsFwd>> GetActInboxAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;

                var query = from a in _dbContext.Projects
                            join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                            join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                            from stackcs in cs1.DefaultIfEmpty()
                            join actm in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actm.StatusActionsMappingId
                            join d in _dbContext.mStatus on actm.StatusId equals d.StatusId
                            join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId

                            join c in _dbContext.tbl_mUnitBranch on b.ToUnitId equals c.unitid into cs
                            from toUnit in cs.DefaultIfEmpty()

                            join g in _dbContext.tbl_mUnitBranch on b.FromUnitId equals g.unitid into cg
                            from fromUnits in cg.DefaultIfEmpty()

                            join j in _dbContext.mStages on d.StageId equals j.StagesId into js
                            from eWithStages in js.DefaultIfEmpty()


                            join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                            from eWithComment in fs.DefaultIfEmpty()

                            where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true && b.IsComplete == false
                            && b.ToUnitId == Logins.unitid && b.IsComment == false

                            orderby b.DateTimeOfUpdate descending

                            select new DTOProjectsFwd
                            {
                                ProjId = a.ProjId,
                                PsmIds = b.PsmId,
                                ProjName = a.ProjName,
                                StakeHolderId = a.StakeHolderId,
                                StakeHolder = stackcs.UnitName,
                                //Remarks= b != null ? b.Remarks : null,
                                Status = d.Status,
                                StatusId=d.StatusId,
                                FromUnitId = b.FromUnitId,
                               
                                ToUnitId = b.ToUnitId,
                                ToUnitName = toUnit.UnitName,
                                Action = k.Actions,
                                TotalDays = 0,

                                ActionId = k.ActionsId,

                                Sponsor = a.Sponsor,

                                //tooltip start
                                UnitName = _psmRepository.GetSponsorUnitName(a.StakeHolderId),
                                FromUnitUserDetail = fromUnits.UnitName,
                                FromUnitName = fromUnits.UnitName + " ( " + b.UserDetails + ")",
                                
                                //end

                                Stage = eWithStages.Stages,

                                StageId = eWithStages.StagesId,
                                EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                IsProcess = a.IsProcess,
                                IsRead = b.IsRead,
                                TimeStamp = b.TimeStamp
                            };

                var projectsWithDetails = await query.ToListAsync();

                return projectsWithDetails;
            }
            else
            {
                return null;
            }
        }



        public async Task<List<DTOProjectsFwd>> GetActSendItemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");


            List<DTOProjectsFwd> lst = new List<DTOProjectsFwd>();
            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;

                var query = from a in _dbContext.Projects
                            join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                            join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                            from stackcs in cs1.DefaultIfEmpty()
                            join actm in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actm.StatusActionsMappingId
                            join d in _dbContext.mStatus on actm.StatusId equals d.StatusId into ds
                            from eWithStatus in ds.DefaultIfEmpty()
                            join c in _dbContext.tbl_mUnitBranch on b.ToUnitId equals c.unitid into cs
                            from toUnit in cs.DefaultIfEmpty()

                            join g in _dbContext.tbl_mUnitBranch on b.FromUnitId equals g.unitid into cg
                            from fromUnits in cg.DefaultIfEmpty()

                            join j in _dbContext.mStages on eWithStatus.StageId equals j.StagesId into js
                            from eWithStages in js.DefaultIfEmpty()
                            join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId into ks
                            from eWithAction in ks.DefaultIfEmpty()

                            join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                            from eWithComment in fs.DefaultIfEmpty()

                            where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true && b.IsComplete == false
                            && b.FromUnitId == Logins.unitid && b.IsComment == false/* && b.StatusId != 5*/

                            orderby b.DateTimeOfUpdate descending

                            select new DTOProjectsFwd
                            {
                                ProjId = a.ProjId,
                                PsmIds = b.PsmId,
                                ProjName = a.ProjName,
                                StakeHolderId = a.StakeHolderId,
                                StakeHolder = stackcs.UnitName,
                                //Remarks = b != null ? b.Remarks : null,

                                Stage = eWithStages.Stages,

                                Sponsor = a.Sponsor,

                                //tooltip start
                                UnitName = _psmRepository.GetSponsorUnitName(a.StakeHolderId),
                                FromUnitUserDetail = fromUnits.UnitName,
                                FromUnitName = fromUnits.UnitName + " ( " + b.UserDetails + ")",
                                //end



                                Status = eWithStatus.Status,
                                FromUnitId = b.FromUnitId,
                               
                                ToUnitId = b.ToUnitId,
                                ToUnitName = toUnit.UnitName,
                                Action = eWithAction.Actions,
                                TotalDays = 0,
                                EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                IsProcess = a.IsProcess,
                                undopsmId = _psmRepository.GetLastRecProjectMov(a.ProjId),
                                StageId = eWithStages.StagesId,
                                TimeStamp = b.TimeStamp,
                                IsComplete = b.IsComplete
                            };

                var projectsWithDetails = await query.ToListAsync();
                lst = projectsWithDetails;

                var queryhist = from a in _dbContext.Projects
                                join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId

                                join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid
                                join actm in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actm.StatusActionsMappingId
                                join d in _dbContext.mStatus on actm.StatusId equals d.StatusId into ds
                                from eWithStatus in ds.DefaultIfEmpty()
                                join c in _dbContext.tbl_mUnitBranch on b.ToUnitId equals c.unitid into cs
                                from toUnit in cs.DefaultIfEmpty()

                                join g in _dbContext.tbl_mUnitBranch on b.FromUnitId equals g.unitid into cg
                                from fromUnits in cg.DefaultIfEmpty()

                                join j in _dbContext.mStages on eWithStatus.StageId equals j.StagesId into js
                                from eWithStages in js.DefaultIfEmpty()
                                join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join fromunit in _dbContext.tbl_mUnitBranch on b.FromUnitId equals fromunit.unitid
                                join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                                from eWithComment in fs.DefaultIfEmpty()

                                where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true && b.IsComplete == true
                                && b.FromUnitId == Logins.unitid && b.IsComment == false/* && b.StatusId != 5*/

                                orderby b.DateTimeOfUpdate descending

                                select new DTOProjectsFwd
                                {
                                    ProjId = a.ProjId,
                                    PsmIds = b.PsmId,
                                    ProjName = a.ProjName,
                                    Sponsor = a.Sponsor,
                                   
                                    Stage = eWithStages.Stages,
                                    
                                    UnitName = _psmRepository.GetSponsorUnitName(a.StakeHolderId),
                                    FromUnitUserDetail = fromunit.UnitName,
                                    FromUnitName = " " + fromunit.UnitName + " ( " + b.UserDetails + ")",

                                    
                                    StakeHolderId = a.StakeHolderId,
                                    StakeHolder = stackc.UnitName,
                                    //Remarks = b != null ? b.Remarks : null,
                                    Status = eWithStatus.Status,
                                    FromUnitId = b.FromUnitId,
                                    //FromUnitName = fromUnits.UnitName + " (" + b.UserDetails + ")",
                                    ToUnitId = b.ToUnitId,
                                    ToUnitName = toUnit.UnitName,
                                    Action = eWithAction.Actions,
                                    TotalDays = 0,
                                    EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                    EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                    IsProcess = a.IsProcess,
                                    undopsmId = 0,
                                    StageId = eWithStages.StagesId,
                                    TimeStamp = b.TimeStamp,
                                    IsComplete = b.IsComplete
                                };

                var history = await queryhist.ToListAsync();
                lst.AddRange(history);

                return lst;
            }
            else
            {
                return null;
            }
        }


        public async Task<List<tbl_Projects>> GetAllProjectsAsync()
        {

            return await _dbContext.Projects.ToListAsync();


        }


        public async Task<List<tbl_Projects>> GetMyProjectsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");



            if (Logins != null)
            {

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;



                var querys = from a in _dbContext.ProjStakeHolderMov
                             join b in _dbContext.Projects on a.PsmId equals b.CurrentPslmId
                             join c in _dbContext.tbl_mUnitBranch on a.ToUnitId equals c.unitid into currentStakeHolder
                             from current in currentStakeHolder.DefaultIfEmpty()
                             join m in _DBContext.tbl_mUnitBranch on b.StakeHolderId equals m.unitid into stakehold
                             from stk in stakehold.DefaultIfEmpty()
                             join d in _dbContext.tbl_mUnitBranch on a.ToUnitId equals d.unitid into toStakeHolder
                             from to in toStakeHolder.DefaultIfEmpty()
                             join e in _dbContext.tbl_mUnitBranch on a.FromUnitId equals e.unitid into fromStakeHolder
                             from fromStake in fromStakeHolder.DefaultIfEmpty()
                             join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                             from eWithComment in fs.DefaultIfEmpty()
                             join actm in _dbContext.TrnStatusActionsMapping on a.StatusActionsMappingId equals actm.StatusActionsMappingId
                             join h in _dbContext.mStatus on actm.StatusId equals h.StatusId into hs
                             from eWithStatus in hs.DefaultIfEmpty()
                             join j in _dbContext.mStages on eWithStatus.StageId equals j.StagesId into js
                             from eWithStages in js.DefaultIfEmpty()
                             join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId into ks
                             from eWithAction in ks.DefaultIfEmpty()
                             where b.StakeHolderId == Logins.unitid
                             // && a.TostackholderDt !=null

                             select new tbl_Projects

                             {
                                 ProjId = b.ProjId,
                                 ProjName = b.ProjName,
                                 StakeHolderId = b.StakeHolderId,
                                 
                                 CurrentPslmId = b.CurrentPslmId,
                                 InitiatedDate = b.InitiatedDate,
                                 CompletionDate = b.CompletionDate,
                                 IsWhitelisted = b.IsWhitelisted,
                                 InitialRemark = b.InitialRemark,
                                 EditDeleteBy = a.EditDeleteBy,
                                 EditDeleteDate = a.EditDeleteDate,
                                 UpdatedByUserId = a.UpdatedByUserId,
                                 DateTimeOfUpdate = a.DateTimeOfUpdate,
                                 ProjCode = b.ProjCode,
                                 RecdFmUser = fromStake != null ? fromStake.UnitName : null,
                                 FwdtoUser = current != null ? current.UnitName : null,

                                 FwdBy = fromStake != null ? fromStake.UnitName : null,

                                 AdRemarks = a.Remarks,
                                 Comments = eWithComment.Comment,
                                 FwdtoDate = a.TimeStamp,
                                 Status = eWithStatus.Status,
                                 Stages = eWithStages.Stages,
                                 StakeHolder = stk.UnitName,
                                 Action = eWithAction.Actions,
                                 TotalDays = EF.Functions.DateDiffDay(a.EditDeleteDate, a.TimeStamp) ?? 0,
                                 //ActionCde = a.ActionCde,
                                 AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                 HostTypeID = b.HostTypeID,
                                 EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString()),
                                 ActionId = actm.ActionsId



                             };


                var projectsWithDetails = await querys.ToListAsync();



                return projectsWithDetails;
            }
            else
            {
                return null;
            }
        }


        public async Task<tbl_Projects> GetProjectByIdAsync(int projectId)
        {


            return await _dbContext.Projects.FindAsync(projectId);
        }

        public async Task<tbl_Projects> GetProjectByIdAsync1(int? dataProjId)
        {


            return await _dbContext.Projects.FindAsync(dataProjId);
        }


        public async Task<List<ProjHistory>> GetProjectHistory(string userid)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

            if (Logins != null || stkholder != 0)
            {
                try
                {
                    var query = from p in _dbContext.ProjStakeHolderMov
                                join actm in _dbContext.TrnStatusActionsMapping on p.StatusActionsMappingId equals actm.StatusActionsMappingId
                                join b in _dbContext.mStages on actm.StatusId equals b.StagesId into stageJoin
                                from b in stageJoin.DefaultIfEmpty()
                                join c in _dbContext.mStatus on actm.StatusId equals c.StatusId into statusJoin
                                from c in statusJoin.DefaultIfEmpty()
                                join l in _dbContext.mActions on actm.ActionsId equals l.ActionsId into ActionJoin
                                from l in ActionJoin.DefaultIfEmpty()
                                join d in _dbContext.Comment on p.PsmId equals d.PsmId into commentJoin
                                from d in commentJoin.DefaultIfEmpty()
                                join proj in _dbContext.Projects on p.ProjId equals proj.ProjId into projectJoin
                                from proj in projectJoin.DefaultIfEmpty()
                                join fromSH in _dbContext.tbl_mUnitBranch on p.FromUnitId equals fromSH.unitid into fromStakeHolderJoin
                                from fromSH in fromStakeHolderJoin.DefaultIfEmpty()
                                join toSH in _dbContext.tbl_mUnitBranch on p.ToUnitId equals toSH.unitid into toStakeHolderJoin
                                from toSH in toStakeHolderJoin.DefaultIfEmpty()
                                join curSH in _dbContext.tbl_mUnitBranch on p.PsmId equals curSH.unitid into currentStakeHolderJoin
                                from curSH in currentStakeHolderJoin.DefaultIfEmpty()
                                join stakeHolderSH in _dbContext.tbl_mUnitBranch on p.FromUnitId equals stakeHolderSH.unitid into stakeHolderJoin
                                from stakeHolderSH in stakeHolderJoin.DefaultIfEmpty()
                                where proj.ProjName.Length > 1
                                orderby p.ProjId, p.PsmId
                                select new ProjHistory
                                {
                                    ProjId = proj.ProjId,
                                    PsmId = p.PsmId,
                                    ProjName = proj.ProjName,
                                    Stages = b.Stages,
                                    Status = c.Status,
                                    Comment = d.Comment,
                                    FromStakeHolder = fromSH.UnitName,
                                    ToStakeHolder = toSH.UnitName,
                                    CurrentStakeHolder = curSH.UnitName,
                                    InitiatedBy = stakeHolderSH.UnitName,
                                    TimeStamp = p.TimeStamp.HasValue ? p.TimeStamp.Value.ToString("dd-MM-yyyy") : "",
                                    InitialRemarks = p.Remarks,
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == p.PsmId),
                                    ActionName = l.Actions


                                };

                    var result = query.ToList();

                    //where proj.ProjName.Length > 1 && (p.FromStakeHolderId == stkholder || p.ToStakeHolderId == stkholder || p.CurrentStakeHolderId == stkholder)


                    //var query = from a in _dbContext.ProjStakeHolderMov
                    //            join b in _dbContext.mStages on a.StageId equals b.StagesId into stageJoin
                    //            from b in stageJoin.DefaultIfEmpty()
                    //            join c in _dbContext.mStatus on a.StatusId equals c.StatusId into statusJoin
                    //            from c in statusJoin.DefaultIfEmpty()
                    //            join d in _dbContext.Comment on a.PsmId equals d.PsmId into commentJoin
                    //            from d in commentJoin.DefaultIfEmpty()
                    //            join f in _dbContext.Projects on a.ProjId equals f.ProjId into projectJoin
                    //            from f in projectJoin.DefaultIfEmpty()
                    //            join g in _dbContext.tbl_mUnitBranch on a.FromStakeHolderId equals g.Id into fromStakeHolderJoin
                    //            from g in fromStakeHolderJoin.DefaultIfEmpty()
                    //            join h in _dbContext.tbl_mUnitBranch on a.ToStakeHolderId equals h.Id into toStakeHolderJoin
                    //            from h in toStakeHolderJoin.DefaultIfEmpty()
                    //            join i in _dbContext.tbl_mUnitBranch on a.CurrentStakeHolderId equals i.Id into currentStakeHolderJoin
                    //            from i in currentStakeHolderJoin.DefaultIfEmpty()
                    //            join j in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals j.Id into stakeHolderJoin
                    //            from j in stakeHolderJoin.DefaultIfEmpty()

                    //            orderby a.ProjId, a.PsmId
                    //            select new ProjHistory
                    //            {
                    //                ProjId = f.ProjId,
                    //                PsmId = a.PsmId,
                    //                ProjName = f.ProjName,
                    //                Stages = b.Stages,
                    //                Status = c.Status,
                    //                Comment = d.Comment,
                    //                FromStakeHolder = g.UnitName,
                    //                ToStakeHolder = h.UnitName,
                    //                CurrentStakeHolder = i.UnitName,
                    //                InitiatedBy = j.UnitName,
                    //                TimeStamp = a.TimeStamp.HasValue ? a.TimeStamp.Value.ToString("dd-MM-yyyy") : "",
                    //                InitialRemarks = a.AddRemarks,
                    //                Attachments = ""
                    //            };

                    var projHistory = await query.ToListAsync();
                    return projHistory;

                }
                catch (Exception ex)
                {

                    return null;
                }
            }
            else
            {
                return null;
            }
        }



        public async Task<List<ProjHistory>> GetProjectHistorybyID(int? dtaProjID)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

            if (Logins != null || stkholder != 0)
            {
                try
                {
                    var query = from p in _dbContext.ProjStakeHolderMov
                                join actm in _dbContext.TrnStatusActionsMapping on p.StatusActionsMappingId equals actm.StatusActionsMappingId
                                join c in _dbContext.mStatus on actm.StatusId equals c.StatusId into statusJoin
                                from c in statusJoin.DefaultIfEmpty()
                                join b in _dbContext.mStages on c.StageId equals b.StagesId into stageJoin
                                from b in stageJoin.DefaultIfEmpty()
                                join l in _dbContext.mActions on actm.ActionsId equals l.ActionsId into ActionJoin
                                from l in ActionJoin.DefaultIfEmpty()
                                join d in _dbContext.Comment on p.PsmId equals d.PsmId into commentJoin
                                from d in commentJoin.DefaultIfEmpty()
                                join proj in _dbContext.Projects on p.ProjId equals proj.ProjId into projectJoin
                                from proj in projectJoin.DefaultIfEmpty()
                                join fromSH in _dbContext.tbl_mUnitBranch on p.FromUnitId equals fromSH.unitid into fromStakeHolderJoin
                                from fromSH in fromStakeHolderJoin.DefaultIfEmpty()

                                join h in _DBContext.mHostType on proj.HostTypeID equals h.HostTypeID into hs
                                from hostType in hs.DefaultIfEmpty()

                                join i in _DBContext.mAppType on proj.Apptype equals i.Apptype into ms
                                from eWithAppType in ms.DefaultIfEmpty()

                                join toSH in _dbContext.tbl_mUnitBranch on p.ToUnitId equals toSH.unitid into toStakeHolderJoin
                                from toSH in toStakeHolderJoin.DefaultIfEmpty()
                                join curSH in _dbContext.tbl_mUnitBranch on proj.StakeHolderId equals curSH.unitid into currentStakeHolderJoin
                                from curSH in currentStakeHolderJoin.DefaultIfEmpty()
                                    // join stakeHolderSH in _dbContext.tbl_mUnitBranch on p.CurrentStakeHolderId equals stakeHolderSH.unitid into stakeHolderJoin
                                    //from stakeHolderSH in stakeHolderJoin.DefaultIfEmpty()
                                where proj.ProjName.Length > 1 && proj.ProjId == dtaProjID
                                orderby p.ProjId, p.PsmId
                                select new ProjHistory
                                {
                                    ProjId = proj.ProjId,
                                    PsmId = p.PsmId,
                                    ProjName = proj.ProjName,
                                    Stages = b.Stages,
                                    Status = c.Status,
                                    Comment = d.Comment,
                                    FromStakeHolder = fromSH.UnitName,
                                    ToStakeHolder = toSH.UnitName,
                                    // CurrentStakeHolder = stakeHolderSH.UnitName,
                                    InitiatedBy = curSH.UnitName,
                                    TimeStamp = p.TimeStamp.HasValue ? p.TimeStamp.Value.ToString("dd-MM-yyyy") : "",
                                    InitialRemarks = proj.AdRemarks,
                                    Remarks = p.Remarks,  //  work
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == p.PsmId),
                                    ActionName = l.Actions,
                                    AppDesc = eWithAppType.AppDesc,
                                    HostedOn = hostType.HostingDesc


                                };


                    var result = await query.ToListAsync();

                    return result;
                }

                catch (Exception ex)
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        public async Task<tbl_Projects> GetProjectByPsmIdAsync(int psmId)
        {
            return await _dbContext.Projects.FirstOrDefaultAsync(a => a.CurrentPslmId == psmId);
        }



        public Task<List<tbl_Projects>> GetStatusProjAsync(int statusid)
        {
            throw new NotImplementedException();
        }

        public async Task<tbl_ProjStakeHolderMov> GettXNByPsmIdAsync(int psmId)
        {
            return await _dbContext.ProjStakeHolderMov
               .FirstOrDefaultAsync(a => a.PsmId == psmId);
        }

        public async Task<bool> UpdateProjectAsync(tbl_Projects project)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {

                // tbl_ProjStakeHolderMov psmove = _dbContext.ProjStakeHolderMov.FirstOrDefault(x => x.PsmId == project.CurrentPslmId);

                _dbContext.Entry(project).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return true;



            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateTxnAsync(tbl_ProjStakeHolderMov psmov)
        {
            _dbContext.ProjStakeHolderMov.Update(psmov);
            await _dbContext.SaveChangesAsync();


            return true;
        }
        public async Task<List<tbl_Projects>> GetActProjectsAsync()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");



            if (Logins != null && Logins.Role == "Unit")
            {
                string username = Logins.UserName;

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                var projects = await (from a in _DBContext.Projects
                                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                      join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()


                                      join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted && (a.StakeHolderId == stkholder || e.ToUnitId == stkholder || e.ToUnitId == stkholder || e.ToUnitId == stkholder)
                                      && actm.ActionsId > 4
                                      orderby e.TimeStamp descending
                                      select new tbl_Projects
                                      {
                                          ProjId = a.ProjId,
                                          ProjName = a.ProjName,
                                          StakeHolderId = a.StakeHolderId,
                                          CurrentPslmId = a.CurrentPslmId,
                                          InitiatedDate = a.InitiatedDate,
                                          CompletionDate = a.CompletionDate,
                                          IsWhitelisted = a.IsWhitelisted,
                                          InitialRemark = a.InitialRemark,
                                          IsDeleted = a.IsDeleted,
                                          IsActive = a.IsActive,
                                          EditDeleteBy = a.EditDeleteBy,
                                          EditDeleteDate = a.EditDeleteDate,
                                          UpdatedByUserId = a.UpdatedByUserId,
                                          DateTimeOfUpdate = e.DateTimeOfUpdate,
                                          //ToUnitId = a.ToUnitId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          // ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();




                return projects;


            }
            else if (Logins != null && Logins.Role == "Dte")
            {
                string username = Logins.UserName;

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                var projects = await (from a in _DBContext.Projects
                                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                      join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()


                                      join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      // && e.ActionCde > 0
                                      orderby e.TimeStamp descending
                                      select new tbl_Projects
                                      {
                                          ProjId = a.ProjId,
                                          ProjName = a.ProjName,
                                          StakeHolderId = a.StakeHolderId,
                                          CurrentPslmId = a.CurrentPslmId,
                                          InitiatedDate = a.InitiatedDate,
                                          CompletionDate = a.CompletionDate,
                                          IsWhitelisted = a.IsWhitelisted,
                                          InitialRemark = a.InitialRemark,
                                          IsDeleted = a.IsDeleted,
                                          IsActive = a.IsActive,
                                          EditDeleteBy = a.EditDeleteBy,
                                          EditDeleteDate = a.EditDeleteDate,
                                          UpdatedByUserId = a.UpdatedByUserId,
                                          DateTimeOfUpdate = e.DateTimeOfUpdate,
                                          // ToUnitId = a.ToUnitId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          // ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();


                //var projects = await (from a in _DBContext.Projects
                //                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                //                      from e in bs.DefaultIfEmpty()
                //                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                //                      from eWithStatus in ds.DefaultIfEmpty()
                //                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                //                      from eWithUnit in cs.DefaultIfEmpty()


                //                      join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                //                      from curstk in csh.DefaultIfEmpty()

                //                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                //                      from eWithComment in fs.DefaultIfEmpty()
                //                      where a.IsActive && !a.IsDeleted
                //                      && e.ActionCde > 0
                //                      orderby e.TimeStamp descending
                //                      select new tbl_Projects
                //                      {
                //                          ProjId = a.ProjId,
                //                          ProjName = a.ProjName,
                //                          StakeHolderId = a.StakeHolderId,
                //                          CurrentPslmId = a.CurrentPslmId,
                //                          InitiatedDate = a.InitiatedDate,
                //                          CompletionDate = a.CompletionDate,
                //                          IsWhitelisted = a.IsWhitelisted,
                //                          InitialRemark = a.InitialRemark,
                //                          IsDeleted = a.IsDeleted,
                //                          IsActive = a.IsActive,
                //                          EditDeleteBy = a.EditDeleteBy,
                //                          EditDeleteDate = a.EditDeleteDate,
                //                          UpdatedByUserId = a.UpdatedByUserId,
                //                          DateTimeOfUpdate = e.DateTimeOfUpdate,
                //                          ToUnitId = a.ToUnitId,
                //                          StakeHolder = eWithUnit.UnitName,
                //                          FwdtoUser = curstk.UnitName,
                //                          Status = eWithStatus.Status,
                //                          Comments = eWithComment.Comment,
                //                          ActionCde = e.ActionCde,
                //                          AimScope = a.AimScope,
                //                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId),
                //                          DynamicColumns = _DBContext.tbl_mUnitBranch
                //                                              .Where(u => u.commentreqdid && u.unitid == a.StakeHolderId)
                //                                              .Select(u => new UnitDtl
                //                                              {
                //                                                  unitid = u.unitid,
                //                                                  UnitName = u.UnitName,
                //                                                  commentreqdid = u.commentreqdid,
                //                                                  Remarks = _DBContext.PorjIniStat
                //                                                              .Where(p => p.StakeHolderId == u.unitid && p.psmid == a.CurrentPslmId)
                //                                                              .Select(p => p.Remarks)
                //                                                              .FirstOrDefault(),
                //                                                  InitialStatus = _DBContext.PorjIniStat
                //                                                                  .Where(p => p.StakeHolderId == u.unitid && p.psmid == a.CurrentPslmId)
                //                                                                  .Select(p => p.InitialStatus)
                //                                                                  .FirstOrDefault()
                //                                              })
                //                                              .ToList()
                //                      }).ToListAsync();


                return projects;


            }
            else
            {

                return null;
            }





            //var query = from a in _dbContext.Projects
            //            join e in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals e.PsmId
            //            join d in _dbContext.mStatus on e.StatusId equals d.StatusId into ds
            //            from eWithStatus in ds.DefaultIfEmpty()
            //            join c in _dbContext.mStakeHolder on e.ToUnitId equals c.StakeHolderId into cs
            //            from eWithStakeHolder in cs.DefaultIfEmpty()
            //            join msh in _dbContext.mStakeHolder on e.ToUnitId equals msh.StakeHolderId into mshs
            //            from eWithMStakeHolder in mshs.DefaultIfEmpty()
            //            where a.IsActive && !a.IsDeleted



            //select new tbl_Projects
            //            {
            //                ProjId = a.ProjId,
            //                ProjName = a.ProjName,
            //                StakeHolderId = a.StakeHolderId,
            //                CurrentPslmId = a.CurrentPslmId,
            //                InitiatedDate = a.InitiatedDate,
            //                CompletionDate = a.CompletionDate,
            //                IsWhitelisted = a.IsWhitelisted,
            //                InitialRemark = a.InitialRemark,
            //                IsDeleted = a.IsDeleted,
            //                IsActive = a.IsActive,
            //                EditDeleteBy = a.EditDeleteBy,
            //                EditDeleteDate = a.EditDeleteDate,
            //                UpdatedByUserId = a.UpdatedByUserId,
            //                DateTimeOfUpdate = e.DateTimeOfUpdate,
            //                ToUnitId = a.ToUnitId,
            //                StakeHolder = eWithStakeHolder.StakeHolder,
            //                Status = eWithStatus.Status

            //            };

            //var projectsWithDetails = await query.ToListAsync();

            //return projectsWithDetails;

        }
        public async Task<List<DToWhiteListed>> GetWhiteListedActionProj()
        {


            //var currentDateMinus15Days = DateTime.Now.AddDays(-30);
            var results = (from a in _dbContext.trnWhiteListed
                           join b in _dbContext.mWhiteListedHeader on a.HeaderID equals b.Id
                           orderby a.HeaderID descending
                           select new DToWhiteListed
                           {
                               Id = a.Id,
                               HeaderID = b.Id,
                               ProjName = a.ProjName,
                               Appt = a.Appt,
                               Fmn = a.Fmn,
                               ContactNo = a.ContactNo,
                               Clearence = a.Clearence == null ? string.Empty : ((DateTime)a.Clearence).ToString("dd/MM/yy"),
                               CertNo = a.CertNo,
                               date = a.date == null ? string.Empty : ((DateTime)a.date).ToString("dd/MM/yy"),
                               ValidUpto = a.ValidUpto == null ? string.Empty : ((DateTime)a.ValidUpto).ToString("dd/MM/yy"),
                               Remarks = a.Remarks,
                               Header = b.Header
                           }).ToList();


            return results;

            // 'results' contains the list of TimeExceeds objects with the specified conditions.
            // Actions = "YourActionsValue", 
            //TimeLimit = b.TimeLimit, // Assuming TimeLimit is a property in ProjStakeHolderMov
            //                   dayss = 15, // You can set this to 15 as it is a constant value
            //                   exceeds = (b.TimeLimit - 15) // Calculation for exceeds
        }
        public async Task<List<tbl_Projects>> GetProcProjAsync()
        {
            bool? cmtreqd = false;

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {

                cmtreqd = await _DBContext.tbl_mUnitBranch
                 .Where(a => a.unitid == Logins.unitid)
                 .Select(b => b.commentreqdid)
                 .FirstOrDefaultAsync();

            }

            if (Logins != null && Logins.Role == "Unit")
            {
                string username = Logins.UserName;

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                //   change the query

                var projects = await (from a in _DBContext.Projects
                                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                      join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join h in _DBContext.mHostType on a.HostTypeID equals h.HostTypeID into hs
                                      from hostType in hs.DefaultIfEmpty()
                                      join i in _DBContext.mAppType on a.Apptype equals i.Apptype into ms
                                      from eWithAppType in ms.DefaultIfEmpty()
                                      join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted && (a.StakeHolderId == stkholder || e.FromUnitId == stkholder || e.ToUnitId == stkholder || e.ToUnitId == stkholder)
                                     && actm.StatusId == 4
                                      orderby e.TimeStamp descending
                                      select new tbl_Projects
                                      {
                                          ProjId = a.ProjId,
                                          ProjName = a.ProjName,
                                          StakeHolderId = a.StakeHolderId,
                                          CurrentPslmId = a.CurrentPslmId,
                                          InitiatedDate = a.InitiatedDate,
                                          CompletionDate = a.CompletionDate,
                                          IsWhitelisted = a.IsWhitelisted,
                                          InitialRemark = a.InitialRemark,
                                          IsDeleted = a.IsDeleted,
                                          IsActive = a.IsActive,
                                          EditDeleteBy = a.EditDeleteBy,
                                          EditDeleteDate = a.EditDeleteDate,
                                          UpdatedByUserId = a.UpdatedByUserId,
                                          DateTimeOfUpdate = e.DateTimeOfUpdate,
                                          //ToUnitId = a.ToUnitId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          //ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          ReqmtJustification = a.ReqmtJustification,
                                          Deplytype = eWithAppType.AppDesc,
                                          Hostedon = hostType.HostingDesc,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();

                return projects;

            }

            else if (Logins != null && Logins.Role == "Dte" && cmtreqd == true)

            {
                string username = Logins.UserName;

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                var projects = await (from a in _DBContext.Projects
                                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                      join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join h in _DBContext.mHostType on a.HostTypeID equals h.HostTypeID into hs
                                      from hostType in hs.DefaultIfEmpty()

                                      join i in _DBContext.mAppType on a.Apptype equals i.Apptype into ms
                                      from eWithAppType in ms.DefaultIfEmpty()


                                      join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      //&& e.ActionCde > 0 
                                      && actm.StatusId == 4
                                      orderby e.TimeStamp descending
                                      select new tbl_Projects
                                      {
                                          ProjId = a.ProjId,
                                          ProjName = a.ProjName,
                                          StakeHolderId = a.StakeHolderId,
                                          CurrentPslmId = a.CurrentPslmId,
                                          InitiatedDate = a.InitiatedDate,
                                          CompletionDate = a.CompletionDate,
                                          IsWhitelisted = a.IsWhitelisted,
                                          InitialRemark = a.InitialRemark,
                                          IsDeleted = a.IsDeleted,
                                          IsActive = a.IsActive,
                                          EditDeleteBy = a.EditDeleteBy,
                                          EditDeleteDate = a.EditDeleteDate,
                                          UpdatedByUserId = a.UpdatedByUserId,
                                          DateTimeOfUpdate = e.DateTimeOfUpdate,
                                          //ToUnitId = a.ToUnitId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          //ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          ReqmtJustification = a.ReqmtJustification,
                                          Deplytype = eWithAppType.AppDesc,
                                          Hostedon = hostType.HostingDesc,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();

                return projects;


            }
            else
            {
                var projects = await (from a in _DBContext.Projects
                                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                      join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join h in _DBContext.mHostType on a.HostTypeID equals h.HostTypeID into hs
                                      from hostType in hs.DefaultIfEmpty()
                                      join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join i in _DBContext.mAppType on a.Apptype equals i.Apptype into ms
                                      from eWithAppType in ms.DefaultIfEmpty()


                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      //&& e.ActionCde > 9999999 
                                      && actm.StatusId == 9999999
                                      orderby e.TimeStamp descending
                                      select new tbl_Projects
                                      {
                                          ProjId = a.ProjId,
                                          ProjName = a.ProjName,
                                          StakeHolderId = a.StakeHolderId,
                                          CurrentPslmId = a.CurrentPslmId,
                                          InitiatedDate = a.InitiatedDate,
                                          CompletionDate = a.CompletionDate,
                                          IsWhitelisted = a.IsWhitelisted,
                                          InitialRemark = a.InitialRemark,
                                          IsDeleted = a.IsDeleted,
                                          IsActive = a.IsActive,
                                          EditDeleteBy = a.EditDeleteBy,
                                          EditDeleteDate = a.EditDeleteDate,
                                          UpdatedByUserId = a.UpdatedByUserId,
                                          DateTimeOfUpdate = e.DateTimeOfUpdate,
                                          //ToUnitId = a.ToUnitId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          //ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          ReqmtJustification = a.ReqmtJustification,
                                          Deplytype = eWithAppType.AppDesc,
                                          Hostedon = hostType.HostingDesc,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();

                return projects;
            }

        }

        public async Task<List<tbl_Projects>> GetProjforCommentsAsync()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            //if (Logins != null && Logins.Role == "Unit")
            //{
            string username = Logins.UserName;

            int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

            var projects = await (from a in _DBContext.Projects
                                  join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                  from e in bs.DefaultIfEmpty()
                                  join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                  join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                  from eWithStatus in ds.DefaultIfEmpty()
                                  join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                  from eWithUnit in cs.DefaultIfEmpty()
                                  join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                  from curstk in csh.DefaultIfEmpty()

                                  join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                  from eWithComment in fs.DefaultIfEmpty()

                                  join i in _DBContext.mAppType on a.Apptype equals i.Apptype into ms
                                  from eWithAppType in ms.DefaultIfEmpty()

                                  where a.IsActive && !a.IsDeleted && a.StakeHolderId == Logins.unitid
                                  //&& e.ActionCde > 0 
                                  //&& e.ActionId > 4
                                  orderby e.TimeStamp descending
                                  select new tbl_Projects
                                  {
                                      ProjId = a.ProjId,
                                      ProjName = a.ProjName,
                                      StakeHolderId = a.StakeHolderId,
                                      CurrentPslmId = a.CurrentPslmId,
                                      InitiatedDate = a.InitiatedDate,
                                      CompletionDate = a.CompletionDate,
                                      IsWhitelisted = a.IsWhitelisted,
                                      InitialRemark = a.InitialRemark,
                                      IsDeleted = a.IsDeleted,
                                      IsActive = a.IsActive,
                                      EditDeleteBy = a.EditDeleteBy,
                                      EditDeleteDate = a.EditDeleteDate,
                                      UpdatedByUserId = a.UpdatedByUserId,
                                      DateTimeOfUpdate = e.DateTimeOfUpdate,
                                      //ToUnitId = a.ToUnitId,
                                      StakeHolder = eWithUnit.UnitName,
                                      FwdtoUser = curstk.UnitName,
                                      Status = eWithStatus.Status,
                                      Comments = eWithComment.Comment,
                                      Deplytype = eWithAppType.AppDesc,
                                      //ActionCde = e.ActionCde,
                                      AimScope = a.AimScope,
                                      ReqmtJustification = a.ReqmtJustification,
                                      EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                      AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                  }).ToListAsync();


            return projects;


            //}

        }




        public async Task<List<tbl_Projects>> GetProjforCommentsAsync1()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            //if (Logins != null && Logins.Role == "Unit")
            //{
            string username = Logins.UserName;

            int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

            var projects = await (from a in _DBContext.Projects
                                  join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                  from e in bs.DefaultIfEmpty()
                                  join actm in _dbContext.TrnStatusActionsMapping on e.StatusActionsMappingId equals actm.StatusActionsMappingId
                                  join d in _DBContext.mStatus on actm.StatusId equals d.StatusId into ds
                                  from eWithStatus in ds.DefaultIfEmpty()
                                  join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                  from eWithUnit in cs.DefaultIfEmpty()
                                  join g in _DBContext.tbl_mUnitBranch on e.ToUnitId equals g.unitid into csh
                                  from curstk in csh.DefaultIfEmpty()

                                  join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                  from eWithComment in fs.DefaultIfEmpty()
                                  where a.IsActive && !a.IsDeleted && a.StakeHolderId == Logins.unitid
                                  //&& e.ActionCde > 0 
                                  //  && actm.ActionsId > 4
                                  orderby e.TimeStamp descending
                                  select new tbl_Projects
                                  {
                                      ProjId = a.ProjId,
                                      ProjName = a.ProjName,
                                      StakeHolderId = a.StakeHolderId,
                                      CurrentPslmId = a.CurrentPslmId,
                                      InitiatedDate = a.InitiatedDate,
                                      CompletionDate = a.CompletionDate,
                                      IsWhitelisted = a.IsWhitelisted,
                                      InitialRemark = a.InitialRemark,
                                      IsDeleted = a.IsDeleted,
                                      IsActive = a.IsActive,
                                      EditDeleteBy = a.EditDeleteBy,
                                      EditDeleteDate = a.EditDeleteDate,
                                      UpdatedByUserId = a.UpdatedByUserId,
                                      DateTimeOfUpdate = e.DateTimeOfUpdate,
                                      //ToUnitId = a.ToUnitId,
                                      StakeHolder = eWithUnit.UnitName,
                                      FwdtoUser = curstk.UnitName,
                                      Status = eWithStatus.Status,
                                      Comments = eWithComment.Comment,
                                      //ActionCde = e.ActionCde,
                                      AimScope = a.AimScope,
                                      ReqmtJustification = a.ReqmtJustification,
                                      EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                      AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                  }).ToListAsync();


            return projects;


            //}

        }

        public async Task<List<DTOUnderProcessProj>> GetHoldActionProj()
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

                               where !b.IsComplete && !b.IsComment && !b.IsDeleted

                               orderby b.TimeStamp descending
                               select new DTOUnderProcessProj
                               {
                                   ProjName = a.ProjName,
                                   StatusName = ststus.Status,
                                   Sponser = a.Sponsor,
                                   StageName = stge.Stages,
                                   ActionName = act.Actions,

                                   //tooltip start
                                   UnitName = _psmRepository.GetSponsorUnitName(a.StakeHolderId),
                                   FromUnitUserDetail = fromunit.UnitName,
                                   FromUnitName = " " + fromunit.UnitName + " ( " + b.UserDetails + ")",
                                   Remark = b.Remarks,
                                   //end

                                   ToUnitName = tounit.UnitName,

                                   DateTimeOfUpdate = b.TimeStamp,


                               }).ToListAsync();

            return query;

        }

       
    }

    }
