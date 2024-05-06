using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using swas.UI.Models;
using swas.BAL.Helpers;
using swas.BAL.Utility;
using swas.BAL.Interfaces;
using swas.BAL.Repository;
using swas.DAL.Models;
using swas.BAL;
using Microsoft.AspNetCore.Authorization;
using swas.BAL.DTO;
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Identity;
using swas.DAL;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.NetworkInformation;
using Humanizer;
using System.Globalization;
using System;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.DataProtection;

namespace swas.UI.Controllers
{
    public class SearchController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IProjectsRepository _projectsRepository;
        //private readonly RepositoryUser _repositoryUser;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        private readonly IDdlRepository _dlRepository;
        private readonly ApplicationDbContext _context;
        private readonly IUnitRepository _unitRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;
        private readonly IProjStakeHolderMovRepository _psmRepository;
        private readonly IAttHistoryRepository _attHistoryRepository;
        private readonly IProjStakeHolderMovRepository _stkholdmove;
        public SearchController(IProjectsRepository projectsRepository, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager, IDdlRepository dlRepository, ApplicationDbContext context, IUnitRepository unitRepository, IHttpContextAccessor httpContextAccessor, IProjStakeHolderMovRepository psmRepository, IAttHistoryRepository attHistoryRepository, IProjStakeHolderMovRepository stkholdmove, IDataProtectionProvider DataProtector)
        {
            //  _logger = logger; _repositoryUser = repositoryUser;
            _projectsRepository = projectsRepository;

            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;

            _dlRepository = dlRepository;
            _context = context;
            _unitRepository = unitRepository;
            _httpContextAccessor = httpContextAccessor;
            _psmRepository = psmRepository;
            _attHistoryRepository = attHistoryRepository;
            _stkholdmove = stkholdmove;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");

        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public IActionResult CurrentSearch()
        {

            var dateTime = DateTime.Now;
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";

                TempData["ipadd"] = watermarkText;

                if (Logins.IsNotNull())
                {
                    int? stakeholderId = Logins.unitid;


                    var proj = _context.Projects.FirstOrDefault();
                    ViewBag.proj = proj;

                    var stack = _context.tbl_mUnitBranch.Select(e => e.UnitName).ToList();
                    ViewBag.stack = stack;

                    var stage = _context.mStages.Select(e => e.Stages).ToList();
                    ViewBag.stage = stage;

                    var Status = _context.mStatus.Select(e => e.Status).ToList();
                    ViewBag.status = Status;

                    var action = _context.mActions.Select(e => e.Actions).ToList();
                    ViewBag.action = action;

                    return View();
                }
                else
                {
                    return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SearchResults(string SearchText, string TimeStampFrom, string TimeStampTo)
        {
            //string SearchText = seo.SearchText;

            //string TimeStampFrom = seo.TimeStampFrom;
            //string TimeStampTo = seo.TimeStampTo;

            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(TimeStampFrom) || string.IsNullOrEmpty(TimeStampTo))
            {
                return BadRequest("Invalid input parameters.");
            }

            if (Logins.IsNotNull())
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";

                TempData["ipadd"] = watermarkText;

                try
                {
                    int? stakeholderId = Logins.unitid;

                    DateTime fromDate;
                    DateTime toDate;

                    // Check if TimeStampFrom and TimeStampTo are not empty or null
                    if (!string.IsNullOrEmpty(TimeStampFrom) && !string.IsNullOrEmpty(TimeStampTo))
                    {
                        fromDate = DateTime.ParseExact(TimeStampFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(TimeStampTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // If no date is selected, set fromDate and toDate to the minimum and maximum possible dates
                        fromDate = DateTime.MinValue;
                        toDate = DateTime.MaxValue;
                    }

                    var projectsQuery = await (from a in _context.Projects
                                               join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                               from e in bs.DefaultIfEmpty()
                                               join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                               from eWithStatus in ds.DefaultIfEmpty()
                                               join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                               from eWithUnit in cs.DefaultIfEmpty()
                                               join g in _context.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                                               from eWithUnits in cg.DefaultIfEmpty()
                                               join j in _context.mStages on e.StageId equals j.StagesId into js
                                               from eWithStages in js.DefaultIfEmpty()
                                               join k in _context.mActions on e.ActionId equals k.ActionsId into ks
                                               from eWithAction in ks.DefaultIfEmpty()
                                               join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                               from eWithComment in fs.DefaultIfEmpty()
                                               where a.IsActive && !a.IsDeleted
                                                    && (string.IsNullOrEmpty(SearchText) || a.ProjName != null && a.ProjName.Contains(SearchText))
                                                    && ((string.IsNullOrEmpty(TimeStampFrom) && string.IsNullOrEmpty(TimeStampTo)) || (e.TimeStamp >= fromDate && e.TimeStamp <= toDate))
                                               orderby e.DateTimeOfUpdate ascending
                                               select new
                                               {
                                                   ProjectId = a.ProjId,
                                                   ProjectName = a.ProjName,
                                                   InitiatedDate = a.InitiatedDate,
                                                   Stage = eWithStages.Stages,
                                                   Status = eWithStatus.Status,
                                                   HeldWith = eWithUnit.UnitName,
                                                   Action = eWithAction.Actions,
                                                   Comment = eWithComment.Comment,

                                               }).ToListAsync();


                    return Json(projectsQuery);
                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                return Json(0);
                //return Redirect("/Identity/Account/Login");
            }
        }


        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> SearchstackResult([FromBody] SearObj seo)
        {


            string[] searchStackHolder = seo.searchStakename ?? Array.Empty<string>();

            string TimeStampFrom = seo.TimeStampFrom;
            string TimeStampTo = seo.TimeStampTo;




            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;

            var dateTime = DateTime.Now;
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (User.Identity.IsAuthenticated)
                {
                    int? stakeholderId = Logins.unitid;

                    DateTime fromDate;
                    DateTime toDate;

                    // Check if TimeStampFrom and TimeStampTo are not empty or null
                    if (!string.IsNullOrEmpty(TimeStampFrom) && !string.IsNullOrEmpty(TimeStampTo))
                    {
                        fromDate = DateTime.ParseExact(TimeStampFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(TimeStampTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        // If no date is selected, set fromDate and toDate to the minimum and maximum possible dates
                        fromDate = DateTime.MinValue;
                        toDate = DateTime.MaxValue;
                    }

                    var projectsQueryheldwith = await (from a in _context.Projects
                                                       join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                                       from e in bs.DefaultIfEmpty()
                                                       join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                                       from eWithStatus in ds.DefaultIfEmpty()
                                                       join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                                       from eWithStakeholder in cs.DefaultIfEmpty()
                                                       join g in _context.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                                                       from eWithstack in cg.DefaultIfEmpty()
                                                       join j in _context.mStages on e.StageId equals j.StagesId into js
                                                       from eWithStages in js.DefaultIfEmpty()
                                                       join k in _context.mActions on e.ActionId equals k.ActionsId into ks
                                                       from eWithAction in ks.DefaultIfEmpty()
                                                       join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                                       from eWithComment in fs.DefaultIfEmpty()
                                                       where a.IsActive
                                                       && !a.IsDeleted

                                                       && ((searchStackHolder == null || searchStackHolder.Length == 0) || searchStackHolder.Contains(eWithstack.UnitName))
                                                       && ((string.IsNullOrEmpty(TimeStampFrom) && string.IsNullOrEmpty(TimeStampTo)) || (e.TimeStamp >= fromDate && e.TimeStamp <= toDate))
                                                       orderby e.DateTimeOfUpdate ascending
                                                       select new
                                                       {
                                                           ProjectId = a.ProjId,
                                                           ProjectName = a.ProjName,
                                                           InitiatedDate = a.InitiatedDate,
                                                           Stage = eWithStages.Stages,
                                                           Status = eWithStatus.Status,
                                                           HeldWith = eWithstack.UnitName,
                                                           Action = eWithAction.Actions,
                                                           Comment = eWithComment.Comment,
                                                           EncyID = _dataProtector.Protect(a.ProjId.ToString())
                                                       }).ToListAsync();

                    return Json(projectsQueryheldwith);
                }
                else
                {
                    return Json(0);
                    //return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> SearchStageResult([FromBody] SearObj seo)
        {
            string[] searchStageResult = seo.searchStakename ?? Array.Empty<string>();
            string TimeStampFrom = seo.TimeStampFrom;
            string TimeStampTo = seo.TimeStampTo;

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;
                var dateTime = DateTime.Now;
                try
                {
                    int? stakeholderId = Logins.unitid;
                    DateTime fromDate;
                    DateTime toDate;
                    // Check if TimeStampFrom and TimeStampTo are not empty or null
                    if (!string.IsNullOrEmpty(TimeStampFrom) && !string.IsNullOrEmpty(TimeStampTo))
                    {
                        fromDate = DateTime.ParseExact(TimeStampFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(TimeStampTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    }
                    else
                    {
                        // If no date is selected, set fromDate and toDate to the minimum and maximum possible dates
                        fromDate = DateTime.MinValue;
                        toDate = DateTime.MaxValue;
                    }

                    var projectsQueryStages = await (from a in _context.Projects
                                                     join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                                     from e in bs.DefaultIfEmpty()
                                                     join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                                     from eWithStatus in ds.DefaultIfEmpty()
                                                     join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                                     from eWithStakeholder in cs.DefaultIfEmpty() // Changed variable name to eWithStakeholder
                                                     join g in _context.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                                                     from eWithstack in cg.DefaultIfEmpty()
                                                     join j in _context.mStages on e.StageId equals j.StagesId into js
                                                     from eWithStages in js.DefaultIfEmpty()
                                                     join k in _context.mActions on e.ActionId equals k.ActionsId into ks
                                                     from eWithAction in ks.DefaultIfEmpty()
                                                     join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                                     from eWithComment in fs.DefaultIfEmpty()
                                                     where a.IsActive
                                                    && !a.IsDeleted

                                                     && ((searchStageResult == null || searchStageResult.Length == 0) || searchStageResult.Contains(eWithStages.Stages))
                                                     && ((string.IsNullOrEmpty(TimeStampFrom) && string.IsNullOrEmpty(TimeStampTo)) || (e.TimeStamp >= fromDate && e.TimeStamp <= toDate))
                                                     orderby e.DateTimeOfUpdate ascending
                                                     select new
                                                     {
                                                         ProjectId = a.ProjId,
                                                         ProjectName = a.ProjName,
                                                         InitiatedDate = a.InitiatedDate,
                                                         Stage = eWithStages.Stages,
                                                         Status = eWithStatus.Status,
                                                         HeldWith = eWithstack.UnitName,
                                                         Action = eWithAction.Actions,
                                                         Comment = eWithComment.Comment,
                                                         EncyID = _dataProtector.Protect(a.ProjId.ToString())
                                                     }).ToListAsync();

                    return Json(projectsQueryStages);

                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                return Json(0);
                //return Redirect("/Identity/Account/Login");
            }
        }

        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> SearchStatusResult([FromBody] SearObj seo)
        {
            string[] searchStatusResult = seo.searchStakename ?? Array.Empty<string>();
            string TimeStampFrom = seo.TimeStampFrom;
            string TimeStampTo = seo.TimeStampTo;

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            var dateTime = DateTime.Now;
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins.IsNotNull())
                {
                    int? stakeholderId = Logins.unitid;

                    DateTime fromDate;
                    DateTime toDate;

                    // Check if TimeStampFrom and TimeStampTo are not empty or null
                    if (!string.IsNullOrEmpty(TimeStampFrom) && !string.IsNullOrEmpty(TimeStampTo))
                    {
                        fromDate = DateTime.ParseExact(TimeStampFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(TimeStampTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // If no date is selected, set fromDate and toDate to the minimum and maximum possible dates
                        fromDate = DateTime.MinValue;
                        toDate = DateTime.MaxValue;
                    }

                    var projectsQueryStatus = await (from a in _context.Projects
                                                     join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                                     from e in bs.DefaultIfEmpty()
                                                     join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                                     from eWithStatus in ds.DefaultIfEmpty()
                                                     join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                                     from eWithUnit in cs.DefaultIfEmpty()
                                                     join g in _context.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                                                     from eWithUnits in cg.DefaultIfEmpty()
                                                     join j in _context.mStages on e.StageId equals j.StagesId into js
                                                     from eWithStages in js.DefaultIfEmpty()
                                                     join k in _context.mActions on e.ActionId equals k.ActionsId into ks
                                                     from eWithAction in ks.DefaultIfEmpty()
                                                     join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                                     from eWithComment in fs.DefaultIfEmpty()
                                                     where a.IsActive && !a.IsDeleted
                                                         && (searchStatusResult == null || searchStatusResult.Length == 0 || searchStatusResult.Contains(eWithStatus.Status))
                                                         && e.TimeStamp >= fromDate
                                                         && e.TimeStamp <= toDate
                                                     orderby e.DateTimeOfUpdate descending
                                                     select new
                                                     {
                                                         ProjectId = a.ProjId,
                                                         ProjectName = a.ProjName,
                                                         InitiatedDate = a.InitiatedDate,
                                                         Stage = eWithStages.Stages,
                                                         Status = eWithStatus.Status,
                                                         HeldWith = eWithUnit.UnitName,
                                                         Action = eWithAction.Actions,
                                                         Comment = eWithComment.Comment,
                                                         EncyID = _dataProtector.Protect(a.ProjId.ToString())
                                                     }).ToListAsync();

                    return Json(projectsQueryStatus);
                }
                else
                {
                    return Json(0);
                    //return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> SearchActionResult([FromBody] SearObj seo)
        {
            string[] searchActionResult = seo.searchStakename ?? Array.Empty<string>();
            string TimeStampFrom = seo.TimeStampFrom;
            string TimeStampTo = seo.TimeStampTo;
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins.IsNotNull())
                {
                    int? stakeholderId = Logins.unitid;

                    DateTime fromDate;
                    DateTime toDate;

                    if (string.IsNullOrWhiteSpace(TimeStampFrom) || string.IsNullOrWhiteSpace(TimeStampTo))
                    {
                        // If TimeStampFrom or TimeStampTo is not provided or empty, set a wide date range to include all dates.
                        fromDate = DateTime.MinValue;
                        toDate = DateTime.MaxValue;
                    }
                    else
                    {
                        fromDate = DateTime.ParseExact(TimeStampFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(TimeStampTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    }

                    var projectsQueryAction = await (from a in _context.Projects
                                                     join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                                     from e in bs.DefaultIfEmpty()
                                                     join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                                     from eWithStatus in ds.DefaultIfEmpty()
                                                     join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                                     from eWithUnit in cs.DefaultIfEmpty()
                                                     join g in _context.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                                                     from eWithUnits in cg.DefaultIfEmpty()
                                                     join j in _context.mStages on e.StageId equals j.StagesId into js
                                                     from eWithStages in js.DefaultIfEmpty()
                                                     join k in _context.mActions on e.ActionId equals k.ActionsId into ks
                                                     from eWithAction in ks.DefaultIfEmpty()
                                                     join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                                     from eWithComment in fs.DefaultIfEmpty()
                                                     where a.IsActive && !a.IsDeleted
                                                         && (searchActionResult == null || searchActionResult.Length == 0 || searchActionResult.Contains(eWithAction.Actions))
                                                        && (string.IsNullOrWhiteSpace(TimeStampFrom) || string.IsNullOrWhiteSpace(TimeStampTo) ||
                                                        (e.TimeStamp >= fromDate && e.TimeStamp <= toDate) &&
                                                         (searchActionResult == null || searchActionResult.Length == 0 || searchActionResult.Any(selectedAction => eWithAction.Actions.Contains(selectedAction)))
                                                           )
                                                     orderby e.DateTimeOfUpdate ascending
                                                     select new
                                                     {
                                                         ProjectId = a.ProjId,
                                                         ProjectName = a.ProjName,
                                                         InitiatedDate = a.InitiatedDate,
                                                         Stage = eWithStages.Stages,
                                                         Status = eWithStatus.Status,
                                                         HeldWith = eWithUnit.UnitName,
                                                         Action = eWithAction.Actions,
                                                         Comment = eWithComment.Comment,
                                                         EncyID = _dataProtector.Protect(a.ProjId.ToString())
                                                     }).ToListAsync();

                    return Json(projectsQueryAction);
                }
                else
                {
                    return Json(0);
                    //return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        ///Created  by : manish & reviewd by Sub Maj Sanal on 18 Jan for ACG.

        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> SearchInitiatedByResult([FromBody] SearObj seo)
        {
            string[] searchStakename = seo.searchStakename ?? Array.Empty<string>();
            string TimeStampFrom = seo.TimeStampFrom;
            string TimeStampTo = seo.TimeStampTo;
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                if (Logins.IsNotNull())
                {
                    int? stakeholderId = Logins.unitid;

                    DateTime fromDate;
                    DateTime toDate;

                    if (string.IsNullOrWhiteSpace(TimeStampFrom) || string.IsNullOrWhiteSpace(TimeStampTo))
                    {
                        // If TimeStampFrom or TimeStampTo is not provided or empty, set a wide date range to include all dates.
                        fromDate = DateTime.MinValue;
                        toDate = DateTime.MaxValue;
                    }
                    else
                    {
                        fromDate = DateTime.ParseExact(TimeStampFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        toDate = DateTime.ParseExact(TimeStampTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    }
                    var projectsQueryStakeholder = await (from a in _context.Projects
                                                          join b in _context.ProjStakeHolderMov on a.CurrentPslmId equals b.PsmId into bs
                                                          from e in bs.DefaultIfEmpty()
                                                          join d in _context.mStatus on e.StatusId equals d.StatusId into ds
                                                          from eWithStatus in ds.DefaultIfEmpty()
                                                          join c in _context.tbl_mUnitBranch on e.CurrentStakeHolderId equals c.unitid into cs
                                                          from eWithStakeholder in cs.DefaultIfEmpty()
                                                          join g in _context.tbl_mUnitBranch on e.FromStakeHolderId equals g.unitid into cg
                                                          from eWithstack in cg.DefaultIfEmpty()
                                                          join j in _context.mStages on e.StageId equals j.StagesId into js
                                                          from eWithStages in js.DefaultIfEmpty()
                                                          join k in _context.mActions on e.ActionId equals k.ActionsId into ks
                                                          from eWithAction in ks.DefaultIfEmpty()
                                                          join f in _context.Comment on a.CurrentPslmId equals f.PsmId into fs
                                                          from eWithComment in fs.DefaultIfEmpty()
                                                          where a.IsActive && !a.IsDeleted
                                                              && (searchStakename == null || searchStakename.Length == 0 || searchStakename.Contains(eWithstack.UnitName))
                                                              && e.TimeStamp >= fromDate
                                                              && e.TimeStamp <= toDate
                                                          orderby e.DateTimeOfUpdate descending
                                                          select new
                                                          {
                                                              ProjectId = a.ProjId,
                                                              ProjectName = a.ProjName,
                                                              InitiatedDate = a.InitiatedDate,
                                                              Stage = eWithStages.Stages,
                                                              Status = eWithStatus.Status,
                                                              StakeholderName = eWithstack.UnitName, // Changed property name to StakeholderName
                                                              Action = eWithAction.Actions,
                                                              Comment = eWithComment.Comment,
                                                              EncyID = _dataProtector.Protect(a.ProjId.ToString())
                                                          }).ToListAsync();

                    return Json(projectsQueryStakeholder);
                }
                else
                {
                    return Json(0);
                    //return Redirect("/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> SearchProjHistory(string userid, int? dataProjId, int? dtaProjID, string? AttPath, int? psmid, string? Projpin)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {

                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                DateTime today = DateTime.Now;
                TempData["ipadd"] = ip + "   " + today;

                if (dataProjId != null)
                {
                    ViewBag.SubmitCde = false;
                }
                else
                {
                    ViewBag.SubmitCde = true;
                }
                ViewBag.PsmId = psmid ?? 0;
                ViewBag.PjIR = Projpin;
                List<tbl_AttHistory> atthis = new List<tbl_AttHistory>();
                if (dtaProjID != null)
                {
                    List<ProjHistory> prohis = await _projectsRepository.GetProjectHistorybyID(dtaProjID);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(dtaProjID ?? 0);


                    if (prohis.Count > 0)
                    {
                        prohis[0].Attachments = AttPath;
                    }

                    atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(psmid ?? 0);
                    prohis[0].Atthistory = atthis;
                    prohis[0].ProjectDetl.Add(projects);

                    return View(prohis);
                }


                ViewBag.DataProjId = dataProjId;
                List<ProjHistory> projHistory = await _projectsRepository.GetProjectHistory(userid);
                if (dataProjId == null && userid != null)
                {
                    if (projHistory == null)
                    {
                        ViewBag.DataProjId = projHistory.Select(a => a.ProjId).FirstOrDefault();

                        return View(new List<ProjHistory>());
                    }
                }
                else if (psmid > 0)
                {
                    int psmId = psmid ?? 0;

                    tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();

                    psmove = await _psmRepository.GetProjStakeHolderMovByIdAsync(psmId);
                    List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID(psmove.ProjId);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(psmove.ProjId);
                    projHist[0].ProjectDetl.Add(projects);
                    ViewBag.DataProjId = projHist.Select(a => a.ProjId).FirstOrDefault();


                    if (projHist != null)
                    {

                        projHist[0].Attachments = AttPath;
                        atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(psmove.PsmId);
                        projHist[0].Atthistory = atthis;
                    }


                    projHist[0].Attachments = AttPath;
                    return View(projHist);
                }
                else if (dataProjId > 0)
                {

                    List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID1(dataProjId);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(dataProjId ?? 0);
                    projHist[0].ProjectDetl.Add(projects);

                    var stholder = await _psmRepository.GetProjStakeHolderMovByIdAsync(projects.CurrentPslmId);
                    if (stholder.ActionCde == 1)
                    {
                        stholder.ActionDt = DateTime.Now;
                        stholder.ActionCde = 2;


                        await _psmRepository.UpdateProjStakeHolderMovAsync(stholder);

                        int cnt = await _stkholdmove.CountinboxAsync(Logins.unitid ?? 0);

                        Logins.totmsgin = cnt;

                        SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Logins);
                    }
                    ViewBag.DataProjId = projHist.Select(a => a.ProjId).FirstOrDefault();
                    return View(projHist);

                }
                else
                {
                    List<ProjHistory> projHist = await _projectsRepository.GetProjectHistory(Logins.UserName);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(projHist[0].ProjId);
                    projHist[0].ProjectDetl.Add(projects);

                    ViewBag.DataProjId = projHist.Select(a => a.ProjId).FirstOrDefault();
                    if (projHist != null)
                        projHist[0].Attachments = AttPath;
                    return View(projHist);

                }




                return null;

            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }

        }

        ///Created and Reviewed by : Sub Maj Sanal
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public IActionResult GetProjLogview()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            List<ProjLogView> ProjLVs = new List<ProjLogView>();
            return View(ProjLVs);


        }
        ///Created and Reviewed by : Sub Maj Sanal
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> GetProjLogview(string fromDate, string toDate)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            List<ProjLogView> ProjLV = await _psmRepository.GetProjLogviewAsync(fromDate, toDate);

            if (ProjLV.IsNotNull())
            {
                return View(ProjLV);
            }
            else
            {
                List<ProjLogView> ProjLVs = new List<ProjLogView>();
                return View(ProjLVs);
            }
        }

    }
}
