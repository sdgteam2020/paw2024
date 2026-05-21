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
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using swas.BAL.Utility;
using static Grpc.Core.ChannelOption;
using static swas.DAL.Models.LegacyHistory;
using Dapper;


namespace swas.BAL.Repository
{
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

        public async Task<List<DTOProjectsFwd>> ProjectMovement(int? ProjectId)
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
                               select new DTOProjectsFwd
                               {
                                   PsmIds = b.PsmId,
                                   Stage = stge.Stages,
                                   StageId = stge.StagesId,
                                   ProjId = a.ProjId,
                                   ProjName = a.ProjName,
                                   StautsForComment = tounit.UnitName + " " + "For Comments",
                                   Status = ststus.Status,
                                   StatusId = ststus.StatusId,
                                   Action = act.Actions,
                                   ActionId = b.StatusActionsMappingId,
                                   FromUnitName = $" {b.UserDetails} ({fromunit.UnitName})",
                                   ToUnitName = tounit.UnitName,
                                   ToUnitId = tounit.unitid,
                                   DateTimeOfUpdate = b.TimeStamp,
                                   Remarks = b.Remarks,
                                   StakeHolderId = a.StakeHolderId,
                                   IsComment = b.IsComment,
                                   AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.PsmId)

                               }).ToListAsync();



            return query;
        }


        public async Task<DTOProjectMovHistory> ProjectMovHistory(int? ProjectId)
        {
            try
            {
                DTOProjectMovHistory lst = new DTOProjectMovHistory();
                var queryforstackholderself = await (from a in _dbContext.Projects
                                                     join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                                                     where a.ProjId == ProjectId && b.IsComment == true
                                                     && a.StakeHolderId == b.ToUnitId //&& b.StatusActionsMappingId == 21
                                                     select new DTOForStackHolderCout
                                                     {
                                                         PsmId = b.PsmId
                                                     }
                                                     ).ToListAsync();




                var query = await (from a in _dbContext.Projects
                                   join b in _dbContext.ProjStakeHolderMov on a.ProjId equals b.ProjId
                                   join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid
                                   join tounit in _dbContext.tbl_mUnitBranch on b.ToUnitId equals tounit.unitid
                                   join fromunit in _dbContext.tbl_mUnitBranch on b.FromUnitId equals fromunit.unitid
                                   join actmap in _dbContext.TrnStatusActionsMapping on b.StatusActionsMappingId equals actmap.StatusActionsMappingId
                                   join ststus in _dbContext.mStatus on actmap.StatusId equals ststus.StatusId
                                   join stge in _dbContext.mStages on ststus.StageId equals stge.StagesId
                                   join act in _dbContext.mActions on actmap.ActionsId equals act.ActionsId

                                   let legacy = _dbContext.LegacyHistory
             .Where(l => l.ProjectId == a.ProjId)
             .OrderByDescending(l => l.HistoryId)
             .FirstOrDefault()
                                   where b.ProjId == ProjectId
                                   orderby b.TimeStamp descending
                                   select new DTOProjectMovHistorypsm
                                   {
                                       PsmId = b.PsmId,
                                       Stages = stge.Stages,
                                      
                                       Status = ststus.Status,
                                       StatusId = ststus.StatusId,
                                       Actions = act.Actions,
                                       FromUnitName = $"{b.UserDetails} ({fromunit.UnitName})",
                                       ToUnitName = tounit.UnitName,
                                       FromUser = "",
                                       ToUser = "",
                                       Date = b.TimeStamp,
                                       Remarks = b.Remarks,
                                       UndoRemarks = b.UndoRemarks,
                                       IsComment = b.IsComment,
                                       AttCnt = _dbContext.AttHistory.Count(f => f.PsmId == b.PsmId),
                                       UserDetails = b.UserDetails,
                                       LatestActionType = legacy != null ? legacy.ActionType : default(ActionTypeEnum),
                                       StageId = stge.StagesId,
                                       ActionsId = act.ActionsId,
                                       FromUnitId = b.FromUnitId,
                                       StakeHolderId = a.StakeHolderId,
                                       IsPulledBack = b.IsPullBack,
                                       StatusActionsMappingId = b.StatusActionsMappingId,
                                       IsCc = b.IsCc,
                                       CcUnits = string.Join(", ",
        _dbContext.ProjStakeHolderCcMov
            .Where(cc => cc.PsmId == b.PsmId)
            .Join(_dbContext.tbl_mUnitBranch,
                  cc => cc.ToCcUnitId,
                  unit => unit.unitid,
                  (cc, unit) => unit.UnitName)
            .ToList()
    )

                                   }).ToListAsync();
                if (queryforstackholderself != null && queryforstackholderself.Count == 2)
                    lst.DTOProjectMovHistorypsmlst = query.Where(i => i.PsmId != queryforstackholderself[1].PsmId).ToList();
                else
                    lst.DTOProjectMovHistorypsmlst = query;
                 var blank = "____";
                var comments = await (from mov in _dbContext.ProjStakeHolderMov
                                      join stk in _dbContext.StkComment on mov.PsmId equals stk.PsmId
                                      join stksts in _dbContext.StkStatus on stk.StkStatusId equals stksts.StkStatusId
                                      where mov.ProjId == ProjectId
                                      select new DTOProjectMovHistorycmd
                                      {
                                          PsmId = mov.PsmId,
                                          Status = stksts.Status,
                                          Comments = stk.Comments,
                                          DateTimeOfUpdate = stk.DateTimeOfUpdate,
                                          UserDetails = stk.UserDetails != null ? stk.UserDetails : blank,



                                      }).ToListAsync();
             
                lst.DTOProjectMovHistorycmdlst = comments;

                var retcc = await (from a in _dbContext.Projects
                                   join b in _dbContext.ProjStakeHolderCcMov on a.ProjId equals b.ProjId
                                   join stackc in _dbContext.tbl_mUnitBranch on a.StakeHolderId equals stackc.unitid
                                   join tounit in _dbContext.tbl_mUnitBranch on b.ToCcUnitId equals tounit.unitid
                                   select new DTOProjectCCHistory
                                   {
                                       PsmId = b.PsmId,
                                       UnitName = tounit.UnitName,
                                       IsRead = b.IsRead,
                                       ReadDate = b.ReadDate,
                                       UserDetails = b.UserDetails != null ? b.UserDetails : blank
                                   }).ToListAsync();
                lst.DTOProjectCCHistorylst = retcc;
                return lst;
            }
            catch(Exception ex)
            {
                throw ex;

            }
         
        }
       public async Task<List<DTOProjectHold>> ProjectHolsTimeCalculate(int ProjectId)
        {
            try
            {
                List<DTOProjectHold> lst = new List<DTOProjectHold>();

                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings") ?? "";

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Database connection string is missing.");

                await using var connection = new SqlConnection(connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@ProjectId", ProjectId, DbType.Int32);

                var databyprojectid = (await connection.QueryAsync<DTOProjectHold>(
                    "dbo.ProjectHolsTimeCalculate_Dapper",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 60
                )).ToList();

                for (int i = 0; i < databyprojectid.Count(); i++)
                {
                    DTOProjectHold db = new DTOProjectHold();
                    db.PsmId = databyprojectid[i].PsmId;

                    if (databyprojectid[i].IsComment == false)
                    {
                        if (i == 0)
                        {
                            db.FromunitId = databyprojectid[i].FromunitId;
                            db.Fromunit = databyprojectid[i].Fromunit;
                            db.TimeStampfrom = databyprojectid[i].TimeStamp;
                            db.IsComment = databyprojectid[i].IsComment;
                            db.IsComplete = databyprojectid[i].IsComplete;

                            if (databyprojectid.Count() == 1)
                            {
                                db.TimeStampTo = DateTime.Now;
                            }

                            db.TounitId = databyprojectid[i].TounitId;
                            db.Tounit = databyprojectid[i].Tounit;
                            db.Status = databyprojectid[i].Status;
                            db.Action = databyprojectid[i].Action;
                            db.UndoRemarks = databyprojectid[i].UndoRemarks;
                            db.StatusId = databyprojectid[i].StatusId;
                        }
                        else
                        {
                            if (lst[0].TimeStampTo == null)
                                lst[0].TimeStampTo = databyprojectid[i].TimeStamp;

                            db.FromunitId = databyprojectid[i].FromunitId;
                            db.Fromunit = databyprojectid[i].Fromunit;
                            db.TimeStampfrom = databyprojectid[i].TimeStamp;
                            db.IsComment = databyprojectid[i].IsComment;
                            db.TimeStampTo = DateTime.Now;
                            db.IsComplete = databyprojectid[i].IsComplete;
                            db.UndoRemarks = databyprojectid[i].UndoRemarks;
                            db.StatusId = databyprojectid[i].StatusId;

                            int j = i;
                            j++;

                            db.TounitId = databyprojectid[i].TounitId;
                            db.Tounit = databyprojectid[i].Tounit;
                            db.Status = databyprojectid[i].Status;
                            db.Action = databyprojectid[i].Action;

                            if (j < databyprojectid.Count())
                            {
                                int psmid1 = databyprojectid[j].PsmId;
                                db.TimeStampTo = databyprojectid[j].TimeStamp;
                            }
                        }
                    }
                    else
                    {
                        db.FromunitId = databyprojectid[i].FromunitId;
                        db.Fromunit = databyprojectid[i].Fromunit;
                        db.TimeStampfrom = databyprojectid[i].TimeStamp;
                        db.UndoRemarks = databyprojectid[i].UndoRemarks;

                        if (databyprojectid[i].LatestCommentDate != null &&
                            databyprojectid[i].IsComment == true)
                        {
                            db.FirstActionDate = databyprojectid[i].FirstActionDate;
                            db.TimeStampTo = databyprojectid[i].LatestCommentDate;
                        }
                        else
                        {
                            db.TimeStampTo = DateTime.Now;
                        }

                        db.IsComment = databyprojectid[i].IsComment;
                        db.FirstStkStatus = databyprojectid[i].FirstStkStatus;
                        db.IsComplete = databyprojectid[i].IsComplete;
                        db.Approveddate = databyprojectid[i].Approveddate;
                        db.RejectedDt = databyprojectid[i].RejectedDt;
                        db.TounitId = databyprojectid[i].TounitId;
                        db.Tounit = databyprojectid[i].Tounit;
                        db.ApprovedStatusId = databyprojectid[i].IsApproved;
                        db.Status = databyprojectid[i].Status;
                        db.Action = databyprojectid[i].Action;
                        db.StkStauts = databyprojectid[i].StkStauts;
                    }

                    lst.Add(db);
                }

                return lst.OrderByDescending(x => x.PsmId).ToList();
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetLastRecProjectMov(int? ProjectId)
        {
            try
            {
              
                var maxPsmIdParameter = new SqlParameter("@MaxPsmId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                _context.Database.ExecuteSqlRaw(
                    "EXEC GetMaxPsmId @ProjectId = {0}, @MaxPsmId = @MaxPsmId OUTPUT",
                    ProjectId,
                    maxPsmIdParameter
                );
                return (int)(maxPsmIdParameter.Value ?? 0);

            }
            catch (Exception ex) { return 0; }
          
        }
        public async Task<int> GetLastRecProjectMovForUnod(int ProjectId, int? TounitId)
        {
            try
            {
               var query =await _context.ProjStakeHolderMov.Where(i => i.ProjId == ProjectId && i.IsActive == true && i.IsComment == false && i.ToUnitId== TounitId).OrderByDescending(i=>i.PsmId).Take(1).Select(i=>i.PsmId).SingleOrDefaultAsync();
                return query;

            }
            catch (Exception ex) { return 0; }

        }
        public async Task<DTODashboard> DashboardCount(int UserId)
        {
            DTODashboard db = new DTODashboard();

            try
            {
                using (var conn = _dbContext.Database.GetDbConnection())
                {
                    if (conn.State != ConnectionState.Open)
                        await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.usp_DashboardCount";
                        cmd.CommandType = CommandType.StoredProcedure;

                        var param = cmd.CreateParameter();
                        param.ParameterName = "@UserId";
                        param.Value = UserId;
                        cmd.Parameters.Add(param);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            try
                            {
                                // ✅ 1st result set
                                db.DTODashboardCountlst = new List<DTODashboardCount>();
                                while (await reader.ReadAsync())
                                {
                                    db.DTODashboardCountlst.Add(new DTODashboardCount
                                    {
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        Stages = reader["Stages"]?.ToString() ?? "",
                                        StagesId = reader["StagesId"] != DBNull.Value ? Convert.ToInt32(reader["StagesId"]) : 0,
                                        Status = reader["Status"]?.ToString() ?? "",
                                        IsComplete = reader["IsComplete"] != DBNull.Value && Convert.ToBoolean(reader["IsComplete"]),
                                        Tot = reader["Tot"] != DBNull.Value ? Convert.ToInt32(reader["Tot"]) : 0,
                                        ActionId = 0
                                    });
                                }

                                // ✅ 2nd result set
                                await reader.NextResultAsync();
                                while (await reader.ReadAsync())
                                {
                                    db.DTODashboardCountlst.Add(new DTODashboardCount
                                    {
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        Stages = reader["Stages"]?.ToString() ?? "",
                                        StagesId = reader["StagesId"] != DBNull.Value ? Convert.ToInt32(reader["StagesId"]) : 0,
                                        Status = reader["Status"]?.ToString() ?? "",
                                        IsComplete = reader["IsComplete"] != DBNull.Value && Convert.ToBoolean(reader["IsComplete"]),
                                        Tot = reader["Tot"] != DBNull.Value ? Convert.ToInt32(reader["Tot"]) : 0,
                                        ActionId = 0
                                    });
                                }

                                // ✅ 3rd result set
                                await reader.NextResultAsync();
                                db.DTODashboardCountlstForAction = new List<DTODashboardCount>();
                                while (await reader.ReadAsync())
                                {
                                    db.DTODashboardCountlstForAction.Add(new DTODashboardCount
                                    {
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        Stages = reader["Stages"]?.ToString() ?? "",
                                        StagesId = reader["StagesId"] != DBNull.Value ? Convert.ToInt32(reader["StagesId"]) : 0,
                                        Status = reader["Status"]?.ToString() ?? "",
                                        IsComplete = reader["IsComplete"] != DBNull.Value && Convert.ToBoolean(reader["IsComplete"]),
                                        Tot = reader["Tot"] != DBNull.Value ? Convert.ToInt32(reader["Tot"]) : 0,
                                        ActionId = reader["ActionId"] != DBNull.Value ? Convert.ToInt32(reader["ActionId"]) : 0
                                    });
                                }

                                // ✅ 4th result set
                                await reader.NextResultAsync();
                                db.DTODashboardHeaderlst = new List<DTODashboardHeader>();
                                while (await reader.ReadAsync())
                                {
                                    db.DTODashboardHeaderlst.Add(new DTODashboardHeader
                                    {
                                        StageId = reader["StageId"] != DBNull.Value ? Convert.ToInt32(reader["StageId"]) : 0,
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        Status = reader["Status"]?.ToString() ?? "",
                                        Stages = reader["Stages"]?.ToString() ?? "",
                                        Icons = reader["Icons"] == DBNull.Value ? null : reader["Icons"].ToString(),
                                        Statseq = reader["Statseq"] != DBNull.Value ? Convert.ToInt32(reader["Statseq"]) : 0
                                    });
                                }

                                // ✅ 5th result set
                                await reader.NextResultAsync();
                                var approvedList = new List<DTOApprovedCount>();

                                while (await reader.ReadAsync())
                                {
                                    approvedList.Add(new DTOApprovedCount
                                    {
                                        ProjId = reader["ProjId"] != DBNull.Value ? Convert.ToInt32(reader["ProjId"]) : 0,
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        StatusActionsMappingId = reader["StatusActionsMappingId"] != DBNull.Value ? Convert.ToInt32(reader["StatusActionsMappingId"]) : 0,
                                        Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0
                                    });
                                }

                                db.DTOApprovedCountlst = approvedList
                                    .GroupBy(p => new { p.StatusId, p.StatusActionsMappingId })
                                    .Select(g => new DTOApprovedCount
                                    {
                                        StatusId = g.Key.StatusId,
                                        StatusActionsMappingId = g.Key.StatusActionsMappingId,
                                        Total = g.Sum(x => x.Total)
                                    }).ToList();

                                // ✅ 6th result set
                                await reader.NextResultAsync();
                                while (await reader.ReadAsync())
                                {
                                    db.DTOApprovedCountlst.Add(new DTOApprovedCount
                                    {
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        Status = reader["Status"]?.ToString() ?? "",
                                        StatusActionsMappingId = reader["StatusActionsMappingId"] != DBNull.Value ? Convert.ToInt32(reader["StatusActionsMappingId"]) : 0,
                                        Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0,
                                        ProjId = 0
                                    });
                                }

                                // ✅ 7th result set
                                await reader.NextResultAsync();
                                while (await reader.ReadAsync())
                                {
                                    db.DTOApprovedCountlst.Add(new DTOApprovedCount
                                    {
                                        StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                        Status = reader["Status"]?.ToString() ?? "",
                                        StatusActionsMappingId = reader["StatusActionsMappingId"] != DBNull.Value ? Convert.ToInt32(reader["StatusActionsMappingId"]) : 0,
                                        Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0,
                                        ProjId = 0
                                    });
                                }

                                // ✅ 8th result set (AI/ML)
                                if (await reader.NextResultAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        db.DTOApprovedCountlst.Add(new DTOApprovedCount
                                        {
                                            StatusId = reader["StatusId"] != DBNull.Value ? Convert.ToInt32(reader["StatusId"]) : 0,
                                            Status = reader["Status"]?.ToString() ?? "",
                                            StatusActionsMappingId = reader["StatusActionsMappingId"] != DBNull.Value ? Convert.ToInt32(reader["StatusActionsMappingId"]) : 0,
                                            Total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0,
                                            ProjId = 0
                                        });
                                    }
                                }
                            }
                            catch (Exception exReader)
                            {
                                throw new Exception("Error while reading dashboard data from database", exReader);
                            }
                        }
                    }
                }

                // ✅ Sorting
                db.DTODashboardCountlst = db.DTODashboardCountlst
                    .OrderBy(x => x.StagesId)
                    .ThenBy(x => x.StatusId)
                    .ToList();

                db.DTODashboardHeaderlst = db.DTODashboardHeaderlst
                    .OrderBy(x => x.Statseq)
                    .ThenBy(x => x.StageId)
                    .ToList();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while fetching dashboard data", sqlEx);
            }
            catch (InvalidOperationException invOpEx)
            {
                throw new Exception("Connection or command execution error", invOpEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in DashboardCount method", ex);
            }

            return db;
        }



        public async Task<DTOChartSummarylist> CreateChartSummary(int UserId)
        {
            try
            {
                DTOChartSummarylist lst = new DTOChartSummarylist();
                using (var conn = _dbContext.Database.GetDbConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.usp_DashboardChartSummary";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = UserId });

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            List<DTOChartSummary> lstdb = new List<DTOChartSummary>();
                            while (await reader.ReadAsync())
                            {
                                DTOChartSummary db = new DTOChartSummary();
                                db.Name = Convert.ToString(reader["Year"]);
                                db.Total = Convert.ToInt32(reader["Total"]);
                                lstdb.Add(db);
                            }
                            lst.ProjectStatus=lstdb;
                            if (await reader.NextResultAsync())
                            {
                                List<DTOChartSummary> lstdbApproved = new List<DTOChartSummary>();
                                while (await reader.ReadAsync())
                                {
                                    DTOChartSummary db = new DTOChartSummary();
                                    db.Name = Convert.ToString(reader["Status"]);
                                    db.Total = Convert.ToInt32(reader["Total"]);
                                    lstdbApproved.Add(db);
                                    
                                }
                                lst.ApprovedProjectsPre = lstdbApproved;
                            }
                            if (await reader.NextResultAsync())
                            {
                                List<DTOChartSummary> lstdbApproved = new List<DTOChartSummary>();
                                while (await reader.ReadAsync())
                                {
                                    DTOChartSummary db = new DTOChartSummary();
                                    db.Name = Convert.ToString(reader["Status"]);
                                    db.Total = Convert.ToInt32(reader["Total"]);
                                    lstdbApproved.Add(db);

                                }
                                lst.ApprovedProjectsPost = lstdbApproved;
                            }
                           
                            if (await reader.NextResultAsync())
                            {
                                List<DTOChartSummary> lstdbWhitelisted = new List<DTOChartSummary>();
                                while (await reader.ReadAsync())
                                {
                                    DTOChartSummary db = new DTOChartSummary();
                                    db.Name = Convert.ToString(reader["Year"]);
                                    db.Total = Convert.ToInt32(reader["Total"]);
                                    lstdbWhitelisted.Add(db);
                                }
                                lst.WhitelistedProjects = lstdbWhitelisted;
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
        }

        public async Task<bool> CheckFwdCondition(int ProjId, int StatusId, string Actionsname)
        {
            
            if (Actionsname != "Info")
            {
                if (StatusId != 1)
                {
                   
                    var ret = await (from act in _dbContext.TrnStatusActionsMapping
                                     join sts in _dbContext.mStatus on act.StatusId equals sts.StatusId
                                     join mov in _dbContext.ProjStakeHolderMov on act.StatusActionsMappingId equals mov.StatusActionsMappingId
                                     where act.ActionsId == 2 && act.StatusId == StatusId && mov.ProjId == ProjId
                                     && mov.IsActive == true && mov.UndoRemarks == null
                                     select new TrnStatusActionsMapping
                                     {
                                         StatusActionsMappingId = act.StatusActionsMappingId
                                     }).FirstOrDefaultAsync();
                    if (ret != null)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    var ret = await (from act in _dbContext.TrnStatusActionsMapping
                                     join sts in _dbContext.mStatus on act.StatusId equals sts.StatusId
                                     join mov in _dbContext.ProjStakeHolderMov on act.StatusActionsMappingId equals mov.StatusActionsMappingId
                                     where act.ActionsId == 1 && mov.ProjId == ProjId && mov.ToUnitId == 1 && mov.IsComment == true
                                     && mov.IsActive == true && mov.UndoRemarks == null
                                     select new TrnStatusActionsMapping
                                     {
                                         StatusActionsMappingId = act.StatusActionsMappingId
                                     }).FirstOrDefaultAsync();
                    if (ret != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
          
        }
        public async Task<tbl_ProjStakeHolderMov> GetProjStakeHolderMovByIdAsync(int psmId)
        {
            return await _dbContext.ProjStakeHolderMov.FindAsync(psmId);
        }
        public async Task<List<tbl_ProjStakeHolderMov>> GetAllProjStakeHolderMovAsync()
        {
            return await _dbContext.ProjStakeHolderMov.ToListAsync();
        }
        public async Task<bool> UpdateProjStakeHolderMovAsync(tbl_ProjStakeHolderMov projStakeHolderMov)
        {
            _dbContext.Entry(projStakeHolderMov).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteProjStakeHolderMovAsync(int psmId)
        {
            var projStakeHolderMov = await _dbContext.ProjStakeHolderMov.FindAsync(psmId);
            if (projStakeHolderMov == null)
                return false;

            _dbContext.ProjStakeHolderMov.Remove(projStakeHolderMov);
            await _dbContext.SaveChangesAsync();
            return true;
        }
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
           .Where(p => p.ProjId == ProjId )
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
             

                #region GetProjLogviewAsyncWithProc
                List<ProjLogView> resultList = new List<ProjLogView>();

                using (SqlConnection conn = new SqlConnection(_dbContext.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetProjLogView", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(startDate));
                        cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(endDate));

                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ProjLogView view = new ProjLogView
                                {
                                    PsmId = Convert.ToInt32(reader["PsmId"]),
                                    ProjId = Convert.ToInt32(reader["ProjId"]),
                                    ProjName = reader["ProjName"].ToString(),
                                    UnitName = reader["UnitName"].ToString(),
                                    Stages = reader["Stages"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    Actions = reader["Actions"].ToString(),
                                    TimeStamp = Convert.ToDateTime(reader["TimeStamp"]),
                                    FwdBy = reader["FwdBy"].ToString(),
                                    FwdTo = reader["FwdTo"].ToString(),
                                    Comment = reader["Comment"]?.ToString(),
                                    AttDocu = reader["AttDocu"]?.ToString(),
                                    Comments = reader["Comments"]?.ToString(),
                                    AddRemarks = reader["Remarks"]?.ToString(),
                                    ActionByUser = Convert.ToInt32(reader["UpdatedByUserId"]),
                                };

                                view.EncyId = _dataProtector.Protect(view.PsmId.ToString());

                                resultList.Add(view);
                            }
                        }
                    }
                }
                return resultList;

                #endregion
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


        public string GetSponsorUnitName(int StakeHolderId)
        {
            try
            {
                var unitName = _context.tbl_mUnitBranch
                                       .Where(i => i.unitid == StakeHolderId)
                                       .Select(i => i.UnitName)
                                       .FirstOrDefault();

                return unitName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<int> GetProjectId(string? ProjName)
        {
        
            int? ProjId  = _dbContext.Projects
           .Where(p => p.ProjName == ProjName)
            .Select(p => (int?)p.ProjId)
           .Max();

            int result = ProjId ?? 0;
            return result;
        }



        public async Task<int> AddNotificationCommentAsync (Notification notifications )
        {
            _dbContext.Notification.Add(notifications);
            return await _dbContext.SaveChangesAsync();

        }

        public async Task<string> CheckPreviousApprovals(
    int statusId,
    int projId,
    int actionsId)
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings") ?? "";

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("Database connection string is missing.");

                await using var connection = new SqlConnection(connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@StatusId", statusId, DbType.Int32);
                parameters.Add("@ProjId", projId, DbType.Int32);
                parameters.Add("@ActionsId", actionsId, DbType.Int32);

                var result = await connection.QueryFirstOrDefaultAsync<string>(
                    "dbo.CheckPreviousApprovals_Dapper",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 60
                );

                return string.IsNullOrWhiteSpace(result) ? "OK" : result;
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
