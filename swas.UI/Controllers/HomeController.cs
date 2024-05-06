using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using swas.UI.Models;
using swas.BAL.Helpers;
using swas.BAL.Utility;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using swas.BAL.DTO;
using ASPNetCoreIdentityCustomFields.Data;

using Microsoft.AspNetCore.Identity;
using swas.DAL;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

using iText.Kernel.Pdf;

using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout.Properties;


using Microsoft.AspNetCore.DataProtection;
//using Org.BouncyCastle.Asn1.Ocsp;
using System.Timers;
using Color = iText.Kernel.Colors.Color;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using Rectangle = iText.Kernel.Geom.Rectangle;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;
using MessagePack.Formatters;
using swas.BAL.Repository;
using Org.BouncyCastle.Utilities;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNet.Identity;

namespace swas.UI.Controllers
{
    ///
    ///Developer Name :- Sub Maj M Sanal Kumar
    ///Purpose :-
    ///Authority & Reference :- 
    ///Kind Of Request :- 
    ///Version :- 
    ///Dated :- 29/07/2023
    ///Remarks :- 

    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IProjStakeHolderMovRepository _stkholdmove;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IChartService _chartService;
        private readonly IDataProtector _dataProtector;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IWebHostEnvironment webHostEnvironment;
        private readonly IActionsRepository _ActionsRepository;
        private readonly IAttHistoryRepository _attHistoryRepository;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        private readonly IDdlRepository _dlRepository;
        private readonly ApplicationDbContext _context;
        private readonly IUnitRepository _unitRepository;
        private readonly ICommentRepository _commentRepository;
        DateTime Currentdate = DateTime.Now;
        private System.Timers.Timer aTimer;
        public HomeController(IProjectsRepository projectsRepository, ICommentRepository commentRepository, SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IDdlRepository dlRepository, ApplicationDbContext context, IUnitRepository unitRepository, IProjStakeHolderMovRepository stkholdmove, IChartService chartService, IWebHostEnvironment _webHostEnvironment, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager, IDataProtectionProvider dataProtector, IActionsRepository actionsRepository, IAttHistoryRepository attHistoryRepository)
        {
            //  _logger = logger; _repositoryUser = repositoryUser;
            _projectsRepository = projectsRepository;
            _commentRepository = commentRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            webHostEnvironment = _webHostEnvironment;
            _dlRepository = dlRepository;
            _context = context;
            _unitRepository = unitRepository;
            _stkholdmove = stkholdmove;
            _chartService = chartService;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
            _ActionsRepository = actionsRepository;
            _attHistoryRepository = attHistoryRepository;
        }



        ///Developer Name :- Sub Maj M Sanal Kumar
        ///Revised on :- 01/10/2023
        ///    chart generation corrected df fgdgd
        ///    
       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
                if (Logins == null)
                {
                    return Redirect("/Identity/Account/Login");
                }
                var stgip = 0;
                var stgipwlist = 0;
                var stghwstkhol = 0;
                int uid = 0;
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;

                var dateTime = DateTime.Now;
                var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                ViewBag.stakeholders = stakeholders;

                var Stk_Status = _context.StkStatus.ToList();
                ViewBag.Stk_Status = Stk_Status;

                List<mCommand> cl = await _dlRepository.ddlCommand();

                if (cl == null)
                {
                    cl = new List<mCommand>();
                }

               
                if (cl != null)
                {
                    cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                }
                ViewBag.cl = cl.ToList();

                try
                {
                    if (Logins.IsNotNull())
                    {
                        if (Logins.Role == "StakeHolder")
                        {
                            return Redirect("/Home/newproject");

                        }

                        else
                        {
                            int cnt = await _stkholdmove.CountinboxAsync(Logins.unitid ?? 0);
                            ViewBag.Count = cnt;
                            Logins.totmsgin = cnt;
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Logins);

                            int tocommentin = _context.Notification
                        .Where(notification => notification.IsRead == false)
                        .ToList()
                        .Count();
                            ViewBag.tocommentin = tocommentin;
                            Logins.tocommentin = tocommentin;
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Logins);

                            var projects = await _projectsRepository.GetActProjectsAsync();
                            ViewBag.projects = projects;
                            var sql = "";
                            var sqlsecond = "";

                            if (Logins.Role == "Dte")
                            {
                                sql = @"
                                        SELECT
                                        s.StatusId,
                                        s.StageId,
                                        s.Status,
                                        s.IsDeleted,
                                        s.IsActive,
                                        s.EditDeleteBy,
                                        s.EditDeleteDate,
                                        s.UpdatedByUserId,
                                        s.DateTimeOfUpdate,
                                        s.InitiaalID,
                                        s.FininshID,
                                    s.ViewDescUnit,
                                    s.ViewDescStkHold,
                                    s.Statseq,
                                    
                                    
                                        COALESCE(p.TotalProj, 0) AS TotalProj
                                    FROM
                                        mStatus s
                                    LEFT JOIN
                                        (SELECT t.StatusId, COUNT(p.ProjId) AS TotalProj
                                         FROM ProjStakeHolderMov t
                                         LEFT JOIN projects p ON t.PsmId = p.CurrentPslmId
                                         WHERE  t.ActionCde>0
                                         GROUP BY t.StatusId) p
                                    ON s.StatusId = p.StatusId
                                    
                                    ORDER BY s.Statseq
                                                        ";

                                stgip = (from p in _context.ProjStakeHolderMov
                                         where new[] { 7, 11, 14 }.Contains(p.ActionId) &&
                                               !_context.ProjStakeHolderMov.Any(sub1 => sub1.ProjId == p.ProjId && sub1.StatusId > 19) &&
                                               _context.ProjStakeHolderMov.Any(sub2 => sub2.ProjId == p.ProjId && sub2.ActionCde > 0)
                                         group p by p.ProjId into g
                                         where g.Select(p => p.ActionId).Distinct().Count() == 3
                                         select g.Key).Count();



                                stgipwlist = (from a in _context.Projects
                                              join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into projStakeHolderMovGroup
                                              from b in projStakeHolderMovGroup.DefaultIfEmpty()
                                              where _context.ProjStakeHolderMov
                                                  .Where(psm => psm.StageId == 29 && new[] { 47, 48 }.Contains(psm.ActionId)
                                                 && b.ActionCde > 0
                                                  )
                                                  .Select(psm => psm.ProjId)
                                                  .Contains(a.ProjId)
                                              select a).Count();



                                stghwstkhol = (from sb in _context.Projects
                                               join sa in _context.ProjStakeHolderMov on sb.ProjId equals sa.ProjId into saGroup
                                               from sa in saGroup.DefaultIfEmpty()
                                               where (sa == null || (sa.StatusId == 21 && sa.ActionId == 20))
                                                     && !_context.ProjStakeHolderMov.Any(psh => psh.ProjId == sb.ProjId && psh.StatusId > 21)
                                               group new { sb, sa } by new { sb.ProjId, sb.ProjName } into grouped
                                               select grouped.Count()
                                                ).FirstOrDefault();


                            }
                            else
                            {
                                sql = @"

                                       SELECT
                                           s.StatusId,
                                           s.StageId,
                                           s.Status,
                                           s.IsDeleted,
                                           s.IsActive,
                                           s.EditDeleteBy,
                                           s.EditDeleteDate,
                                           s.UpdatedByUserId,
                                           s.DateTimeOfUpdate,
                                           s.InitiaalID,
                                           s.FininshID,
                                       s.ViewDescUnit,
                                       s.ViewDescStkHold,
                                       s.Statseq,
                                       
                                           COALESCE(p.TotalProj, 0) AS TotalProj
                                       FROM
                                           mStatus s
                                       LEFT JOIN
                                           (SELECT t.StatusId, COUNT(p.ProjId) AS TotalProj
                                            FROM ProjStakeHolderMov t
                                            LEFT JOIN projects p ON t.PsmId = p.CurrentPslmId
                                            WHERE  p.StakeHolderId= " + Logins.unitid + @"
                                       and t.ActionCde>0
                                            GROUP BY t.StatusId) p
                                       ON s.StatusId = p.StatusId
                                       
                                       ORDER BY s.Statseq
                                       
                                       ";


                                stgip = (from p in _context.ProjStakeHolderMov
                                         join project in _context.Projects on p.ProjId equals project.ProjId into projJoin
                                         from projs in projJoin.DefaultIfEmpty()
                                         where new[] { 7, 11, 14 }.Contains(p.ActionId) &&
                                               !_context.ProjStakeHolderMov.Any(sub1 => sub1.ProjId == p.ProjId && sub1.StatusId > 19) &&
                                               _context.ProjStakeHolderMov.Any(sub2 => sub2.ProjId == p.ProjId && sub2.ActionCde > 0) &&
                                               projs.StakeHolderId == Logins.unitid
                                         group p by p.ProjId into g
                                         where g.Select(p => p.ActionId).Distinct().Count() == 3
                                         select g.Key).Count();


                                stgipwlist = (from a in _context.Projects
                                              join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into projStakeHolderMovGroup
                                              from b in projStakeHolderMovGroup.DefaultIfEmpty()
                                              where _context.ProjStakeHolderMov
                                                  .Where(psm => psm.StageId == 29 && new[] { 47, 48 }.Contains(psm.ActionId)
                                                  && a.StakeHolderId == Logins.unitid && b.ActionCde > 0
                                                  )
                                                  .Select(psm => psm.ProjId)
                                                  .Contains(a.ProjId)
                                              select a).Count();

                                stghwstkhol = (from sb in _context.Projects
                                               join sa in _context.ProjStakeHolderMov on sb.ProjId equals sa.ProjId into saGroup
                                               from sa in saGroup.DefaultIfEmpty()
                                               where (sa == null || (sa.StatusId == 21 && sa.ActionId == 20) && sb.StakeHolderId == Logins.unitid)
                                                     && !_context.ProjStakeHolderMov.Any(psh => psh.ProjId == sb.ProjId && psh.StatusId > 21)
                                               group new { sb, sa } by new { sb.ProjId, sb.ProjName } into grouped
                                               select grouped.Count()
                                                ).FirstOrDefault();

                            }


                            List<tbl_mStatus> statuses = new List<tbl_mStatus>();
                            statuses = await _context.mStatus.FromSqlRaw(sql).ToListAsync();
                            List<Resultss> reslt = new List<Resultss>();



                            foreach (var status in statuses)
                            {
                                status.EncID = _dataProtector.Protect(status.StatusId.ToString());

                                try
                                {
                                    if (status != null)
                                    {

                                        if (status.StatusId == 5 || status.StatusId == 7 || status.StatusId == 11)
                                        {

                                            if (status.StatusId == 5)
                                                uid = 4;
                                            if (status.StatusId == 7)
                                                uid = 3;
                                            if (status.StatusId == 11)
                                                uid = 5;

                                            sqlsecond = @"

                                                         SELECT CONVERT(VARCHAR, COUNT(*)) AS Result FROM (
                SELECT ProjName, ProjId, PsmId, StatusId, StageId 
                FROM ProjStakeHolderMov a
                LEFT JOIN Projects b ON a.PsmId = b.CurrentPslmId
                WHERE a.ProjId IN (
                    SELECT aa.ProjId 
                    FROM Projects bb 
                    LEFT JOIN ProjStakeHolderMov aa ON a.PsmId = b.CurrentPslmId AND aa.StatusId = 4
                )
                AND a.ProjId IN (
                    SELECT ProjId 
                    FROM ProjStakeHolderMov 
                    WHERE ActionId IN (
                        SELECT ActionsId 
                        FROM mActions 
                        WHERE StageId = 2 AND StatCompId = " + uid + @" AND ProjId = a.ProjId
                    )
                )
                AND a.ProjId NOT IN (
                    SELECT ProjId 
                    FROM ProjStakeHolderMov 
                    WHERE ActionId IN (
                        SELECT ActionsId 
                        FROM mActions 
                        WHERE StatusId > 15 AND ProjId = a.ProjId
                    )
                )
            ) AS ss";


                                            if (Logins.Role == "Unit")
                                            {
                                                sqlsecond = @"

                                                            select convert(varchar, count(*)) as Result from
                                                            (
                                                            
                                                            select b.ProjName,  a.ProjId,a.PsmId,a.StatusId,a.StageId from ProjStakeHolderMov a
                                                            left join Projects b on a.PsmId=b.CurrentPslmId
                                                            where a.ProjId in 
                                                            (select aa.ProjId from Projects bb 
                                                            left join ProjStakeHolderMov aa on a.PsmId=b.CurrentPslmId and aa.StatusId=4)
                                                            
                                                            and  a.ProjId in 
                                                            (select ProjId from ProjStakeHolderMov where ActionId in
                                                            (select ActionsId from mActions where StageId=2 and StatCompId=" + uid + @" and ProjId=a.ProjId)
                                                            ) and b.StakeHolderId= " + Logins.unitid + @" 
                                                            
                                                            and  a.ProjId not in 
                                                            (select ProjId from ProjStakeHolderMov where ActionId in
                                                            (select ActionsId from mActions where StatusId>15
                                                            
                                                            ) and ProjId=a.ProjId )
                                                            )  as ss
                                                            
                                                            
                                                            ";



                                            }


                                            reslt = await _context.Resultstr.FromSqlRaw(sqlsecond).ToListAsync();

                                            // var matchingStatSecond = statsecond.FirstOrDefault();

                                            if (reslt != null)
                                            {
                                                // string res = reslt.Select(a => a.Result).FirstOrDefault().ToString();
                                                status.TotalProj = int.Parse(reslt.Select(a => a.Result).FirstOrDefault().ToString());
                                            }
                                            else
                                            {
                                                // Handle the case where result.Result is null.
                                                status.TotalProj = 0; // Or any other default value
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string msg = ex.Message;
                                }
                            }

                            //** san


                            List<tbl_viewActionsum> actions = new List<tbl_viewActionsum>();

                            var stg1 = statuses.Where(a => a.StageId == 1).ToList();
                            var stg2 = statuses.Where(a => a.StageId == 2 || a.StageId == 0).ToList();

                            var sumstg2 = stg1
                              .Where(a => a.StageId == 1 && a.StatusId == 4)
                              .Select(b => b.TotalProj).ToList();
                            //** san

                            var sumstg1 = stg1
                            .Where(a => a.StageId == 1 && a.StatusId != 31)
                            .Select(b => b.TotalProj)
                            .Sum();
                           
                                                ViewBag.TotalNew = sumstg1;

                            var stg3 = statuses.Where(a => a.StageId == 3).ToList();
                            ViewBag.Statusone = stg1.ToList();
                            ViewBag.Statustwo = stg2.ToList();
                            ViewBag.Statusthree = stg3.ToList();
                            ViewBag.SumfwdExt = sumstg2.ToList();
                            ViewBag.Sumip = stgip;
                            ViewBag.Whitlst = stgipwlist;
                            ViewBag.PendiStkh = stghwstkhol;

                            int totalSum = stg1.Sum(item => item.TotalProj ?? 0);
                            ViewBag.Statustotone = totalSum;
                            totalSum = stg2.Sum(item => item.TotalProj ?? 0);
                            ViewBag.Statustottwo = totalSum;
                            totalSum = stg3.Sum(item => item.TotalProj ?? 0);
                            ViewBag.Statustotthree = totalSum;

                            sql = "SELECT ActionsId, Actions, total, SUM(total) OVER() as gttotal " +
                                                    "FROM(SELECT e.ActionsId,e.Actions,COUNT(b.ActionId) as total " +
                                                    "FROM mActions e LEFT JOIN ProjStakeHolderMov b ON e.ActionsId = b.actionid " +
                                                    "LEFT JOIN Projects c ON c.CurrentPslmId = b.PsmId WHERE e.StagesId = 1 " +
                                                    "GROUP BY e.ActionsId, e.Actions, e.StagesId) AS subquery";

                            actions = _context.Viewaction.FromSqlRaw(sql).ToList();

                            ViewBag.ActionStgone = actions.ToList();

                            sql = "SELECT ActionsId, Actions, total, SUM(total) OVER() as gttotal " +
                                                     "FROM(SELECT e.ActionsId,e.Actions,COUNT(b.ActionId) as total " +
                                                     "FROM mActions e LEFT JOIN ProjStakeHolderMov b ON e.ActionsId = b.actionid " +
                                                     "LEFT JOIN Projects c ON c.CurrentPslmId = b.PsmId WHERE e.StagesId = 2 " +
                                                     "GROUP BY e.ActionsId, e.Actions, e.StagesId) AS subquery";

                            actions = _context.Viewaction.FromSqlRaw(sql).ToList();
                            ViewBag.ActionStgtwo = actions.ToList();

                            sql = "SELECT ActionsId, Actions, total, SUM(total) OVER() as gttotal " +
                                                  "FROM(SELECT e.ActionsId,e.Actions,COUNT(b.ActionId) as total " +
                                                  "FROM mActions e LEFT JOIN ProjStakeHolderMov b ON e.ActionsId = b.actionid " +
                                                  "LEFT JOIN Projects c ON c.CurrentPslmId = b.PsmId WHERE e.StagesId = 3 " +
                                                  "GROUP BY e.ActionsId, e.Actions, e.StagesId) AS subquery";

                            actions = _context.Viewaction.FromSqlRaw(sql).ToList();
                            ViewBag.ActionStgthree = actions.ToList();



                            ///Developer Name :- Sub Maj M Sanal Kumar

                            ///Revised on :- 08/10/2023

                            ///    proj originator and current user error rectified
                            ///    


                            var proj = await (from a in _context.Projects
                                              join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                              from e in bs.DefaultIfEmpty()
                                              join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                              from eWithStatus in ds.DefaultIfEmpty()
                                              join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                              from eWithUnit in cs.DefaultIfEmpty()

                                              join g in _context.tbl_mUnitBranch on a.StakeHolderId equals g.unitid into psh
                                              from prosh in psh.DefaultIfEmpty()

                                              join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                              from eWithComment in fs.DefaultIfEmpty()
                                              where a.IsActive && !a.IsDeleted && (a.StakeHolderId == a.StakeHolderId || e.FromStakeHolderId == a.StakeHolderId || e.ToStakeHolderId == a.StakeHolderId || e.CurrentStakeHolderId == a.StakeHolderId)

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
                                                  StakeHolder = prosh.UnitName,   
                                                  FwdtoUser = eWithUnit.UnitName,  
                                                  Status = eWithStatus.Status,
                                                  Comments = eWithComment.Comment

                                              }).ToListAsync();


                            ViewBag.proj = proj;


                            // Create dictionaries to store count and data for each status group

                            var statusGroups = proj.GroupBy(p => p.Status);

                            var countDictionary = new Dictionary<string, int>();
                            var statusDataDictionary = new Dictionary<string, List<tbl_Projects>>();

                            try
                            {


                                foreach (var group in statusGroups)
                                {
                                    var statusName = group.Key;

                                    // Check if statusName is null before adding it to dictionaries
                                    if (statusName != null)
                                    {
                                        var projectsInGroup = group.ToList();

                                        // Check if projectsInGroup is null before adding it to dictionaries
                                        if (projectsInGroup != null)
                                        {
                                            countDictionary[statusName] = projectsInGroup.Count;
                                            statusDataDictionary[statusName] = projectsInGroup;
                                        }
                                        else
                                        {
                                            // Handle the case when projectsInGroup is null (if necessary)
                                            // statusDataDictionary[statusName] = new List<Project>();
                                        }
                                    }
                                    else
                                    {
                                        // Handle the case when statusName is null (if necessary)
                                        // For example: statusName = "Unknown Status";
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                string ss = ex.Message;
                            }

                            ViewBag.StatusDataDictionary = statusDataDictionary;
                            ViewBag.CountDictionary = countDictionary;

                            ViewBag.statusName = statusDataDictionary.Keys.ToList();

                            // ViewBag.unitstatus = _projectsRepository.GetStkHolderStatus();
                            List<TimeExceedsAlerts> tlex = await _projectsRepository.GetStkHolderStatus();

                            sql = @"select h.StatusId as ID, count(h.Status) as Projcount, h.Status as Stagescleared from Projects e
                                    left join ProjStakeHolderMov f on e.ProjId = f.ProjId
                                            left join mActions g on f.ActionId = g.ActionsId
                                                    left join mStatus h on f.StatusId = h.StatusId
                                                        where f.ActionId in (

                                                            select b.ActionsId from mStatus a left join
                                                            mActions b on a.StatusId = b.StatCompId
                                                            where b.ActionsId is not null) and h.StatusId !=1
                                                            group by h.Status, h.StatusId";


                            // List<tbl_viewStageSummary> stgsum = await _projectsRepository.GetStageSummary();
                            List<tbl_viewStageSummary> stgsum = new List<tbl_viewStageSummary>();

                            stgsum = _context.StageSummary.FromSqlRaw(sql).ToList();

                            ViewBag.unitstatus = tlex;
                            ViewBag.stgsum = stgsum;
                            ///  end data grid tab

                            return View();

                        }
                    }

                    else
                        return Redirect("/Identity/Account/Login");

                }
                catch (Exception ex)
                {

                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return RedirectToAction("Error", "Home");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ProjComments()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;
                var projects = await _projectsRepository.GetProcProjAsync();
                ViewBag.projects = projects;
                var dateTime = DateTime.Now;

                var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();

                ViewBag.stakeholders = stakeholders;


                var Stk_Status = _context.StkStatus.ToList();
                ViewBag.Stk_Status = Stk_Status;

                var Stk_StatusForDte = _context.StkStatus.Where(s => s.StkStatusId == 4).ToList();
                ViewBag.Stk_StatusForDte = Stk_StatusForDte;


                var queryes = (from comment in _context.StkComment
                               join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                               join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                               from status in statusGroup.DefaultIfEmpty() // Left Join
                               join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                               orderby comment.StkCommentId descending
                               select new
                               {
                                   StakeholderName = stakeholder.UnitName,
                                   StatusName = status != null ? status.Status : null,
                                   Comments = comment.Comments ?? "",
                                   ProjId = comment.ProjId ?? 0, // Include ProjId
                                   PsmId = project.CurrentPslmId,
                                   Date = comment.DateTimeOfUpdate,
                                   CommentId = comment.StkCommentId,
                                   StakeholderId = comment.StakeHolderId

                               }).ToList();


                ViewBag.queryes = queryes;
                StkComment stkcm = new StkComment();

                return View("ProjComments", stkcm);
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }


        ///Developer Name :- Sub Maj M Sanal Kumar

        ///Revised on :- 17 & 23 Sep 2023

        ///    chart generation full dynamic tested and error rectified
        ///    
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> indexToPieChart()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");


                var stageNamesList = _context.mStages.Select(stage => stage.Stages).ToList();
                ViewBag.stageNamesList = stageNamesList;

                if (Logins.IsNotNull())
                {

                    //var g = stageNamesList.GroupBy(i => i);
                    //var countDictionary = new Dictionary<string, int>();

                    //foreach (var grp in g)
                    //{
                    //    countDictionary[grp.Key] = grp.Count();
                    //    Console.WriteLine("{0} {1}", grp.Key, grp.Count());
                    //}

                    var sql = @"
    SELECT
        s.StatusId,
        s.StageId,
        s.Status,
s.IsDeleted,
    s.IsActive,
    s.EditDeleteBy,
    s.EditDeleteDate,
    s.UpdatedByUserId,
    s.DateTimeOfUpdate,
    s.InitiaalID,
    s.FininshID,
s.ViewDescUnit,
s.ViewDescStkHold,
s.Statseq,

        COUNT(p.ProjId) AS TotalProj
    FROM
        mStatus s
    LEFT JOIN
        ProjStakeHolderMov t ON s.StatusId = t.StatusId
    LEFT JOIN
        projects p ON t.PsmId = p.CurrentPslmId AND t.ActionCde > 0
    GROUP BY
        s.StatusId,
        s.StageId,
        s.Status,
s.IsDeleted,
    s.IsActive,
    s.EditDeleteBy,
    s.EditDeleteDate,
    s.UpdatedByUserId,
    s.DateTimeOfUpdate,
    s.InitiaalID,
    s.FininshID,
s.ViewDescUnit,
s.ViewDescStkHold,
s.Statseq

";
                    List<tbl_mStatus> statuses = new List<tbl_mStatus>();

                    statuses = _context.mStatus.FromSqlRaw(sql).ToList();
                    string jsonStatuses = JsonConvert.SerializeObject(statuses); // Serialize to JSON

                    return Content(jsonStatuses, "application/json");

                    //return Json(statuses);
                }

                else
                    return Redirect("/Identity/Account/Login");
            }
            catch (Exception ex)
            {

                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }


        }


        public async Task<IActionResult> indexTo()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");


                if (Logins.IsNotNull())
                {
                    var projects = await _projectsRepository.GetActProjectsAsync();
                    ViewBag.projects = projects;

                    return Json(projects);
                }


                else
                    return Redirect("/Identity/Account/Login");
            }
            catch (Exception ex)
            {

                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }


        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        ///Developer Name :- Sub Maj M Sanal Kumar

        ///Revised on :- 03 & 10 Sep 2023

        ///    chart generation full dynamic tested and error rectified
        ///    
    


        [HttpGet]
        public async Task<IActionResult> NewProject()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
                string userName = TempData["UserName"]?.ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                if (Logins.IsNotNull())
                {
                    var userflag = await _userManager.FindByNameAsync(Logins.UserName);

                    var flag = userflag.Flag;
                    ViewBag.flag = flag;

                  //  bool isUserRegistered = await _projectsRepository.UserRegistered(Logins.UserName); comment by kapoor
                    if (true)
                    {
                        ViewBag.ProcessButtonColor = "green";
                        ViewBag.ButtonText = "Sign In";
                        ViewBag.flag = userflag.Flag;

                    }
                    else
                    {

                        ViewBag.ProcessButtonColor = "red";
                        ViewBag.ButtonText = "Sign Up";
                    }

                  

                    ViewBag.LoggedInUserName = Logins.UserName;

                    var users = await _userManager.FindByNameAsync(Logins.UserName);
                    var list = new List<Users>();

                    if (users != null)
                    {
                        var roles = await _roleManager.FindByIdAsync(users.RoleName);
                        var userRole = roles; // Get the first role found for the user
                        ViewBag.userRole = userRole;

                    }
                    // Pass roles data to the view

                    var data = await _projectsRepository.GetWhitelistedProjAsync();
                    ViewBag.ProjectList = data;
                    ViewBag.WhiteListed = await _projectsRepository.GetWhiteListedActionProj();
                    ViewBag.RecentAction = await _projectsRepository.GetRecentActionProj();
                    ViewBag.HoldProj = await _projectsRepository.GetHoldActionProj();
                    ViewBag.RFPProj = await _projectsRepository.GetHoldRFPProj();

             
                    ViewBag.ipadd = watermarkText;

                    string path = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDF");

                    List<string> Courses = new List<string>();

                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        Courses.Add(file.Replace(path + "\\", ""));
                    }
                    ViewBag.Courses = Courses;
                    return View();
                }
                else
                {
                    bool isUserRegistered = false; // Default value if Logins or UserName is null
                    if (!string.IsNullOrEmpty(userName))
                    {

                        ViewBag.ProcessButtonColor = "red";
                        ViewBag.ButtonText = "Sign Up";

                        ViewBag.LoggedInUserName = userName;
                    }


                    TempData["Tabshift"] = 0;
                    ViewBag.corpsId = 0;
                    List<mCommand> cl = await _dlRepository.ddlCommand();
                    if (cl == null)
                    {
                        cl = new List<mCommand>();
                    }

                    // Insert item only if cl is not null
                    if (cl != null)
                    {
                        cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                    }
                    ViewBag.cl = cl.ToList();

                    List<Types> ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();
                    ViewBag.ty = ty.ToList();

                    var data = await _projectsRepository.GetWhitelistedProjAsync();
                    ViewBag.ProjectList = data;
                    ViewBag.RecentAction = await _projectsRepository.GetRecentActionProj();
                    ViewBag.HoldProj = await _projectsRepository.GetHoldActionProj();
                    ViewBag.RFPProj = await _projectsRepository.GetHoldRFPProj();

                    ViewBag.ipadd = watermarkText;

                    string path = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDF");

                    List<string> Courses = new List<string>();

                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        Courses.Add(file.Replace(path + "\\", ""));
                    }
                    ViewBag.Courses = Courses;

                    return View();
                }

            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                //return Redirect("/Home/Error");
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> UnitAddView()
        {

            ViewBag.corpsId = 0;
            List<mCommand> cl = new List<mCommand>();

            cl = await _dlRepository.ddlCommand();

            cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });

            ViewBag.cl = cl.ToList();

            List<Types> ty = new List<Types>();
            ty = await _dlRepository.ddlType();


            ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
            ViewBag.ty = ty.ToList();
            ViewBag.ty = ty.ToList();

            UnitDtl ude = new UnitDtl();
            ude.UnitName = "";


            return View("_unitdetls", ude);
        }

        ///Developer Name :- Sub Maj M Sanal Kumar

        ///Revised on :- 19,20 & 26, 27 Aug 2023

        ///    New Project Creation with multiple forms and upload errors
        ///    
    


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            swas.BAL.Utility.Error.ExceptionHandle("Request failed & Status Code : " + HttpContext.Response.StatusCode.ToString() ?? Activity.Current?.Id);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        ///Developer Name :- Sub Maj M Sanal Kumar

        ///Revised on :- 19,20 & 26, 27 Aug 2023

        ///   Required code sync before release *****************  imp
        /// 

        public async void ddlGen()
        {

            ViewBag.corpsId = 0;
            List<mCommand> cl = new List<mCommand>();

            cl = await _dlRepository.ddlCommand();

            cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });

            ViewBag.cl = cl.ToList();

            List<Types> ty = new List<Types>();
            ty = await _dlRepository.ddlType();


            ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
            ViewBag.ty = ty.ToList();
            ViewBag.ty = ty.ToList();

        }


        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start <summary>
        /// Created and Reviewed by : Sub Maj Sanal
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult CheckLogin(int r)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            if (Logins.IsNotNull())
            {
                //ModelState.AddModelError("", "Other User Already Login Or Not Proper Logout");
                return Json("1");
            }
            else
            {
                return Json("0");

            }

        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 07 Aug 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start <summary>
        /// Created and Reviewed by : Sub Maj Sanal
        /// </summary>
        /// <returns></returns>

        public IActionResult LogOut()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {

                SessionHelper.ClaerObjectAsJson(HttpContext.Session, "User");
                return View();
            }
            else
                return Redirect("/Identity/Account/Login");
        }
        ///end
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 07 Aug 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start
        //public int GetRoleId(string rolename)
        //{   
        //    int roleid = _repositoryUser.GetRoleId(rolename);
        //    return roleid;

        //}
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 07 Aug 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start


        string filepathpdf = "";
      
        public IActionResult WaterMark2(string id)
        {
            //var stream = new FileStream(@"path\to\file", FileMode.Open);
            //return new FileStreamResult(stream, "application/pdf");
            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                // var filePath = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/PDF/" + id + "");
                var filePath = System.IO.Path.Combine(_env.WebRootPath, "PDF\\" + id + "");
                //  var filePath = System.IO.Path.Combine(_env.WebRootPath, "PDF\\" + id + "");
                filepathpdf = generate2(filePath, ip);

                aTimer = new System.Timers.Timer(60000);
                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += OnTimer;

                aTimer.Enabled = true;
                return Redirect("../../Download/" + filepathpdf + ".pdf");
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                //Comman.ExceptionHandle(ex.Message);
                return Json(0);
            }
        }

        public void OnTimer(Object source, ElapsedEventArgs e)
        {

            try
            {
                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Download/" + filepathpdf + ".pdf");

                if (System.IO.File.Exists(filePath1))
                {
                    // If file found, delete it    

                    System.IO.File.Delete(filePath1);


                }
            }
            catch (Exception ex)
            {
                //Comman.ExceptionHandle(ex.Message);
            }
        }

      
        public string generate2(string Path, string ip)
        {
            try
            {
                Random rnd = new Random();
                string Dfilename = rnd.Next(1, 1000).ToString();


                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Download/" + Dfilename + ".pdf");
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(Path), new PdfWriter(filePath1));
                Document doc = new Document(pdfDoc);
                PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
                Paragraph paragraph = new Paragraph(ip + " " + DateTime.Now).SetFont(font).SetFontSize(30);

                PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.2f);
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfPage pdfPage = pdfDoc.GetPage(i);
                    Rectangle pageSize = pdfPage.GetPageSize();
                    float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                    float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                    PdfCanvas over = new PdfCanvas(pdfPage);
                    over.SaveState();
                    over.SetExtGState(gs1);

                    doc.ShowTextAligned(paragraph, 297, 450, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);

                    over.RestoreState();
                }

                doc.Close();
                return Dfilename;
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                //Comman.ExceptionHandle(ex.Message);
                return "";
            }
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> indexToBarChartS()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins.IsNotNull())
                {

                    List<ChartModelS> chtmod = await _chartService.GetChartDataS();





                    string jsonchtmod = JsonConvert.SerializeObject(chtmod); // Serialize to JSON

                    return Content(jsonchtmod, "application/json");


                }

                else
                    return Redirect("/Identity/Account/Login");
            }
            catch (Exception ex)
            {

                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }

        }



        [Authorize(Policy = "Admin")]

        public async Task<IActionResult> indexToBarChart()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins.IsNotNull())
                {

                    List<ChartModel> chtmod = await _chartService.GetChartData();
                    string jsonchtmod = JsonConvert.SerializeObject(chtmod); // Serialize to JSON

                    return Content(jsonchtmod, "application/json");


                }

                else
                    return Redirect("/Identity/Account/Login");
            }
            catch (Exception ex)
            {

                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }

        }




        [Authorize(Policy = "Admin")]
        public IActionResult StratgicAnalysis()
        {
            return View();
        }

        // Added by Manish  Reviewed by Sub Maj Sanal
        // Reviewed..   Incorrect Stakeholder join corrected && All stkholder comment reqd chjanged on 24 Nov 23
        //  Comment Error Rectified on 20 Nov 23

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetComments(int psmId, int stakeholderId, int projId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                var query = (from comment in _context.StkComment
                             join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                             join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                             from status in statusGroup.DefaultIfEmpty() // Left Join
                             join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                             where comment.ProjId == projId // && comment.StakeHolderId == stakeholderId
                             orderby comment.StkCommentId descending
                             select new
                             {
                                 StakeholderName = stakeholder.UnitName,
                                 StatusName = status != null ? status.Status : null,
                                 Comments = comment.Comments,
                                 ProjId = comment.ProjId,
                                 PsmId = project.CurrentPslmId,
                                 Date = comment.DateTimeOfUpdate,
                                 CommentId = comment.StkCommentId,
                                 StakeholderId = comment.StakeHolderId,
                                 state = comment != null ? comment.Attpath : null

                             }).ToList();

                tbl_Projects stkc = new tbl_Projects();
                stkc = await _projectsRepository.GetProjectByPsmIdAsync(psmId);

                //int cntcomment = await _commentRepository.GetNotificationAsync(projId);

                ViewBag.ProjDetl = stkc;
                ViewBag.query = query.OrderByDescending(s => s.CommentId).ToList();

                var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                ViewBag.stakeholders = stakeholders;


                var notificationsToUpdate = _context.Notification
        .Where(notification => notification.ProjId == projId && notification.IsRead == false && notification.NotificationFrom == stakeholderId)
        .ToList();

                // Update IsRead property to true and set ReadDateTime to current time
                foreach (var notification in notificationsToUpdate)
                {
                    notification.IsRead = true;
                    notification.ReadDateTime = DateTime.Now;
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return partial view with the filtered comments
                return Json(query);


            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        // Created by Manish 
        // Reviewed on 18 Nov by Sub Maj Sanal
        // Revied Purpose --->> Code mode changed.... Upload included for proj  move 

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(StkComment? stkcom, IFormFile uploadfile)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                //try
                //{
                var stkComment = new StkComment
                {
                    StkStatusId = stkcom.StkStatusId,
                    Comments = stkcom.Comments,
                    StakeHolderId = Logins.unitid,
                    ProjId = stkcom.ProjId,
                    PsmId = stkcom.PsmId,
                    DateTimeOfUpdate = DateTime.Now,
                    ActFileName = null,
                    Attpath = null,
                    AttDesc = null

                };



                if (uploadfile == null)
                    ModelState.Remove("uploadfile");

                if (ModelState.IsValid && stkComment.Comments != null)
                {
                    // Model is valid, proceed with processing
                    // ...
                    List<ProjectDetailsDTO> projectDetailsDTOs = new List<ProjectDetailsDTO>();
                    projectDetailsDTOs = await _ActionsRepository.GetNextStgStatAct(stkcom.ProjId, stkcom.PsmId, 2);



                    if (projectDetailsDTOs.Count > 0)
                    {
                        //int? ProjID, int? psmID, int? stgID

                        if ((int)projectDetailsDTOs[0].NextActionId > 0)
                        {
                            if (stkComment.StkStatusId == 1)
                            {
                                // Accepted with comments only  
                                Projmove projm = new Projmove();
                                projm.DataProjId = stkcom.ProjId;
                                projm.ProjMov.ProjId = stkcom.ProjId ?? 0;
                                projm.ProjMov.StakeHolderId = projectDetailsDTOs[0].StkHoldID ?? 0;
                                projm.ProjMov.StageId = projectDetailsDTOs[0].NextStage ?? 0;
                                projm.ProjMov.StatusId = projectDetailsDTOs[0].NextStatus ?? 0;
                                projm.ProjMov.ActionId = projectDetailsDTOs[0].NextActionId ?? 0;
                                projm.ProjMov.ActionDt = DateTime.Now;
                                projm.ProjMov.TostackholderDt = DateTime.Now;
                                projm.ProjMov.AddRemarks = projectDetailsDTOs[0].ActionName ?? "";



                                // att start

                                if (uploadfile != null && uploadfile.Length > 0)
                                {


                                    string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                                    string filePath = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        uploadfile.CopyTo(stream);
                                    }

                                    tbl_AttHistory atthis = new tbl_AttHistory();
                                    atthis.AttPath = uniqueFileName;
                                    atthis.PsmId = stkcom.PsmId ?? 0;
                                    atthis.UpdatedByUserId = Logins.unitid;
                                    atthis.DateTimeOfUpdate = DateTime.Now;
                                    atthis.IsDeleted = false;
                                    atthis.IsActive = true;
                                    atthis.EditDeleteBy = Logins.unitid;
                                    atthis.EditDeleteDate = DateTime.Now;
                                    atthis.ActionId = 0;
                                    atthis.TimeStamp = DateTime.Now;
                                    atthis.Reamarks = Logins.Unit + "  : " + stkcom.Reamarks;
                                    atthis.ActFileName = uploadfile.FileName;
                                    projm.Atthistory.Add(atthis);
                                    projm.Atthistory.RemoveAll(a => a.AttPath == null);
                                    stkComment.ActFileName = uploadfile.FileName;
                                    stkComment.Attpath = uniqueFileName;

                                    // await _attHistoryRepository.AddAttHistoryAsync(atthis);
                                    // TempData["SuccessMessage"] = "New Files Attached  !";

                                }

                                await _stkholdmove.AddProStkMovBlogAsync(projm);
                                await _context.SaveChangesAsync();
                                await _context.StkComment.AddAsync(stkComment);
                                await _context.SaveChangesAsync();

                                var Commentdata = from comment in _context.StkComment
                                                  join branch in _context.tbl_mUnitBranch on comment.StakeHolderId equals branch.unitid
                                                  join project in _context.Projects on comment.ProjId equals project.ProjId
                                                  join projMov in _context.ProjStakeHolderMov on comment.PsmId equals projMov.PsmId
                                                  join StackHolder in _context.tbl_mUnitBranch on comment.StakeHolderId equals StackHolder.unitid
                                                  join actions in _context.mActions on comment.ActionsId equals actions.ActionsId into actionsGroup
                                                  from actions in actionsGroup.DefaultIfEmpty()
                                                  where comment.ProjId == stkcom.ProjId
                                                  select new Notification
                                                  {
                                                      ProjId = (int)comment.ProjId,
                                                      NotificationFrom = (int)comment.StakeHolderId,
                                                      NotificationTo = projMov.StakeHolderId

                                                  };

                                foreach (var item in Commentdata)
                                {
                                    int projId = item.ProjId;


                                    var commentsForProj = await _context.ProjStakeHolderMov
                                                                        .Where(sc => sc.ProjId == projId)
                                                                        .ToListAsync();

                                    var notificationsToAdd = commentsForProj.Select(comment => new Notification
                                    {
                                        ProjId = projId,
                                        NotificationFrom = comment.FromStakeHolderId,
                                        NotificationTo = comment.ToStakeHolderId,
                                        IsRead = false,
                                        ReadDateTime = DateTime.Now,
                                    }).ToList();

                                    _context.Notification.AddRange(notificationsToAdd);
                                }

                                await _context.SaveChangesAsync();
                                // att end


                                TempData["SuccessMessage"] = "Successfully Approved..!";
                                return RedirectToActionPermanent("ProjComments", "Home");
                            }
                            if (stkComment.StkStatusId == 2 || stkComment.StkStatusId == 3 || stkComment.StkStatusId == 4)
                            {

                                if (uploadfile != null && uploadfile.Length > 0)
                                {
                                    string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                                    string filePath = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        uploadfile.CopyTo(stream);
                                    }
                                    stkComment.ActFileName = uploadfile.FileName;
                                    stkComment.Attpath = uniqueFileName;

                                    // await _attHistoryRepository.AddAttHistoryAsync(atthis);
                                    // TempData["SuccessMessage"] = "New Files Attached  !";

                                }

                                TempData["SuccessMessage"] = "Comments Updated..! ";
                                // Obsn and comments only here 
                                _context.StkComment.Add(stkComment);
                                _context.SaveChanges();

                                var Commentdata = from comment in _context.StkComment
                                                  join branch in _context.tbl_mUnitBranch on comment.StakeHolderId equals branch.unitid
                                                  join project in _context.Projects on comment.ProjId equals project.ProjId
                                                  join projMov in _context.ProjStakeHolderMov on comment.PsmId equals projMov.PsmId
                                                  join StackHolder in _context.tbl_mUnitBranch on comment.StakeHolderId equals StackHolder.unitid
                                                  join actions in _context.mActions on comment.ActionsId equals actions.ActionsId into actionsGroup
                                                  from actions in actionsGroup.DefaultIfEmpty()
                                                  where comment.ProjId == stkcom.ProjId
                                                  select new Notification
                                                  {
                                                      ProjId = (int)comment.ProjId,
                                                      NotificationFrom = (int)comment.StakeHolderId,
                                                      NotificationTo = projMov.StakeHolderId

                                                  };



                                foreach (var item in Commentdata)
                                {
                                    int projId = item.ProjId;


                                    var commentsForProj = await _context.ProjStakeHolderMov
                                                                        .Where(sc => sc.ProjId == projId)
                                                                        .ToListAsync();

                                    var notificationsToAdd = commentsForProj.Select(comment => new Notification
                                    {
                                        ProjId = projId,
                                        NotificationFrom = comment.FromStakeHolderId,
                                        NotificationTo = comment.ToStakeHolderId,
                                        IsRead = true,
                                        ReadDateTime = DateTime.Now,
                                    }).ToList();

                                    _context.Notification.AddRange(notificationsToAdd);
                                }

                                await _context.SaveChangesAsync();

                                return RedirectToActionPermanent("ProjComments", "Home");
                            }
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Already Approved..! ";
                            return RedirectToActionPermanent("ProjComments", "Home");
                            // already accepted message here 
                        }
                        TempData["SuccessMessage"] = "Satus Error..! ";
                        return RedirectToActionPermanent("ProjComments", "Home");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Project Not Exist for this action..! ";
                        return RedirectToActionPermanent("ProjComments", "Home");
                    }


                }
                else
                {
                    TempData["SuccessMessage"] = "Comment Not Found..! ";
                    return RedirectToActionPermanent("ProjComments", "Home");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }

        }


        
        public async Task<IActionResult> ProjUnitComments()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;
                var projects = await _projectsRepository.GetProjforCommentsAsync();
                ViewBag.projects = projects;
                var dateTime = DateTime.Now;
                var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                ViewBag.stakeholders = stakeholders;
                var Stk_Status = _context.StkStatus.FirstOrDefault(status => status.StkStatusId == 4);
                ViewBag.Stk_Status = Stk_Status;

                var queryes = (from comment in _context.StkComment
                               join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                               join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                               from status in statusGroup.DefaultIfEmpty() // Left Join
                               join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                               select new
                               {
                                   StakeholderName = stakeholder.UnitName,
                                   StatusName = status != null ? status.Status : null,
                                   Comments = comment.Comments,
                                   ProjId = comment.ProjId, // Include ProjId
                                   PsmId = project.CurrentPslmId,
                                   Date = comment.DateTimeOfUpdate,
                                   CommentId = comment.StkCommentId,
                                   StakeholderId = comment.StakeHolderId
                                   // Include PsmId (assuming it's in the 'Projects' table)
                               }).ToList();


                ViewBag.queryes = queryes;


                return View("ProjUnitComments");
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }

        }

       
        public async Task<IActionResult> GetUnitComments(int psmId, int stakeholderId, int projId)
        {
            // Your existing code for getting comments
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                var query = (from comment in _context.StkComment
                             join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                             join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                             from status in statusGroup.DefaultIfEmpty() // Left Join
                             join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                             where comment.ProjId == projId
                             //where project.CurrentPslmId == psmId && comment.StakeHolderId == stakeholderId
                             orderby comment.StkCommentId descending                                                                     //where project.ProjId == projId
                             select new
                             {
                                 StakeholderName = stakeholder.UnitName,
                                 StatusName = status != null ? status.Status : null,
                                 Comments = comment.Comments,
                                 ProjId = comment.ProjId, // Include ProjId
                                 PsmId = project.CurrentPslmId,
                                 Date = comment.DateTimeOfUpdate,
                                 CommentId = comment.StkCommentId,
                                 StakeholderId = comment.StakeHolderId,
                                 state = comment != null ? comment.Attpath : null

                             }).ToList();


                ViewBag.query = query.OrderByDescending(s => s.CommentId).ToList();

                var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                ViewBag.stakeholders = stakeholders;


                return Json(query);
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUnitStatus(int StatusId, string Comment, int StakeholderId, int ProjId, int PsmID)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                var stkComment = new StkComment
                {
                    StkStatusId = StatusId,
                    Comments = Comment,
                    StakeHolderId = StakeholderId,
                    ProjId = ProjId,
                    PsmId = PsmID,
                    DateTimeOfUpdate = DateTime.Now
                };

                if (ModelState.IsValid && stkComment.Comments != null)
                {
                    // Model is valid, proceed with processing
                    // ...

                    _context.StkComment.Add(stkComment);
                    _context.SaveChanges();
                    var projects = await _projectsRepository.GetActProjectsAsync();
                    ViewBag.projects = projects;

                    var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                    ViewBag.stakeholders = stakeholders;


                    var Stk_Status = _context.StkStatus.ToList();
                    ViewBag.Stk_Status = Stk_Status;

                    var model = _context.StkComment.ToList();
                    ViewBag.model = model;

                    var query = (from comment in _context.StkComment
                                 join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                                 join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                                 from status in statusGroup.DefaultIfEmpty() // Left Join
                                 join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                                 select new
                                 {
                                     StakeholderName = stakeholder.UnitName,
                                     StatusName = status != null ? status.Status : null,
                                     Comments = comment.Comments,
                                     ProjId = comment.ProjId, // Include ProjId
                                     PsmId = project.CurrentPslmId,
                                     Date = comment.DateTimeOfUpdate
                                 }).ToList();

                    ViewBag.query = query;
                }
                else
                {
                    var projects = await _projectsRepository.GetActProjectsAsync();
                    ViewBag.projects = projects;

                    var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                    ViewBag.stakeholders = stakeholders;


                    var Stk_Status = _context.StkStatus.ToList();
                    ViewBag.Stk_Status = Stk_Status;


                    var query = (from comment in _context.StkComment
                                 join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                                 join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                                 from status in statusGroup.DefaultIfEmpty() // Left Join
                                 join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                                 select new
                                 {
                                     StakeholderName = stakeholder.unitid,
                                     StatusName = status != null ? status.Status : null,
                                     Comments = comment.Comments,
                                     ProjId = comment.ProjId, // Include ProjId
                                     PsmId = project.CurrentPslmId // Include PsmId (assuming it's in the 'Projects' table)
                                 }).ToList();

                    ViewBag.query = query;
                    return RedirectToActionPermanent("ProjUnitComments", "Home");

                }
                if (stkComment.StkStatusId > 0)
                {

                    return RedirectToAction("Error", "Home");
                }
                else
                {

                    return RedirectToAction("Error", "Home");

                }

            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return View("Error");
            }
        }


        //   created by ajay for unit comments on 24 Nov 23
        string filepathpdf1 = "";
        
        public IActionResult WaterMark3(string id)
        {
            //var stream = new FileStream(@"path\to\file", FileMode.Open);
            //return new FileStreamResult(stream, "application/pdf");
            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                // var filePath = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Uploads/" + id);
                var filePath = System.IO.Path.Combine(_env.WebRootPath, "Uploads\\" + id + "");
                filepathpdf1 = generate3(filePath, ip);

                aTimer = new System.Timers.Timer(60000);
                // Hook up the Elapsed event for the timer.
                aTimer.Elapsed += OnTimer1;

                aTimer.Enabled = true;
                return Redirect("../../Download/" + filepathpdf1 + ".pdf");
            }
            catch (Exception ex)
            {

                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Json(0);
            }
        }


        //   created by ajay for unit comments on 24 Nov 23
        public void OnTimer1(Object source, ElapsedEventArgs e)
        {

            try
            {
                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot\\Download\\" + filepathpdf + ".pdf");

                // var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Download/" + filepathpdf + ".pdf");

                if (System.IO.File.Exists(filePath1))
                {
                    // If file found, delete it    

                    System.IO.File.Delete(filePath1);


                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
            }
        }


        //   created by ajay for unit comments on 24 Nov 23
        
        public string generate3(string Path, string ip)
        {
            try
            {
                Random rnd = new Random();
                string Dfilename = rnd.Next(1, 1000).ToString();
                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot\\Download\\" + Dfilename + ".pdf");
                // var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Download/" + Dfilename + ".pdf");
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(Path), new PdfWriter(filePath1));
                Document doc = new Document(pdfDoc);
                PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
                Paragraph paragraph = new Paragraph(ip + " " + DateTime.Now).SetFont(font).SetFontSize(30);

                PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.2f);
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfPage pdfPage = pdfDoc.GetPage(i);
                    Rectangle pageSize = pdfPage.GetPageSize();
                    float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                    float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                    PdfCanvas over = new PdfCanvas(pdfPage);
                    over.SaveState();
                    over.SetExtGState(gs1);

                    doc.ShowTextAligned(paragraph, 297, 450, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);

                    over.RestoreState();
                }

                doc.Close();
                return Dfilename;
            }
            catch (Exception ex)
            {

                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return "";
            }
        }


        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> StkHDashbard()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins != null)
                {
                    if (Logins.Role == "Unit")
                    {
                        Login Db = new Login();
                        Db.UserName = Logins.UserName;
                        Db.Comdid = Logins.unitid;
                        Db.Corpsid = Logins.Corpsid;
                        Db.Iamuserid = Logins.Iamuserid;
                        Db.Unit = Logins.Unit;
                        Db.unitid = Logins.unitid;
                        Db.ActualUserName = Logins.ActualUserName;
                        Db.Role = "Dte";

                        SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Db);

                        return Redirect("/Home/Index");
                    }
                    else
                    {


                        return Redirect("/Home/Index");
                    }

                }
                else
                {
                    return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UnitDashbard()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins != null)
                {
                    if (Logins.Role == "Dte")
                    {
                        Login Db = new Login();
                        Db.UserName = Logins.UserName;
                        Db.Comdid = Logins.unitid;
                        Db.Corpsid = Logins.Corpsid;
                        Db.Iamuserid = Logins.Iamuserid;
                        Db.Unit = Logins.Unit;
                        Db.unitid = Logins.unitid;
                        Db.ActualUserName = Logins.ActualUserName;
                        Db.Role = "Unit";

                        SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Db);

                        return Redirect("/Home/Index");
                    }
                    else
                    {
                        Login Db = new Login();
                        Db.UserName = Logins.UserName;
                        Db.Comdid = Logins.unitid;
                        Db.Corpsid = Logins.Corpsid;
                        Db.Iamuserid = Logins.Iamuserid;
                        Db.Unit = Logins.Unit;
                        Db.unitid = Logins.unitid;
                        Db.ActualUserName = Logins.ActualUserName;
                        Db.Role = "Unit";

                        SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Db);

                         return RedirectToActionPermanent("Index", "Home");
                    }

                }
                else
                {
                    return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        public async Task<IActionResult> GetNotification(int stakeHolderId)
        {

            List<Notification> notifications = await _commentRepository.GetNotificationAsync(stakeHolderId);




            return View();
        }




    }
}