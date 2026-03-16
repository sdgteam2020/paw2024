using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Timers;

using System.Web;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Org.BouncyCastle.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.Build.Evaluation;
using swas.BAL.Repository;
using Microsoft.EntityFrameworkCore;
using swas.UI.Helpers;
using System.Threading;
using System.Security.Cryptography.Xml;
using iText.Commons.Actions.Contexts;
using Grpc.Core;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using ASPNetCoreIdentityCustomFields.Data;
using System.Globalization;
using System.Configuration;
using Microsoft.Extensions.Options;
using swas.DAL;
using Document = iText.Layout.Document;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Kernel.Events;
using static swas.DAL.Models.LegacyHistory;
using System.Threading.Tasks;
using swas.BAL.Utility;
using Path = System.IO.Path;

namespace swas.UI.Controllers
{
    public class ProjectsController : Controller
    {

        private readonly IProjectsRepository _projectsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDdlRepository _DDLRepository;
        private readonly IDdlRepository _dlRepository;
        private readonly IProjStakeHolderMovRepository _psmRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAttHistoryRepository _attHistoryRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataProtector _dataProtector;
        private readonly ICommentRepository _commentRepository;
        private readonly IActionsRepository _actionsRepository;
        private readonly IProjComments _projComments;
        private readonly IProjStakeHolderMovRepository _projStakeHolderMovRepository;
        private readonly ApplicationDbContext _dbContext;

        private IWebHostEnvironment webHostEnvironment;
        private System.Timers.Timer aTimer;
        private readonly IStkCommentRepository _stkCommentRepository;
        private readonly IProjStakeHolderMovRepository _stkholdmove;
        private readonly IProjStakeHolderCcMovRepository _projStakeHolderCcMovRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectsController> _logger;
        private readonly ILegacyHistoryRepository _legacyHistoryRepository;
        private readonly IRemainder _Remainder;

        public ProjectsController(IProjectsRepository projectsRepository, IDdlRepository ddlRepository,
            IProjStakeHolderMovRepository psmRepository, IHttpContextAccessor httpContextAccessor,
            IDdlRepository DDLRepository, IAttHistoryRepository attHistoryRepository,
            IWebHostEnvironment environment, IProjStakeHolderMovRepository stkholdmove,
            IDataProtectionProvider DataProtector, IWebHostEnvironment _webHostEnvironment,
            ICommentRepository commentRepository, IActionsRepository actionsRepository,
            IProjComments projComments, IStkCommentRepository stkCommentRepository,
            IProjStakeHolderMovRepository projStakeHolderMovRepository,
            UserManager<ApplicationUser> userManager, IUnitRepository unitRepository, IConfiguration configuration, ApplicationDbContext context,
            ILogger<ProjectsController> logger, ILegacyHistoryRepository legacyHistoryRepository,
            IProjStakeHolderCcMovRepository projStakeHolderCcMovRepository
            , IRemainder Remainder

            )
        {
            _projectsRepository = projectsRepository;
            _dlRepository = ddlRepository;
            _psmRepository = psmRepository;
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = _webHostEnvironment;

            _DDLRepository = ddlRepository;
            _stkholdmove = stkholdmove;
            _attHistoryRepository = attHistoryRepository;
            _environment = environment;
            _commentRepository = commentRepository;
            _actionsRepository = actionsRepository;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
            _projComments = projComments;
            _stkCommentRepository = stkCommentRepository;
            _projStakeHolderMovRepository = projStakeHolderMovRepository;
            _userManager = userManager;
            _unitRepository = unitRepository;
            _configuration = configuration;
            _dbContext = context;
            _logger = logger;
            _legacyHistoryRepository = legacyHistoryRepository;
            _projStakeHolderCcMovRepository = projStakeHolderCcMovRepository;
            _Remainder = Remainder;

        }


        [HttpGet]
        public async Task<IActionResult> Index()

        {
            CommonDTO dto = new CommonDTO();

            dto.Projects = await _projectsRepository.GetAllProjectsAsync();

            return View(dto);
        }

       [Authorize]
[HttpGet]
public async Task<IActionResult> Details(int id)
{
    if (id <= 0)
    {
        return BadRequest("Invalid project id");
    }

    var project = await _projectsRepository.GetProjectByIdAsync(id);

    if (project == null)
    {
        return NotFound();
    }

    return Ok(new { success = true, project });
}



        [HttpGet]

        public async Task<IActionResult> ProjStatDashBdView(string? id, string? status)
        {
            string EncyID = id;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (Logins.UserName != null && EncyID != null)
            {
                int dataProjId = 0;

                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;

                if (EncyID != null)
                {
                    try
                    {
                        string decryptedValue = _dataProtector.Unprotect(EncyID);
                        dataProjId = int.Parse(decryptedValue);
                    }
                    catch (Exception ex)
                    {
                        swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                        return Redirect("~/Home/Error");
                    }
                    MailBox mbx = new MailBox();

                    mbx.SendItems = null;

                    return View(mbx);
                }
                return null;
            }

            else
            {
                return Redirect("~/Identity/Account/Login");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ProjDetails()
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                    TempData["ipadd"] = watermarkText;
                    ViewBag.SubmitCde = "0";

                    ViewBag.remainder = _dbContext.TrnRemainders.ToList();
                    ViewBag.tbl_mUnitBranch = _dbContext.tbl_mUnitBranch.ToList();
                    MailBox mbx = new MailBox();
                    mbx.Remainder = await _Remainder.GetAllAsync();

                    var notificationContent = _configuration.GetSection("NotificationContent").Get<NotificationContent>();
                    ViewBag.NotificationContent = notificationContent;


                    if (Logins != null && Logins.unitid != null)
                    {
                        ViewBag.unitid = Logins.unitid;
                    }
                    ViewBag.remainder = _dbContext.TrnRemainders.ToList();

                    mbx.InBox = await _projectsRepository.GetActInboxAsync();



                    mbx.Draft = await _projectsRepository.GetActDraftItemsAsync();


                    mbx.SendItems = await _projectsRepository.GetActSendItemsAsync();
                    mbx.CompletedItems = await _projectsRepository.GetActComplettemsAsync();


                    return View(mbx);
                }
                else
                {
                    return LocalRedirect("~/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetActCcProject()
        {
            return Json(await _projectsRepository.GetActCcItemsAsync());
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllStatus()
        {
            var ss = await _dlRepository.ddlStatus();
            return View(ss);

        }

        #region CreateProject
        [HttpGet]
        public async Task<IActionResult> Create(string id)
        {
            try
            {


                var options = _configuration.GetSection("WhitelistStatusOptions").Get<List<SelectListItem>>();
                options?.Insert(0, new SelectListItem { Text = "--Select--", Value = "", Disabled = true, Selected = true });
                ViewBag.WhitelistOptions = options;


                var securityclassification = _configuration.GetSection("SecurityClassification").Get<List<SelectListItem>>();
                securityclassification?.Insert(0, new SelectListItem { Text = "--Select--", Value = "", Disabled = true, Selected = true });
                ViewBag.SecurityClassifications = securityclassification;
                    

                var TypeofSW = _configuration.GetSection("TypeofSWOptions").Get<List<SelectListItem>>();
                TypeofSW.Insert(0, new SelectListItem { Text = "--Select--", Value = "", Disabled = true, Selected = true });
                ViewBag.TypeofSWOption = TypeofSW;

                var BeingDevpInhouse = _configuration.GetSection("BeingDevpInhouseOptions").Get<List<SelectListItem>>();
                BeingDevpInhouse.Insert(0, new SelectListItem { Text = "--Select--", Value = "", Disabled = true, Selected = true });
                ViewBag.BeingDevpInhouseOption = BeingDevpInhouse;


                var EndorsmentbyHeadof = _configuration.GetSection("EndorsmentbyHeadofOptions").Get<List<SelectListItem>>();
                EndorsmentbyHeadof.Insert(0, new SelectListItem { Text = "--Select--", Value = "", Disabled = true, Selected = true });
                ViewBag.EndorsmentbyHeadofOption = EndorsmentbyHeadof;

                var notificationContent = _configuration.GetSection("NotificationContent").Get<NotificationContent>();
                ViewBag.NotificationContent = notificationContent;



                int ids = 0;
                if (id != null)
                {
                    string decryptedValue = _dataProtector.Unprotect(id);
                    ids = int.Parse(decryptedValue);
                    tbl_Projects tbl_Projects = new tbl_Projects();
                    tbl_Projects = await _projectsRepository.GetProjectByPsmIdAsync(ids);
                    ViewBag.ProjectEncyId = id;
                    ViewBag.Projects = await _projectsRepository.GetMyProjects();
                    return View(tbl_Projects);

                }
                TempData["SubCde"] = false;
                TempData.Keep("SubCde");



                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                    ViewBag.Projects = await _projectsRepository.GetMyProjects();
                    return View(null);

                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }

            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }

        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 83886080)]
        public async Task<IActionResult> UploadMultiFile(IFormFile uploadfile, string Reamarks, int PsmId,int DocumentTypeId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (uploadfile != null && uploadfile.Length <= 10 * 1024 * 1024)
            {
                if (uploadfile != null && uploadfile.Length > 0)
                {

                    string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";
                    if (System.IO.Path.GetExtension(uniqueFileName).ToLower() == ".pdf")
                    {
                        string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadfile.CopyTo(stream);
                        }
                        if (PsmId != null && PsmId != 0)
                        {
                            tbl_AttHistory atthis = new tbl_AttHistory();
                            atthis.ActionId = 0;
                            atthis.AttPath = uniqueFileName;
                            atthis.DocumentTypeId = DocumentTypeId;
                            atthis.Reamarks = Reamarks;
                            atthis.PsmId = PsmId;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.IsDeleted = false;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.ActFileName = uploadfile.FileName;

                            await _attHistoryRepository.AddAttHistoryAsync(atthis);
                        }
                        else
                        {
                            return Json(-1);
                        }
                    }
                    else
                    {
                        return Json(-2);
                    }
                }
            }
            else
            {
                return Json(-5);
            }
            return Json(1);

        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProject(tbl_Projects Data, string RequestRemarks)
        {
         
          
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                int projid = 0;
               
                Data.StakeHolderId = Logins.unitid ?? 0;
                Data.IsActive = true;
                Data.EditDeleteDate = DateTime.Now;
                Data.EditDeleteBy = 0;
                Data.IsDeleted = false;
                Data.IsSubmited = false;
                Data.UpdatedByUserId = Logins.UserIntId;
                Data.Comments = Data.InitialRemark;
               
                Data.MobileNo = Data.MobileNo;
                Data.AsconNo = Data.AsconNo;
                if(Data.Date_type ==1)
                {
                    Data.InitiatedDate = Data.InitiatedDate;
                    Data.DateTimeOfUpdate = Data.InitiatedDate;
                }else
                {
                    Data.InitiatedDate = DateTime.Now;
                    Data.DateTimeOfUpdate = DateTime.Now;
                }

                    bool isEdit = Data.ProjId != 0;

                if (!isEdit)
                {


                    if (Data.IsWhitelisted == "Re-Vetted")
                    {
                        Data.ProjName = await GetReVettedProjectName(Data);
                    }
                    bool projectExists = await _projectsRepository.ProjectNameExists(Data);


                    if (projectExists)
                    {
                        return Json(-3);
                    }
                }
                else
                {

                    var existingProject = await _projectsRepository.GetProjectByIdAsync(Data.ProjId);


                    _dbContext.Entry(existingProject).State = EntityState.Detached;


                    if (Data.IsWhitelisted == "Re-Vetted" && existingProject?.IsWhitelisted != "Re-Vetted")
                    {
                        Data.ProjName = await GetReVettedProjectName(Data);


                        bool projectExists = await _projectsRepository.ProjectNameExists(Data);
                        if (projectExists)
                        {
                            return Json(-3);
                        }
                    }
                    else if (Data.IsWhitelisted == "Re-Vetted" && existingProject?.IsWhitelisted == "Re-Vetted" && !Data.ProjName.Contains("Re-Vetted"))
                    {
                        return Json(-5);
                    }


                }

                if (Data.ProjId == 0)
                {
                    Data.CurrentPslmId = 0;
                    projid = await _projectsRepository.AddProjectAsync(Data, RequestRemarks);
                    Data = await _projectsRepository.GetProjectByIdAsync(projid);
                }
                else
                {
                    Data.EditDeleteDate = DateTime.Now;
                    await _projectsRepository.UpdateProjectAsync(Data, RequestRemarks);
                    Data = await _projectsRepository.GetProjectByIdAsync(Data.ProjId);
                }

                if (Data.OldPsmid != 0)
                {
                    var oldAttachments = _dbContext.AttHistory
                        .Where(x => x.PsmId == Data.OldPsmid)
                        .ToList();

                    foreach (var old in oldAttachments)
                    {
                        var newAttachment = new tbl_AttHistory
                        {
                            PsmId = Data.CurrentPslmId,
                            ActionId = old.ActionId,
                            TimeStamp = old.EditDeleteDate,
                            IsDeleted = false,
                            IsActive = true,
                            EditDeleteBy = old.EditDeleteBy,
                            AttPath = old.AttPath,
                            EditDeleteDate = DateTime.Now,
                            UpdatedByUserId = old.UpdatedByUserId,
                            ActFileName = old.ActFileName,
                            Reamarks = old.Reamarks,
                            DateTimeOfUpdate = old.DateTimeOfUpdate
                        };
                        _dbContext.AttHistory.Add(newAttachment);
                    }

                    await _dbContext.SaveChangesAsync();
                }
                return Json(Data);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "AddProjectError");

                _logger.LogError(eventId, ex,
                    "An error occurred while adding a project in ProjectsController.");

                // Log internally only (no raw message exposed)
                swas.BAL.Utility.Error.ExceptionHandle(
                    "Add Project failed in ProjectsController.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to add project at this time. Please try again later."
                });
            }

        }

        public async Task<IActionResult> GetAtthHistoryByProjectId(int PslmId)
        {
            try
            {

                return Json(await _attHistoryRepository.GetAttHistoryByIdAsync(PslmId));

            }
            catch (Exception ex)
            {
                return Json(-1);
            }
        }
        public async Task<IActionResult> DeleteProjects(int ProjectId)
        {
            var ret = await _projectsRepository.DeleteProjectAsync(ProjectId);
            if (ret == true)
                return Json(1);
            else
                return Json(0);
        }

        [HttpPost]
        public async Task<IActionResult> ProjectSubmited(int projid, int type, string Remarks)
        {
            try
            {

                var project = await _projectsRepository.GetProjectByIdAsync(projid);
                // Get uploaded document types
                // 1️⃣ Get required document type IDs from DB
              
                if (type == 1)
                {
                    var requiredDocIds = await _dbContext.DocumentTypes
                  .Where(d => d.IsRequired && d.IsActive)
                  .Select(d => d.Id)
                  .ToListAsync();

                    // 2️⃣ Get uploaded document type IDs for this project
                    var uploadedDocIds = await _dbContext.AttHistory
                        .Where(a => a.PsmId == project.CurrentPslmId)
                        .Select(a => a.DocumentTypeId)
                        .Distinct()
                        .ToListAsync();



                    var missingDocIds = requiredDocIds
                        .Except(uploadedDocIds.Where(x => x.HasValue).Select(x => x.Value))
                        .ToList();

                    if (missingDocIds.Any())
                    {
                        return Json(new
                        {
                            type = 404,
                            message = "Please upload all required documents."
                        });
                    }
                    project.IsSubmited = true;
                }
                else
                {
                    project.IsSubmited = false;
                }
                await _projectsRepository.UpdateProjectAsync(project, Remarks);

                return Json(project.ProjId);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "ProjectSubmited");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while Projected Submited in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }

        }
        [HttpPost]
        public async Task<IActionResult> FwdProjConfirm(int PslmId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                if (Logins != null)
                {
                    try
                    {
                        tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                        psmove = await _projectsRepository.GettXNByPsmIdAsync(PslmId);
                        psmove.IsComplete = true;
                        await _projectsRepository.UpdateTxnAsync(psmove);
                        return Json(4);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred in ProjectsController FwdProjConfirm Function.");

                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Something went wrong. Please try again later."
                        });
                    }

                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "FwdProjConfirm");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while Fwd Proj Confirm in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }

        }

        [HttpPost]
        public async Task<IActionResult> IsReadInbox(int PsmId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    try
                    {
                        tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                        psmove = await _projectsRepository.GettXNByPsmIdwithUnitId(PsmId, Convert.ToInt32(Logins.unitid));
                        if (psmove != null)
                        {
                            psmove.IsRead = true;
                            await _projectsRepository.UpdateTxnAsync(psmove);
                            return Json(PsmId);
                        }
                        var psCcmove = await _projStakeHolderCcMovRepository.GetdataBuPsmiandTounitId(PsmId, Convert.ToInt32(Logins.unitid));
                        if (psCcmove != null)
                        {
                            psCcmove.IsRead = true;
                            psCcmove.ReadDate = DateTime.Now;
                            psCcmove.UserDetails = Helper.LoginDetails(Logins);
                            await _projStakeHolderCcMovRepository.Update(psCcmove);
                            return Json(PsmId);
                        }

                        return Json(0);
                    }
                    catch (Exception ex)
                    {
                        swas.BAL.Utility.Error.ExceptionHandle(ex.ToString()); // log full internally
                        return Json(new { success = false, message = "Something went wrong." });
                    }

                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "IsReadInbox");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while IsRead Inbox in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }

        }


     

        [HttpPost]
        public async Task<IActionResult> IsProcessProjConfirm(int ProjId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;
                if (Logins != null)
                {

                    try
                    {
                        tbl_Projects proj = new tbl_Projects();
                        proj = await _projectsRepository.GetProjectByIdAsync(ProjId);
                        proj.DateTimeOfUpdate = DateTime.Now;
                        proj.IsProcess = true;

                        await _projectsRepository.UpdateProjectAsync(proj, "1");
                        return Json(ProjId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred in IsProcessProjConfirm ProjectsController.");

                        return StatusCode(500, new
                        {
                            success = false,
                            message = "An unexpected error occurred. Please try again later."
                        });
                    }

                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "IsProcessProjConfirm");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while Process Project Confirm in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }

        }
        public async Task<IActionResult> DeleteAttech(int AttechId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                if (Logins != null)
                {
                    var ret = await _attHistoryRepository.DeleteAttHistoryAsync(AttechId);

                    if (ret == null)
                    {
                        return Json(0);
                    }
                    else return Json(1);
                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }

            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }


        }
        #endregion

        public byte[] generate2(string Path, string ip)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfDocument pdfDoc = new PdfDocument(new PdfReader(Path), new PdfWriter(memoryStream));
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

        #region Project Movment For PROcess For Comment
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessMail(int ProjId, int unitid, DateTime FwdDateForComment)
        {
            if (ProjId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid project id."
                });
            }

            try
            {
                var login = SessionHelper.GetObjectFromJson<Login>(
                    _httpContextAccessor.HttpContext.Session, "User");

                if (login == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Session expired. Please login again."
                    });
                }

                var project = await _projectsRepository.GetProjectByIdAsync(ProjId);

                if (project == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Project not found."
                    });
                }

                // 🔒 Prevent IDOR (Burp changing project id)
                if (1 != login.unitid)
                {
                    return Forbid();
                }

                unitid = project.StakeHolderId;

                int[] statusIds = { 26, 31, 37, 21, 21 };
                int[] unitIds = { 4, 3, 5, 1, unitid };
                int[] skipUnitIds = { 4, 3, 5, 1 };

                for (int i = 0; i < statusIds.Length; i++)
                {
                    if (i == 4 && skipUnitIds.Contains(unitid) && statusIds[i] == 21)
                        continue;

                    var psmove = new tbl_ProjStakeHolderMov
                    {
                        ProjId = ProjId,
                        StatusActionsMappingId = statusIds[i],
                        Remarks = "",
                        FromUnitId = login.unitid ?? 0,
                        UserDetails = Helper1.LoginDetails(login),
                        UpdatedByUserId = login.unitid,
                        DateTimeOfUpdate = DateTime.UtcNow,
                        IsActive = true,
                        EditDeleteDate = DateTime.UtcNow,
                        EditDeleteBy = login.unitid,
                        TimeStamp = DateTime.UtcNow,
                        IsComplete = false,
                        ToUnitId = unitIds[i],
                        IsComment = true
                    };

                    await _psmRepository.AddProjStakeHolderMovAsync(psmove);
                }

                return Ok(new
                {
                    success = true,
                    message = "Process mail executed successfully."
                });
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "ProcessMail");

                _logger.LogError(eventId, ex, "Error occurred in ProjectsController.ProcessMail");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error."
                });
            }
        }


        public async Task<IActionResult> CheckFwdCondition(int ProjId, int StatusId,string Actionsname)
        {
            var Ret = await _psmRepository.CheckFwdCondition(ProjId, StatusId, Actionsname);
            return Json(Ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FwdToProject([FromForm] tbl_ProjStakeHolderMov psmove, [FromForm] string currentpsmid)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            // SECURITY: Early exit if session/user is missing
            if (Logins == null)
            {
                return Json(-401); // or return Unauthorized();
            }

            // SECURITY: Basic parameter validation
            if (string.IsNullOrWhiteSpace(currentpsmid) || !int.TryParse(currentpsmid, out int currentPsmId))
            {
                return Json(-999); // invalid currentpsmid format
            }

            if (psmove.ToUnitId == 0)
            {
                return Json(-7);
            }
            ;

            // SECURITY: File upload restrictions (add these checks before processing attachments)
            const long MAX_FILE_SIZE_BYTES = 10 * 1024 * 1024; // 10 MB – change as needed
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf"
        // ← add/remove types YOUR application really needs
    };

            if (psmove.Attachments != null && psmove.Attachments.Count > 0)
            {
                foreach (var attachment in psmove.Attachments)
                {
                    if (attachment.File == null || attachment.File.Length == 0) continue;

                    // SECURITY: Size check
                    if (attachment.File.Length > MAX_FILE_SIZE_BYTES)
                    {
                        return Json(-10); // file too large
                    }

                    // SECURITY: Extension check (server-side – client can be bypassed)
                    var fileExt = Path.GetExtension(attachment.File.FileName)?.ToLowerInvariant();
                    if (string.IsNullOrEmpty(fileExt) || !allowedExtensions.Contains(fileExt))
                    {
                        return Json(-11); // invalid file type
                    }

                    // SECURITY: Basic content/magic number check (very important)
                    if (!await HasValidFileSignatureAsync(attachment.File, fileExt))
                    {
                        return Json(-12); // suspicious file content
                    }
                }
            }

            if (psmove.StatusActionsMappingId == 88)
            {
                var projname = _dbContext.Projects.Find(psmove.ProjId);
                var Whitelist = _dbContext.trnWhiteListed
                    .FirstOrDefault(x => x.ProjName == projname.ProjName);
                if (Whitelist != null)
                {
                    Whitelist.CertNo = Convert.ToString(DateTime.Now);
                    Whitelist.IsWhiteListed = true;
                    Whitelist.ValidUpto = DateTime.Now;
                    _dbContext.trnWhiteListed.UpdateRange(Whitelist);
                    _dbContext.SaveChanges();
                }
            }
            else if (psmove.StatusActionsMappingId == 78)
            {
                var projname = _dbContext.Projects.Find(psmove.ProjId);
                var Whitelist = _dbContext.trnWhiteListed
                    .FirstOrDefault(x => x.ProjName == projname.ProjName);
                if (Whitelist != null)
                {
                    Whitelist.Clearence = DateTime.Now;
                    _dbContext.trnWhiteListed.UpdateRange(Whitelist);
                    _dbContext.SaveChanges();
                }
            }
            bool ret = false;
            if (psmove.CcId != null)
            {
                ret = psmove.CcId.Contains(psmove.ToUnitId);
            }
            int psmid = Convert.ToInt32(currentpsmid);
            var getprojidbypsmid = _dbContext.ProjStakeHolderMov.FirstOrDefault(x => x.PsmId == psmid).ProjId;
            var latst = _dbContext.ProjStakeHolderMov.Where(r => r.PsmId == psmid && r.ToUnitId == Logins.unitid && r.IsComplete == false && r.IsComment == false).FirstOrDefault();
            if (latst == null)
            {
                return Json(-4);
            }
            if (!ret)
            {
                var legacy_approval = _dbContext.LegacyHistory.Where(x => x.ProjectId == getprojidbypsmid).OrderByDescending(x => x.HistoryId).FirstOrDefault();
                psmove.ProjId = getprojidbypsmid;
                psmove.StatusActionsMappingId = psmove.StatusActionsMappingId;
                psmove.Remarks = psmove.Remarks;
                psmove.FromUnitId = Logins.unitid ?? 0;
                psmove.ToUnitId = psmove.ToUnitId; //
                int oldpsmid = Convert.ToInt32(currentpsmid);
                var updateiscomplete = await _projectsRepository.GettXNByPsmIdAsync(oldpsmid);
                updateiscomplete.IsComplete = true;
                await _projectsRepository.UpdateTxnAsync(updateiscomplete);
                psmove.UserDetails = Helper.LoginDetails(Logins);
                psmove.UpdatedByUserId = Logins.UserIntId; // change with userid
                if (legacy_approval != null && legacy_approval.ActionType == ActionTypeEnum.Approved)
                {
                    psmove.DateTimeOfUpdate = psmove.TimeStamp;
                    psmove.EditDeleteDate = DateTime.Now;
                    psmove.TimeStamp = psmove.TimeStamp;
                }
                else
                {
                    psmove.DateTimeOfUpdate = DateTime.Now;
                    psmove.EditDeleteDate = DateTime.Now;
                    psmove.TimeStamp = DateTime.Now;
                }
                psmove.IsActive = true;
                psmove.EditDeleteBy = Logins.UserIntId;

                psmove.IsComplete = false;
                psmove.IsComment = false;
                psmove.IsPullBack = false;
                if (psmove.FromUnitId == psmove.ToUnitId)
                {
                    psmove.IsRead = true;
                }
                if (psmove.CcId != null && psmove.CcId.Length > 0)
                {
                    psmove.IsCc = true;
                }
                _dbContext.SaveChanges();
                var remainders = await _dbContext.TrnRemainders
                    .Where(r => r.Projid == getprojidbypsmid && r.ReadDate == null && r.ToUserDetails == null && r.Tounitid == Logins.unitid)
                    .ToListAsync();
                if (remainders.Count > 0)
                {
                    await _Remainder.UpdateReaminderRead(getprojidbypsmid, 0);
                }
                var projectMovements = await _dbContext.ProjStakeHolderMov
                    .Where(x => x.ProjId == getprojidbypsmid && x.IsRead == true && x.IsComment == true)
                    .ToListAsync();
                foreach (var item in projectMovements)
                {
                    item.IsRead = false;
                }
                _dbContext.ProjStakeHolderMov.UpdateRange(projectMovements);
                await _dbContext.SaveChangesAsync();
                var Ret = await _psmRepository.AddWithReturn(psmove);
                var latestpsmid = _projStakeHolderMovRepository.GetLastRecProjectMov(getprojidbypsmid);
                var errors = new List<int>(); // List to collect errors
                if (psmove.Attachments != null && psmove.Attachments.Count > 0)
                {
                    foreach (var attachment in psmove.Attachments)
                    {
                        var saveResult = await SaveAttachmentAsync(attachment.File, attachment.Remarks, latestpsmid, Logins, psmove.TimeStamp);
                        if (saveResult is JsonResult jsonResult)
                        {
                            var resultValue = jsonResult.Value as int?;
                            if (resultValue != 1) // If not successful, collect the error code
                            {
                                errors.Add(resultValue.GetValueOrDefault());
                            }
                        }
                    }
                }
                if (errors.Any())
                {
                    return Json(errors);
                }
                if (Ret != null)
                {
                    if (psmove.CcId != null && psmove.CcId.Length > 0)
                    {
                        foreach (int ccId in psmove.CcId)
                        {
                            tbl_ProjStakeHolderCcMov ccMov = new tbl_ProjStakeHolderCcMov();
                            ccMov.PsmId = Ret.PsmId;
                            ccMov.ProjId = getprojidbypsmid;
                            ccMov.ToCcUnitId = ccId;
                            ccMov.IsActive = true;
                            ccMov.IsDeleted = false;
                            ccMov.IsRead = false;
                            ccMov.UserDetails = "";
                            ccMov.ReadDate = DateTime.Now;
                            var Retcc = await _projStakeHolderCcMovRepository.AddWithReturn(ccMov);
                        }
                    }
                    return Json(Ret);
                }
                else
                {
                    return Json(nmum.NotSave);
                }
            }
            else
            {
                return Json(nmum.TounitEqualsCCUnitID);
            }
        }
        // SECURITY: Basic file signature validation to detect fake extensions
        private async Task<bool> HasValidFileSignatureAsync(IFormFile file, string extension)
        {
            if (file == null || file.Length == 0) return false;

            var signatures = new Dictionary<string, byte[]>
    {
        { ".pdf",  new byte[] { 0x25, 0x50, 0x44, 0x46 } }           // %PDF
        
        // Add more types if needed
    };

            if (!signatures.TryGetValue(extension, out byte[] expected) || expected == null)
                return true; // unknown type → allow (or return false if strict)

            using var stream = file.OpenReadStream();
            var header = new byte[expected.Length];
            int bytesRead = await stream.ReadAsync(header, 0, expected.Length);

            return bytesRead == expected.Length && header.SequenceEqual(expected);
        }
        public async Task<IActionResult> SaveAttachmentAsync(IFormFile attdata, string remarks, int psmid, Login Logins, DateTime? TimeStamp)
        {
            var MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
            if (attdata == null || attdata.Length == 0)
                return Json(-10); // no file

            if (attdata.Length > MaxFileSizeBytes)
                return Json(-3);  // too large

            var originalName = attdata.FileName?.Trim() ?? "";
            var ext = Path.GetExtension(originalName).ToLowerInvariant();

            if (ext != ".pdf")
                return Json(-2);  // only pdf allowed

            if (psmid <= 0)
                return Json(-1);  // invalid psmid
            var uploadsDir = Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/");
            Directory.CreateDirectory(uploadsDir);

            var uniqueFileName = $"Swas_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await attdata.CopyToAsync(stream);
            }
            var atthis = new tbl_AttHistory
            {
                ActionId = 0,
                AttPath = uniqueFileName,        // saved name on disk
                Reamarks = remarks ?? string.Empty,
                PsmId = psmid,
                UpdatedByUserId = Logins?.unitid,
                IsDeleted = false,
                IsActive = true,
                EditDeleteBy = Logins?.unitid,
                EditDeleteDate = DateTime.Now,
                TimeStamp = TimeStamp,
                ActFileName = originalName       // original file name from user
            };

            await _attHistoryRepository.AddAttHistoryAsync(atthis);
            _dbContext.SaveChanges();
            return Json(1); // success
        }



        public async Task<IActionResult> ProjectMovHistory(int ProjectId)
        {
            var Ret = await _psmRepository.ProjectMovHistory(ProjectId);
            return Json(Ret);
        }
        public async Task<IActionResult> UndoProject(int ProjectId, int PsmId, string Remarks, int StageId)
        {
            try
            {

                if (StageId == 1)
                {
                    return Json(nmum.NotSave);
                }
                else
                {
                    var movent = await _psmRepository.GetByByte(PsmId);
                    movent.IsRead = false;
                    movent.UndoRemarks = Remarks;
                    movent.IsComplete = true;
                    movent.IsPullBack = true;
                    var Ret = await _psmRepository.UpdateWithReturn(movent);

                    Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                    var psmidold = await _psmRepository.GetLastRecProjectMovForUnod(ProjectId, Logins.unitid);
                    var movent1 = await _psmRepository.GetByByte(psmidold);
                    movent1.Remarks = "";
                    movent1.IsComplete = false;
                    movent1.IsRead = false;
                    var Ret1 = await _psmRepository.UpdateWithReturn(movent);
                }


                return Json(nmum.Update);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "UndoProject");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while UndoProject in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");

                return Json(nmum.Exception);
            }
        }
        #endregion
        #region PullBack
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PullBAckProject(int ProjectId, int PsmId, string Remarks, int StageId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                var movent = new tbl_ProjStakeHolderMov();

					var remainders = await _dbContext.TrnRemainders
			  .Where(r => r.Projid == ProjectId && r.ReadDate == null && r.ToUserDetails == null)
			  .ToListAsync();

					if (remainders.Count > 0)
					{

					await _Remainder.UpdateReaminderRead(ProjectId,1);

					}


					await _dbContext.SaveChangesAsync();

					int psmData = _psmRepository.GetLastRecProjectMov(ProjectId);
                    if (psmData != 0)
                    {
                        if (psmData == PsmId)
                        {
                            movent = await _psmRepository.GetByByte(psmData);
                            movent.IsRead = true;
                            movent.UndoRemarks = Remarks;
                            movent.IsComplete = true;
                            movent.DateTimeOfUpdate = DateTime.Now;
                            var Ret = await _psmRepository.UpdateWithReturn(movent);
                        }
                        else
                        {
                            movent = await _psmRepository.GetByByte(psmData);
                            movent.IsComplete = true;
                            await _psmRepository.UpdateWithReturn(movent);

                            movent = await _psmRepository.GetByByte(PsmId);
                            movent.IsRead = true;
                            movent.UndoRemarks = Remarks;
                            movent.IsComplete = true;
                            movent.DateTimeOfUpdate = DateTime.Now;
                            var Ret = await _psmRepository.UpdateWithReturn(movent);
                        }


                        UnitDtl unitDetail = new UnitDtl();
                        if (psmData != PsmId)
                        {
                            movent = await _psmRepository.GetByByte(psmData);
                            unitDetail = await _unitRepository.GetUnitDtl(movent.ToUnitId);
                        }
                        else
                        {
                            unitDetail = await _unitRepository.GetUnitDtl(movent.ToUnitId);
                        }
                        if (unitDetail != null)
                        {
                            ApplicationUser userdet = await _projectsRepository.GetUserByUnitId(unitDetail.unitid);
                            if (userdet != null)
                            {
                                var rankName = _dbContext.mRank.FirstOrDefault(x => x.Id == userdet.Rank);
                                if (rankName != null)
                                {
                                    movent.UserDetails = rankName.RankName + " " + userdet.Offr_Name.Trim() + " / " + userdet.UserName.Trim() + "";
                                }
                            }
                            else
                            {
                                movent.UserDetails = "";
                            }
                        }
                        if (psmData != PsmId)
                        {
                            movent = await _psmRepository.GetByByte(psmData);
                            movent.FromUnitId = movent.ToUnitId;
                        }
                        else
                        {
                            movent.FromUnitId = movent.ToUnitId;
                        }
                        movent.PsmId = 0;
                        movent.ToUnitId = Convert.ToInt32(Logins.unitid);
                        movent.IsComplete = false;
                        movent.IsRead = false;
                        movent.IsPullBack = true;
                        movent.UndoRemarks = null;
                        movent.Remarks = Helper.LoginDetails(Logins) + "("+(Logins.Unit)+ ") 𝐑𝐞𝐦𝐚𝐫𝐤𝐬: " + Remarks; 
                        movent.UpdatedByUserId = Logins.UserIntId;
                        movent.DateTimeOfUpdate = DateTime.Now;


                        movent.EditDeleteDate = DateTime.Now;
                        movent.EditDeleteBy = Logins.UserIntId;
                        movent.TimeStamp = DateTime.Now;
                        movent.IsComplete = false;
                        movent.IsComment = false;

                        movent.IsCc = false;
                        var Ret1 = await _psmRepository.AddWithReturn(movent);
                        return Json(nmum.Update);
                    }
                    return Json(nmum.NotSave);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "PullBAckProject");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while Pull Back Project in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");

                return Json(nmum.Exception);
            }
        }
        #endregion

        #region ProjComments
        public async Task<IActionResult> ProjComments()
        {

            return View();
        }
        public async Task<IActionResult> GetProjCommentsByUnitId(int StatusId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                return Json(await _projComments.GetAllStkForComment(Convert.ToInt32(Logins?.unitid),StatusId));
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 26214400)]
        public async Task<IActionResult> SendCommentonProject(IFormFile uploadfile, string Comments, int StkStatusId, int ProjectId, int psmid, DateTime CommentDate)
        {
            int count = 0;
            count++;
            var count1 = count;
            try
            {
                StkComment cmmets = new StkComment();
                string uniqueFileName = "";
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                int psmove = await _stkCommentRepository.GetCommentStatusByPsmiId(psmid);
                int allowForInfo = _stkCommentRepository.IsAllowForCommentByStkStatusId(StkStatusId);
                if (psmove != 1 || allowForInfo == 1)
                {
                    if (uploadfile != null)
                    {
                        if (uploadfile.Length <= 10485760)
                        {
                            uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }


                            cmmets.ActFileName = uploadfile.FileName;
                        }
                        else
                        {
                            return Json(nmum.PdfSizeEx);
                        }
                    }
                    var approval_legacy = _dbContext.LegacyHistory.Where(x =>x.ProjectId==ProjectId).OrderByDescending(x=>x.HistoryId).FirstOrDefault();
                    cmmets.Attpath = uniqueFileName;
                    cmmets.Comments = Comments;
                    cmmets.PsmId = psmid;
                    cmmets.ProjId = ProjectId;
                    cmmets.UpdatedByUserId = Logins.UserIntId;
                    if(approval_legacy !=null && approval_legacy.ActionType ==ActionTypeEnum.Approved)
                    {
                    cmmets.DateTimeOfUpdate = CommentDate;

                    cmmets.EditDeleteDate = DateTime.Now; 
                    }
                    else
                    {
                        cmmets.DateTimeOfUpdate = DateTime.Now;

                        cmmets.EditDeleteDate = DateTime.Now; 
                    }
                    cmmets.IsDeleted = false;
                    cmmets.IsActive = true;
                    cmmets.EditDeleteBy = Logins.unitid;
                    cmmets.StkStatusId = StkStatusId;
                    cmmets.UserDetails = Helper.LoginDetails(Logins);
                    cmmets.StakeHolderId = Logins.unitid; 

                    var projectStkHolderMovementData = await _projectsRepository.GetProjStkHolderMovmentByPsmiId(cmmets.PsmId);
                    if (projectStkHolderMovementData != null)
                    {
                        var projectMovements = await _dbContext.ProjStakeHolderMov
                               .Where(x => x.ProjId == projectStkHolderMovementData.ProjId && x.PsmId != psmid && x.IsComment == true) // Exclude the current record
                               .ToListAsync();


                        foreach (var item in projectMovements)
                        {
                            item.IsRead = false;
                            if (item.PsmId == 5434)
                            {
                                break;
                            }
                        }
                        var latestPsmId = await _dbContext.ProjStakeHolderMov
         .Where(x => x.ProjId == projectStkHolderMovementData.ProjId && x.IsComplete == false)
         .OrderByDescending(x => x.PsmId)
         .Select(x => x.PsmId)
         .FirstOrDefaultAsync();

                        if (latestPsmId != 0)
                        {
                            if (latestPsmId == 5434)
                            {
                                Console.WriteLine("id");
                            }

                            var latestMovement = await _dbContext.ProjStakeHolderMov
                                .FirstOrDefaultAsync(x => x.PsmId == latestPsmId);
                            if (latestMovement.PsmId == 5434)
                            {
                                Console.WriteLine("id");
                            }
                            if (latestMovement != null)
                            {
                                latestMovement.IsRead = false;

                                await _dbContext.SaveChangesAsync();
                            }
                        }



                        _dbContext.ProjStakeHolderMov.UpdateRange(projectMovements);
                        await _dbContext.SaveChangesAsync();




                        
                        projectStkHolderMovementData.DateTimeOfUpdate = CommentDate; // To show the comment date on the dashboard btnGetsummay 

                       
                        var rets = await _projectsRepository.UpdateProjectStkMovementAsync(projectStkHolderMovementData);

                        if (rets != null)
                        {
                            if (cmmets.PsmId == 5434)
                            {
                                Console.WriteLine("Break");
                            }
                            var ret = await _stkCommentRepository.AddWithReturn(cmmets);

                            if (ret != null)
                                return Json(nmum.Save);
                            else
                                return Json(0);
                        }
                        else
                        {
                            return Json(0);
                        }
                    }
                    return Json(0);

                  
                }

                else
                {
                    return Json(nmum.NotSave);
                }

            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "SendCommentonProject");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while Send Comment on Project in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");

                return Json(nmum.Exception);
            }
        }


        public async Task<IActionResult> GetAllCommentBypsmId_UnitId(int psmId, int stakeholderId, int projId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                StkComment stkComment = new StkComment();
                stkComment.ProjId = projId;
                stkComment.PsmId = psmId;
                var ret = await _stkCommentRepository.GetAllCommentBypsmId_UnitId(stkComment);

                return Json(ret);

            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }
        public async Task<IActionResult> GetCommentStatus(int UnitId)
        {
            var ret = await _projComments.GetCommentStatus(UnitId);
            return Json(ret);
        }
        #endregion



        #region Project History
        [HttpGet]

        public async Task<IActionResult> ProjHistory(string userid, int? dataProjId, int? dtaProjID, string? AttPath, int? psmid, string? Projpin, string? EncyID, EncryModel? encryModel, string Type)
        {
            Thread.Sleep(500);
            try
            {

                var Logins = SessionHelper.GetObjectFromJson<Login>(
                    _httpContextAccessor.HttpContext.Session, "User");

                ViewBag.logins = Logins;
                string actufilename = "";
                string AttDocuDescs = "";

                if (EncyID != null)
                {
                    ViewBag.SubmitCde = true;
                    ViewBag.EncyID = EncyID;
                    ViewBag.Type = Type;
                }
               
                if (userid == null && dataProjId == null && dtaProjID == null && AttPath == null && psmid == null && EncyID == null)
                {
                    EncyID = ViewBag.EncyID;
                    if (TempData.ContainsKey("Psmiiddel"))
                    {
                        if (TempData["Psmiiddel"] is int)
                        {

                            psmid = (int)TempData["Psmiiddel"];
                            TempData.Remove("Psmiiddel");
                            dataProjId = null;
                            userid = Logins.UserName;
                            ViewBag.SubmitCde = true;
                        }
                    }
                }

                if (encryModel.EncryItem != null)
                {
                    var UnprotectedValue = _dataProtector.Unprotect(encryModel.EncryItem.ToString() ?? "");
                    var originalData = JsonConvert.DeserializeObject<MyRequestModel>(UnprotectedValue);
                    dtaProjID = originalData.DtaProjID;
                    if (dtaProjID == 0)
                    {
                        dtaProjID = null;
                    }

                    AttPath = originalData.AttPath;
                    Projpin = originalData.Projpin;
                    psmid = originalData.PsmId;
                    actufilename = originalData.ActFileName;
                    AttDocuDescs = originalData.AttDocuDesc;

                    ViewBag.SubmitCde = true;
                    encryModel.EncryItem = null;
                }
                else
                {
                    ViewBag.SubmitCde = false;
                }

                if (EncyID != null)
                {

                    TempData["EncyID"] = EncyID;
                }
                else
                {
                    dataProjId = dataProjId;
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                TempData["ipadd"] = watermarkText;

                if (EncyID != null)
                {
                    try
                    {
                        string decryptedValue = _dataProtector.Unprotect(EncyID);
                        dataProjId = int.Parse(decryptedValue);
                        var udpate = await _Remainder.UpdateReaminderRead(dataProjId, 0);
                        ViewBag.IsCommentPsmiId = await _projectsRepository.GetIsCommentPsmiId(dataProjId, Logins.unitid);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.Message;
                        swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    }

                }
                else
                {
                    dataProjId = dataProjId;
                }


                int statgeIDMAx = await _stkholdmove.GetlaststageId(dataProjId);
                ViewBag.stageid = statgeIDMAx;
                var projdetails = await _projectsRepository.GetProjectByIdAsync1(dataProjId);


                var dto3 = await _commentRepository.GetCommentByPsmIdAsync(projdetails.CurrentPslmId);

                ViewBag.CommentByStakeholderList = dto3;

                var ProjMovementHist = await _projStakeHolderMovRepository.ProjectMovHistory(dataProjId);
                ViewBag.ProjMovementHist = ProjMovementHist.DTOProjectMovHistorypsmlst;
                ViewBag.ProjMovementHistcomd = ProjMovementHist.DTOProjectMovHistorycmdlst;
                ViewBag.Projid = dataProjId;

                bool isprocess = _dbContext.Projects.FirstOrDefault(x => x.ProjId == dataProjId).IsProcess;
                ViewBag.Isprocess = isprocess;
                var latestpsmid = _projStakeHolderMovRepository.GetLastRecProjectMov(dataProjId);
                ViewBag.lasttounit = _dbContext.ProjStakeHolderMov.FirstOrDefault(x => x.PsmId == latestpsmid)?.ToUnitId;
                var getcurrentpsmid = _dbContext.Projects.Find(dataProjId).CurrentPslmId;

                ViewBag.documenttype = _dbContext.AttHistory
    .Any(x => x.PsmId == getcurrentpsmid && x.DocumentTypeId == 3);
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
                        prohis[0].ActFileName = actufilename;
                        prohis[0].DocumentDesc = AttDocuDescs;
                    }

                    atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(psmid ?? 0);

                    prohis[0].Atthistory = atthis;
                    prohis[0].ProjectDetl.Add(projects);

                    
                    return View(prohis);
                }


                ViewBag.DataProjId = dataProjId;
                List<ProjHistory> projHistory = await _projectsRepository.GetProjectHistorybyID(Logins.unitid);
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
                        projHist[0].ActFileName = actufilename;
                        projHist[0].DocumentDesc = AttDocuDescs;
                        atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(psmove.PsmId);
                        projHist[0].Atthistory = atthis;
                    }


                    projHist[0].Attachments = AttPath;
                    projHist[0].ActFileName = actufilename;
                    projHist[0].DocumentDesc = AttDocuDescs;
                    return View(projHist);
                }
                else if (dataProjId > 0)
                {

                    List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID(dataProjId);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(dataProjId ?? 0);
                    projHist[0].ProjectDetl.Add(projects);

                    var stholder = await _psmRepository.GetProjStakeHolderMovByIdAsync(projects.CurrentPslmId);

                    ViewBag.DataProjId = projHist.Select(a => a.ProjId).FirstOrDefault();
                    return View(projHist);

                }
                else
                {
                    List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID(dataProjId);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(projHist[0].ProjId);
                    projHist[0].ProjectDetl.Add(projects);

                    ViewBag.DataProjId = projHist.Select(a => a.ProjId).FirstOrDefault();
                    if (projHist != null)
                        projHist[0].Attachments = AttPath;
                    projHist[0].ActFileName = actufilename;
                    projHist[0].DocumentDesc = AttDocuDescs;
                    return View(projHist);

                }
                return null;
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }


        #endregion


        #region Attchment Document

        public async Task<IActionResult> AttDetails(int Id)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                List<tbl_AttHistory> atthis = new List<tbl_AttHistory>();
                atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(Id);
                return PartialView("_attachmetsview", atthis);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }

        public async Task<IActionResult> AttDetailsRead(int Id)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                List<tbl_AttHistory> atthis = new List<tbl_AttHistory>();
                atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(Id);
                return PartialView("_attachmetsread", atthis);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }

        #endregion


        #region Watermarkpdf for attach

        string filepathpdf = "";

        public IActionResult WatermarkWithPdf(string id)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                try
                {
                    var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                    var filePath = System.IO.Path.Combine(_environment.WebRootPath, "Uploads\\" + id + "");
                    if (System.IO.File.Exists(filePath))
                    {
                        Random rnd = new Random();
                        string Dfilename = rnd.Next(1, 1000).ToString() + ".pdf";
                        var pdfBytes = generate2(filePath, ip);
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
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);

                    int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                    var eventId = new EventId(dynamicEventId, "WatermarkWithPdf");
                    _logger.Log(LogLevel.Error, eventId, "An error occurred while Watermark With Pdf in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                    return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }

        public void OnTimer(Object source, ElapsedEventArgs e)
        {

            try
            {
                var filePath1 = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot\\DownloadFile\\" + filepathpdf + ".pdf");

                if (System.IO.File.Exists(filePath1))
                {

                    System.IO.File.Delete(filePath1);


                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion


        [HttpPost]
        public async Task<IActionResult> IsUnReadInbox(int PsmId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                if (Logins != null)
                {
                    try
                    {
                        tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                        psmove = await _projectsRepository.GettXNByPsmIdAsync(PsmId);
                        psmove.IsRead = false;
                        await _projectsRepository.UpdateTxnAsync(psmove);

                        return Json(PsmId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred in ProjectsController.");

                        return StatusCode(500, new
                        {
                            success = false,
                            message = "An unexpected error occurred. Please try again later."
                        });
                    }

                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "IsUnReadInbox");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on IsUnReadInbox in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }

        }


        [HttpPost]
        public async Task<IActionResult> IsUnReadComment(int Projid, int PsmId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                try
                {
                    List<tbl_ProjStakeHolderMov> inboxComments = await _projectsRepository.GetCommentByExcludingPsmId(Projid, Logins.unitid);
                    foreach (var comment in inboxComments)
                    {
                        comment.IsRead = false;
                        await _projectsRepository.UpdateTxnAsync(comment);
                    }

                    return Json(Projid);
                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                    var eventId = new EventId(dynamicEventId, "IsUnReadComment");
                    _logger.Log(LogLevel.Error, eventId, "An error occurred while on IsUnReadComment in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                    return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }



        [HttpGet]
        public async Task<JsonResult> GetProjectCommentCount()
        {
            try
            {
                int count = await _projectsRepository.GetNotificationCommentCount();
                return new JsonResult(count); // Returns the count as JSON
            }
            catch (Exception ex)
            {
                var errorId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
                _logger.LogError(ex, "Unhandled exception. ErrorId={ErrorId}", errorId);

                return new JsonResult(new
                {
                    message = "Something went wrong. Please contact the administrator.",
                    errorId
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

        [HttpGet]
        public async Task<JsonResult> GetProjectInboxCount()
        {
            try
            {
                int count = await _commentRepository.GetNotificationInboxCount();
                return new JsonResult(count); // Returns the count as JSON
            }
            catch (Exception ex)
            {
                var errorId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
                _logger.LogError(ex, "Unhandled exception. ErrorId={ErrorId}", errorId);

                return new JsonResult(new
                {
                    message = "Something went wrong. Please contact the administrator.",
                    errorId
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }


        [HttpGet]

        public async Task<IActionResult> ProjectMovement(string? ProjName)
        {
            return View();

        }
        public async Task<IActionResult> GetProjectMov(int Id)
        {

            try
            {
                var ret = await _projStakeHolderMovRepository.ProjectMovement(Id);
                return Json(ret);
            }
            catch (Exception ex)
            {
                return Json(-1);
            }
        }

        public async Task<IActionResult> ProjectMovementUpdate(tbl_ProjStakeHolderMov psmove)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            var data = await _projectsRepository.GettXNByPsmIdAsync(psmove.PsmId);
            data.ProjId = psmove.ProjId;
            data.StatusActionsMappingId = psmove.StatusActionsMappingId;
            data.Remarks = psmove.Remarks;
            data.ToUnitId = psmove.ToUnitId;
            data.UpdatedByUserId = Logins.UserIntId;
            data.DateTimeOfUpdate = psmove.TimeStamp;
            data.IsActive = true;
            data.EditDeleteDate = psmove.TimeStamp;
            data.EditDeleteBy = Logins.UserIntId;
            data.TimeStamp = psmove.TimeStamp;

            var Ret = await _psmRepository.UpdateWithReturn(data);

            if (Ret != null)
            {

                var nextPsmMove = await _projectsRepository.GetNextPsmMoveAsync(psmove.ProjId, psmove.PsmId);


                if (nextPsmMove != null)
                {

                    UnitDtl unitDetail = new UnitDtl();
                    unitDetail = await _unitRepository.GetUnitDtl(psmove.ToUnitId);
                    if (unitDetail != null)
                    {
                        ApplicationUser userdet = await _userManager.FindByNameAsync(unitDetail.UnitName);
                        if (userdet != null)
                        {

                            var rankName = _dbContext.mRank.FirstOrDefault(x => x.Id == userdet.Rank);
                            if (rankName != null)
                            {
                                nextPsmMove.UserDetails = rankName.RankName + " " + userdet.Offr_Name.Trim() + " / " + userdet.UserName.Trim() + "";
                            }
                        }

                    }

                    nextPsmMove.FromUnitId = psmove.ToUnitId;

                    await _psmRepository.UpdateWithReturn(nextPsmMove);
                }

                return Json(Ret);
            }


            else
            {
                return Json(nmum.NotSave);
            }
        }


        public async Task<IActionResult> GetALLByProjectName(string? ProjName)
        {
            var ProjectName = await _projectsRepository.GetALLByProjectName(ProjName);
            return Json(ProjectName);
        }


        public async Task<IActionResult> ProcessNotification(int ProjId, int unitid, DateTime FwdDateForComment)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (ProjId != null)
                    {
                        var project = await _projectsRepository.GetProjectByIdAsync(ProjId);

                        unitid = project.StakeHolderId;
                        if (unitid == 1)
                        {
                            int[] stausid = { 26, 31, 37, 21 };
                            int[] unitids = { 4, 3, 5, 1 };
                            for (int i = 0; i < stausid.Length; i++)
                            {
                                Notification notify = new Notification();

                                notify.ProjId = ProjId;
                                notify.NotificationFrom = Logins.unitid ?? 0;
                                notify.NotificationTo = unitids[i];
                                notify.IsRead = false;
                                notify.ReadDateTime = FwdDateForComment;
                                notify.NotificationType = 1;

                                await _psmRepository.AddNotificationCommentAsync(notify);

                            }
                        }
                        else
                        {
                            int[] stausid = { 26, 31, 37, 21, 21 };
                            int[] unitids = { 4, 3, 5, 1, unitid };
                            for (int i = 0; i < stausid.Length; i++)
                            {
                                Notification notify = new Notification();

                                notify.ProjId = ProjId;
                                notify.NotificationFrom = Logins.unitid ?? 0;
                                notify.NotificationTo = unitids[i];
                                notify.IsRead = false;
                                notify.ReadDateTime = FwdDateForComment;
                                notify.NotificationType = 1;

                                await _psmRepository.AddNotificationCommentAsync(notify);

                            }
                        }


                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
                }
                else
                {
                    return Redirect("/Identity/Account/login");
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "ProcessNotification");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Process Notification in ProjectsController.", ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        [HttpPost]
        public async Task<IActionResult> IsReadComment(int ProjId, int PsmId)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                try
                {
                  ;
                    tbl_ProjStakeHolderMov inboxComments = await _projectsRepository.GettXNByPsmIdAsync(PsmId);
                    if (inboxComments != null)
                    {
                        if (inboxComments.IsRead == false)
                        {
                            inboxComments.IsRead = true;
                            await _projectsRepository.UpdateTxnAsync(inboxComments);

                           }
                    }
                    int getunreadComments=   _dbContext.ProjStakeHolderMov.Where(x => x.ToUnitId == Logins.unitid && x.IsRead == false && x.IsComment == true).Count();

                    return Json(getunreadComments);
                }
                catch (Exception ex)
                {
                    int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                    var eventId = new EventId(dynamicEventId, "IsReadComment");

                    // Log full exception on server
                    _logger.LogError(eventId, ex, "Unhandled error in ProjectsController.IsReadComment.");

                    // If you keep your utility, DON'T pass ex.Message (it may leak). Pass ex only or a generic text.
                    swas.BAL.Utility.Error.ExceptionHandle("Unhandled error in IsReadComment."); // or ExceptionHandle(ex) if overload exists

                    // Return generic error (no exception details)
                    return Json(new { success = false, message = "Something went wrong." });
                    // If your frontend strictly expects 0/1:
                    // return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }

        [HttpPost]
        public IActionResult SetCalendarModeInSession(int mode)
        {
            HttpContext.Session.SetInt32("CalendarMode", mode);
            return Json(new { success = true, message = "Calendar mode saved in session." });
        }







        [HttpPost("Projects/LogDateApprovalWithRemarks")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogDateApproval(int ProjId, bool UserReq, int actiontype, string remarks)
        {
            var user = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (ProjId == 0 || user == null)
                return BadRequest(new { success = false, message = "Invalid input." });

            try
            {

                var dateApproval = new DateApproval
                {
                    ProjId = Convert.ToInt32(ProjId),
                    UnitId = user.unitid,
                    Request_Date = DateTime.Now,
                    UserRequest = UserReq,
                    DDGIT_approval = false,
                    DDGIT_Approval_dat = null,
                    User = Helper1.LoginDetails(user),
                    IsRead = false,
                    RequestType = 1
                };

                _dbContext.DateApproval.Add(dateApproval);
                await _dbContext.SaveChangesAsync();


                var legacyLog = new LegacyHistory
                {
                    ProjectId = ProjId,
                    UnitId = user.unitid,
                    FromUnit = user.unitid, // Optional: update if needed
                    ActionBy = $"{user.Rank} {user.Offr_Name}"
                 ,
                    ActionType = (ActionTypeEnum)actiontype,
                    Remarks = remarks,
                    ActionDate = DateTime.Now,
                    Userdetails = Helper1.LoginDetails(user)
                };

                await _legacyHistoryRepository.AddHistoryAsync(legacyLog);
                return Json(new { success = true, message = "Request has been forward to admin for legacy project ingestion." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error logging date approval." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ApproveDateRequest(int id, int actiontype, string remarks)
        {
            var user = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext?.Session, "User");
            try
            {
                var entry = await _dbContext.DateApproval.FindAsync(id);
                if (entry == null)
                    return Json(new { success = false, message = "Record not found." });

                entry.DDGIT_approval = !(entry.DDGIT_approval ?? false);
                if (actiontype == 3)
                {
                    entry.DDGIT_approval = false;
                    entry.UserRequest = false;
                }

                entry.DDGIT_Approval_dat = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                var legacyLog = new LegacyHistory
                {
                    ProjectId = entry.ProjId ?? 0,
                    UnitId = user?.unitid,
                    FromUnit = user?.unitid, // Optional: update if needed
                    ActionBy = (user?.Rank ?? "") + " " + (user?.Offr_Name ?? ""),
                    ActionType = (ActionTypeEnum)actiontype,
                    Remarks = remarks,
                    ActionDate = DateTime.Now,
                    Userdetails = Helper1.LoginDetails(user)
                };

                await _legacyHistoryRepository.AddHistoryAsync(legacyLog);

                var message = entry.DDGIT_approval == true ? "Request approved successfully." : "Request Rejected.";

                return Json(new { success = true, message = message, currentStatus = entry.DDGIT_approval });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating." });
            }
        }





        [HttpGet]
        public async Task<IActionResult> GetRevettedProjects([FromQuery] string searchQuery)
        {
            var query = from p in _dbContext.Projects
                        join stkmov in _dbContext.ProjStakeHolderMov on p.ProjId equals stkmov.ProjId
                        join tsam in _dbContext.TrnStatusActionsMapping on stkmov.StatusActionsMappingId equals tsam.StatusActionsMappingId
                        join ms in _dbContext.mStatus on tsam.StatusId equals ms.StatusId
                        join ma in _dbContext.mActions on tsam.ActionsId equals ma.ActionsId
                        where tsam.StatusActionsMappingId == 103
                        select new
                        {
                            p.ProjId,
                            p.ProjName,
                            StatusName = ms.Status,
                            ActionName = ma.Actions
                        };

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(x => x.ProjName.Contains(searchQuery));
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }





        [HttpGet]
        public async Task<IActionResult> GetProjectDetails([FromQuery] int projId)
        {
            var project = await _dbContext.Projects
                                          .Where(p => p.ProjId == projId)
                                          .FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        public async Task<string> GetReVettedProjectName(tbl_Projects project)
        {
            string originalName = project.ProjName.Trim();
            string baseName = originalName;
            int currentCount = 0;


            var reVettedPattern = new System.Text.RegularExpressions.Regex(@"(.*)\sRe-Vetted\s(\d+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var match = reVettedPattern.Match(originalName);
            if (match.Success)
            {
                baseName = match.Groups[1].Value.Trim();
                currentCount = int.Parse(match.Groups[2].Value);
            }


            var count = await _dbContext.Projects
                .Where(i => i.ProjName.Trim().ToUpper().StartsWith(baseName.ToUpper()) &&
                            i.ProjName.Contains("Re-Vetted"))
                .CountAsync();


            int newCount = Math.Max(count, currentCount) + 1;


            return $"{baseName} Re-Vetted {newCount}";
        }

        [HttpPost]
        public async Task<IActionResult> GetProjectLegacyHistory(int ProjectId)
        {

            if (ProjectId <= 0)
                return BadRequest(new { success = false, message = "Invalid project ID." });

            var history = await _legacyHistoryRepository.GetHistoryByProjectIdAsync(ProjectId);


            if (history == null || !history.Any())
                return Json(new { success = false, message = "No legacy history found." });



            return Json(history);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectByKeyup(string searchQuery)
        {


            var query = _dbContext.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(x => x.ProjName.Contains(searchQuery));
            }
            var result = await query.ToListAsync();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> SendRemainder(int ProjId, string Remarks)
        {
            var user = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext?.Session, "User");
            if (user == null)
                return Json(0); // or return Unauthorized();

            var latestpsmid = _projStakeHolderMovRepository.GetLastRecProjectMov(ProjId);
            var latestpsmiddata = _dbContext.ProjStakeHolderMov.Find(latestpsmid);

            latestpsmiddata.IsRead = false;


            _dbContext.ProjStakeHolderMov.Update(latestpsmiddata);

            if (latestpsmiddata == null)
                return Json(0); // not found
            int Psmid = latestpsmid;
            int fromUnitId = user.unitid ?? 0;
            int toUnitId = latestpsmiddata.ToUnitId;
            string userDetails = Helper1.LoginDetails(user);
            int result = await _Remainder.AddRemainder(ProjId, Psmid, fromUnitId, toUnitId, Remarks, userDetails);

            return Json(result); // 1 if success
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectRemainderHistory(int ProjectId)
        {
            try
            {
                _logger.LogInformation("Fetching history for ProjectId: {ProjectId}", ProjectId);
                if (ProjectId <= 0)
                    return BadRequest(new { success = false, message = "Invalid project ID." });

                var history = await _Remainder.ProjectRemainderMovHistory(ProjectId);

                if (history == null || !history.Any())
                    return Json(new { success = false, message = "No legacy history found." });

                return Json(new { success = true, data = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching project history for ProjectId: {ProjectId}", ProjectId);
                return StatusCode(500, new { success = false, message = "An error occurred while fetching project remainder history." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRemaRead(int ProjectId)
        {
            try
            {
                var udpate = await _Remainder.UpdateReaminderRead(ProjectId,0);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "An error occurred while Update project remainder history." });
            }
        }

        public async Task<IActionResult> FindProjectForComment(string searchQuery)
        {
            try
            {
                var user = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext?.Session, "User");

                var projects = await _projComments.FindForComment(user.unitid, searchQuery);
                return Json(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while Find Project For Comment." });

            }
        }
       
        [HttpGet]
        public IActionResult GetDate()
        {
            var nowIst = DateTime.Now;
            
            var dateYmd = nowIst.ToString("yyyy-MM-dd");
            var dateTimeLocal = nowIst.ToString("yyyy-MM-ddTHH:mm:ss");
            var analy = DateTime.Now.TimeOfDay.ToString();
            return Json(new { dateYmd, dateTimeLocal, analy});
        }
        [HttpGet]
        public async Task<IActionResult> GetStkCommentBystkId(int PsmId)
        {
            var psmove = await _stkCommentRepository.GetCommentByPsmid(PsmId);
            return Json(psmove);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStkcomments(int stkcommentid, string comments, int ddlstatus, DateTime? CommentDateFwd)
        {
            if (stkcommentid <= 0)
                return BadRequest("Invalid Comment ID");
            var commentEntity = await _dbContext.StkComment.FirstOrDefaultAsync(c => c.StkCommentId == stkcommentid);

            if (commentEntity == null)
                return NotFound("Comment not found");
            commentEntity.Comments = comments;
            commentEntity.StkStatusId = ddlstatus;
            if (CommentDateFwd.HasValue)
                commentEntity.DateTimeOfUpdate = CommentDateFwd.Value;
            await _dbContext.SaveChangesAsync();
            return Json(1);
        }



		[HttpPost]
		public IActionResult CheckPreviousApprovals(int ProjId, int StatusId, int Actionsid)
		{

			try
			{
				var notapproved = _projStakeHolderMovRepository.CheckPreviousApprovals(StatusId, ProjId, Actionsid);
				return Json(new { message = notapproved });
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
        
        
        [HttpGet]
		public async Task<IActionResult> GetActParkedProject()
		{

			try
			{
				var List = await _projectsRepository.GetActParkedItemsAsync();
				return Json(List);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
		[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ParkedProject(int psmid)
        {
            try
            {
                var unpack = _dbContext.ProjStakeHolderMov.FirstOrDefault(x => x.PsmId == psmid);

                if (unpack == null)
                    return Json(new { message = "Project not found!" });

                string parkmsg;

                if (!unpack.IsParked)
                {
                    unpack.IsParked = true;
                    parkmsg = "Project Successfully Parked";
                }
                else
                {
                    unpack.IsParked = false;
                    parkmsg = "Project Successfully Unparked";
                }

                _dbContext.ProjStakeHolderMov.Update(unpack);
                _dbContext.SaveChanges();

                return Json(new { message = parkmsg });
            }
            // add via DI: ILogger<YourController> _logger;

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in {Action}", nameof(ParkedProject));

                return Json(new
                {
                    success = false,
                    message = "Something went wrong. Please try again or contact admin."
                });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetDocumentTypes()
        {
            var docs = await _dbContext.DocumentTypes
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    isRequired = x.IsRequired
                })
                .ToListAsync();

            return Json(docs);
        }

        [HttpGet]
        public async Task<IActionResult> GetUploadedDocument(int projid, int DocumentTypeId)
        {
            var currentpsmid = _dbContext.Projects.Find(projid).CurrentPslmId;

            var doc = await _dbContext.AttHistory
                .Where(x => x.PsmId == currentpsmid &&
                            x.DocumentTypeId == DocumentTypeId)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefaultAsync();

            if (doc == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                filePath = doc.AttPath
            });
        }
    }

}
