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

        public ProjectsRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ApplicationDbContext DBContext, IDataProtectionProvider dataProtector)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _DBContext = DBContext;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");

        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<int> AddProjectAsync(tbl_Projects project)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            DefaultValueID dft = new DefaultValueID();
            dft.ActionId = _dbContext.mActions
                        .Where(action => action.InitiaalID == true)
                        .Select(action => action.ActionsId)
                        .FirstOrDefault();

            dft.StatusId = _dbContext.mStatus
                .Where(stat => stat.InitiaalID == true)
                .Select(stat => stat.StatusId)
                .FirstOrDefault();

            dft.StageID = _dbContext.mStages
                .Where(stg => stg.InitiaalID == true)
                .Select(stat => stat.StagesId)
                .FirstOrDefault();



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
            psmove.StageId = dft.StageID;
            psmove.StatusId = dft.StatusId;
            psmove.ActionId = dft.ActionId;
            psmove.AddRemarks = project.InitialRemark;
            psmove.FromStakeHolderId = Logins.unitid ?? 0;
            psmove.ToStakeHolderId = 0; //  
            //psmove.TostackholderDt = DateTime.Now;  
            psmove.CurrentStakeHolderId = 1; // 1 is for DDGIT..  This is for automatically fwd to DDGIT.
            psmove.CommentId = 0;
            psmove.StakeHolderId = project.StakeHolderId;
            psmove.UpdatedByUserId = Logins.unitid; // change with userid
            psmove.DateTimeOfUpdate = DateTime.Now;
            psmove.IsActive = true;
            psmove.ActionCde = 0;
            psmove.EditDeleteDate = DateTime.Now;
            psmove.EditDeleteBy = Logins.unitid; 
            psmove.TimeStamp = DateTime.Now;
            _dbContext.ProjStakeHolderMov.Add(psmove);
            await _dbContext.SaveChangesAsync();

            var projectToUpdate = await _dbContext.Projects.FirstOrDefaultAsync(a => a.ProjId == project.ProjId);
            if (projectToUpdate != null)
            {
                projectToUpdate.CurrentPslmId = psmove.PsmId;
                await _dbContext.SaveChangesAsync();
            }

            cmt.EditDeleteDate = DateTime.Now;
            cmt.IsDeleted = false;
            cmt.IsActive = true;
            cmt.DateTimeOfUpdate = DateTime.Now;
            cmt.Comment = project.InitialRemark;
            cmt.PsmId = psmove.PsmId;
            cmt.UpdatedByUserId = Logins.unitid;
            cmt.EditDeleteBy = Logins.unitid;
            _dbContext.Comment.Add(cmt);
            await _dbContext.SaveChangesAsync();
            if (project.UploadedFile != null)
            {
                tbl_AttHistory atthis = new tbl_AttHistory();
                atthis.AttPath = project.UploadedFile;
                atthis.ActFileName = project.ActFileName;
                atthis.PsmId = psmove.PsmId;
                atthis.UpdatedByUserId = Logins.unitid;
                atthis.DateTimeOfUpdate = DateTime.Now;
                atthis.IsDeleted = false;
                atthis.IsActive = true;
                atthis.EditDeleteBy = Logins.unitid;
                atthis.ActionId = dft.ActionId;
                atthis.TimeStamp = DateTime.Now;
                atthis.Reamarks = psmove.AttRemarks ?? "File Attached";

                _dbContext.AttHistory.Add(atthis);
                await _dbContext.SaveChangesAsync();
            }
            project.CurrentPslmId = psmove.PsmId;

            return project.ProjId;
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<tbl_Projects> GetProjectByIdAsync(int projectId)
        {


            return await _dbContext.Projects.FindAsync(projectId);
        }

        public async Task<tbl_Projects> GetProjectByIdAsync1(int dataProjId)
        {


            return await _dbContext.Projects.FindAsync(dataProjId);
        }



        public async Task<List<tbl_Projects>> GetProjByIdLstAsync(int projectId)
        {
            List<tbl_Projects> tblproj = new List<tbl_Projects>();
            tblproj = await _dbContext.Projects.Where(a => a.ProjId == projectId).
                ToListAsync();

            return tblproj;
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<List<tbl_Projects>> GetAllProjectsAsync()
        {

            return await _dbContext.Projects.ToListAsync();


        }

        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23


        public bool VerifyProjectNameAsync(string projname)
        {

            return _dbContext.Projects.Any(e => e.ProjName == projname);

        }


        public async Task<List<NextActionsDto>> NextActionGet(int projid)
        {

            var query = (from f in _dbContext.mActions
                         join e in _dbContext.mStatus on f.StatusId equals e.StatusId
                         join g in _dbContext.mStages on e.StageId equals g.StagesId
                         where f.Actionseq >
                             (from a in _dbContext.Projects
                              join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId
                              join c in _dbContext.mStatus on b.StatusId equals c.StatusId
                              where b.StageId < 4 && a.ProjId == projid
                              orderby b.ActionId
                              select b.ActionId).Take(1).FirstOrDefault()
                             && e.StageId < 4
                         orderby g.StagesId, f.StatusId
                         select new NextActionsDto
                         {
                             ActionsId = f.ActionsId,
                             StatusId = f.StatusId,
                             Stages = g.Stages,
                             Actions = g.Stages + "-----" + e.Status + "----" + f.Actions,
                             NextAction = f.ActionsId.ToString() + "," + f.StatusId.ToString()
                         }).Take(4);

            var nextactionlist = await query.ToListAsync();

            return nextactionlist;

        }










        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 11 Sep 23 & 3 Oct 23

        public async Task<List<TimeExceeds>> DelayedProj()
        {
            var query = from a in _dbContext.mActions
                        join b in _dbContext.ProjStakeHolderMov on a.ActionsId equals b.ActionId
                        join c in _dbContext.Projects on b.PsmId equals c.CurrentPslmId
                        join d in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals d.unitid into dGroup
                        from d in dGroup.DefaultIfEmpty()
                        join e in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals e.unitid into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        join f in _dbContext.Comment on b.PsmId equals f.PsmId into fGroup
                        from f in fGroup.DefaultIfEmpty()
                        where c.ProjId > 0 && a.TimeLimit > 0 && a.TimeLimit < EF.Functions.DateDiffDay(b.EditDeleteDate, DateTime.Now) && d.unitid < 6
                        select new TimeExceeds
                        {
                            ProjId = c.ProjId,
                            psmid = b.PsmId,
                            ProjName = c.ProjName,
                            helwith = d.UnitName,
                            fwdby = e.UnitName,
                            Actions = a.Actions,
                            Comment = f.Comment,
                            EditDeleteDate = b.EditDeleteDate,
                            TimeLimit = a.TimeLimit,
                            dayss = EF.Functions.DateDiffDay(b.EditDeleteDate, DateTime.Now),
                            exceeds = EF.Functions.DateDiffDay(b.EditDeleteDate, DateTime.Now) - a.TimeLimit,

                        };

            var delayedproj = await query.ToListAsync();

            return delayedproj;

        }

        ///Created and Reviewed by : Sub Maj M Sanal Kumar on 09 Nov 23
        // Reviewed Date : 10,11 & 12 Nov  23
        // purpose :   Stage validations for _pfwdupload //  user reqmt by ACG


        public async Task<string> ValidateActionSel(int? ProjID, int? ActionId, int? StatusId)
        {

            var sql = @"  SELECT 'Clearance awaited..!' + STRING_AGG(' ' + g.Status + ' awaited for ' +f.actions, ', ') + ' Action '  AS Result
FROM mActionmapping e
LEFT JOIN mActions f ON e.mandatoryactionid = f.ActionsId
left join mStatus g on f.StatusId=g.StatusId
WHERE e.mandatoryactionid NOT IN (
    SELECT a.ActionId
    FROM ProjStakeHolderMov a
    LEFT JOIN Projects b ON a.ProjId = b.ProjId
    WHERE b.ProjId = " + ProjID + @"
) and 
e.statusid=" + StatusId;

            Resultss rslt = new Resultss();

            var result = await _DBContext.Resultstr.FromSqlRaw<Resultss>(sql).ToListAsync();
            if (result != null)
            {
                rslt.Result = result[0].Result;
                if (rslt.Result == null)
                {
                    rslt.Result = "Ok";
                }


            }
            else
            {
                rslt.Result = "Ok";
            }



            //       var projStakeHolderMovIds = _dbContext.ProjStakeHolderMov
            //.Where(a => a.ProjId == 340)
            //.Select(a => a.ActionId)
            //.ToList();

            //       var pendingActions = (
            //           from e in _dbContext.mActionmapping
            //           join f in _dbContext.mActions on e.MandatoryActionId equals f.ActionsId
            //           join g in _dbContext.mStatus on f.StatusId equals g.StatusId
            //           where !projStakeHolderMovIds.Contains(e.MandatoryActionId??0)
            //           group new { g, f } by 1 into grouped
            //           select string.Join(", ", grouped.Select(x => $"Status: {x.g.Status} & Action: {x.f.Actions}"))
            //       ) + " Pending";

            //       // pendingActions now contains the concatenated string similar to your SQL query
            //       var pendingActionss = pendingActions.FirstOrDefault();

            //       var concatenatedActions = string.Join(", ", pendingActionss) + " Pending";

            return rslt.Result.ToString();
        }




        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 26 Jul 23
        public async Task<List<tbl_Projects>> GetActInboxAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");



            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;

                var query = from a in _dbContext.Projects
                            join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                            from e in bs.DefaultIfEmpty()
                            join d in _dbContext.mStatus on e.StatusId equals d.StatusId into ds
                            from eWithStatus in ds.DefaultIfEmpty()
                            join c in _dbContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                            from eWithUnit in cs.DefaultIfEmpty()

                            join g in _dbContext.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                            from eWithUnits in cg.DefaultIfEmpty()

                            join j in _dbContext.mStages on e.StageId equals j.StagesId into js
                            from eWithStages in js.DefaultIfEmpty()
                            join k in _dbContext.mActions on e.ActionId equals k.ActionsId into ks
                            from eWithAction in ks.DefaultIfEmpty()

                            join f in _dbContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                            from eWithComment in fs.DefaultIfEmpty()

                            where a.IsActive && !a.IsDeleted && e.CurrentStakeHolderId == Logins.unitid && e.ActionCde > 0 && e.ActionId != 48 // ** san

                            orderby e.DateTimeOfUpdate descending

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
                                EditDeleteDate = e.TimeStamp,
                                UpdatedByUserId = a.UpdatedByUserId,
                                DateTimeOfUpdate = e.DateTimeOfUpdate,
                                CurrentStakeHolderId = a.CurrentStakeHolderId,
                                StakeHolder = eWithUnit.UnitName,
                                Status = eWithStatus.Status,
                                Comments = eWithComment.Comment,
                                RecdFmUser = eWithUnits.UnitName,
                                Stages = eWithStages.Stages,
                                Action = eWithAction.Actions,
                                TotalDays = EF.Functions.DateDiffDay(e.TimeStamp, DateTime.Now),
                                ActionCde = e.ActionCde,
                                AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId),
                                EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                ActionId = eWithAction.ActionsId,
                                EncyPsmID = _dataProtector.Protect(a.CurrentPslmId.ToString())


                            };

                var projectsWithDetails = await query.ToListAsync();

                return projectsWithDetails;
            }
            else
            {
                return null;
            }
        }

        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 26 Jul 23
        public async Task<List<tbl_Projects>> GetActSendItemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");



            if (Logins != null)
            {

                //int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                // string username = Logins.UserName;



                var querys = from a in _dbContext.ProjStakeHolderMov
                             join b in _dbContext.Projects on a.ProjId equals b.ProjId
                             join c in _dbContext.tbl_mUnitBranch on a.CurrentStakeHolderId equals c.unitid into currentStakeHolder
                             from current in currentStakeHolder.DefaultIfEmpty()
                             join m in _DBContext.tbl_mUnitBranch on b.StakeHolderId equals m.unitid into stakehold
                             from stk in stakehold.DefaultIfEmpty()
                             join d in _dbContext.tbl_mUnitBranch on a.ToStakeHolderId equals d.unitid into toStakeHolder
                             from to in toStakeHolder.DefaultIfEmpty()
                             join e in _dbContext.tbl_mUnitBranch on a.FromStakeHolderId equals e.unitid into fromStakeHolder
                             from fromStake in fromStakeHolder.DefaultIfEmpty()
                             join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                             from eWithComment in fs.DefaultIfEmpty()
                             join h in _dbContext.mStatus on a.StatusId equals h.StatusId into hs
                             from eWithStatus in hs.DefaultIfEmpty()
                             join j in _dbContext.mStages on a.StageId equals j.StagesId into js
                             from eWithStages in js.DefaultIfEmpty()
                             join k in _dbContext.mActions on a.ActionId equals k.ActionsId into ks
                             from eWithAction in ks.DefaultIfEmpty()
                             where a.FromStakeHolderId == Logins.unitid && a.ActionCde > 0 && a.ActionId != 48
                             orderby a.EditDeleteDate descending
                             // ** san
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

                                 AdRemarks = a.AddRemarks,
                                 Comments = eWithComment.Comment,
                                 FwdtoDate = a.TimeStamp,
                                 Status = eWithStatus.Status,
                                 Stages = eWithStages.Stages,
                                 StakeHolder = stk.UnitName,
                                 Action = eWithAction.Actions,
                                 TotalDays = EF.Functions.DateDiffDay(a.EditDeleteDate, a.TimeStamp) ?? 0,
                                 ActionCde = a.ActionCde,
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



        public async Task<List<tbl_Projects>> GetMyProjectsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");



            if (Logins != null)
            {

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;



                var querys = from a in _dbContext.ProjStakeHolderMov
                             join b in _dbContext.Projects on a.PsmId equals b.CurrentPslmId
                             join c in _dbContext.tbl_mUnitBranch on a.CurrentStakeHolderId equals c.unitid into currentStakeHolder
                             from current in currentStakeHolder.DefaultIfEmpty()
                             join m in _DBContext.tbl_mUnitBranch on b.StakeHolderId equals m.unitid into stakehold
                             from stk in stakehold.DefaultIfEmpty()
                             join d in _dbContext.tbl_mUnitBranch on a.ToStakeHolderId equals d.unitid into toStakeHolder
                             from to in toStakeHolder.DefaultIfEmpty()
                             join e in _dbContext.tbl_mUnitBranch on a.FromStakeHolderId equals e.unitid into fromStakeHolder
                             from fromStake in fromStakeHolder.DefaultIfEmpty()
                             join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                             from eWithComment in fs.DefaultIfEmpty()
                             join h in _dbContext.mStatus on a.StatusId equals h.StatusId into hs
                             from eWithStatus in hs.DefaultIfEmpty()
                             join j in _dbContext.mStages on a.StageId equals j.StagesId into js
                             from eWithStages in js.DefaultIfEmpty()
                             join k in _dbContext.mActions on a.ActionId equals k.ActionsId into ks
                             from eWithAction in ks.DefaultIfEmpty()
                             where b.StakeHolderId == Logins.unitid && a.ActionCde > 0 && a.ActionCde < 90
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

                                 AdRemarks = a.AddRemarks,
                                 Comments = eWithComment.Comment,
                                 FwdtoDate = a.TimeStamp,
                                 Status = eWithStatus.Status,
                                 Stages = eWithStages.Stages,
                                 StakeHolder = stk.UnitName,
                                 Action = eWithAction.Actions,
                                 TotalDays = EF.Functions.DateDiffDay(a.EditDeleteDate, a.TimeStamp) ?? 0,
                                 ActionCde = a.ActionCde,
                                 AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                 HostTypeID = b.HostTypeID,
                                 EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString()),
                                 ActionId = a.ActionId



                             };


                var projectsWithDetails = await querys.ToListAsync();



                return projectsWithDetails;
            }
            else
            {
                return null;
            }
        }





        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 11 Sep 23 & 3 Oct 23


        public async Task<List<tbl_Projects>> GetActComplettemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                DefaultValueID dft = new DefaultValueID();
                dft.ActionId = _dbContext.mActions
                            .Where(action => action.FininshID == true)
                            .Select(action => action.ActionsId)
                            .FirstOrDefault();

                dft.StatusId = _dbContext.mStatus
                    .Where(stat => stat.FininshID == true)
                    .Select(stat => stat.StatusId)
                    .FirstOrDefault();

                dft.StageID = _dbContext.mStages
                    .Where(stg => stg.FininshID == true)
                    .Select(stat => stat.StagesId)
                    .FirstOrDefault();

                string username = Logins.UserName;



                if (Logins.Role == "Dte")
                {
                    var querys = from a in _dbContext.ProjStakeHolderMov
                                 join b in _dbContext.Projects on a.ProjId equals b.ProjId
                                 join c in _dbContext.tbl_mUnitBranch on a.CurrentStakeHolderId equals c.unitid into currentStakeHolder
                                 from current in currentStakeHolder.DefaultIfEmpty()
                                 join d in _dbContext.tbl_mUnitBranch on a.ToStakeHolderId equals d.unitid into toStakeHolder
                                 from to in toStakeHolder.DefaultIfEmpty()
                                 join e in _dbContext.tbl_mUnitBranch on a.FromStakeHolderId equals e.unitid into fromStakeHolder
                                 from fromStake in fromStakeHolder.DefaultIfEmpty()
                                 join l in _dbContext.tbl_mUnitBranch on b.StakeHolderId equals l.unitid into stkhol
                                 from stk in stkhol.DefaultIfEmpty()


                                 join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                                 from eWithComment in fs.DefaultIfEmpty()
                                 join h in _dbContext.mStatus on a.StatusId equals h.StatusId into hs
                                 from eWithStatus in hs.DefaultIfEmpty()
                                 join j in _dbContext.mStages on a.StageId equals j.StagesId into js
                                 from eWithStages in js.DefaultIfEmpty()
                                 join k in _dbContext.mActions on a.ActionId equals k.ActionsId into ks
                                 from eWithAction in ks.DefaultIfEmpty()

                                 where a.ActionId == dft.ActionId && a.ActionCde > 0
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
                                     EditDeleteDate = a.TimeStamp,
                                     UpdatedByUserId = a.UpdatedByUserId,
                                     DateTimeOfUpdate = a.DateTimeOfUpdate,
                                     ProjCode = b.ProjCode,
                                     RecdFmUser = fromStake != null ? fromStake.UnitName : null,
                                     FwdtoUser = to != null ? to.UnitName : null,
                                     FwdBy = current != null ? current.UnitName : null,
                                     AdRemarks = a.AddRemarks,
                                     Comments = eWithComment.Comment,
                                     FwdtoDate = a.TostackholderDt,
                                     Status = eWithStatus.Status,
                                     Stages = eWithStages.Stages,
                                     Action = eWithAction.Actions,
                                     StakeHolder = stk.UnitName,
                                     TotalDays = EF.Functions.DateDiffDay(a.TimeStamp, a.TostackholderDt ?? DateTime.Now),
                                     ActionCde = a.ActionCde,
                                     AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                     HostTypeID = b.HostTypeID,
                                     EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString())



                                 };


                    var projectsWithDetails = await querys.ToListAsync();
                    return projectsWithDetails;
                }


                else
                {
                    var querys = from a in _dbContext.ProjStakeHolderMov
                                 join b in _dbContext.Projects on a.ProjId equals b.ProjId
                                 join c in _dbContext.tbl_mUnitBranch on a.CurrentStakeHolderId equals c.unitid into currentStakeHolder
                                 from current in currentStakeHolder.DefaultIfEmpty()
                                 join d in _dbContext.tbl_mUnitBranch on a.ToStakeHolderId equals d.unitid into toStakeHolder
                                 from to in toStakeHolder.DefaultIfEmpty()
                                 join e in _dbContext.tbl_mUnitBranch on a.FromStakeHolderId equals e.unitid into fromStakeHolder
                                 from fromStake in fromStakeHolder.DefaultIfEmpty()
                                 join l in _dbContext.tbl_mUnitBranch on b.StakeHolderId equals l.unitid into stkhol
                                 from stk in stkhol.DefaultIfEmpty()

                                 join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                                 from eWithComment in fs.DefaultIfEmpty()
                                 join h in _dbContext.mStatus on a.StatusId equals h.StatusId into hs
                                 from eWithStatus in hs.DefaultIfEmpty()
                                 join j in _dbContext.mStages on a.StageId equals j.StagesId into js
                                 from eWithStages in js.DefaultIfEmpty()
                                 join k in _dbContext.mActions on a.ActionId equals k.ActionsId into ks
                                 from eWithAction in ks.DefaultIfEmpty()

                                 where b.StakeHolderId == Logins.unitid && a.ActionId == dft.ActionId && a.ActionCde > 0
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
                                     EditDeleteDate = a.TimeStamp,
                                     UpdatedByUserId = a.UpdatedByUserId,
                                     DateTimeOfUpdate = a.DateTimeOfUpdate,
                                     ProjCode = b.ProjCode,
                                     RecdFmUser = fromStake != null ? fromStake.UnitName : null,
                                     FwdtoUser = to != null ? to.UnitName : null,
                                     FwdBy = current != null ? current.UnitName : null,
                                     AdRemarks = a.AddRemarks,
                                     Comments = eWithComment.Comment,
                                     FwdtoDate = a.TostackholderDt,
                                     Status = eWithStatus.Status,
                                     Stages = eWithStages.Stages,
                                     Action = eWithAction.Actions,
                                     StakeHolder = stk.UnitName,
                                     TotalDays = EF.Functions.DateDiffDay(a.TimeStamp, a.TostackholderDt ?? DateTime.Now),
                                     ActionCde = a.ActionCde,
                                     AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                     HostTypeID = b.HostTypeID,
                                     EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString())



                                 };


                    var projectsWithDetails = await querys.ToListAsync();
                    return projectsWithDetails;

                }


                return null;
            }
            else
            {
                return null;
            }
        }




        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 26 Jul 23
        public async Task<List<tbl_Projects>> XGetActComplettemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                DefaultValueID dft = new DefaultValueID();
                dft.ActionId = _dbContext.mActions
                            .Where(action => action.FininshID == true)
                            .Select(action => action.ActionsId)
                            .FirstOrDefault();

                dft.StatusId = _dbContext.mStatus
                    .Where(stat => stat.FininshID == true)
                    .Select(stat => stat.StatusId)
                    .FirstOrDefault();

                dft.StageID = _dbContext.mStages
                    .Where(stg => stg.FininshID == true)
                    .Select(stat => stat.StagesId)
                    .FirstOrDefault();

                string username = Logins.UserName;




                var query = from a in _dbContext.Projects
                            join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                            from e in bs.DefaultIfEmpty()
                            join d in _dbContext.mStatus on e.StatusId equals d.StatusId into ds
                            from eWithStatus in ds.DefaultIfEmpty()
                            join c in _dbContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                            from eWithUnit in cs.DefaultIfEmpty()
                            join f in _dbContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                            from eWithComment in fs.DefaultIfEmpty()
                            where a.IsActive && !a.IsDeleted && e.ActionId == dft.ActionId && e.ActionCde > 0
                            orderby e.DateTimeOfUpdate descending
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
                                EditDeleteDate = e.TimeStamp,
                                UpdatedByUserId = a.UpdatedByUserId,
                                DateTimeOfUpdate = e.DateTimeOfUpdate,
                                CurrentStakeHolderId = a.CurrentStakeHolderId,
                                StakeHolder = eWithUnit.UnitName,
                                Status = eWithStatus.Status,
                                Comments = eWithComment.Comment,
                                TotalDays = EF.Functions.DateDiffDay(e.TimeStamp, e.TostackholderDt ?? DateTime.Now),
                                ActionCde = a.ActionCde,
                                AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId),
                                HostTypeID = a.HostTypeID,
                                EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString())

                            };

                var projectsWithDetails = await query.ToListAsync();

                return projectsWithDetails;
            }
            else
            {
                return null;
            }
        }


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23 & 6 Oct 23

        public async Task<List<tbl_Projects>> GActProjectsAsync()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (Logins != null)
            {

                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                string username = Logins.UserName;

                var projects = await (from a in _dbContext.Projects
                                      join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join d in _dbContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _dbContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join f in _dbContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted && (e.FromStakeHolderId == stkholder || e.ToStakeHolderId == stkholder || e.CurrentStakeHolderId == stkholder)
                                      && e.ActionCde > 0
                                      orderby e.DateTimeOfUpdate descending
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment
                                      }).ToListAsync();


                return projects;


            }
            else
            {

                return null;
            }



        }



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23 & 3 Oct 23
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
                                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()


                                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted && (a.StakeHolderId == stkholder || e.FromStakeHolderId == stkholder || e.ToStakeHolderId == stkholder || e.CurrentStakeHolderId == stkholder)
                                      && e.ActionCde > 0 && e.ActionId > 4
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          ActionCde = e.ActionCde,
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
                                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()


                                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      && e.ActionCde > 0
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          ActionCde = e.ActionCde,
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


                //                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
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
                //                          CurrentStakeHolderId = a.CurrentStakeHolderId,
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
            //            join c in _dbContext.mStakeHolder on e.CurrentStakeHolderId equals c.StakeHolderId into cs
            //            from eWithStakeHolder in cs.DefaultIfEmpty()
            //            join msh in _dbContext.mStakeHolder on e.CurrentStakeHolderId equals msh.StakeHolderId into mshs
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
            //                CurrentStakeHolderId = a.CurrentStakeHolderId,
            //                StakeHolder = eWithStakeHolder.StakeHolder,
            //                Status = eWithStatus.Status

            //            };

            //var projectsWithDetails = await query.ToListAsync();

            //return projectsWithDetails;

        }



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23 & 3 Oct 23
        public async Task<List<tbl_Projects>> GetProjforEditAsync()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");



            if (Logins != null)
            {


                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                var projects = await (from a in _DBContext.Projects
                                      join b in _DBContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                      from e in bs.DefaultIfEmpty()
                                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()


                                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      && e.ActionCde > 0 && e.ActionId == 2 && a.StakeHolderId == Logins.unitid
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          EncyID = _dataProtector.Protect(a.ProjId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();




                return projects;


            }

            else
            {

                return null;
            }

        }


        public async Task<tbl_Projects> EditWithHistory(int proid)
        {
            tbl_Projects? tbproj = new tbl_Projects();

            tbproj = await _DBContext.Projects
                .Where(p => p.ProjId == proid)
                .FirstOrDefaultAsync();

            if (tbproj == null)
            {
                return null;
            }

            return tbproj;
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
                                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join h in _DBContext.mHostType on a.HostTypeID equals h.HostTypeID into hs
                                      from hostType in hs.DefaultIfEmpty()
                                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted && (a.StakeHolderId == stkholder || e.FromStakeHolderId == stkholder || e.ToStakeHolderId == stkholder || e.CurrentStakeHolderId == stkholder)
                                      && e.ActionCde > 0 && e.StatusId == 4
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          ReqmtJustification = a.ReqmtJustification,
                                          HostTypeID = hostType.HostTypeID,
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
                                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join h in _DBContext.mHostType on a.HostTypeID equals h.HostTypeID into hs
                                      from hostType in hs.DefaultIfEmpty()
                                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      && e.ActionCde > 0 && e.StatusId == 4
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          ReqmtJustification = a.ReqmtJustification,
                                          HostTypeID = hostType.HostTypeID,
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
                                      join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                      from eWithStatus in ds.DefaultIfEmpty()
                                      join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                      from eWithUnit in cs.DefaultIfEmpty()
                                      join h in _DBContext.mHostType on a.HostTypeID equals h.HostTypeID into hs
                                      from hostType in hs.DefaultIfEmpty()
                                      join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                      from curstk in csh.DefaultIfEmpty()

                                      join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                      from eWithComment in fs.DefaultIfEmpty()
                                      where a.IsActive && !a.IsDeleted
                                      && e.ActionCde > 9999999 && e.StatusId == 9999999
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
                                          CurrentStakeHolderId = a.CurrentStakeHolderId,
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          ActionCde = e.ActionCde,
                                          AimScope = a.AimScope,
                                          ReqmtJustification = a.ReqmtJustification,
                                          HostTypeID = hostType.HostTypeID,
                                          Hostedon = hostType.HostingDesc,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();

                return projects;
            }

        }


        public async Task<bool> EdtSaveProjAsync(tbl_Projects project)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins.IsNotNull())
            {
                try
                {
                    project.UpdatedByUserId = Logins.unitid ;
                    project.DateTimeOfUpdate = DateTime.Now;
                    project.InitialRemark = project.InitialRemark + ".. Edited on " + DateTime.Now;
                    project.IsDeleted = false;
                    project.IsActive = true;

                    _dbContext.Projects.Update(project);

                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<bool> UpdateProjectAsync(tbl_Projects project)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {

                tbl_ProjStakeHolderMov psmove = _dbContext.ProjStakeHolderMov.FirstOrDefault(x => x.PsmId == project.CurrentPslmId);
                tbl_Comment cmt = new tbl_Comment();

                if (psmove != null)
                {

                    psmove.ProjId = project.ProjId;

                    psmove.AddRemarks = project.InitialRemark;

                    psmove.CurrentStakeHolderId = project.StakeHolderId;
                    // psmove.CommentId = 0;
                    psmove.CurrentStakeHolderId = 1;

                    psmove.StakeHolderId = project.StakeHolderId;
                    psmove.UpdatedByUserId = Logins.unitid;
                    psmove.DateTimeOfUpdate = DateTime.Now;
                    psmove.IsActive = true;
                    psmove.ActionCde = 1;
                    psmove.EditDeleteDate = DateTime.Now;
                    psmove.EditDeleteBy = Logins.unitid;
                    psmove.TimeStamp = DateTime.Now;

                    _dbContext.ProjStakeHolderMov.Attach(psmove);
                    _dbContext.Entry(psmove).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    tbl_Comment compsmid = _dbContext.Comment.FirstOrDefault(x => x.PsmId == psmove.PsmId);

                    if (compsmid != null)
                    {
                        compsmid.EditDeleteDate = DateTime.Now;
                        compsmid.IsDeleted = false;
                        compsmid.IsActive = true;
                        compsmid.DateTimeOfUpdate = DateTime.Now;
                        compsmid.Comment = project.InitialRemark;
                        compsmid.PsmId = psmove.PsmId;
                        compsmid.UpdatedByUserId = Logins.unitid;
                        compsmid.EditDeleteBy = Logins.unitid;
                        _dbContext.Comment.Update(compsmid);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        cmt.EditDeleteDate = DateTime.Now;
                        cmt.IsDeleted = false;
                        cmt.IsActive = true;
                        cmt.DateTimeOfUpdate = DateTime.Now;
                        cmt.Comment = project.InitialRemark;
                        cmt.PsmId = psmove.PsmId;
                        cmt.UpdatedByUserId = Logins.unitid;
                        cmt.EditDeleteBy = Logins.unitid;
                        _dbContext.Comment.Add(cmt);
                        await _dbContext.SaveChangesAsync();
                    }

                    _dbContext.Projects.Update(project);

                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }




        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
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

        // Method to get attachments based on PsmId
        public List<tbl_AttHistory> GetAttachmentsByPsmId(int psmId)
        {
            // Assuming you have access to a data context or repository to query the database
            var attachments = _dbContext.AttHistory.Where(att => att.PsmId == psmId).ToList();
            return attachments;
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
                                join b in _dbContext.mStages on p.StageId equals b.StagesId into stageJoin
                                from b in stageJoin.DefaultIfEmpty()
                                join c in _dbContext.mStatus on p.StatusId equals c.StatusId into statusJoin
                                from c in statusJoin.DefaultIfEmpty()
                                join l in _dbContext.mActions on p.ActionId equals l.ActionsId into ActionJoin
                                from l in ActionJoin.DefaultIfEmpty()
                                join d in _dbContext.Comment on p.PsmId equals d.PsmId into commentJoin
                                from d in commentJoin.DefaultIfEmpty()
                                join proj in _dbContext.Projects on p.ProjId equals proj.ProjId into projectJoin
                                from proj in projectJoin.DefaultIfEmpty()
                                join fromSH in _dbContext.tbl_mUnitBranch on p.FromStakeHolderId equals fromSH.unitid into fromStakeHolderJoin
                                from fromSH in fromStakeHolderJoin.DefaultIfEmpty()
                                join toSH in _dbContext.tbl_mUnitBranch on p.ToStakeHolderId equals toSH.unitid into toStakeHolderJoin
                                from toSH in toStakeHolderJoin.DefaultIfEmpty()
                                join curSH in _dbContext.tbl_mUnitBranch on p.CurrentStakeHolderId equals curSH.unitid into currentStakeHolderJoin
                                from curSH in currentStakeHolderJoin.DefaultIfEmpty()
                                join stakeHolderSH in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals stakeHolderSH.unitid into stakeHolderJoin
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
                                    InitialRemarks = p.AddRemarks,
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
                                join b in _dbContext.mStages on p.StageId equals b.StagesId into stageJoin
                                from b in stageJoin.DefaultIfEmpty()
                                join c in _dbContext.mStatus on p.StatusId equals c.StatusId into statusJoin
                                from c in statusJoin.DefaultIfEmpty()
                                join l in _dbContext.mActions on p.ActionId equals l.ActionsId into ActionJoin
                                from l in ActionJoin.DefaultIfEmpty()
                                join d in _dbContext.Comment on p.PsmId equals d.PsmId into commentJoin
                                from d in commentJoin.DefaultIfEmpty()
                                join proj in _dbContext.Projects on p.ProjId equals proj.ProjId into projectJoin
                                from proj in projectJoin.DefaultIfEmpty()
                                join fromSH in _dbContext.tbl_mUnitBranch on p.FromStakeHolderId equals fromSH.unitid into fromStakeHolderJoin
                                from fromSH in fromStakeHolderJoin.DefaultIfEmpty()
                                join toSH in _dbContext.tbl_mUnitBranch on p.ToStakeHolderId equals toSH.unitid into toStakeHolderJoin
                                from toSH in toStakeHolderJoin.DefaultIfEmpty()
                                join curSH in _dbContext.tbl_mUnitBranch on proj.StakeHolderId equals curSH.unitid into currentStakeHolderJoin
                                from curSH in currentStakeHolderJoin.DefaultIfEmpty()
                                join stakeHolderSH in _dbContext.tbl_mUnitBranch on p.CurrentStakeHolderId equals stakeHolderSH.unitid into stakeHolderJoin
                                from stakeHolderSH in stakeHolderJoin.DefaultIfEmpty()
                                where proj.ProjName.Length > 1 && proj.ProjId == dtaProjID && (p.ActionCde > 0 || p.ActionCde == -1)
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
                                    CurrentStakeHolder = stakeHolderSH.UnitName,
                                    InitiatedBy = curSH.UnitName,
                                    TimeStamp = p.TimeStamp.HasValue ? p.TimeStamp.Value.ToString("dd-MM-yyyy") : "",
                                    InitialRemarks = proj.AdRemarks,
                                    Remarks = p.AddRemarks,  //  work
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == p.PsmId),
                                    ActionName = l.Actions,
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



        public async Task<List<ProjHistory>> GetProjectHistorybyID1(int? dataProjId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

            if (Logins != null || stkholder != 0)
            {
                try
                {
                    var query = from p in _dbContext.ProjStakeHolderMov
                                join b in _dbContext.mStages on p.StageId equals b.StagesId into stageJoin
                                from b in stageJoin.DefaultIfEmpty()
                                join c in _dbContext.mStatus on p.StatusId equals c.StatusId into statusJoin
                                from c in statusJoin.DefaultIfEmpty()
                                join l in _dbContext.mActions on p.ActionId equals l.ActionsId into ActionJoin
                                from l in ActionJoin.DefaultIfEmpty()
                                join d in _dbContext.Comment on p.PsmId equals d.PsmId into commentJoin
                                from d in commentJoin.DefaultIfEmpty()
                                join proj in _dbContext.Projects on p.ProjId equals proj.ProjId into projectJoin
                                from proj in projectJoin.DefaultIfEmpty()
                                join fromSH in _dbContext.tbl_mUnitBranch on p.FromStakeHolderId equals fromSH.unitid into fromStakeHolderJoin
                                from fromSH in fromStakeHolderJoin.DefaultIfEmpty()
                                join toSH in _dbContext.tbl_mUnitBranch on p.ToStakeHolderId equals toSH.unitid into toStakeHolderJoin
                                from toSH in toStakeHolderJoin.DefaultIfEmpty()
                                join curSH in _dbContext.tbl_mUnitBranch on p.CurrentStakeHolderId equals curSH.unitid into currentStakeHolderJoin
                                from curSH in currentStakeHolderJoin.DefaultIfEmpty()
                                join stakeHolderSH in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals stakeHolderSH.unitid into stakeHolderJoin
                                from stakeHolderSH in stakeHolderJoin.DefaultIfEmpty()
                                where proj.ProjId == dataProjId
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
                                    InitialRemarks = p.AddRemarks,
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == p.PsmId),
                                    ActionName = l.Actions


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





        public Task<tbl_Projects> GetProjbyprojCode(string ProjCode)
        {
            return Task.FromResult<tbl_Projects>(_dbContext.Projects
                .Where(a => a.ProjCode == ProjCode).FirstOrDefault());
        }

        public async Task<tbl_Projects> GetProjectByPsmIdAsync(int psmId)
        {
            return await _dbContext.Projects
                .FirstOrDefaultAsync(a => a.CurrentPslmId == psmId);
        }


        public async Task<tbl_ProjStakeHolderMov> GettXNByPsmIdAsync(int psmId)
        {
            return await _dbContext.ProjStakeHolderMov
               .FirstOrDefaultAsync(a => a.PsmId == psmId);
        }

        public async Task<bool> UpdateTxnAsync(tbl_ProjStakeHolderMov psmov)
        {
            _dbContext.ProjStakeHolderMov.Update(psmov);
            await _dbContext.SaveChangesAsync();


            return true;
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
                               Appt=a.Appt,
                               Fmn = a.Fmn,
                               ContactNo = a.ContactNo,
                               Clearence = a.Clearence == null ? string.Empty : ((DateTime)a.Clearence).ToString("dd/MM/yy"),
                               CertNo = a.CertNo,
                               date = a.date == null ? string.Empty : ((DateTime)a.date).ToString("dd/MM/yy"),
                               ValidUpto= a.ValidUpto == null ? string.Empty : ((DateTime)a.ValidUpto).ToString("dd/MM/yy"),
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

        public async Task<List<TimeExceeds>> GetRecentActionProj()
        {


            var currentDateMinus15Days = DateTime.Now.AddDays(-30);

            var results = (from a in _dbContext.Projects
                           join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into projStakeHolderMovGroup
                           from b in projStakeHolderMovGroup.DefaultIfEmpty()
                           join c in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals c.unitid into currentStakeHolderGroup
                           from c in currentStakeHolderGroup.DefaultIfEmpty()
                           join e in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals e.unitid into fromStakeHolderGroup
                           from e in fromStakeHolderGroup.DefaultIfEmpty()
                           join d in _dbContext.Comment on b.PsmId equals d.PsmId into commentGroup
                           from d in commentGroup.DefaultIfEmpty()
                           join f in _dbContext.mActions on b.ActionId equals f.ActionsId into actionGroup
                           from f in actionGroup.DefaultIfEmpty()

                           where b != null && b.TimeStamp > currentDateMinus15Days && f.TimeLimit > 0
                           && b.ActionCde > 0 && b.ActionCde < 80 && b.StageId == 1
                           orderby a.DateTimeOfUpdate descending
                           select new TimeExceeds
                           {
                               ProjId = a.ProjId,
                               psmid = b.PsmId,
                               ProjName = a.ProjName,
                               helwith = c.UnitName,
                               fwdby = e.UnitName,
                               Comment = d.Comment,
                               StrDate = b.TimeStamp == null ? string.Empty : ((DateTime)b.TimeStamp).ToString("dd/MM/yy")

                           }).ToList();

            return results;

            // 'results' contains the list of TimeExceeds objects with the specified conditions.
            // Actions = "YourActionsValue", 
            //TimeLimit = b.TimeLimit, // Assuming TimeLimit is a property in ProjStakeHolderMov
            //                   dayss = 15, // You can set this to 15 as it is a constant value
            //                   exceeds = (b.TimeLimit - 15) // Calculation for exceeds
        }




        public async Task<List<TimeExceeds>> GetHoldActionProj()
        {

            var currentDateMinus15Days = DateTime.Now.AddDays(-30);

            var results = (from a in _dbContext.Projects
                           join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into projStakeHolderMovGroup
                           from b in projStakeHolderMovGroup.DefaultIfEmpty()
                           join c in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals c.unitid into currentStakeHolderGroup
                           from c in currentStakeHolderGroup.DefaultIfEmpty()
                           join e in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals e.unitid into fromStakeHolderGroup
                           from e in fromStakeHolderGroup.DefaultIfEmpty()
                           join d in _dbContext.Comment on b.PsmId equals d.PsmId into commentGroup
                           from d in commentGroup.DefaultIfEmpty()
                           join f in _dbContext.mActions on b.ActionId equals f.ActionsId into actionGroup
                           from f in actionGroup.DefaultIfEmpty()
                           where (b != null && b.ActionCde > 0 && b.ActionCde < 80) /*&& a.DateTimeOfUpdate < currentDateMinus15Days*/
                           && b.CurrentStakeHolderId < 8
                           orderby a.DateTimeOfUpdate descending
                           select new TimeExceeds
                           {
                               ProjId = a.ProjId,
                               psmid = b.PsmId,
                               ProjName = a.ProjName,
                               helwith = c.UnitName,
                               fwdby = e.UnitName,
                               Comment = d.Comment,
                               StrDate = b.TimeStamp == null ? string.Empty : ((DateTime)b.TimeStamp).ToString("dd/MM/yy")


                           }).ToList();

            return results;

            // 'results' contains the list of TimeExceeds objects with the specified conditions.
            // Actions = "YourActionsValue", 
            //TimeLimit = b.TimeLimit, // Assuming TimeLimit is a property in ProjStakeHolderMov
            //                   dayss = 15, // You can set this to 15 as it is a constant value
            //                   exceeds = (b.TimeLimit - 15) // Calculation for exceeds
        }


        public async Task<List<TimeExceedsAlerts>> GetStkHolderStatus()
        {
            var redQuery = from a in _dbContext.Projects
                           join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId
                           join c in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals c.unitid
                           join d in _dbContext.mActions on b.ActionId equals d.ActionsId
                           join e in _dbContext.Comment on b.PsmId equals e.PsmId
                           join f in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals f.unitid
                           where c.unitid < 6 &&
                                 b.TostackholderDt == null &&
                                 d.TimeLimit <= EF.Functions.DateDiffDay(b.TimeStamp, DateTime.Now)
                           select new
                           {
                               c.unitid,
                               c.UnitName,
                               color = "Red",
                               forecolor = "White"
                           };

            var yellowQuery = from a in _dbContext.Projects
                              join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId
                              join c in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals c.unitid
                              join d in _dbContext.mActions on b.ActionId equals d.ActionsId
                              join e in _dbContext.Comment on b.PsmId equals e.PsmId
                              join f in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals f.unitid
                              where c.unitid < 6 &&
                                    b.TostackholderDt == null &&
                                    (d.TimeLimit - (d.TimeLimit * 0.2)) < EF.Functions.DateDiffDay(b.TimeStamp, DateTime.Now) &&
                                    d.TimeLimit > EF.Functions.DateDiffDay(b.TimeStamp, DateTime.Now) &&
                                    d.TimeLimit > 0 &&
                                    !_dbContext.tbl_mUnitBranch
                                             .Where(u => u.unitid < 6 &&
                                                         _dbContext.ProjStakeHolderMov
                                                                 .Where(p => p.TostackholderDt == null &&
                                                                             d.TimeLimit <= EF.Functions.DateDiffDay(b.TimeStamp, DateTime.Now))
                                                                 .Select(p => p.CurrentStakeHolderId)
                                                                 .Contains(u.unitid))
                                             .Select(u => u.unitid)
                                             .Contains(c.unitid)
                              select new
                              {
                                  c.unitid,
                                  c.UnitName,
                                  color = "Yellow",
                                  forecolor = "Black"
                              };
            var greenQuery = from unit in _dbContext.tbl_mUnitBranch
                             where unit.unitid < 6 &&
                             !(from project in _dbContext.Projects
                               join stakeholderMov in _dbContext.ProjStakeHolderMov on project.CurrentPslmId equals stakeholderMov.PsmId
                               join action in _dbContext.mActions on stakeholderMov.ActionId equals action.ActionsId
                               join branch in _dbContext.tbl_mUnitBranch on stakeholderMov.CurrentStakeHolderId equals branch.unitid
                               where branch.unitid < 6 &&
                                     stakeholderMov.TostackholderDt == null &&
                                     action.TimeLimit <= EF.Functions.DateDiffDay(stakeholderMov.TimeStamp, DateTime.Now)
                               select branch.unitid).Contains(unit.unitid) &&
                             !(from project in _dbContext.Projects
                               join stakeholderMov in _dbContext.ProjStakeHolderMov on project.CurrentPslmId equals stakeholderMov.PsmId
                               join action in _dbContext.mActions on stakeholderMov.ActionId equals action.ActionsId
                               join branch in _dbContext.tbl_mUnitBranch on stakeholderMov.CurrentStakeHolderId equals branch.unitid
                               where branch.unitid < 6 &&
                                     stakeholderMov.TostackholderDt == null &&
                                     (action.TimeLimit - (action.TimeLimit * 0.2)) < EF.Functions.DateDiffDay(stakeholderMov.TimeStamp, DateTime.Now) &&
                                     action.TimeLimit > EF.Functions.DateDiffDay(stakeholderMov.TimeStamp, DateTime.Now) &&
                                     action.TimeLimit > 0
                               select branch.unitid).Contains(unit.unitid)
                             select new
                             {
                                 unit.unitid,
                                 unit.UnitName,
                                 color = "Green",
                                 forecolor = "White"
                             };


            List<TimeExceedsAlerts> tlex = new List<TimeExceedsAlerts>();
            var greenresult = greenQuery.ToList();
            var resultone = redQuery.ToList();
            var resulttwo = yellowQuery.ToList();

            tlex = redQuery
                .Union(yellowQuery)
                .Union(greenQuery)
                .Select(item => new TimeExceedsAlerts
                {
                    unitid = item.unitid,
                    unitname = item.UnitName,
                    color = item.color,
                    forecolor = item.forecolor
                })
                .ToList();

            return tlex;


        }



        // Created by Ajay on 20 Secp 23 for recall send messages to inbox


        public async Task<bool> UndoChanges(int id)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (Logins != null)
            {
                tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                psmove = await _dbContext.ProjStakeHolderMov
                    .Where(movement => movement.ActionCde == 1 && movement.PsmId == id)
                    .FirstOrDefaultAsync();
                //  where a.FromStakeHolderId == Logins.unitid && a.CurrentStakeHolderId > 0 && a.ActionCde>0

                if (psmove != null)
                {
                    psmove.CurrentStakeHolderId = Logins.unitid ?? 0;

                    psmove.FromStakeHolderId = 8;
                    psmove.TostackholderDt = null;
                    psmove.ToStakeHolderId = 0;
                    psmove.ActionDt = null;

                    try
                    {
                        _dbContext.ProjStakeHolderMov.UpdateRange(psmove);
                        await _dbContext.SaveChangesAsync();

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                    //foreach (var movement in stakeHolderMovements)
                    //{
                    //    movement.CurrentStakeHolderId = Logins.unitid ?? 0;

                    //    movement.FromStakeHolderId = Logins.unitid ?? 0;
                    //    movement.TostackholderDt = null;


                    //}

                    //try
                    //{
                    //    await _dbContext.SaveChangesAsync();
                    //    return true;
                    //}
                    //catch (Exception ex)
                    //{
                    //    // Handle the exception here (e.g., log or re-throw)
                    //    return false;
                    //}
                }
            }

            return false;
        }

        public async Task<List<tbl_Projects>> GetWhitelistedProjAsync()
        {

            var query = from a in _dbContext.Projects
                        join b in _dbContext.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                        from e in bs.DefaultIfEmpty()
                        join d in _dbContext.mStatus on e.StatusId equals d.StatusId into ds
                        from eWithStatus in ds.DefaultIfEmpty()
                        join c in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                        from eWithUnit in cs.DefaultIfEmpty()
                        join g in _dbContext.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                        from eFromUnit in cg.DefaultIfEmpty()
                        join k in _dbContext.mActions on e.ActionId equals k.ActionsId into ks
                        from eWithAction in ks.DefaultIfEmpty()

                        join f in _dbContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                        from eWithComment in fs.DefaultIfEmpty()

                        where a.IsActive && !a.IsDeleted && eWithAction.FininshID == true

                        orderby e.DateTimeOfUpdate descending

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
                            EditDeleteDate = e.TimeStamp,
                            UpdatedByUserId = a.UpdatedByUserId,
                            DateTimeOfUpdate = e.DateTimeOfUpdate,
                            CurrentStakeHolderId = a.CurrentStakeHolderId,
                            StakeHolder = eWithUnit.UnitName,
                            Status = eWithStatus.Status,
                            Comments = eWithComment.Comment,
                            FwdBy = eFromUnit.UnitName,
                            Action = eWithAction.Actions,
                            AimScope = a.AimScope

                        };

            var projectsWithDetails = await query.ToListAsync();
          
            return projectsWithDetails;

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
                             join c in _dbContext.tbl_mUnitBranch on a.CurrentStakeHolderId equals c.unitid into currentStakeHolder
                             from current in currentStakeHolder.DefaultIfEmpty()
                             join m in _DBContext.tbl_mUnitBranch on b.StakeHolderId equals m.unitid into stakehold
                             from stk in stakehold.DefaultIfEmpty()
                             join d in _dbContext.tbl_mUnitBranch on a.ToStakeHolderId equals d.unitid into toStakeHolder
                             from to in toStakeHolder.DefaultIfEmpty()
                             join e in _dbContext.tbl_mUnitBranch on a.FromStakeHolderId equals e.unitid into fromStakeHolder
                             from fromStake in fromStakeHolder.DefaultIfEmpty()
                             join f in _dbContext.Comment on a.PsmId equals f.PsmId into fs
                             from eWithComment in fs.DefaultIfEmpty()
                             join h in _dbContext.mStatus on a.StatusId equals h.StatusId into hs
                             from eWithStatus in hs.DefaultIfEmpty()
                             join j in _dbContext.mStages on a.StageId equals j.StagesId into js
                             from eWithStages in js.DefaultIfEmpty()
                             join k in _dbContext.mActions on a.ActionId equals k.ActionsId into ks
                             from eWithAction in ks.DefaultIfEmpty()
                             where a.FromStakeHolderId == Logins.unitid && a.CurrentStakeHolderId > 0 && a.ActionCde == 0 || a.ActionCde == null

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
                                 EditDeleteDate = a.TimeStamp,
                                 UpdatedByUserId = a.UpdatedByUserId,
                                 DateTimeOfUpdate = a.DateTimeOfUpdate,
                                 ProjCode = b.ProjCode,
                                 RecdFmUser = fromStake != null ? fromStake.UnitName : null,
                                 FwdtoUser = to != null ? to.UnitName : null,
                                 FwdBy = current != null ? current.UnitName : null,
                                 AdRemarks = a.AddRemarks,
                                 Comments = eWithComment.Comment,
                                 FwdtoDate = a.TostackholderDt,
                                 Status = eWithStatus.Status,
                                 Stages = eWithStages.Stages,
                                 StakeHolder = stk.UnitName,
                                 Action = eWithAction.Actions,
                                 TotalDays = 15,
                                 ActionCde = a.ActionCde,
                                 AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.CurrentPslmId),
                                 HostTypeID = b.HostTypeID,
                                 EncyID = _dataProtector.Protect(b.CurrentPslmId.ToString()),
                                 PsmIds = a.PsmId,
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

        // proj select stage 2 and status 20
        public async Task<List<TimeExceeds>> GetHoldRFPProj()
        {
            var query = from a in _dbContext.mActions
                        join b in _dbContext.ProjStakeHolderMov on a.ActionsId equals b.ActionId
                        join c in _dbContext.Projects on b.PsmId equals c.CurrentPslmId
                        join d in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals d.unitid into dGroup
                        from d in dGroup.DefaultIfEmpty()
                        join e in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals e.unitid into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        join f in _dbContext.Comment on b.PsmId equals f.PsmId into fGroup
                        from f in fGroup.DefaultIfEmpty()
                        where c.ProjId > 0 && b.StageId == 2 && b.StatusId == 21 && d.unitid < 6 && b.ActionCde <= 1
                        orderby c.DateTimeOfUpdate descending
                        select new TimeExceeds
                        {
                            ProjId = c.ProjId,
                            psmid = b.PsmId,
                            ProjName = c.ProjName,
                            helwith = d.UnitName,
                            fwdby = e.UnitName,
                            Actions = a.Actions,
                            Comment = f.Comment,
                            EditDeleteDate = b.EditDeleteDate,
                            TimeLimit = a.TimeLimit,
                            dayss = EF.Functions.DateDiffDay(b.EditDeleteDate, DateTime.Now),
                            exceeds = EF.Functions.DateDiffDay(b.EditDeleteDate, DateTime.Now) - a.TimeLimit,

                        };

            var delayedproj = await query.ToListAsync();

            return delayedproj;

        }


        public async Task<List<tbl_Projects>> GetStatusProjAsync(int statusid)
        {
            var actionIds = new[] { 7, 11, 14 };
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (statusid == 5 || statusid == 7 || statusid == 11)
            {
                statusid = 4;
            }
            if (statusid == 19)
            {
                if (Logins != null && Logins.Role == "Dte")
                {
                    var query = from p in _dbContext.Projects
                                join t in _dbContext.ProjStakeHolderMov on p.CurrentPslmId equals t.PsmId
                                join q in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals q.unitid into stakeHolderJoin
                                from stakeHolder in stakeHolderJoin.DefaultIfEmpty()
                                join u in _dbContext.tbl_mUnitBranch on t.CurrentStakeHolderId equals u.unitid into fromStakeHolderJoin
                                from fromStakeHolder in fromStakeHolderJoin.DefaultIfEmpty()
                                join r in _dbContext.mStatus on t.StatusId equals r.StatusId into statusJoin
                                from status in statusJoin.DefaultIfEmpty()
                                join s in _dbContext.Comment on t.PsmId equals s.PsmId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join k in _dbContext.mActions on t.ActionId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join l in _dbContext.mStages on t.StageId equals l.StagesId into ss
                                from eStage in ss.DefaultIfEmpty()


                                where _dbContext.ProjStakeHolderMov
                               .Where(sub => actionIds.Contains(sub.ActionId) &&
                               !_dbContext.ProjStakeHolderMov.Any(sub1 => sub1.ProjId == sub.ProjId && sub1.StatusId > 19) &&
                               _dbContext.ProjStakeHolderMov.Any(sub2 => sub2.ProjId == sub.ProjId && sub2.ActionCde > 0))
                               .Select(sub => sub.ProjId)
                               .Contains(p.ProjId)

                                select new tbl_Projects
                                {
                                    CurrentPslmId = t.PsmId,
                                    ProjId = p.ProjId,
                                    Status = status.Status,
                                    ProjName = p.ProjName,
                                    ActionId = t.ActionId,
                                    InitiatedDate = p.InitiatedDate,
                                    InitialRemark = p.InitialRemark,
                                    ActionDt = t.TimeStamp,
                                    RecdFmUser = fromStakeHolder.UnitName, //  currently held with
                                    Comments = comment.Comment,
                                    AdRemarks = t.AddRemarks,
                                    StakeHolder = stakeHolder.UnitName, // initiated by
                                    Action = eWithAction.Actions,
                                    FwdtoUser = stakeHolder.UnitName,
                                    EncyPsmID = _dataProtector.Protect(t.PsmId.ToString()),
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == t.PsmId),
                                    Stages = eStage.Stages,
                                    BlogComment = (
                                    string.Join(", ", (
            from x in _dbContext.StkComment
            join y in _dbContext.tbl_mUnitBranch on x.StakeHolderId equals y.unitid into stkCommentJoin
            from stkComment in stkCommentJoin.DefaultIfEmpty()
            where x.PsmId == t.PsmId
            select $"{stkComment.UnitName} ({x.DateTimeOfUpdate}) Comment: {x.Comments} "
        ).ToList()))
         };
                    var result = query.ToList();
                    return result;
                }
                else if (Logins != null)
                {
                    var query = from p in _dbContext.Projects
                                join t in _dbContext.ProjStakeHolderMov on p.CurrentPslmId equals t.PsmId
                                join q in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals q.unitid into stakeHolderJoin
                                from stakeHolder in stakeHolderJoin.DefaultIfEmpty()
                                join u in _dbContext.tbl_mUnitBranch on t.CurrentStakeHolderId equals u.unitid into fromStakeHolderJoin
                                from fromStakeHolder in fromStakeHolderJoin.DefaultIfEmpty()
                                join r in _dbContext.mStatus on t.StatusId equals r.StatusId into statusJoin
                                from status in statusJoin.DefaultIfEmpty()
                                join s in _dbContext.Comment on t.PsmId equals s.PsmId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join k in _dbContext.mActions on t.ActionId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join l in _dbContext.mStages on t.StageId equals l.StagesId into ss
                                from eStage in ss.DefaultIfEmpty()

                                where _dbContext.ProjStakeHolderMov
                     .Where(sub => actionIds.Contains(sub.ActionId) &&
                                   !_dbContext.ProjStakeHolderMov.Any(sub1 => sub1.ProjId == sub.ProjId && sub1.StatusId > 19) &&
                                   _dbContext.ProjStakeHolderMov.Any(sub2 => sub2.ProjId == sub.ProjId && sub2.ActionCde > 0))
                     .Select(sub => sub.ProjId)
                     .Distinct()
                     .Contains(p.ProjId) &&
                 p.StakeHolderId == Logins.unitid




                                //              where t.ActionCde > 0 &&
                                //t.StatusId == statusid && (p.StakeHolderId == Logins.unitid || t.CurrentStakeHolderId == Logins.unitid || t.FromStakeHolderId == Logins.unitid) &&
                                //(p.StakeHolderId == Logins.unitid || t.CurrentStakeHolderId == Logins.unitid || t.FromStakeHolderId == Logins.unitid) &&
                                //_dbContext.ProjStakeHolderMov
                                //  .Where(psm => (psm.ActionId == 7 || psm.ActionId == 11 || psm.ActionId == 5) && !new[] { 17, 18, 19, 20 }.Contains(psm.ActionId))
                                //  .Select(psm => psm.ProjId)
                                //  .Contains(p.ProjId)

                                select new tbl_Projects
                                {

                                    CurrentPslmId = t.PsmId,
                                    ProjId = p.ProjId,
                                    Status = status.Status,
                                    ProjName = p.ProjName,
                                    ActionId = t.ActionId,
                                    InitiatedDate = p.InitiatedDate,
                                    InitialRemark = p.InitialRemark,
                                    ActionDt = t.TimeStamp,
                                    RecdFmUser = fromStakeHolder.UnitName, //  currently held with
                                    Comments = comment.Comment,
                                    AdRemarks = t.AddRemarks,
                                    StakeHolder = stakeHolder.UnitName, // initiated by
                                    Action = eWithAction.Actions,
                                    FwdtoUser = stakeHolder.UnitName,
                                    EncyPsmID = _dataProtector.Protect(t.PsmId.ToString()),
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == t.PsmId),
                                    Stages = eStage.Stages,
                                    BlogComment = (

                                     string.Join(", ", (
                                    from x in _dbContext.StkComment
                                   join y in _dbContext.tbl_mUnitBranch on x.StakeHolderId equals y.unitid into stkCommentJoin
                                   from stkComment in stkCommentJoin.DefaultIfEmpty()
                                   where x.PsmId == t.PsmId
                                   select $"{stkComment.UnitName} ({x.DateTimeOfUpdate}) Comment: {x.Comments} "
                               ).ToList()))




                                };

                    var result = query.ToList();
                    return result;
                }



            }
            //
            else if (statusid == 23)
            {



                if (Logins != null && Logins.Role == "Dte")


                {
                    var query = from p in _dbContext.Projects
                                join t in _dbContext.ProjStakeHolderMov on p.CurrentPslmId equals t.PsmId
                                join q in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals q.unitid into stakeHolderJoin
                                from stakeHolder in stakeHolderJoin.DefaultIfEmpty()
                                join u in _dbContext.tbl_mUnitBranch on t.CurrentStakeHolderId equals u.unitid into fromStakeHolderJoin
                                from fromStakeHolder in fromStakeHolderJoin.DefaultIfEmpty()
                                join r in _dbContext.mStatus on t.StatusId equals r.StatusId into statusJoin
                                from status in statusJoin.DefaultIfEmpty()
                                join s in _dbContext.Comment on t.PsmId equals s.PsmId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join k in _dbContext.mActions on t.ActionId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join l in _dbContext.mStages on t.StageId equals l.StagesId into ss
                                from eStage in ss.DefaultIfEmpty()
                                join subqueryProjIds in (
                                    from sb in _dbContext.Projects
                                    join sa in _dbContext.ProjStakeHolderMov on sb.ProjId equals sa.ProjId into saGroup
                                    from sa in saGroup.DefaultIfEmpty()
                                    where (sa == null || (sa.StatusId == 21 && sa.ActionId == 20))
                                        && !_dbContext.ProjStakeHolderMov.Any(psh => psh.ProjId == sb.ProjId && psh.StatusId > 21)
                                    group sb by new { sb.ProjId, sb.ProjName } into grouped
                                    select grouped.Key.ProjId
                                ) on p.ProjId equals subqueryProjIds
                                select new tbl_Projects
                                {

                                    CurrentPslmId = t.PsmId,
                                    ProjId = p.ProjId,
                                    Status = status.Status,
                                    ProjName = p.ProjName,
                                    ActionId = t.ActionId,
                                    InitiatedDate = p.InitiatedDate,
                                    InitialRemark = p.InitialRemark,
                                    ActionDt = t.TimeStamp,
                                    RecdFmUser = fromStakeHolder.UnitName, //  currently held with
                                    Comments = comment.Comment,
                                    AdRemarks = t.AddRemarks,
                                    StakeHolder = stakeHolder.UnitName, // initiated by
                                    Action = eWithAction.Actions,
                                    FwdtoUser = stakeHolder.UnitName,
                                    EncyPsmID = _dataProtector.Protect(t.PsmId.ToString()),
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == t.PsmId),
                                    Stages = eStage.Stages,
                                    BlogComment = (

                                    string.Join(", ", (
           from x in _dbContext.StkComment
           join y in _dbContext.tbl_mUnitBranch on x.StakeHolderId equals y.unitid into stkCommentJoin
           from stkComment in stkCommentJoin.DefaultIfEmpty()
           where x.PsmId == t.PsmId
           select $"{stkComment.UnitName} ({x.DateTimeOfUpdate}) Comment: {x.Comments} "
       ).ToList()))
                                };

                    var result = query.ToList();
                    return result;
                }

                else if (Logins != null)
                {


                    var query = from p in _dbContext.Projects
                                join t in _dbContext.ProjStakeHolderMov on p.CurrentPslmId equals t.PsmId
                                join q in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals q.unitid into stakeHolderJoin
                                from stakeHolder in stakeHolderJoin.DefaultIfEmpty()
                                join u in _dbContext.tbl_mUnitBranch on t.CurrentStakeHolderId equals u.unitid into fromStakeHolderJoin
                                from fromStakeHolder in fromStakeHolderJoin.DefaultIfEmpty()
                                join r in _dbContext.mStatus on t.StatusId equals r.StatusId into statusJoin
                                from status in statusJoin.DefaultIfEmpty()
                                join s in _dbContext.Comment on t.PsmId equals s.PsmId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join k in _dbContext.mActions on t.ActionId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join l in _dbContext.mStages on t.StageId equals l.StagesId into ss
                                from eStage in ss.DefaultIfEmpty()



                                where _dbContext.ProjStakeHolderMov
                     .Where(sub => actionIds.Contains(sub.ActionId) &&
                                   !_dbContext.ProjStakeHolderMov.Any(sub1 => sub1.ProjId == sub.ProjId && sub1.StatusId > 19) &&
                                   _dbContext.ProjStakeHolderMov.Any(sub2 => sub2.ProjId == sub.ProjId && sub2.ActionCde > 0))
                     .Select(sub => sub.ProjId)
                     .Distinct()
                     .Contains(p.ProjId) &&
                 p.StakeHolderId == Logins.unitid




                                select new tbl_Projects
                                {

                                    CurrentPslmId = t.PsmId,
                                    ProjId = p.ProjId,
                                    Status = status.Status,
                                    ProjName = p.ProjName,
                                    ActionId = t.ActionId,
                                    InitiatedDate = p.InitiatedDate,
                                    InitialRemark = p.InitialRemark,
                                    ActionDt = t.TimeStamp,
                                    RecdFmUser = fromStakeHolder.UnitName, //  currently held with
                                    Comments = comment.Comment,
                                    AdRemarks = t.AddRemarks,
                                    StakeHolder = stakeHolder.UnitName, // initiated by
                                    Action = eWithAction.Actions,
                                    FwdtoUser = stakeHolder.UnitName,
                                    EncyPsmID = _dataProtector.Protect(t.PsmId.ToString()),
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == t.PsmId),
                                    Stages = eStage.Stages,
                                    BlogComment = (

                                     string.Join(", ", (
                                          from x in _dbContext.StkComment
                                          join y in _dbContext.tbl_mUnitBranch on x.StakeHolderId equals y.unitid into stkCommentJoin
                                          from stkComment in stkCommentJoin.DefaultIfEmpty()
                                          where x.PsmId == t.PsmId
                                          select $"{stkComment.UnitName} ({x.DateTimeOfUpdate}) Comment: {x.Comments} "
                                      ).ToList()))
                                      
                                      


                                };

                    var result = query.ToList();
                    return result;

                }

            }
            else
            {

                if (Logins != null && Logins.Role == "Dte")
                {
                    var query = from p in _dbContext.Projects
                                join t in _dbContext.ProjStakeHolderMov on p.CurrentPslmId equals t.PsmId
                                join q in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals q.unitid into stakeHolderJoin
                                from stakeHolder in stakeHolderJoin.DefaultIfEmpty()
                                join u in _dbContext.tbl_mUnitBranch on t.CurrentStakeHolderId equals u.unitid into fromStakeHolderJoin
                                from fromStakeHolder in fromStakeHolderJoin.DefaultIfEmpty()
                                join r in _dbContext.mStatus on t.StatusId equals r.StatusId into statusJoin
                                from status in statusJoin.DefaultIfEmpty()
                                join s in _dbContext.Comment on t.PsmId equals s.PsmId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join k in _dbContext.mActions on t.ActionId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join l in _dbContext.mStages on t.StageId equals l.StagesId into ss
                                from eStage in ss.DefaultIfEmpty()
                                where t.ActionCde > 0 && t.StatusId == statusid
                                select new tbl_Projects
                                {

                                    CurrentPslmId = t.PsmId,
                                    ProjId = p.ProjId,
                                    Status = status.Status,
                                    ProjName = p.ProjName,
                                    ActionId = t.ActionId,
                                    InitiatedDate = p.InitiatedDate,
                                    InitialRemark = p.InitialRemark,
                                    ActionDt = t.TimeStamp,
                                    RecdFmUser = fromStakeHolder.UnitName, //  currently held with
                                    Comments = comment.Comment,
                                    AdRemarks = t.AddRemarks,
                                    StakeHolder = stakeHolder.UnitName, // initiated by
                                    Action = eWithAction.Actions,
                                    FwdtoUser = stakeHolder.UnitName,
                                    EncyPsmID = _dataProtector.Protect(t.PsmId.ToString()),
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == t.PsmId),
                                    Stages = eStage.Stages,
                                    BlogComment = (

                                    string.Join(", ", (
                                         from x in _dbContext.StkComment 
                                         join y in _dbContext.tbl_mUnitBranch on x.StakeHolderId equals y.unitid into stkCommentJoin
                                         from stkComment in stkCommentJoin.DefaultIfEmpty()
                                          where x.PsmId == t.PsmId
                                         select $"{stkComment.UnitName} ({x.DateTimeOfUpdate}) Comment: {x.Comments} "
                                     ).ToList()))
                                    

                                };
                    var result = query.ToList();
                    return result;
                }
                else if (Logins != null)
                {


                    var query = from p in _dbContext.Projects
                                join t in _dbContext.ProjStakeHolderMov on p.CurrentPslmId equals t.PsmId
                                join q in _dbContext.tbl_mUnitBranch on p.StakeHolderId equals q.unitid into stakeHolderJoin
                                from stakeHolder in stakeHolderJoin.DefaultIfEmpty()
                                join u in _dbContext.tbl_mUnitBranch on t.CurrentStakeHolderId equals u.unitid into fromStakeHolderJoin
                                from fromStakeHolder in fromStakeHolderJoin.DefaultIfEmpty()
                                join r in _dbContext.mStatus on t.StatusId equals r.StatusId into statusJoin
                                from status in statusJoin.DefaultIfEmpty()
                                join s in _dbContext.Comment on t.PsmId equals s.PsmId into commentJoin
                                from comment in commentJoin.DefaultIfEmpty()
                                join k in _dbContext.mActions on t.ActionId equals k.ActionsId into ks
                                from eWithAction in ks.DefaultIfEmpty()
                                join l in _dbContext.mStages on t.StageId equals l.StagesId into ss
                                from eStage in ss.DefaultIfEmpty()
                                where t.ActionCde > 0 && t.StatusId == statusid && p.StakeHolderId == Logins.unitid
                                //  where t.ActionCde > 0 && t.StatusId == statusid && (p.StakeHolderId == Logins.unitid || t.CurrentStakeHolderId == Logins.unitid || t.FromStakeHolderId == Logins.unitid)
                                select new tbl_Projects
                                {

                                    CurrentPslmId = t.PsmId,
                                    ProjId = p.ProjId,
                                    Status = status.Status,
                                    ProjName = p.ProjName,
                                    ActionId = t.ActionId,
                                    InitiatedDate = p.InitiatedDate,
                                    InitialRemark = p.InitialRemark,
                                    ActionDt = t.TimeStamp,
                                    RecdFmUser = fromStakeHolder.UnitName, //  currently held with
                                    Comments = comment.Comment,
                                    AdRemarks = t.AddRemarks,
                                    StakeHolder = stakeHolder.UnitName, // initiated by
                                    Action = eWithAction.Actions,
                                    FwdtoUser = stakeHolder.UnitName,
                                    EncyPsmID = _dataProtector.Protect(t.PsmId.ToString()),
                                    AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == t.PsmId),
                                    Stages = eStage.Stages,
                                    BlogComment = (
                                   
                                      string.Join(", ", (
                                         from x in _dbContext.StkComment
                                         join y in _dbContext.tbl_mUnitBranch on x.StakeHolderId equals y.unitid into stkCommentJoin
                                         from stkComment in stkCommentJoin.DefaultIfEmpty()
                                         where x.PsmId == t.PsmId
                                         select $"{stkComment.UnitName} ({x.DateTimeOfUpdate}) Comment: {x.Comments} "
                                     ).ToList()))
                                   
                                };

                    var result = query.ToList();
                    return result;
                }
            }



            return null;


        }


        //        SELECT t.PsmId as CurrentPslmId, p.ProjId as ProjId, r.Status as Status, p.ProjName, t.TimeStamp as ActionDt, u.unitname aS RecdFmUser, s.Comment, t.AddRemarks, q.unitname as StakeHolder ,


        //        (SELECT STUFF((
        //            SELECT ', ' + CONCAT(y.unitname, ' (', CONVERT(VARCHAR, x.DateTimeOfUpdate, 103), ') Comment: ', x.Comments, ' Dated: ', CAST(x.psmid AS VARCHAR))
        //    FROM StkComment x
        //    LEFT JOIN tbl_mUnitBranch y ON y.unitid = x.StakeHolderId
        //    where x.psmid=t.PsmId
        //    FOR XML PATH('')), 1, 2, '')) AS Comments





        //FROM projects p
        //     JOIN  ProjStakeHolderMov t ON p.CurrentPslmId =t.PsmId
        //     left join tbl_mUnitBranch q on t.CurrentStakeHolderId= q.unitid
        //left join tbl_mUnitBranch u on t.FromStakeHolderId= u.unitid


        //     left join mStatus r on t.StatusId = r.StatusId

        //     left join Comment s on s.PsmId = t.PsmId
        //     WHERE t.ActionCde>0
        //     and t.StatusId= 2

        public async Task<List<CommentBy_StakeHolder>> GetCommentByStakeHolder(int? projid)
        {
            var Commentdata = from comment in _dbContext.StkComment
                              join branch in _dbContext.tbl_mUnitBranch on comment.StakeHolderId equals branch.unitid
                              join project in _dbContext.Projects on comment.ProjId equals project.ProjId
                              join projMov in _dbContext.ProjStakeHolderMov on comment.PsmId equals projMov.PsmId
                              join StackHolder in _dbContext.tbl_mUnitBranch on comment.StakeHolderId equals StackHolder.unitid
                              // comment by nitin kumar 25-04-2024
                              join StkStatus in _dbContext.StkStatus on comment.StkStatusId equals StkStatus.StkStatusId into actionsGroup      
                              //join actions in _dbContext.mActions on comment.ActionsId equals actions.ActionsId into actionsGroup
                              from StkStatus in actionsGroup.DefaultIfEmpty()  // Left outer join
                              where comment.ProjId == projid
                              select new CommentBy_StakeHolder
                              {

                                  stakeholder = StackHolder.UnitName,
                                  DateTimeOfUpdate = comment.DateTimeOfUpdate,
                                  Comment = comment.Comments,
                                  Action = StkStatus.Status


                              };

            var result = await Commentdata.ToListAsync();

            if (result == null || result.Count == 0)
            {

                CommentBy_StakeHolder defaultRow = new CommentBy_StakeHolder
                {
                    stakeholder = string.Empty,
                    DateTimeOfUpdate = DateTime.MinValue,
                    Comment = string.Empty,
                    Action = string.Empty
                };


                result = new List<CommentBy_StakeHolder> { defaultRow };
            }



            return result;
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
                                  join d in _DBContext.mStatus on e.StatusId equals d.StatusId into ds
                                  from eWithStatus in ds.DefaultIfEmpty()
                                  join c in _DBContext.tbl_mUnitBranch on a.StakeHolderId equals c.unitid into cs
                                  from eWithUnit in cs.DefaultIfEmpty()
                                  join g in _DBContext.tbl_mUnitBranch on e.CurrentStakeHolderId equals g.unitid into csh
                                  from curstk in csh.DefaultIfEmpty()

                                  join f in _DBContext.Comment on a.CurrentPslmId equals f.PsmId into fs
                                  from eWithComment in fs.DefaultIfEmpty()
                                  where a.IsActive && !a.IsDeleted && a.StakeHolderId == Logins.unitid
                                   && e.ActionCde > 0 && e.ActionId > 4
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
                                      CurrentStakeHolderId = a.CurrentStakeHolderId,
                                      StakeHolder = eWithUnit.UnitName,
                                      FwdtoUser = curstk.UnitName,
                                      Status = eWithStatus.Status,
                                      Comments = eWithComment.Comment,
                                      ActionCde = e.ActionCde,
                                      AimScope = a.AimScope,
                                      ReqmtJustification = a.ReqmtJustification,
                                      EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                      AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                  }).ToListAsync();


            return projects;


            //}

        }


        public int CurrentPsmGet(int? ProjID)
        {
            int psmid = _dbContext.Projects
                .Where(a => a.ProjId == ProjID)
                .Select(b => b.CurrentPslmId)
                .FirstOrDefault();

            return psmid;
        }






        //ProjId
        //ProjName
        //InitialRemark
        //InitiatedDate
        //IsWhitelisted
        //CompletionDate
        //AimScope
        public async Task<List<tbl_Projects>> GetProjforDocView()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");


            if (Logins != null && Logins.Role == "Dte")
            {
                var query = from p in _dbContext.Projects

                            select new tbl_Projects
                            {

                                ProjId = p.ProjId,
                                ProjName = p.ProjName,
                                InitiatedDate = p.InitiatedDate,
                                InitialRemark = p.InitialRemark,
                                IsWhitelisted = p.IsWhitelisted,
                                CompletionDate = p.CompletionDate,
                                AimScope = p.AimScope,
                                EncyID = _dataProtector.Protect(p.ProjId.ToString())


                            };
                List<tbl_Projects> resul = new List<tbl_Projects>();
                resul = await query.ToListAsync();

                return resul;
            }
            else
            {
                List<tbl_Projects> resul = new List<tbl_Projects>();
                return resul;
            }

        }

        public Task<tbl_Projects> GetProjectHistorybyIDForSearch(int dtaProjID)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserRegistered(string UserName)
        {
            _dbContext.Users.AnyAsync(u => u.UserName == UserName);

            return Task.FromResult(true);
        }
    }




}
