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
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.Data.SqlClient;
using swas.DAL.Mapper;
using Microsoft.Extensions.Configuration;

namespace swas.BAL.Repository
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationDbContext _DBContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;
        private readonly IProjStakeHolderMovRepository _psmRepository;
        private readonly IConfiguration _configuration;
        public ProjectsRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext DBContext, IDataProtectionProvider dataProtector, IProjStakeHolderMovRepository psmRepository, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _DBContext = DBContext;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
            _psmRepository = psmRepository;
            _configuration = configuration;
        }
        public async Task<DTOProjectWiseStatus> GetProjectWiseStatus(int? Projid)
        {
         

            #region GetProjectWiseStatusWithProc
            int projid = Projid ?? 0;

            DTOProjectWiseStatus result = new DTOProjectWiseStatus();
            result.StatusProjectlst = new List<StatusProject>();
            result.MovProjectlst = new List<MovProject>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_dbContext.Database.GetConnectionString()))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("usp_GetProjectWiseStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Projid", projid));

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result.StatusProjectlst.Add(new StatusProject
                                {
                                    StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                                    StageName = reader.GetString(reader.GetOrdinal("StageName")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                });
                            }
                            if (await reader.NextResultAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    result.MovProjectlst.Add(new MovProject
                                    {
                                        ProjId = reader.GetInt32(reader.GetOrdinal("ProjId")),
                                        ProjName = reader.GetString(reader.GetOrdinal("ProjName")),
                                        TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp")),
                                        StatusId = reader.GetInt32(reader.GetOrdinal("StatusId"))
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching project status", ex);
            }

            return result;
            #endregion
        }
        public async Task<List<DTOProjectsFwd>> GetDashboardApproved(int StatuId, int statusActionsMappingId)
        {
         

            #region GetDashBoardApprovedWithProc

            try
            {
          

                var lst = new List<DTOProjectsFwd>();
				Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

				using (var conn = _dbContext.Database.GetDbConnection())
                {
                    await conn.OpenAsync();



                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.usp_GetDashboardApproved";
                        cmd.CommandType = CommandType.StoredProcedure;

                        var paramStatusId = cmd.CreateParameter();
                        paramStatusId.ParameterName = "@StatusId";
                        paramStatusId.Value = StatuId;
                        cmd.Parameters.Add(paramStatusId);

                        var paramStatusActionId = cmd.CreateParameter();
                        paramStatusActionId.ParameterName = "@StatusActionsMappingId";
                        paramStatusActionId.Value = statusActionsMappingId;
                        cmd.Parameters.Add(paramStatusActionId);

                        DataTable dt = new DataTable();

                        using (var da = new SqlDataAdapter((SqlCommand)cmd))
                        {
                            da.Fill(dt);
                        }
                        using ( var reader = await cmd.ExecuteReaderAsync())
                        {   
                            while (await reader.ReadAsync())
                            {
                                int stakeHolderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId"));

                                var item = new DTOProjectsFwd
                                {
                                    ProjId = reader.GetInt32(reader.GetOrdinal("ProjId")),
                                  
                                    PsmIds = reader.GetInt32(reader.GetOrdinal("PsmId")),
                                    ProjName = reader.IsDBNull(reader.GetOrdinal("ProjName")) ? null : reader.GetString(reader.GetOrdinal("ProjName")),
                                    StakeHolder = reader.IsDBNull(reader.GetOrdinal("StakeHolder")) ? null : reader.GetString(reader.GetOrdinal("StakeHolder")),
                                    TimeStamp = reader.IsDBNull(reader.GetOrdinal("TimeStamp")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("TimeStamp")),
                                    StatusactionMappingid = reader.IsDBNull(reader.GetOrdinal("StatusActionsMappingId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StatusActionsMappingId")),
                                   
                                };
                                bool isAllowedUnit =
               Logins.unitid == 1 ||
               Logins.unitid == 2 ||
               Logins.unitid == 3 ||
               Logins.unitid == 4 ||
               Logins.unitid == 5 ||
               Logins.unitid == stakeHolderId;

                                item.isSponsor = isAllowedUnit;
                                item.HasAttachment = reader.GetInt32(reader.GetOrdinal("HasAttachment")) == 1;
                                if (item.StatusactionMappingid == 53)
                                {
                                    item.StakeHolderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId"));
                                    if (isAllowedUnit)
                                    item.ApprovedDt = reader.IsDBNull(reader.GetOrdinal("approveddt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("approveddt"));
                                    item.ApprovedRemarks = reader.IsDBNull(reader.GetOrdinal("ApprovedRemarks")) ? null : reader.GetString(reader.GetOrdinal("ApprovedRemarks"));
                                }
                                else
                                {

                                }

                                item.EncyID = _dataProtector.Protect(item.ProjId.ToString());

                                lst.Add(item);
                            }
                        }
                    }
                }

                return lst.OrderByDescending(i => i.TimeStamp).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            #endregion
        }
        public async Task<List<DTOProjectsFwd>> GetDashboardStatusDetails(int StatuId, int UnitId, bool IsDuplicate)
        {
            try {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                List<DTOProjectsFwd> lst = new List<DTOProjectsFwd>();

                if (Logins != null)
                {
                    #region GetDashboardStatusDetialsWithLinq
                    int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                    string username = Logins.UserName;

                    if (StatuId == 2 || StatuId == 3 || StatuId == 22 || StatuId == 31 || StatuId == 37)
                    {
                        int[] StatusActionsMappingId = null;
                        if (StatuId == 2)
                            StatusActionsMappingId = new int[] { 4 };
                        else if (StatuId == 3)
                            StatusActionsMappingId = new int[] { 118 };
                        else if (StatuId == 22)
                            StatusActionsMappingId = new int[] { 49, 54 };
                        else if (StatuId == 31)
                            StatusActionsMappingId = new int[] { 64, 69, 74, 79, 84, 89 };
                        else if (StatuId == 37)
                            StatusActionsMappingId = new int[] { 3 };

                        var query = await (from a in _dbContext.Projects
                                           join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                                           join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid into cs1
                                           from stackcs in cs1.DefaultIfEmpty()
                                           join actm in _dbContext.TrnStatusActionsMapping
                                           on
                                           b.StatusActionsMappingId equals actm.StatusActionsMappingId
                                           join d in _dbContext.mStatus on actm.StatusId equals d.StatusId

                                           join k in _dbContext.mActions on actm.ActionsId equals k.ActionsId

                                           join c in _dbContext.tbl_mUnitBranch on b.ToUnitId equals c.unitid into cs
                                           from toUnit in cs.DefaultIfEmpty()

                                           join g in _dbContext.tbl_mUnitBranch on b.FromUnitId equals g.unitid into cg
                                           from fromUnits in cg.DefaultIfEmpty()

                                           join j in _dbContext.mStages on d.StageId equals j.StagesId

                                           join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                                           from eWithComment in fs.DefaultIfEmpty()

                                           let StkStatusId =
                                          (from cr1 in _dbContext.StkComment
                                           join Stdkst in _dbContext.StkStatus on cr1.StkStatusId equals Stdkst.StkStatusId
                                           where cr1.StakeHolderId == b.ToUnitId && cr1.PsmId == b.PsmId
                                           orderby cr1.StkCommentId descending
                                           select cr1.StkStatusId
                                          ).FirstOrDefault()

                                           where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true

                                           && StatusActionsMappingId.Contains(b.StatusActionsMappingId)

                                           orderby a.ProjName, b.DateTimeOfUpdate descending

                                           select new DTOProjectsFwd
                                           {
                                               ProjId = a.ProjId,
                                               PsmIds = b.PsmId,
                                               ProjName = a.ProjName,
                                               StakeHolderId = a.StakeHolderId,
                                               StakeHolder = stackcs.UnitName,

                                               Status = d.Status,
                                               Stage = j.Stages,
                                               FromUnitId = b.FromUnitId,
                                               FromUnitName = fromUnits.UnitName,
                                               ToUnitId = b.ToUnitId,
                                               ToUnitName = toUnit.UnitName,
                                               Action = k.Actions,
                                               TotalDays = 0,
                                               StageId = j.StagesId,
                                               EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                               EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                               IsProcess = a.IsProcess,
                                               IsRead = b.IsRead,
                                               IsComplete = b.IsComplete,
                                               StkStatusId = Convert.ToInt32(StkStatusId),
                                               DateTimeOfUpdate = b.DateTimeOfUpdate,
                                               InitiatedDate = a.InitiatedDate

                                           }).ToListAsync();

                        lst = query;
                    }
                    else
                    {
                        if (IsDuplicate == false)
                        {
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

                                               join j in _dbContext.mStages on d.StageId equals j.StagesId


                                               join f in _dbContext.Comment on b.PsmId equals f.PsmId into fs
                                               from eWithComment in fs.DefaultIfEmpty()

                                               let StkStatusId =
                                              (from cr1 in _dbContext.StkComment
                                               join Stdkst in _dbContext.StkStatus on cr1.StkStatusId equals Stdkst.StkStatusId
                                               where cr1.StakeHolderId == b.ToUnitId && cr1.PsmId == b.PsmId
                                               orderby cr1.StkCommentId descending
                                               select cr1.StkStatusId
                                              ).FirstOrDefault()

                                               let psmiis = (from mov1 in _dbContext.ProjStakeHolderMov
                                                             join stst1 in _dbContext.TrnStatusActionsMapping on mov1.StatusActionsMappingId equals stst1.StatusActionsMappingId
                                                             where mov1.ProjId == a.ProjId && stst1.StatusId == StatuId
                                                             orderby mov1.PsmId descending
                                                             select mov1.PsmId).FirstOrDefault()

                                               where a.IsActive && !a.IsDeleted && b.IsActive && !b.IsDeleted && a.IsSubmited == true

                                                && actm.StatusId == StatuId
                                                && b.PsmId == psmiis
                                               orderby a.ProjName, b.DateTimeOfUpdate descending

                                               select new DTOProjectsFwd
                                               {
                                                   ProjId = a.ProjId,
                                                   PsmIds = b.PsmId,
                                                   ProjName = a.ProjName,
                                                   StakeHolderId = a.StakeHolderId,
                                                   StakeHolder = stackcs.UnitName,

                                                   Status = d.Status,
                                                   Stage = j.Stages,
                                                   FromUnitId = b.FromUnitId,
                                                   FromUnitName = fromUnits.UnitName,
                                                   ToUnitId = b.ToUnitId,
                                                   ToUnitName = toUnit.UnitName,
                                                   Action = k.Actions,
                                                   TotalDays = 0,
                                                   StageId = j.StagesId,
                                                   EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                                   EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                                   IsProcess = a.IsProcess,
                                                   IsRead = b.IsRead,
                                                   IsComplete = b.IsComplete,
                                                   //StkStatusId = Convert.ToInt32(StkStatusId),
                                                   DateTimeOfUpdate = b.DateTimeOfUpdate,
                                                   InitiatedDate = a.InitiatedDate

                                               }).ToListAsync();

                            lst = query;
                        }
                        else
                        {
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

                                               join j in _dbContext.mStages on d.StageId equals j.StagesId


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
                                                        && b.StatusActionsMappingId != 118 && b.StatusActionsMappingId != 4                                                                                        //&& b.ToUnitId == Logins.unitid 
                                                && actm.StatusId == StatuId

                                               orderby a.ProjName, b.DateTimeOfUpdate descending

                                               select new DTOProjectsFwd
                                               {
                                                   ProjId = a.ProjId,
                                                   PsmIds = b.PsmId,
                                                   ProjName = a.ProjName,
                                                   StakeHolderId = a.StakeHolderId,
                                                   StakeHolder = stackcs.UnitName,

                                                   Status = d.Status,
                                                   Stage = j.Stages,
                                                   FromUnitId = b.FromUnitId,
                                                   FromUnitName = fromUnits.UnitName,
                                                   ToUnitId = b.ToUnitId,
                                                   ToUnitName = toUnit.UnitName,
                                                   Action = k.Actions,
                                                   TotalDays = 0,
                                                   StageId = j.StagesId,
                                                   EncyID = _dataProtector.Protect(a.ProjId.ToString()),
                                                   EncyPsmID = _dataProtector.Protect(b.PsmId.ToString()),
                                                   IsProcess = a.IsProcess,
                                                   IsRead = b.IsRead,
                                                   IsComplete = b.IsComplete,
                                                   //StkStatusId = Convert.ToInt32(StkStatusId),
                                                   DateTimeOfUpdate = b.DateTimeOfUpdate,
                                                   InitiatedDate = a.InitiatedDate

                                               }).ToListAsync();


                            lst = query;
                        }
                    }
                    var RETT = lst.OrderByDescending(i => i.DateTimeOfUpdate).ToList();

                    return RETT;
                    #endregion



                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        
        }
        public async Task<bool> ProjectNameExists(tbl_Projects project)
        {
            var ret = await _dbContext.Projects.AnyAsync(i => i.ProjName.Trim().ToUpper() == project.ProjName.Trim().ToUpper() && i.ProjId != project.ProjId);
            return ret;
        }
        public async Task<int> AddProjectAsync(tbl_Projects project, string Remarks)
        {
            try
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
                psmove.Remarks = project.InitialRemark;
                psmove.FromUnitId = Logins.unitid ?? 0;
                psmove.ToUnitId = 1; //  
                psmove.UserDetails = Helper1.LoginDetails(Logins);
                psmove.UpdatedByUserId = Logins.unitid; // change with userid
                psmove.DateTimeOfUpdate = project.InitiatedDate;
                psmove.IsActive = true;

                psmove.EditDeleteDate = DateTime.Now;
                psmove.EditDeleteBy = Logins.unitid;
                psmove.TimeStamp = project.InitiatedDate;
                psmove.IsComplete = false;
                psmove.IsComment = false;
                psmove.IsPullBack = false;
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
                    atthis.DateTimeOfUpdate = DateTime.Now;
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
            catch (Exception ex)
            {
                return nmum.Exception;
            }
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
         

            #region GetActComplettemsAsyncWithProc

            if (Logins != null)
            {
                List<tbl_Projects> result = new List<tbl_Projects>();

                using (SqlConnection conn = new SqlConnection(_dbContext.Database.GetConnectionString()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("usp_GetActCompleteItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UnitId", Logins.unitid ?? 0);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tbl_Projects project = new tbl_Projects
                                {
                                    ProjId = reader.GetInt32(reader.GetOrdinal("ProjId")),
                                    PsmIds = reader.GetInt32(reader.GetOrdinal("PsmIds")),
                                    ProjName = reader.GetString(reader.GetOrdinal("ProjName")),
                                    StakeHolderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId")),
                                    CurrentPslmId = reader.GetInt32(reader.GetOrdinal("CurrentPslmId")),
                                    InitiatedDate = reader.IsDBNull(reader.GetOrdinal("InitiatedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("InitiatedDate")),
                                    CompletionDate = reader.IsDBNull(reader.GetOrdinal("CompletionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletionDate")),
                                    IsWhitelisted = reader.GetOrdinal("IsWhitelisted").ToString(),
                                    InitialRemark = reader.IsDBNull(reader.GetOrdinal("InitialRemark")) ? null : reader.GetString(reader.GetOrdinal("InitialRemark")),
                                    EditDeleteBy = reader.IsDBNull(reader.GetOrdinal("EditDeleteBy")) ? null : reader.GetOrdinal("EditDeleteBy"),
                                    EditDeleteDate = reader.IsDBNull(reader.GetOrdinal("EditDeleteDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EditDeleteDate")),
                                    UpdatedByUserId = reader.IsDBNull(reader.GetOrdinal("UpdatedByUserId")) ? null : reader.GetOrdinal("UpdatedByUserId"),
                                    DateTimeOfUpdate = reader.GetDateTime(reader.GetOrdinal("DateTimeOfUpdate")),
                                    ProjCode = reader.IsDBNull(reader.GetOrdinal("ProjCode")) ? null : reader.GetString(reader.GetOrdinal("ProjCode")),
                                    RecdFmUser = reader.IsDBNull(reader.GetOrdinal("RecdFmUser")) ? null : reader.GetString(reader.GetOrdinal("RecdFmUser")),
                                    FwdtoUser = reader.IsDBNull(reader.GetOrdinal("FwdtoUser")) ? null : reader.GetString(reader.GetOrdinal("FwdtoUser")),
                                    FwdBy = reader.IsDBNull(reader.GetOrdinal("FwdBy")) ? null : reader.GetString(reader.GetOrdinal("FwdBy")),
                                    AdRemarks = reader.IsDBNull(reader.GetOrdinal("AdRemarks")) ? null : reader.GetString(reader.GetOrdinal("AdRemarks")),
                                    Comments = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                                    Stages = reader.IsDBNull(reader.GetOrdinal("Stages")) ? null : reader.GetString(reader.GetOrdinal("Stages")),
                                    Action = reader.IsDBNull(reader.GetOrdinal("Actions")) ? null : reader.GetString(reader.GetOrdinal("Actions")),
                                    StakeHolder = reader.IsDBNull(reader.GetOrdinal("StakeHolder")) ? null : reader.GetString(reader.GetOrdinal("StakeHolder")),
                                    AttCnt = reader.GetInt32(reader.GetOrdinal("AttCnt")),
                                    HostTypeID = reader.GetInt32(reader.GetOrdinal("HostTypeID"))
                                };
                                project.EncyID = _dataProtector.Protect(project.ProjId.ToString());

                                result.Add(project);
                            }
                        }
                    }
                }

                return result;
            }
            else
            {
                return null;
            }
            #endregion
        }

        public async Task<List<tbl_Projects>> GetActDraftItemsAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

          


            #region GetActDraftItemsAsyncWithProc

            var result = new List<tbl_Projects>();

            if (Logins == null)
                return null;
            int unitId = Logins.unitid ?? 0;
            using (SqlConnection connection = new SqlConnection(_dbContext.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "usp_GetActDraftItems";
                command.CommandType = CommandType.StoredProcedure;

                var unitParam = command.CreateParameter();
                unitParam.ParameterName = "@UnitId";
                unitParam.Value = unitId;
                command.Parameters.Add(unitParam);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int psmId = reader.GetInt32(reader.GetOrdinal("PsmIds"));
                    int projId = reader.GetInt32(reader.GetOrdinal("ProjId"));
                    int currentPslmId = reader.GetInt32(reader.GetOrdinal("CurrentPslmId"));

                    result.Add(new tbl_Projects
                    {
                        ProjId = projId,
                        PsmIds = psmId,
                        ProjName = reader["ProjName"]?.ToString(),
                        StakeHolderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId")),
                        CurrentPslmId = currentPslmId,
                        InitiatedDate = reader.GetDateTime(reader.GetOrdinal("InitiatedDate")),
                        CompletionDate = reader.IsDBNull("CompletionDate") ? null : reader.GetDateTime(reader.GetOrdinal("CompletionDate")),
                        IsWhitelisted = reader["IsWhitelisted"]?.ToString(),
                        InitialRemark = reader["InitialRemark"]?.ToString(),
                        EditDeleteBy = (int)reader["EditDeleteBy"],
                        EditDeleteDate = reader.GetDateTime(reader.GetOrdinal("EditDeleteDate")),
                        UpdatedByUserId = (int)reader["UpdatedByUserId"],
                        DateTimeOfUpdate = reader.GetDateTime(reader.GetOrdinal("DateTimeOfUpdate")),
                        ProjCode = reader["ProjCode"]?.ToString(),
                        RecdFmUser = reader["RecdFmUser"]?.ToString(),
                        FwdtoUser = reader["FwdtoUser"]?.ToString(),
                        FwdBy = reader["FwdBy"]?.ToString(),
                        AdRemarks = reader["AdRemarks"]?.ToString(),
                        Comments = reader["Comment"]?.ToString(),
                        Status = reader["Status"]?.ToString(),
                        Stages = reader["Stages"]?.ToString(),
                        StakeHolder = reader["StakeHolder"]?.ToString(),
                        Action = reader["Action"]?.ToString(),
                        TotalDays = 15,
                        AttCnt = reader.GetInt32(reader.GetOrdinal("AttCnt")),
                        HostTypeID = reader.GetInt32(reader.GetOrdinal("HostTypeID")),
                        EncyID = _dataProtector.Protect(currentPslmId.ToString()),
                        EncyPsmID = _dataProtector.Protect(psmId.ToString())
                    });
                }

                return result;
            }

            #endregion

        }

        public async Task<List<DTOProjectsFwd>> GetActInboxAsync()
        {
        

            #region GetActInboxAsyncWithProc
            var result = new List<DTOProjectsFwd>();
            var login = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (login == null)
                return result;

            var unitId = login.unitid ?? 0;
            try
            {
                using (var connection = _dbContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using var command = connection.CreateCommand();
                    command.CommandText = "usp_GetActInbox";
                    command.CommandType = CommandType.StoredProcedure;

                    var param = command.CreateParameter();
                    param.ParameterName = "@UnitId";
                    param.Value = unitId;
                    command.Parameters.Add(param);

                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var projId = reader.GetInt32(reader.GetOrdinal("ProjId"));
                        var psmId = reader.GetInt32(reader.GetOrdinal("PsmIds"));
                        var stakeholderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId"));
                        var unitName = reader["FromUnitUserDetail"]?.ToString() ?? "";
                        var userDetails = reader["UserDetails"]?.ToString() ?? "";

                        result.Add(new DTOProjectsFwd
                        {
                            ProjId = projId,
                            PsmIds = psmId,
                            ProjName = reader["ProjName"]?.ToString(),
                            StakeHolderId = stakeholderId,
                            StakeHolder = reader["StakeHolder"]?.ToString(),
                            Status = reader["Status"]?.ToString(),
                            StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                            FromUnitId = reader.GetInt32(reader.GetOrdinal("FromUnitId")),
                            ToUnitId = reader.GetInt32(reader.GetOrdinal("ToUnitId")),
                            ToUnitName = reader["ToUnitName"]?.ToString(),
                            Action = reader["Action"]?.ToString(),
                            TotalDays = 0,
                            ActionId = reader.GetInt32(reader.GetOrdinal("ActionId")),
                            Sponsor = reader["Sponsor"]?.ToString(),
                            Stage = reader["Stage"]?.ToString(),
                            StageId = reader.GetInt32(reader.GetOrdinal("StageId")),
                            IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead")),
                            IsProcess = reader.GetBoolean(reader.GetOrdinal("IsProcess")),
                            TimeStamp = reader.IsDBNull(reader.GetOrdinal("TimeStamp")) ? null : reader.GetDateTime(reader.GetOrdinal("TimeStamp")),
                            UnitName = _psmRepository.GetSponsorUnitName(stakeholderId),
                            FromUnitUserDetail = unitName,
                            UserDetails = userDetails ?? "",
                            FromUnitName = string.IsNullOrWhiteSpace(userDetails)
        ? unitName
        : $"{unitName} ({userDetails})",
                            EncyID = _dataProtector.Protect(projId.ToString()),
                            EncyPsmID = _dataProtector.Protect(psmId.ToString()),

                            Date_type = (int)reader["Date_type"],
                            AdminApprovalStatus = reader.GetInt32(reader.GetOrdinal("AdminApprovalStatus")) == 1,
                            UserRequest = reader.GetInt32(reader.GetOrdinal("UserRequest")) == 1,
                            //reader.GetBoolean(reader.GetOrdinal("UserRequest")),
                            LatestActionType = reader.IsDBNull(reader.GetOrdinal("LatestActionType")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("LatestActionType")),


                            RequestUnitId = reader.IsDBNull(reader.GetOrdinal("RequestUnitId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("RequestUnitId")),
                            IsCc = (bool)reader["IsCc"],
                            IssentCC = (bool)reader["IssentCC"],
                            CCUnitName = reader["CCUnitName"]?.ToString(),

                            // ✅ New fields
                            LatestRemarks = reader["LatestRemarks"]?.ToString(),
                            HasRemainder1 = reader.GetInt32(reader.GetOrdinal("HasRemainder1")) == 1,
                            RemainderCount = reader.GetInt32(reader.GetOrdinal("RemainderCount"))
                        });


                    }

                    return result.OrderByDescending(x => x.TimeStamp).ToList();
                }
                ;
            }
            catch (Exception ex)
            {

                throw;
            }

            #endregion
        }

        public async Task<List<DTOProjectsFwd>> GetActSendItemsAsync()
        {
          

            #region GetActSendItemsWithProc
try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins == null) return null;

                var lst = new List<DTOProjectsFwd>();
                int unitId = Logins.unitid ?? 0;


                using (SqlConnection conn = new SqlConnection(_dbContext.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetActSendItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UnitId", unitId);


                        await conn.OpenAsync();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        var data = dt;


                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var dto = new DTOProjectsFwd
                                {
                                    ProjId = reader.GetInt32(reader.GetOrdinal("ProjId")),
                                    PsmIds = reader.GetInt32(reader.GetOrdinal("PsmIds")),
                                    ProjName = reader["ProjName"]?.ToString(),
                                    Sponsor = reader["Sponsor"]?.ToString(),
                                    StakeHolderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId")),
                                    StakeHolder = reader["StakeHolder"]?.ToString(),
                                    Stage = reader["Stages"]?.ToString(),
                                    StageId = reader.GetInt32(reader.GetOrdinal("StageId")),
                                    Status = reader["Status"]?.ToString(),
                                    StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                    FromUnitId = reader.GetInt32(reader.GetOrdinal("FromUnitId")),
                                    FromUnitUserDetail = reader["FromUnitUserDetail"]?.ToString(),
                                    FromUnitName = string.IsNullOrWhiteSpace(reader["FromUnitUserDetail"]?.ToString())
    ? reader["UserDetails"]?.ToString()
    : $"{reader["FromUnitUserDetail"]} ({reader["UserDetails"]})",

                                    ToUnitId = reader.GetInt32(reader.GetOrdinal("ToUnitId")),
                                 
                                    ToUnitName = reader["ToUnitName"]?.ToString(),
                                    Action = reader["Actions"]?.ToString(),
                                    ActionId = reader.GetInt32(reader.GetOrdinal("ActionId")),
                                    TotalDays = 0,
                                    TimeStamp = reader["TimeStamp"] != DBNull.Value ? Convert.ToDateTime(reader["TimeStamp"]) : DateTime.MinValue,
                                    IsProcess = reader["IsProcess"] != DBNull.Value && Convert.ToBoolean(reader["IsProcess"]),
                                    IsRead = reader["IsRead"] != DBNull.Value && Convert.ToBoolean(reader["IsRead"]),
                                    IsComplete = reader["IsComplete"] != DBNull.Value && Convert.ToBoolean(reader["IsComplete"]),
                                    IsPullBack = reader["IsPullBack"] != DBNull.Value && Convert.ToBoolean(reader["IsPullBack"]),
                                    UnitName = reader["StakeHolder"]?.ToString(),
                                    EncyID = _dataProtector.Protect(reader["ProjId"].ToString()),
                                    EncyPsmID = _dataProtector.Protect(reader["PsmIds"].ToString()),
                                    PullbackAction = reader["PullbackAction"] != DBNull.Value && Convert.ToBoolean(reader["PullbackAction"]),
                                    IsHosted = Convert.ToInt32(reader["IsHosted"] != DBNull.Value ? Convert.ToInt32(reader["IsHosted"]) : 0),
                                    HasRemainder = reader["HasRemainder"] != DBNull.Value && Convert.ToBoolean(reader["HasRemainder"]),
                                    IsCc = (bool)reader["IsCc"],
                                    HideReminderIcon = reader["PullbackAction"] != DBNull.Value && Convert.ToBoolean(reader["PullbackAction"]),

                                    CCUnitName = reader["CCUnitName"]?.ToString()

                                };

                                lst.Add(dto);
                            }
                        }
                    }
                }

                return lst;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        
            #endregion

        }
        public async Task<List<DTOProjectsFwd>> GetActCcItemsAsync()
        {

            #region GetActSendItemsWithProc
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins == null) return null;

                var lst = new List<DTOProjectsFwd>();
                int unitId = Logins.unitid ?? 0;


                using (SqlConnection conn = new SqlConnection(_dbContext.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetActCcItems", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UnitId", unitId);


                        await conn.OpenAsync();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        var data = dt;


                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var dto = new DTOProjectsFwd
                                {
                                    ProjId = reader.GetInt32(reader.GetOrdinal("ProjId")),
                                    PsmIds = reader.GetInt32(reader.GetOrdinal("PsmIds")),
                                    ProjName = reader["ProjName"]?.ToString(),
                                    Sponsor = reader["Sponsor"]?.ToString(),
                                    StakeHolderId = reader.GetInt32(reader.GetOrdinal("StakeHolderId")),
                                    StakeHolder = reader["StakeHolder"]?.ToString(),
                                    Stage = reader["Stages"]?.ToString(),
                                    StageId = reader.GetInt32(reader.GetOrdinal("StageId")),
                                    Status = reader["Status"]?.ToString(),
                                    StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                    FromUnitId = reader.GetInt32(reader.GetOrdinal("FromUnitId")),
                                    FromUnitUserDetail = reader["FromUnitUserDetail"]?.ToString(),
                                    FromUnitName = string.IsNullOrWhiteSpace(reader["FromUnitUserDetail"]?.ToString())
    ? reader["UserDetails"]?.ToString()
    : $"{reader["FromUnitUserDetail"]} ({reader["UserDetails"]})",
                                  
                                    ToUnitId = reader.GetInt32(reader.GetOrdinal("ToUnitId")),
                                    ToUnitName = reader["ToUnitName"]?.ToString(),
                                    Action = reader["Actions"]?.ToString(),
                                    ActionId = reader.GetInt32(reader.GetOrdinal("ActionId")),
                                    TotalDays = 0,
                                    TimeStamp = reader["TimeStamp"] != DBNull.Value ? Convert.ToDateTime(reader["TimeStamp"]) : DateTime.MinValue,
                                    IsProcess = reader["IsProcess"] != DBNull.Value && Convert.ToBoolean(reader["IsProcess"]),
                                    IsRead = reader["IsRead"] != DBNull.Value && Convert.ToBoolean(reader["IsRead"]),
                                    UnitName = reader["StakeHolder"]?.ToString(),
                                    EncyID = _dataProtector.Protect(reader["ProjId"].ToString()),
                                    EncyPsmID = _dataProtector.Protect(reader["PsmIds"].ToString()),
                                    IsHosted = Convert.ToInt32(reader["IsHosted"] != DBNull.Value ? Convert.ToInt32(reader["IsHosted"]) : 0),
                                    IsCc = (bool)reader["IsCc"],
                                    CCUnitName = reader["CCUnitName"]?.ToString(),
                                    ReadDate = reader["ReadDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReadDate"]) : DateTime.MinValue,
                                    UserDetails = reader["UserDetails"]?.ToString()

                                };

                                lst.Add(dto);
                            }
                        }
                    }

                }


                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
            #endregion

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


        public async Task<List<AddNewProject>> GetMyProjects()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (Logins != null)
            {
                int stkholder = Logins.unitid.HasValue ? Logins.unitid.Value : 0;

                var projects = await _dbContext.AddNewProjects
                    .FromSqlRaw("EXEC GetMyProjects @StakeHolderId", new SqlParameter("@StakeHolderId", stkholder))
                    .ToListAsync();

                foreach (var project in projects)
                {
                    project.EncyID = _dataProtector.Protect(project.CurrentPslmId.ToString());
                }

                return projects;
            }

            return null;
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
        public async Task<tbl_ProjStakeHolderMov> GettXNByPsmIdwithUnitId(int psmId, int UnitID)
        {
            return await _dbContext.ProjStakeHolderMov
               .FirstOrDefaultAsync(a => a.PsmId == psmId && a.ToUnitId == UnitID);
        }

        public async Task<bool> UpdateProjectAsync(tbl_Projects project, string Remarks)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
               
                if (project.Date_type == 1 && project.IsSubmited == true && Remarks!="1")
                {

                    var existpsmid = _dbContext.ProjStakeHolderMov.FirstOrDefault(x => x.PsmId == project.CurrentPslmId);

                    if (existpsmid != null)
                    {
                        existpsmid.TimeStamp = project.InitiatedDate;
                        _dbContext.ProjStakeHolderMov.Update(existpsmid);
                        _dbContext.SaveChanges();
                    }
                    var dateApp = _dbContext.DateApproval
                        .Where(x => x.ProjId == project.ProjId)
                        .OrderByDescending(x => x.ProjId) // Ideally use a DateTime or Id field
                        .FirstOrDefault();

                    if (dateApp == null)
                    {
                        dateApp = new DateApproval
                        {
                            ProjId = project.ProjId,
                            UnitId = Logins.unitid,
                            Request_Date = DateTime.Now,
                            UserRequest = true,
                            User = Helper1.LoginDetails(Logins),
                            IsRead = false,
                            RequestType = 1
                        };
                        _dbContext.DateApproval.Add(dateApp);
                    }
                    else
                    {
                        dateApp.ProjId = project.ProjId;
                        dateApp.UnitId = Logins.unitid;
                        dateApp.Request_Date = DateTime.Now;
                        dateApp.UserRequest = true;
                        dateApp.User = Helper1.LoginDetails(Logins);
                        dateApp.IsRead = false;
                        dateApp.RequestType = 1;

                        _dbContext.DateApproval.Update(dateApp);
                    }

                    _dbContext.SaveChanges();
                    var legacyHistory = _dbContext.LegacyHistory
                        .Where(x => x.ProjectId == project.ProjId)
                        .OrderByDescending(x => x.ProjectId)
                        .FirstOrDefault();

                    if (legacyHistory == null)
                    {
                        legacyHistory = new LegacyHistory
                        {
                            ProjectId = project.ProjId,
                            UnitId = Logins.unitid,
                            FromUnit = Logins.unitid,
                            ActionBy = Logins.Rank + " " + Logins.Offr_Name,
                            ActionType = (LegacyHistory.ActionTypeEnum)1,
                            Remarks = Remarks ?? "No Remarks",
                            ActionDate = DateTime.Now,
                            Userdetails = Helper1.LoginDetails(Logins)
                        };
                        _dbContext.LegacyHistory.Add(legacyHistory);
                    }
                    else
                    {
                        legacyHistory.ProjectId = project.ProjId;
                        legacyHistory.UnitId = Logins.unitid;
                        legacyHistory.FromUnit = Logins.unitid;
                        legacyHistory.ActionBy = Logins.Rank + " " + Logins.Offr_Name;
                        legacyHistory.ActionType = (LegacyHistory.ActionTypeEnum)1;
                        legacyHistory.Remarks = Remarks ?? "No Remarks";
                        legacyHistory.ActionDate = DateTime.Now;
                        legacyHistory.Userdetails = Helper1.LoginDetails(Logins);

                        _dbContext.LegacyHistory.Update(legacyHistory);

                        _dbContext.SaveChanges();
                    }

                }


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
                                         
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                       
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
                                        
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          
                                          AimScope = a.AimScope,
                                          EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),
                                          AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                      }).ToListAsync();

                return projects;
            }
            else
            {

                return null;
            }
        }
        public async Task<List<DToWhiteListeds>> GetWhiteListedActionProj(int TypeId)
        {
            try
            {
                
                var isAllow = _configuration.GetSection("AllowWhiteListedProjSync");
                if (isAllow.Value == "1")
                {
                    var connectionString = _configuration.GetConnectionString("DB");
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("InsertWhiteListedProjects", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                var results = await _dbContext.WhiteListedProjects.FromSqlRaw("EXEC GetLatestWhiteListedProjects @TypeId = {0}", TypeId).ToListAsync();
                return results;
            }
            catch (Exception)
            {

                throw;
            }

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
                                         
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                       
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
                                         
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                          
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
                                     
                                          StakeHolder = eWithUnit.UnitName,
                                          FwdtoUser = curstk.UnitName,
                                          Status = eWithStatus.Status,
                                          Comments = eWithComment.Comment,
                                         
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
                                   
                                      StakeHolder = eWithUnit.UnitName,
                                      FwdtoUser = curstk.UnitName,
                                      Status = eWithStatus.Status,
                                      Comments = eWithComment.Comment,
                                      Deplytype = eWithAppType.AppDesc,
                                 
                                      AimScope = a.AimScope,
                                      ReqmtJustification = a.ReqmtJustification,
                                      EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                      AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                  }).ToListAsync();

            return projects;

          

        }




        public async Task<List<tbl_Projects>> GetProjforCommentsAsync1()
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

          
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
                                     
                                      StakeHolder = eWithUnit.UnitName,
                                      FwdtoUser = curstk.UnitName,
                                      Status = eWithStatus.Status,
                                      Comments = eWithComment.Comment,
                                    
                                      AimScope = a.AimScope,
                                      ReqmtJustification = a.ReqmtJustification,
                                      EncyID = _dataProtector.Protect(a.CurrentPslmId.ToString()),

                                      AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == a.CurrentPslmId)
                                  }).ToListAsync();


            return projects;
          

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

                                 
                                   UnitName = _psmRepository.GetSponsorUnitName(a.StakeHolderId),
                                   FromUnitUserDetail = fromunit.UnitName,
                                   FromUnitName = $"{fromunit.UnitName} {b.UserDetails}",
                                   Remark = b.Remarks,
                                 
                                   ToUnitName = tounit.UnitName,
                                   DateTimeOfUpdate = b.TimeStamp,
                               }).ToListAsync();

            return query;

        }

        public async Task<List<tbl_ProjStakeHolderMov>> GetInboxByProjIdAsync(int projId)
        {
            return await _dbContext.ProjStakeHolderMov
                .Where(a => a.ProjId == projId)
                .ToListAsync();
        }



        public async Task<Notification> GetNotificationByProjId(int ProjId)
        {
            return await _dbContext.Notification
               .FirstOrDefaultAsync(a => a.ProjId == ProjId);

        }


        public async Task<int> GetNotificationCommentCount()
        {
            var loginUser = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            int notificationCount = await _dbContext.Notification
                .Where(n => n.NotificationType == 1 && n.IsRead == false && n.NotificationTo == loginUser.unitid)
                .CountAsync();
            return notificationCount;
        }

        public async Task<bool> UpdateNotification(Notification notify)
        {
            var loginUser = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (loginUser == null)
            {
                return false;
            }

            var existingNotification = await _dbContext.Notification
                .FirstOrDefaultAsync(x => x.NotificationTo == loginUser.unitid && x.ProjId == notify.ProjId && x.NotificationType == 1);

            if (existingNotification != null)
            {
                existingNotification.IsRead = true;
                existingNotification.ReadDateTime = notify.ReadDateTime; // Update ReadDateTime here
                _dbContext.Notification.Update(existingNotification);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUnReadNotification(Notification notify)
        {
            var loginUser = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (loginUser == null)
            {
                return false;
            }

            var existingNotification = await _dbContext.Notification
                .FirstOrDefaultAsync(x => x.NotificationTo == loginUser.unitid && x.ProjId == notify.ProjId);

            if (existingNotification != null)
            {
                existingNotification.IsRead = false;
                existingNotification.ReadDateTime = notify.ReadDateTime; // Update ReadDateTime here
                _dbContext.Notification.Update(existingNotification);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<tbl_ProjStakeHolderMov> GetNextPsmMoveAsync(int projId, int currentPsmId)
        {
            return await _DBContext.ProjStakeHolderMov
                .Where(x => x.ProjId == projId && x.PsmId > currentPsmId)
                .OrderBy(x => x.PsmId)
                .FirstOrDefaultAsync();
        }


        public async Task<List<DTODDLComman>> GetALLByProjectName(string ProjName)
        {
            try
            {
                var GetALL = (from A in _DBContext.Projects
                              where A.ProjName.Contains(ProjName) && A.IsSubmited == true
                              select new DTODDLComman
                              {
                                  Id = A.ProjId,
                                  Name = A.ProjName,
                              }).Take(5).ToList();
                return await Task.FromResult(GetALL);
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public async Task<bool> UpdateNotificationByProjID(Notification notify)
        {
            _dbContext.Notification.Update(notify);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<List<tbl_ProjStakeHolderMov>> GetInboxByProjIdExcludingPsmIdAsync(int projId, int psmId)
        {
            return await _dbContext.ProjStakeHolderMov
                                   .Where(c => c.ProjId == projId && c.PsmId != psmId)
                                   .ToListAsync();
        }


        public async Task<List<tbl_ProjStakeHolderMov>> GetCommentByExcludingPsmId(int projId, int? ToUnitId)
        {
            var comments = await _dbContext.ProjStakeHolderMov
                .Where(n => n.IsComment == true && n.ToUnitId != ToUnitId && n.ProjId == projId) // Replace psmId with toUnitId
                .ToListAsync();

            
            var result = comments

               .OrderBy(n => n.ToUnitId) // Replace psmId with toUnitId
               .ToList();

            return result;

        }

        public async Task<List<tbl_ProjStakeHolderMov>> GetInboxByProjAndUnitId(int projId, int? ToUnitId)
        {
            return await _dbContext.ProjStakeHolderMov
                .Where(a => a.ProjId == projId && a.ToUnitId == ToUnitId)
                .ToListAsync();
        }

        public async Task<int> GetIsCommentPsmiId(int? ProjId, int? StackHolderId)
        {
            var ret = await _dbContext.ProjStakeHolderMov.Where(i => i.ProjId == ProjId && i.IsComment == true && i.ToUnitId == StackHolderId && i.IsActive == true).SingleOrDefaultAsync();
            if (ret != null)
                return ret.PsmId;
            else
                return 0;
        }



        public async Task<tbl_ProjStakeHolderMov> GetProjStkHolderMovmentByPsmiId(int? PsmId)
        {
            var ret = await _dbContext.ProjStakeHolderMov.Where(i => i.PsmId == PsmId).SingleOrDefaultAsync();
            if (ret != null)
                return ret;
            else
                return new tbl_ProjStakeHolderMov();
        }

        public async Task<bool> UpdateProjectStkMovementAsync(tbl_ProjStakeHolderMov project)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    _dbContext.Entry(project).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<ApplicationUser> GetUserByUnitId(int? UnitId)
        {
            var ret = await _dbContext.Users.Where(i => i.unitid == UnitId).FirstOrDefaultAsync();
            if (ret != null)
                return ret;
            else
                return new ApplicationUser();
        }



		public async Task<List<DTOProjectsFwd>> GetActParkedItemsAsync()
		{

			#region GetActSendItemsWithProc
			try
			{
				Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
				if (Logins == null) return null;

				var lst = new List<DTOProjectsFwd>();
				int unitId = Logins.unitid ?? 0;


				using (SqlConnection conn = new SqlConnection(_dbContext.Database.GetConnectionString()))
				{
					using (SqlCommand cmd = new SqlCommand("GetParkedProjects", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@UnitId", unitId);


						await conn.OpenAsync();
						SqlDataAdapter adapter = new SqlDataAdapter(cmd);
						DataTable dt = new DataTable();
						adapter.Fill(dt);
						var data = dt;


						using (var reader = await cmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var dto = new DTOProjectsFwd
								{
									ProjId = reader.GetInt32(reader.GetOrdinal("ProjId")),
									PsmIds = reader.GetInt32(reader.GetOrdinal("PsmIds")), // column name fix
									ProjName = reader["ProjName"]?.ToString(),
									Sponsor = reader["Sponsor"]?.ToString(),
									StakeHolderId = reader["StakeHolderId"] != DBNull.Value ? Convert.ToInt32(reader["StakeHolderId"]) : 0,
									StakeHolder = reader["StakeHolder"]?.ToString(),
									Stage = reader["Stages"]?.ToString(),
									StageId = reader["StageId"] != DBNull.Value ? Convert.ToInt32(reader["StageId"]) : 0,
									Status = reader["Status"]?.ToString(),
									StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
									FromUnitId = reader["FromUnitId"] != DBNull.Value ? Convert.ToInt32(reader["FromUnitId"]) : 0,
									FromUnitUserDetail = reader["FromUnitUserDetail"]?.ToString(),
                                    FromUnitName = string.IsNullOrWhiteSpace(reader["FromUnitUserDetail"]?.ToString())
? reader["UserDetails"]?.ToString()
: $"{reader["FromUnitUserDetail"]} ({reader["UserDetails"]})",
                                    ToUnitId = reader["ToUnitId"] != DBNull.Value ? Convert.ToInt32(reader["ToUnitId"]) : 0,
									ToUnitName = reader["ToUnitName"]?.ToString(),
									Action = reader["Actions"]?.ToString(),
									ActionId = reader["ActionId"] != DBNull.Value ? Convert.ToInt32(reader["ActionId"]) : 0,
									TotalDays = 0,
									TimeStamp = reader["TimeStamp"] != DBNull.Value ? Convert.ToDateTime(reader["TimeStamp"]) : DateTime.MinValue,
									IsProcess = reader["IsProcess"] != DBNull.Value && Convert.ToBoolean(reader["IsProcess"]),
									IsRead = reader["IsRead"] != DBNull.Value && Convert.ToBoolean(reader["IsRead"]),
									UnitName = reader["StakeHolder"]?.ToString(),
									EncyID = _dataProtector.Protect(reader["ProjId"].ToString()),
									EncyPsmID = _dataProtector.Protect(reader["PsmIds"].ToString()),
									IsHosted = reader["IsHosted"] != DBNull.Value ? Convert.ToInt32(reader["IsHosted"]) : 0,
									IsCc = reader["IsCc"] != DBNull.Value && Convert.ToBoolean(reader["IsCc"]),
									CCUnitName = reader["CCUnitName"]?.ToString(),
									ReadDate = reader["ReadDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReadDate"]) : DateTime.MinValue,
									UserDetails = reader["UserDetails"]?.ToString()

									
								};

								lst.Add(dto);
							}
						}
					}

				}


				return lst;
			}
			catch (Exception ex)
			{
				throw;
			}
			#endregion

		}
	

	}

}
