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
using System.Timers;
using Color = iText.Kernel.Colors.Color;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using Rectangle = iText.Kernel.Geom.Rectangle;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;

using swas.BAL.Repository;
using Org.BouncyCastle.Utilities;
using iText.Commons.Actions.Contexts;

using iText.StyledXmlParser.Jsoup.Nodes;
using Grpc.Core;
using static swas.DAL.Models.LegacyHistory;
using swas.UI.Helpers;
using iText.Kernel.XMP.Impl;
using System.Security.Cryptography.X509Certificates;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Layout.Borders;
using System.Net;

namespace swas.UI.Controllers
{

    public class HomeController : Controller
    {
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
        private readonly ILogger<HomeController> _logger;
        private readonly IDateApprovalRepository _repo;
        private readonly ILegacyHistoryRepository _legacyHistoryRepository;
        private readonly IWatermarkRepository _watermarkRepo;

        public HomeController(IWatermarkRepository watermark,IProjectsRepository projectsRepository, ICommentRepository commentRepository, SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IDdlRepository dlRepository, ApplicationDbContext context, IUnitRepository unitRepository, IProjStakeHolderMovRepository stkholdmove, IChartService chartService, IWebHostEnvironment _webHostEnvironment, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager, IDataProtectionProvider dataProtector, IActionsRepository actionsRepository, IAttHistoryRepository attHistoryRepository, ILogger<HomeController> logger, IDateApprovalRepository repo, ILegacyHistoryRepository legacyHistoryRepository)
        {
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
            _logger = logger;
            _repo = repo;
            _legacyHistoryRepository = legacyHistoryRepository;
			_watermarkRepo = watermark;
        }

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
                ViewBag.UnitId = Logins?.unitid;
                return View();
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }




        public async Task<IActionResult> GetDashboardStatusDetails(int StatusId, bool IsDuplicate)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            var ss = await _projectsRepository.GetDashboardStatusDetails(StatusId, Convert.ToInt32(Logins.unitid), IsDuplicate);

            return Json(ss);

        }

        public async Task<IActionResult> GetDashboardApproved(int StatusId, int statusActionsMappingId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            var ss = await _projectsRepository.GetDashboardApproved(StatusId, statusActionsMappingId);

            return Json(ss);

        }
        [HttpPost]
        public async Task<IActionResult> GetProjectWiseStatus(int Projid)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            var ss = await _projectsRepository.GetProjectWiseStatus(Projid);

            return Json(ss);

        }
        public async Task<IActionResult> GetProjHoldStatus(int ProjId)
        {
            var ss = await _stkholdmove.ProjectHolsTimeCalculate(ProjId);
            return Json(ss);
        }
        public async Task<IActionResult> ProjectWiseReport()
        {
            return View();
        }
        
        public async Task<IActionResult> GetDashboardCount(int Id)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");


                    return Json(await _stkholdmove.DashboardCount(Convert.ToInt32(Logins.unitid)));
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }
        
        public async Task<IActionResult> CreateChartSummary(int Id)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");


                return Json(await _stkholdmove.CreateChartSummary(Convert.ToInt32(Logins.unitid)));
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
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
                StkComment stkcm = new StkComment();

                return View("ProjComments", stkcm);
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        public async Task<IActionResult> GetNotificationInbox(int ProjId)
        {
            List<Notification> notifications = await _commentRepository.GetNotificationInbox(ProjId);
            return View();


        }
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
	s.IsDashboard,
	s.Icon,

s.Statseq,

        COUNT(p.ProjId) AS TotalProj
    FROM
        mStatus s
    LEFT JOIN
        TrnStatusActionsMapping t ON s.StatusId = t.StatusId
		LEFT join 
		ProjStakeHolderMov m on t.StatusActionsMappingId =m.StatusActionsMappingId

    LEFT JOIN
        projects p ON m.PsmId = p.CurrentPslmId AND m.IsActive > 0
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
	
s.Statseq,
s.IsDashboard,
	s.Icon

";
                    List<tbl_mStatus> statuses = new List<tbl_mStatus>();

                    statuses = _context.mStatus.FromSqlRaw(sql).ToList();
                    string jsonStatuses = JsonConvert.SerializeObject(statuses); // Serialize to JSON

                    return Content(jsonStatuses, "application/json");
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
                               ViewBag.Units = await _unitRepository.GetAllUnitAsync();
                    var typeo = _context.mHostType.Select(x => new { x.HostTypeID, x.HostingDesc }).ToList();
                    ViewBag.hosttype = typeo;

                    var users = await _userManager.FindByNameAsync(Logins.UserName);
                    var list = new List<Users>();

                    if (users != null)
                    {
                        var roles = await _roleManager.FindByIdAsync(users.RoleName);
                        var userRole = roles; // Get the first role found for the user
                        ViewBag.userRole = userRole;

                    }
                    ViewBag.WhiteListed = await _projectsRepository.GetWhiteListedActionProj(0);

                    ViewBag.HoldProj = await _projectsRepository.GetHoldActionProj();


                    ViewBag.ipadd = watermarkText;

                    string path = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDF");

                    string path1 = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDFWhiteListed");

                    List<string> WhiteList = new List<string>();

                    List<string> Courses = new List<string>();

                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        Courses.Add(file.Replace(path + "\\", ""));
                    }

                    foreach (string file in Directory.EnumerateFiles(path1, "*", SearchOption.AllDirectories))
                    {
                        WhiteList.Add(file.Replace(path1 + "\\", ""));
                    }

                    ViewBag.PdfWhiteListed = WhiteList;

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
                    if (cl != null)
                    {
                        cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                    }
                    ViewBag.cl = cl.ToList();

                    List<Types> ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();
                    ViewBag.ty = ty.ToList();
                    ViewBag.WhiteListed = await _projectsRepository.GetWhiteListedActionProj(0);
                    ViewBag.HoldProj = await _projectsRepository.GetHoldActionProj();



                    ViewBag.ipadd = watermarkText;

                    string path = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDF");

                    string path1 = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDFWhiteListed");

                    List<string> WhiteList = new List<string>();

                    List<string> Courses = new List<string>();

                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        Courses.Add(file.Replace(path + "\\", ""));
                    }

                    foreach (string file in Directory.EnumerateFiles(path1, "*", SearchOption.AllDirectories))
                    {
                        WhiteList.Add(file.Replace(path1 + "\\", ""));
                    }

                    ViewBag.PdfWhiteListed = WhiteList;

                    ViewBag.Courses = Courses;

                    return View();
                }

            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
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



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            swas.BAL.Utility.Error.ExceptionHandle("Request failed & Status Code : " + HttpContext.Response.StatusCode.ToString() ?? Activity.Current?.Id);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
        [AllowAnonymous]
        public IActionResult CheckLogin(int r)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            if (Logins.IsNotNull())
            {
                return Json("1");
            }
            else
            {
                return Json("0");

            }

        }

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


        string filepathpdf = "";

        public IActionResult WaterMark2(string id)
        {
            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var filePath = System.IO.Path.Combine(_env.WebRootPath, "PDF\\" + id + "");
                filepathpdf = generate2(filePath, ip);

                aTimer = new System.Timers.Timer(60000);
                aTimer.Elapsed += OnTimer;

                aTimer.Enabled = true;
                return Redirect("../../Download/" + filepathpdf + ".pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in HomeController WaterMark2 function.");

                // Log generic context only
                swas.BAL.Utility.Error.ExceptionHandle(
                    "Unhandled exception occurred in HomeController.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong. Please try again later."
                });
            }

        }


        public IActionResult WaterMark(string id)
        {
            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var filePath = System.IO.Path.Combine(_env.WebRootPath, "PDFWhiteListed\\" + id + "");
                filepathpdf = generate2(filePath, ip);

                aTimer = new System.Timers.Timer(60000);
                aTimer.Elapsed += OnTimer;

                aTimer.Enabled = true;
                return Redirect("../../Download/" + filepathpdf + ".pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in HomeController WaterMark function.");

                // Log generic context only
                swas.BAL.Utility.Error.ExceptionHandle(
                    "Unhandled exception occurred in HomeController.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong. Please try again later."
                });
            }

        }


        public void OnTimer(Object source, ElapsedEventArgs e)
        {

            try
            {
                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot/Download/" + filepathpdf + ".pdf");

                if (System.IO.File.Exists(filePath1))
                {

                    System.IO.File.Delete(filePath1);


                }
            }
            catch (Exception ex)
            {
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

                ViewBag.ProjDetl = stkc;
                ViewBag.query = query.OrderByDescending(s => s.CommentId).ToList();

                var stakeholders = await _context.tbl_mUnitBranch.Where(a => a.commentreqdid == true || a.unitid == Logins.unitid).ToListAsync();
                ViewBag.stakeholders = stakeholders;


                var notificationsToUpdate = _context.Notification
        .Where(notification => notification.ProjId == projId && notification.IsRead == false && notification.NotificationFrom == stakeholderId)
        .ToList();
                foreach (var notification in notificationsToUpdate)
                {
                    notification.IsRead = true;
                    notification.ReadDateTime = DateTime.Now;
                }
                await _context.SaveChangesAsync();
                return Json(query);


            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }


        public async Task<IActionResult> GetWhiteListedActionProj(int TypeId)
        {
            var ret = await _projectsRepository.GetWhiteListedActionProj(TypeId);
            return Json(ret);
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
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

                var query = (from comment in _context.StkComment
                             join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                             join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                             from status in statusGroup.DefaultIfEmpty() // Left Join
                             join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                             where comment.ProjId == projId
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
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "UpdateUnitStatus");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Get All UpdateUnitStatus in CommentController.", ex, (s, e) => $"{s} - {e?.Message}");

                return RedirectToAction("Error", "Home");
            }
        }
        string filepathpdf1 = "";

        public IActionResult WaterMark3(string id)
        {
            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var filePath = System.IO.Path.Combine(_env.WebRootPath, "Uploads\\" + id + "");
                
                if (System.IO.File.Exists(filePath))
                {
                    Random rnd = new Random();
                    string Dfilename = rnd.Next(1, 1000).ToString() + ".pdf";
                    var pdfBytes = GeneratePdfInMemory(filePath, ip);                    
                    Response.Headers["Content-Disposition"] = $"inline; filename={Dfilename}";
                    return File(pdfBytes, "application/pdf");
                }
                else
                {
                    return Content("PDF IS NOT IN FOLDER");
                }  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in HomeController WaterMark3 Function.");

                // Log generic context only
                swas.BAL.Utility.Error.ExceptionHandle(
                    "Unhandled exception occurred in HomeController.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong. Please try again later."
                });
            }

        }
        public void OnTimer1(Object source, ElapsedEventArgs e)
        {
            try
            {
                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot\\Download\\" + filepathpdf + ".pdf");
                if (System.IO.File.Exists(filePath1))
                {
                    System.IO.File.Delete(filePath1);
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
            }
        }

        public string generate3(string Path, string ip)
        {
            try
            {
                Random rnd = new Random();
                string Dfilename = rnd.Next(1, 1000).ToString();
                var filePath1 = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot\\Download\\" + Dfilename + ".pdf");
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


        public byte[] GeneratePdfInMemory(string path, string ip)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfDocument pdfDoc = new PdfDocument(new PdfReader(path), new PdfWriter(memoryStream));
                    Document doc = new Document(pdfDoc);
                    PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
                    Paragraph paragraph = new Paragraph(ip + " " + DateTime.Now)
                                            .SetFont(font)
                                            .SetFontSize(30);

                    PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.2f);
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        PdfPage pdfPage = pdfDoc.GetPage(i);
                        PdfCanvas over = new PdfCanvas(pdfPage);
                        over.SaveState();
                        over.SetExtGState(gs1);

                        doc.ShowTextAligned(paragraph, 297, 450, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);

                        over.RestoreState();
                    }

                    doc.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return null;
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

        [HttpGet]
        public async Task<IActionResult> AllProjectDetails()
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
                    ViewBag.WhiteListed = await _projectsRepository.GetWhiteListedActionProj(0);
                    ViewBag.HoldProj = await _projectsRepository.GetHoldActionProj();
                    ViewBag.ipadd = watermarkText;

                    string path = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDF");

                    string path1 = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDFWhiteListed");

                    List<string> WhiteList = new List<string>();

                    List<string> Courses = new List<string>();

                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        Courses.Add(file.Replace(path + "\\", ""));
                    }

                    foreach (string file in Directory.EnumerateFiles(path1, "*", SearchOption.AllDirectories))
                    {
                        WhiteList.Add(file.Replace(path1 + "\\", ""));
                    }

                    ViewBag.PdfWhiteListed = WhiteList;

                    ViewBag.Courses = Courses;
                    return View();
                }
                else
                {
                    bool isUserRegistered = false; // Default value if Logins or UserName is null


                    TempData["Tabshift"] = 0;
                    ViewBag.corpsId = 0;
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

                    List<Types> ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();
                    ViewBag.ty = ty.ToList();
                    ViewBag.WhiteListed = await _projectsRepository.GetWhiteListedActionProj(0);
                    ViewBag.HoldProj = await _projectsRepository.GetHoldActionProj();



                    ViewBag.ipadd = watermarkText;

                    string path = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDF");

                    string path1 = System.IO.Path.Combine(this.webHostEnvironment.WebRootPath, "PDFWhiteListed");

                    List<string> WhiteList = new List<string>();

                    List<string> Courses = new List<string>();

                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        Courses.Add(file.Replace(path + "\\", ""));
                    }

                    foreach (string file in Directory.EnumerateFiles(path1, "*", SearchOption.AllDirectories))
                    {
                        WhiteList.Add(file.Replace(path1 + "\\", ""));
                    }

                    ViewBag.PdfWhiteListed = WhiteList;

                    ViewBag.Courses = Courses;

                    return View();
                }

            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);

                return View();
            }
        }

        [HttpGet]
        public IActionResult getCountertoday()
        {
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1).AddSeconds(-1);
            var lastMonthEnd = DateTime.Now.AddMonths(-1).AddMonths(1).AddDays(-DateTime.Now.AddMonths(-1).AddMonths(1).Day);

            int visitorsToday = _context.tbl_LoginLog
                .Where(ul => ul.logindate >= todayStart && ul.logindate <= todayEnd)
                .Select(ul => ul.unitid)
                .Distinct()
                .Count();

            int visitorsThisMonth = _context.tbl_LoginLog
                .Where(ul => ul.logindate >= lastMonthEnd && ul.logindate <= DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.Day))
                .GroupBy(ul => ul.logindate.Date)
                .Select(g => g.Select(x => x.unitid).Distinct().Count())
                .Sum();

            int totalVisitors = _context.tbl_LoginLog
                .GroupBy(ul => ul.logindate.Date)
                .Select(g => g.Select(x => x.unitid).Distinct().Count())
                .Sum();
            var counter = new HitCounterDaily
            {
                Today = visitorsToday,
                CurrentMonth = visitorsThisMonth,
                Total = totalVisitors
            };

            return Ok(counter);
        }



        public IActionResult SOPDownload(string pdftype)
        {
            try
            {
                string inputPdfPath;
                if (!string.IsNullOrEmpty(pdftype))
                {
                    inputPdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SOP/annx.pdf");
                }
                else
                {

                    inputPdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SOP/SOP on Whitelisitng of Appl Sw in IA dt 28 May 24.pdf");
                }
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (System.IO.File.Exists(inputPdfPath))
                {
                    Random rnd = new Random();
                    string Dfilename = rnd.Next(1, 1000).ToString() + ".pdf";
                    var pdfBytes = GeneratePdfInMemory(inputPdfPath, ipAddress);
                    Response.Headers["Content-Disposition"] = $"inline; filename={Dfilename}";
                    return File(pdfBytes, "application/pdf");
                }
                else
                {
                    return Content("PDF IS NOT IN FOLDER");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return Ok();
        }


        [HttpGet]
        public IActionResult GetWhiteListedProjectById(int id)
        {
            var record = _context.trnWhiteListed.FirstOrDefault(x => x.Id == id);
            return Json(record);
        }

        [HttpPost]
        public IActionResult UpdateWhiteListedProject(trnWhiteListed model)
        {
            try
            {
                var record = _context.trnWhiteListed.FirstOrDefault(x => x.Id == model.Id);
                if (record != null)
                {
                    record.mHostTypeId = model.mHostTypeId;
                    record.Appt = model.Appt;
                    record.Fmn = model.Fmn;
                    record.ContactNo = model.ContactNo;
                    record.Clearence = model.Clearence;
                    record.CertNo = model.CertNo;
                    record.ValidUpto = model.ValidUpto;
                    record.Remarks = model.Remarks;
                    _context.SaveChanges();

                    return Json(1);
                }
                return Json(-1);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "UpdateWhiteListedProject");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on UpdateWhiteListedProject in HomeController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }


        public async Task<IActionResult> DateApproval()
        {
            return View();
        }





        [HttpGet]
        public async Task<JsonResult> GetDateApprovalList([FromQuery] int projId, [FromQuery] int requestType,string remarks)
        {
            var user = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext?.Session!, "User");
            var ipAddress = HttpContext.Connection?.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;

            if (projId > 0 && requestType == 2)
            {
                var existingRecord = await _context.DateApproval
                    .AnyAsync(x => x.ProjId == projId && x.RequestType == 2);

                if (existingRecord) // Now checking if the record exists (true/false)
                {
                    return Json(new { success = false, message = "Record already exists." });
                }
                var adminLegacy = new DateApproval
                {
                    ProjId = projId,
                    Request_Date = DateTime.Now,
                    User = Helper1.LoginDetails(user),
                    IsRead = false,
                    UnitId = user.unitid,
                    UserRequest = false,
                    DDGIT_approval = false,
                    DDGIT_Approval_dat = null,
                    RequestType = 2
                };

                _context.DateApproval.Add(adminLegacy);
                await _context.SaveChangesAsync();

                var legacyLog = new LegacyHistory
                {
                    ProjectId = projId,
                    UnitId = user?.unitid,
                    FromUnit = user?.unitid, // Optional: update if needed
                    ActionBy = (user?.Rank ?? "") + " " + (user?.Offr_Name ?? ""),
                    ActionType = (ActionTypeEnum)1,
                    Remarks = remarks?? "No Remarks",
                    ActionDate = DateTime.Now,
                    Userdetails = Helper1.LoginDetails(user!)
                };

                await _legacyHistoryRepository.AddHistoryAsync(legacyLog);
            }
            List<DateApproval> data;
            if (requestType == 2)
            {
                data = _repo.GetDateApprovalListForAdmin();
            }
            else
            {
                data = _repo.GetDateApprovalList();
            }

            return Json(data);
        }
        public async Task<IActionResult> SaveWhiteList(trnWhiteListed whitelist)
        {
            var message = "";

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Please fill all the fields" });
            }
            try
            {
                var Exist = _context.trnWhiteListed
    .Any(x => x.ProjName == whitelist.ProjName
          );

                if (Exist)
                {
                    return BadRequest(new { message = "Project Already WhiteListed. Please Edit Project To Escape Double Entry" });
                }


                whitelist.date = DateTime.Now;
                whitelist.IsWhiteListed=true;
                _context.trnWhiteListed.Add(whitelist);
                await _context.SaveChangesAsync();  // Ensure the changes are saved asynchronously
                message = "Whitelisted entry saved successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while processing request.");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred. Please try again later."
                });
            }



            return Json(new { message });
        }


        [HttpGet]
        public IActionResult Generate(string ProjectName, string ApprovedRemarks, string ApprovedDt)
        {
            try
            {
                ProjectName = WebUtility.UrlDecode(ProjectName);
                ApprovedRemarks = WebUtility.UrlDecode(ApprovedRemarks);
                ApprovedDt = WebUtility.UrlDecode(ApprovedDt);
                if (string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ApprovedRemarks) || string.IsNullOrWhiteSpace(ApprovedDt))
                {
                    return BadRequest("All parameters (ProjectName, ApprovedRemarks, ApprovedDt) are required.");
                }
                if (!DateTime.TryParse(ApprovedDt, out DateTime approvedDate))
                {
                    return BadRequest("Invalid ApprovedDt format. Use a valid date string (e.g., '2025-10-27 16:10:00').");
                }

                using (var ms = new MemoryStream())
                {
                    string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    string watermark = $"{ip}   {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
                    PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    document.SetFont(font);
                    Paragraph title = new Paragraph("IPA Certificate: Portal for Application Whitelisting (PAW)")
                        .SetFontSize(18)
                        .SetBold()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBackgroundColor(new DeviceRgb(240, 240, 240))
                        .SetPadding(5)
                        .SetMarginBottom(20);
                    document.Add(title);
                    document.Add(new LineSeparator(new SolidLine(1f))
                        .SetMarginBottom(20)
                        .SetStrokeColor(ColorConstants.BLACK));
                    document.Add(new Paragraph("It is certified that IPA has been granted by Steering Tech Committee.")
                        .SetFontSize(14)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(25)
                        .SetFontColor(ColorConstants.BLACK));
                    Table table = new Table(UnitValue.CreatePercentArray(new float[] { 40, 5,55 }))
                        .UseAllAvailableWidth()
                        .SetMarginBottom(25)
                        .SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f))
                        .SetPadding(8)
                        .SetBackgroundColor(new DeviceRgb(245, 245, 245));
                    table.AddCell(CreateCell("Project Name", true));
                    table.AddCell(CreateCell(":", true).SetPadding(4).SetWidth(15));     // Colon cell narrow width
                    table.AddCell(CreateCell(ProjectName, false));
                    table.AddCell(CreateCell("Approved Remarks (DDGIT)", true));
                    table.AddCell(CreateCell(":", true).SetPadding(4).SetWidth(15));
                    table.AddCell(CreateCell(ApprovedRemarks, false));
                    table.AddCell(CreateCell("Approved Date (PAW)", true));
                    table.AddCell(CreateCell(":", true).SetPadding(4).SetWidth(15));
                    table.AddCell(CreateCell(approvedDate.ToString("dd-MM-yyyy HH:mm:ss"), false));
                    table.AddCell(CreateCell("Date of Generation", true));
                    table.AddCell(CreateCell(":", true).SetPadding(4).SetWidth(15));
                    table.AddCell(CreateCell(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), false));

                    document.Add(table);
                    document.Add(new Paragraph(
                        "This Certificate is auto-generated based on electronic data and does not require an ink sign." +
                        "It may therefore be treated as an authorized document.")
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.JUSTIFIED)
                        .SetMarginTop(20)
                        .SetMarginBottom(15));

                    Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                    var user = Helper.LoginDetails(Logins);
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "Unknown";
                    document.Add(new LineSeparator(new SolidLine(0.5f))
                        .SetMarginBottom(10)
                        .SetStrokeColor(ColorConstants.LIGHT_GRAY));
                    document.Add(new Paragraph($"Generated by [{user}  {"IP: " + ipAddress}] on {DateTime.Now.ToString("dd MMM yyyy HH:mm:ss")}")
                        .SetFontSize(10)
                        .SetBold()
                        .SetOpacity(0.5f)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontColor(ColorConstants.BLACK)
                        );
                    document.Close();
                    byte[] fileBytes = ms.ToArray();
                    Response.Headers["Content-Disposition"] = "inline; filename=Certificate.pdf";
                    Response.Headers["Content-Type"] = "application/pdf";
                    return File(fileBytes, "application/pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating the PDF.");

                return StatusCode(500,
                    "An error occurred while generating the PDF. Please try again later.");
            }

        }
        private Cell CreateCell(string text, bool isBold)
        {
            Paragraph p = new Paragraph(text)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.LEFT);
            if (isBold)
            {
                p.SetBold();
            }
            return new Cell()
                .Add(p)
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                .SetPadding(6);
        }

     

    }
}