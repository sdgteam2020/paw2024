using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Timers;

using swas.BAL.Repository;
using Microsoft.EntityFrameworkCore;
using swas.UI.Helpers;
using Microsoft.AspNetCore.Identity;
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.Extensions.Options;
using swas.DAL;
using Document = iText.Layout.Document;
using Microsoft.Extensions.Logging;
using iText.Kernel.Events;
using static swas.DAL.Models.LegacyHistory;
using swas.BAL.Utility;
using Path = System.IO.Path;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using static swas.UI.Helpers.Helper;
using swas.UI.Models;
using swas.UI.Services;
using Superpower.Model;

namespace swas.UI.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        // ── Dependencies ──────────────────────────────────────────────────────────
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
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IStkCommentRepository _stkCommentRepository;
        private readonly IProjStakeHolderMovRepository _stkholdmove;
        private readonly IProjStakeHolderCcMovRepository _projStakeHolderCcMovRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProjectsController> _logger;
        private readonly ILegacyHistoryRepository _legacyHistoryRepository;
        private readonly IRemainder _Remainder;
        private readonly LoginCryptoKeyService _loginCryptoKeyService;

        // ── Constructor ───────────────────────────────────────────────────────────
        public ProjectsController(
            IProjectsRepository projectsRepository,
            IDdlRepository ddlRepository,
            IProjStakeHolderMovRepository psmRepository,
            IHttpContextAccessor httpContextAccessor,
            IDdlRepository DDLRepository,
            IAttHistoryRepository attHistoryRepository,
            IWebHostEnvironment environment,
            IProjStakeHolderMovRepository stkholdmove,
            IDataProtectionProvider DataProtector,
            IWebHostEnvironment _webHostEnvironment,
            ICommentRepository commentRepository,
            IActionsRepository actionsRepository,
            IProjComments projComments,
            IStkCommentRepository stkCommentRepository,
            IProjStakeHolderMovRepository projStakeHolderMovRepository,
            UserManager<ApplicationUser> userManager,
            IUnitRepository unitRepository,
            IConfiguration configuration,
            ApplicationDbContext context,
            ILogger<ProjectsController> logger,
            ILegacyHistoryRepository legacyHistoryRepository,
            IProjStakeHolderCcMovRepository projStakeHolderCcMovRepository,
            IRemainder Remainder,
            LoginCryptoKeyService loginCryptoKeyService)
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
            _loginCryptoKeyService = loginCryptoKeyService;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Retrieves the session Login object. Returns null if session is missing.
        /// </summary>
        private Login? GetSessionLogin()
            => SessionHelper.GetObjectFromJson<Login>(
                _httpContextAccessor.HttpContext?.Session, "User");

        /// <summary>
        /// Returns a watermark string composed of client IP and current datetime.
        /// </summary>
        private string BuildWatermarkText()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";
            var now = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            return $" {ip}\n  {now}";
        }

        // ═════════════════════════════════════════════════════════════════════════
        // INDEX
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            CommonDTO dto = new CommonDTO();
            dto.Projects = await _projectsRepository.GetAllProjectsAsync();
            return View(dto);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // DETAILS
        // ═════════════════════════════════════════════════════════════════════════

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid project id");

            var project = await _projectsRepository.GetProjectByIdAsync(id);
            if (project == null)
                return NotFound();

            return Ok(new { success = true, project });
        }

        // ═════════════════════════════════════════════════════════════════════════
        // PROJ STAT DASHBOARD VIEW
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> ProjStatDashBdView(string? id, string? status)
        {
            var Logins = GetSessionLogin();
            if (Logins?.UserName == null || id == null)
                return Redirect("~/Identity/Account/Login");

            TempData["ipadd"] = BuildWatermarkText();

            try
            {
                string decryptedValue = _dataProtector.Unprotect(id);
                // dataProjId used for business routing — assigned but current view does not need it further
                _ = int.Parse(decryptedValue);
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                return Redirect("~/Home/Error");
            }

            MailBox mbx = new MailBox { SendItems = null };
            return View(mbx);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // PROJ DETAILS
        // ═════════════════════════════════════════════════════════════════════════
            
        [HttpGet]
        [AuthorizePermission("InboxPermission")]
        public async Task<IActionResult> ProjDetails()
        {
            try
            {
                var Logins = GetSessionLogin();
                if (Logins == null)
                    return LocalRedirect("~/Identity/Account/Login");

                TempData["ipadd"] = BuildWatermarkText();
                ViewBag.SubmitCde = "0";
                ViewBag.tbl_mUnitBranch = _dbContext.tbl_mUnitBranch.ToList();

                MailBox mbx = new MailBox
                {
                    Remainder = await _Remainder.GetAllAsync()
                };

                var notificationContent = _configuration
                    .GetSection("NotificationContent")
                    .Get<NotificationContent>();
                ViewBag.NotificationContent = notificationContent;

                if (Logins.unitid != null)
                    ViewBag.unitid = Logins.unitid;

                ViewBag.remainder = _dbContext.TrnRemainders.ToList();

                mbx.InBox = await _projectsRepository.GetActInboxAsync();
                mbx.Draft = await _projectsRepository.GetActDraftItemsAsync();
                mbx.SendItems = await _projectsRepository.GetActSendItemsAsync();
                mbx.CompletedItems = await _projectsRepository.GetActComplettemsAsync();

                return View(mbx);
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // GET CC PROJECTS
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost]
        public async Task<IActionResult> GetActCcProject()
            => Json(await _projectsRepository.GetActCcItemsAsync());

        // ═════════════════════════════════════════════════════════════════════════
        // GET ALL STATUS
        // ═════════════════════════════════════════════════════════════════════════

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllStatus()
        {
            var ss = await _dlRepository.ddlStatus();
            return View(ss);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // CREATE PROJECT
        // ═════════════════════════════════════════════════════════════════════════

        #region CreateProject

        [HttpGet]
        [AuthorizePermission("AddNewProject")]
        public async Task<IActionResult> Create(string id)
        {
            try
            {
                // Populate dropdowns
                ViewBag.WhitelistOptions = await _projectsRepository.GetDropdown("WhitelistStatusOptions");
                ViewBag.IsAI_ML = await _projectsRepository.GetDropdown("IsAIMLProj");
                ViewBag.SecurityClassifications = await _projectsRepository.GetDropdown("SecurityClassification");
                ViewBag.TypeofSWOption = await _projectsRepository.GetDropdown("TypeofSWOptions");
                ViewBag.BeingDevpInhouseOption = await _projectsRepository.GetDropdown("BeingDevpInhouseOptions");
                ViewBag.EndorsmentbyHeadofOption = await _projectsRepository.GetDropdown("EndorsmentbyHeadofOptions");

                ViewBag.NotificationContent = _configuration
                    .GetSection("NotificationContent")
                    .Get<NotificationContent>();

                if (id != null)
                {
                    string decryptedValue = _dataProtector.Unprotect(id);
                    int ids = int.Parse(decryptedValue);

                    tbl_Projects proj = await _projectsRepository.GetProjectByPsmIdAsync(ids);
                    ViewBag.ProjectEncyId = id;
                    ViewBag.Projects = await _projectsRepository.GetMyProjects();
                    return View(proj);
                }

                TempData["SubCde"] = false;
                TempData.Keep("SubCde");

                var Logins = GetSessionLogin();
                if (Logins == null)
                    return Redirect("/Identity/Account/login");

                ViewBag.Projects = await _projectsRepository.GetMyProjects();
                return View(new tbl_Projects());
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        // ── Upload attachment ─────────────────────────────────────────────────────

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 83_886_080)]
        public async Task<IActionResult> UploadMultiFile(
            IFormFile uploadfile,
            string Reamarks,
            int PsmId,
            int DocumentTypeId)
        {
            // Guard: file must exist and be ≤ 10 MB
            if (uploadfile == null || uploadfile.Length == 0 || uploadfile.Length > 10 * 1024 * 1024)
                return Json(-5);

            var ext = Path.GetExtension(uploadfile.FileName).ToLowerInvariant();
            if (ext != ".pdf")
                return Json(-2);

            if (PsmId <= 0)
                return Json(-1);

            var Logins = GetSessionLogin();
            if (Logins == null)
                return Json(-401);

            string uniqueFileName = $"Swas_{Guid.NewGuid()}{ext}";
            string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await uploadfile.CopyToAsync(stream);

            var atthis = new tbl_AttHistory
            {
                ActionId = 0,
                AttPath = uniqueFileName,
                Reamarks = Reamarks,
                PsmId = PsmId,
                UpdatedByUserId = Logins.unitid,
                IsDeleted = false,
                IsActive = true,
                EditDeleteBy = Logins.unitid,
                EditDeleteDate = DateTime.Now,
                TimeStamp = DateTime.Now,
                ActFileName = uploadfile.FileName
            };

            if (DocumentTypeId > 0)
                atthis.DocumentTypeId = DocumentTypeId;

            await _attHistoryRepository.AddAttHistoryAsync(atthis);
            return Json(1);
        }

        // ── Add / Edit Project ────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
      

        public async Task<IActionResult> AddProject(tbl_Projects Data, string? RequestRemarks)
        {
            try
            {
                if (Data == null)
                    return BadRequest(new { success = false, message = "Invalid project data." });

                // Cross-field validation
                if (Data.CompletionDate.HasValue && Data.InitiatedDate.HasValue
                    && Data.CompletionDate.Value < Data.InitiatedDate.Value)
                {
                    ModelState.AddModelError(
                        nameof(Data.CompletionDate),
                        "Completion Date cannot be earlier than the Initiated Date.");
                }

                if (!ModelState.IsValid)
                {
                    // Log field errors server-side only; do NOT expose raw exception messages to client
                    foreach (var item in ModelState)
                    {
                        foreach (var error in item.Value.Errors)
                        {
                            var msg = string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "(exception)"
                                : error.ErrorMessage;
                            _logger.LogWarning("AddProject ModelState error — Field: {Field}, Error: {Error}",
                                item.Key, msg);
                        }
                    }

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid submitted data.",
                        errors = ModelState
                            .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray())
                    });
                }

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return Unauthorized(new { success = false, message = "Session expired. Please login again." });

                var Logins = SessionHelper.GetObjectFromJson<Login>(httpContext.Session, "User");
                if (Logins == null)
                    return Unauthorized(new { success = false, message = "Session expired. Please login again." });

                // Populate audit / ownership fields
                Data.StakeHolderId = Logins.unitid ?? 0;
                Data.IsActive = true;
                Data.EditDeleteDate = DateTime.Now;
                Data.EditDeleteBy = 0;
                Data.IsDeleted = false;
                Data.IsSubmited = false;
                Data.UpdatedByUserId = Logins.UserIntId;
                Data.Comments = Data.InitialRemark;

                if (Data.Date_type == 1)
                {
                    Data.DateTimeOfUpdate = Data.InitiatedDate;
                }
                else
                {
                    Data.InitiatedDate = DateTime.Now;
                    Data.DateTimeOfUpdate = DateTime.Now;
                }

                bool isEdit = Data.ProjId != 0;

                if (!isEdit)
                {
                    if (Data.IsWhitelisted == "Re-Vetted")
                        Data.ProjName = await GetReVettedProjectName(Data);

                    if (await _projectsRepository.ProjectNameExists(Data))
                        return Json(-3);
                }
                else
                {
                    var existingProject = await _projectsRepository.GetProjectByIdAsync(Data.ProjId);
                    if (existingProject == null)
                        return NotFound(new { success = false, message = "Project not found." });

                    _dbContext.Entry(existingProject).State = EntityState.Detached;

                    if (Data.IsWhitelisted == "Re-Vetted" && existingProject.IsWhitelisted != "Re-Vetted")
                    {
                        Data.ProjName = await GetReVettedProjectName(Data);
                        if (await _projectsRepository.ProjectNameExists(Data))
                            return Json(-3);
                    }
                    else if (Data.IsWhitelisted == "Re-Vetted"
                             && existingProject.IsWhitelisted == "Re-Vetted"
                             && (!string.IsNullOrWhiteSpace(Data.ProjName)
                                 && !Data.ProjName.Contains("Re-Vetted")))
                    {
                        return Json(-5);
                    }
                    else if (Data.IsWhitelisted == "Re-Vetted"
                             && existingProject.IsWhitelisted == "Re-Vetted"
                             && string.IsNullOrWhiteSpace(Data.ProjName))
                    {
                        return Json(-5);
                    }
                }

                int projid;

                if (Data.ProjId == 0)
                {
                    Data.CurrentPslmId = 0;
                    projid = await _projectsRepository.AddProjectAsync(Data, RequestRemarks);
                    Data = await _projectsRepository.GetProjectByIdAsync(projid);

                    if (Data == null)
                        return StatusCode(500, new { success = false, message = "Project was saved but could not be retrieved." });
                }
                else
                {
                    Data.EditDeleteDate = DateTime.Now;
                    await _projectsRepository.UpdateProjectAsync(Data, RequestRemarks);
                    Data = await _projectsRepository.GetProjectByIdAsync(Data.ProjId);

                    if (Data == null)
                        return StatusCode(500, new { success = false, message = "Project was updated but could not be retrieved." });
                }

                // Copy old attachments when re-vetted project links to an old PSM id
                if (Data.OldPsmid != 0)
                {
                    var oldAttachments = await _dbContext.AttHistory
                        .Where(x => x.PsmId == Data.OldPsmid)
                        .AsNoTracking()
                        .ToListAsync();

                    if (oldAttachments.Any())
                    {
                        foreach (var old in oldAttachments)
                        {
                            await _dbContext.AttHistory.AddAsync(new tbl_AttHistory
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
                            });
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                }

                return Json(Data);
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "AddProjectError");
                _logger.LogError(eventId, ex,
                    "An error occurred while adding/updating a project in ProjectsController.");
                Error.ExceptionHandle("Add Project failed in ProjectsController.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to add project at this time. Please try again later."
                });
            }
        }

        // ── Attachment history ────────────────────────────────────────────────────

        public async Task<IActionResult> GetAtthHistoryByProjectId(int PslmId)
        {
            try
            {
                return Json(await _attHistoryRepository.GetAttHistoryByIdAsync(PslmId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAtthHistoryByProjectId for PslmId {PslmId}", PslmId);
                return Json(-1);
            }
        }

        // ── Delete project ────────────────────────────────────────────────────────

        public async Task<IActionResult> DeleteProjects(int ProjectId)
        {
            var ret = await _projectsRepository.DeleteProjectAsync(ProjectId);
            return Json(ret ? 1 : 0);
        }

        // ── Submit project ────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> ProjectSubmited(int projid, int type, string Remarks)
        {
            try
            {
                var project = await _projectsRepository.GetProjectByIdAsync(projid);

                if (type == 1)
                {
                    var requiredDocIds = await _dbContext.DocumentTypes
                        .Where(d => d.IsRequired && d.IsActive)
                        .Select(d => d.Id)
                        .ToListAsync();

                    var uploadedDocIds = await _dbContext.AttHistory
                        .Where(a => a.PsmId == project.CurrentPslmId)
                        .Select(a => a.DocumentTypeId)
                        .Distinct()
                        .ToListAsync();

                    var missingDocIds = requiredDocIds
                        .Except(uploadedDocIds.Where(x => x.HasValue).Select(x => x!.Value))
                        .ToList();

                    if (missingDocIds.Any())
                        return Json(new { type = 404, message = "Please upload all required documents." });

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
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "ProjectSubmited");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while Project Submited in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        // ── Forward confirm ───────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> FwdProjConfirm(int PslmId)
        {
            try
            {
                var Logins = GetSessionLogin();
                if (Logins == null)
                    return Redirect("/Identity/Account/login");

                try
                {
                    var psmove = await _projectsRepository.GettXNByPsmIdAsync(PslmId);
                    psmove.IsComplete = true;
                    await _projectsRepository.UpdateTxnAsync(psmove);
                    return Json(4);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in ProjectsController FwdProjConfirm Function.");
                    return StatusCode(500, new { success = false, message = "Something went wrong. Please try again later." });
                }
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "FwdProjConfirm");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while Fwd Proj Confirm in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        // ── Is-Read inbox ─────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsReadInbox(int PsmId)
        {
            try
            {
                if (PsmId <= 0)
                    ModelState.AddModelError(nameof(PsmId), "Invalid PsmId.");

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        type = 400,
                        message = "Invalid request data.",
                        errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value.Errors.Select(e => e.ErrorMessage).ToList())
                    });
                }

                var Logins = GetSessionLogin();
                if (Logins == null)
                    return Redirect("/Identity/Account/login");

                try
                {
                    var psmove = await _projectsRepository
                        .GettXNByPsmIdwithUnitId(PsmId, Convert.ToInt32(Logins.unitid));

                    if (psmove != null)
                    {
                        psmove.IsRead = true;
                        await _projectsRepository.UpdateTxnAsync(psmove);
                        return Json(PsmId);
                    }

                    var psCcmove = await _projStakeHolderCcMovRepository
                        .GetdataBuPsmiandTounitId(PsmId, Convert.ToInt32(Logins.unitid));

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
                    Error.ExceptionHandle(ex.ToString());
                    return Json(new { success = false, message = "Something went wrong." });
                }
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "IsReadInbox");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while IsRead Inbox in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        // ── Is-Process confirm ────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsProcessProjConfirm(string ProjId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProjId))
                    ModelState.AddModelError(nameof(ProjId), "Invalid Project Id.");

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        type = 400,
                        message = "Invalid request data.",
                        errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value.Errors.Select(e => e.ErrorMessage).ToList())
                    });
                }

                var Logins = GetSessionLogin();
                if (Logins == null)
                    return Redirect("/Identity/Account/login");

                TempData["ipadd"] = BuildWatermarkText();

                int Projid = Convert.ToInt32(_dataProtector.Unprotect(ProjId));

                try
                {
                    var proj = await _projectsRepository.GetProjectByIdAsync(Projid);
                    proj.DateTimeOfUpdate = DateTime.Now;
                    proj.IsProcess = true;
                    _dbContext.Entry(proj).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    return Json(ProjId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in IsProcessProjConfirm ProjectsController.");
                    return StatusCode(500, new { success = false, message = "An unexpected error occurred. Please try again later." });
                }
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "IsProcessProjConfirm");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while Process Project Confirm in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        // ── Delete attachment ─────────────────────────────────────────────────────

        public async Task<IActionResult> DeleteAttech(int AttechId)
        {
            try
            {
                var Logins = GetSessionLogin();
                if (Logins == null)
                    return Redirect("/Identity/Account/login");

                var ret = await _attHistoryRepository.DeleteAttHistoryAsync(AttechId);
                return Json(ret == null ? 0 : 1);
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }
        }

        #endregion

        // ═════════════════════════════════════════════════════════════════════════
        // PDF WATERMARK
        // ═════════════════════════════════════════════════════════════════════════

        public byte[]? generate2(string filePath, string ip)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var pdfDoc = new PdfDocument(new PdfReader(filePath), new PdfWriter(memoryStream));
                using var doc = new Document(pdfDoc);

                var font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
                var paragraph = new Paragraph($"{ip} {DateTime.Now}")
                    .SetFont(font)
                    .SetFontSize(30);

                var gs1 = new PdfExtGState().SetFillOpacity(0.2f);

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var pdfPage = pdfDoc.GetPage(i);
                    var over = new PdfCanvas(pdfPage);
                    over.SaveState();
                    over.SetExtGState(gs1);
                    doc.ShowTextAligned(paragraph, 297, 450, i,
                        TextAlignment.CENTER, VerticalAlignment.MIDDLE, 45);
                    over.RestoreState();
                }

                doc.Close();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                return null;
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // PROJECT MOVEMENT — PROCESS / FORWARD / PULLBACK
        // ═════════════════════════════════════════════════════════════════════════

        #region Project Movement For Process / Comment

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessMail(string encrypted_data)
        {
            if (string.IsNullOrWhiteSpace(encrypted_data))
            {
                _logger.LogWarning("ProcessMail called with empty encrypted_data");
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            int ProjId = 0;
            DateTime FwdDateForComment;
            int unitid = 0;

            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired. Please login again." });

            var cryptoKey = Logins.CryptoKey;
            if (string.IsNullOrWhiteSpace(cryptoKey))
            {
                _logger.LogError("Crypto key not configured");
                return StatusCode(500, new { success = false, message = "Server configuration error." });
            }

            try
            {
                var decryptedJson = CryptoHelper.SafeDecrypt(encrypted_data, cryptoKey);
                if (string.IsNullOrWhiteSpace(decryptedJson))
                {
                    _logger.LogWarning("Decryption returned empty result");
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                var json = JObject.Parse(decryptedJson);

                if (!int.TryParse(json["ProjId"]?.ToString(), out ProjId) ||
                    !DateTime.TryParse(json["FwdDateForComment"]?.ToString(), out FwdDateForComment) ||
                    !int.TryParse(json["unitid"]?.ToString(), out unitid))
                {
                    _logger.LogWarning("Invalid decrypted payload: {Payload}", decryptedJson);
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "JSON parsing failed in ProcessMail");
                return BadRequest(new { success = false, message = "Invalid data format." });
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "Decryption failed in ProcessMail");
                return BadRequest(new { success = false, message = "Decryption failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during decryption in ProcessMail");
                return StatusCode(500, new { success = false, message = "Error processing request." });
            }

            if (ProjId <= 0)
                return BadRequest(new { success = false, message = "Invalid project id." });

            try
            {
                var login = GetSessionLogin();
                if (login == null)
                    return Unauthorized(new { success = false, message = "Session expired. Please login again." });

                var project = await _projectsRepository.GetProjectByIdAsync(ProjId);

                if (project == null)
                    return NotFound(new { success = false, message = "Project not found." });

                if (project.InitiatedDate > FwdDateForComment)
                    return NotFound(new { success = false, message = "Process Date Cannot Less Than Initiated Date" });

                bool legacy = _dbContext.DateApproval?.Find(ProjId)?.DDGIT_approval == true;

                // IDOR protection — only unit 1 may call ProcessMail
                if (login.unitid != 1)
                    return Forbid();

                unitid = project.StakeHolderId;

                int[] statusIds = { 26, 31, 37, 21, 21, 2016, 2021, 2026 };
                int[] unitIds = { 4, 3, 5, 1, unitid, 2, 4105, 4106, };
                int[] skipUnitIds = { 4, 3, 5, 1, 2, 4105, 4106 };

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
                        DateTimeOfUpdate = DateTime.Now,
                        IsActive = true,
                        EditDeleteDate = DateTime.Now,
                        EditDeleteBy = login.unitid,
                        TimeStamp = legacy ? FwdDateForComment : DateTime.Now,
                        IsComplete = false,
                        ToUnitId = unitIds[i],
                        IsComment = true
                    };

                    await _psmRepository.AddProjStakeHolderMovAsync(psmove);
                }

                return Ok(new { success = true, message = "Process mail executed successfully." });
            }
            catch (Exception ex)
            {
                var eventId = new EventId(Guid.NewGuid().GetHashCode(), "ProcessMail");
                _logger.LogError(eventId, ex, "Error occurred in ProjectsController.ProcessMail");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        // ── Check forward condition ───────────────────────────────────────────────

        public async Task<IActionResult> CheckFwdCondition(string encrypted_payload)
        {
            if (string.IsNullOrWhiteSpace(encrypted_payload))
                return BadRequest(new { success = false, message = "Invalid request." });

            int ProjId = 0, StatusId = 0;
            string Actionsname = "";

            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired." });

            var cryptoKey = Logins.CryptoKey;
            if (string.IsNullOrWhiteSpace(cryptoKey))
                return StatusCode(500, new { success = false, message = "Server configuration error." });

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(encrypted_payload, cryptoKey);
                if (string.IsNullOrWhiteSpace(decrypted))
                    return BadRequest(new { success = false, message = "Invalid request data." });

                var obj = JsonConvert.DeserializeObject<dynamic>(decrypted.Trim('"'));
                Actionsname = obj?.Actionsname;

                if (obj == null
                    || !int.TryParse((string?)obj.ProjId, out ProjId)
                    || !int.TryParse((string?)obj.StatusId, out StatusId))
                {
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (CryptographicException)
            {
                return BadRequest(new { success = false, message = "Invalid encrypted data." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckFwdCondition");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }

            var Ret = await _psmRepository.CheckFwdCondition(ProjId, StatusId, Actionsname);
            return Json(Ret);
        }

        // ── Forward project ───────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FwdToProject(
            [FromForm] tbl_ProjStakeHolderMov psmove,
            [FromForm] string encryptedData,
            [FromForm] string currentpsmid)
        {
            try
            {
                // ── 1. Basic model validation ──────────────────────────────────────
                if (psmove == null)
                    ModelState.AddModelError(nameof(psmove), "Invalid request data.");

                if (string.IsNullOrWhiteSpace(encryptedData))
                    ModelState.AddModelError(nameof(encryptedData), "Invalid encrypted data.");

                if (string.IsNullOrWhiteSpace(currentpsmid))
                    ModelState.AddModelError(nameof(currentpsmid), "Invalid current PSM Id.");

                // Attachments are optional here
                ModelState.Remove("Attachments");
                ModelState.Remove("psmove.Attachments");
                ModelState.Remove("Attachments.File");
                ModelState.Remove("psmove.Attachments.File");

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        type = 400,
                        message = "Invalid request data.",
                        errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value.Errors.Select(e => e.ErrorMessage).ToList())
                    });
                }

                // ── 2. Decrypt request data ────────────────────────────────────────
                var cryptoKey = _loginCryptoKeyService.GetLoginCryptoKey(HttpContext);

                try
                {
                    var decryptedJson = CryptoHelper.SafeDecrypt(encryptedData, cryptoKey);
                    currentpsmid = CryptoHelper.SafeDecrypt(currentpsmid, cryptoKey).Trim('"');

                    if (string.IsNullOrWhiteSpace(decryptedJson))
                        return Json(-500);

                    var decryptedModel = JsonConvert.DeserializeObject<tbl_ProjStakeHolderMov>(decryptedJson);
                    if (decryptedModel == null)
                        return Json(-500);

                    // Only override fields that should come from encrypted payload
                    psmove.ProjId = decryptedModel.ProjId;
                    psmove.StatusActionsMappingId = decryptedModel.StatusActionsMappingId;
                    psmove.Remarks = decryptedModel.Remarks;
                    psmove.ToUnitId = decryptedModel.ToUnitId;
                    psmove.TimeStamp = decryptedModel.TimeStamp;
                    // CcId stays from form binding
                }
                catch
                {
                    return Json(-500);
                }

                // ── 3. Session validation ──────────────────────────────────────────
                var Logins = GetSessionLogin();
                if (Logins == null || Logins.unitid == null || Logins.unitid <= 0)
                    return Json(-401);

                // ── 4. Validate decrypted PSM id ───────────────────────────────────
                if (string.IsNullOrWhiteSpace(currentpsmid)
                    || !int.TryParse(currentpsmid, out int currentPsmId)
                    || currentPsmId <= 0)
                {
                    return Json(-999);
                }

                // ── 5. Validate mandatory business fields ──────────────────────────
                if (psmove.ProjId <= 0) return Json(-999);
                if (psmove.StatusActionsMappingId <= 0) return Json(-999);
                if (psmove.ToUnitId == 0) return Json(-7);

                // ── 6. File validation ─────────────────────────────────────────────
                const long MAX_FILE_SIZE_BYTES = 10 * 1024 * 1024;
                var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".pdf" };

                if (psmove.Attachments != null && psmove.Attachments.Count > 0)
                {
                    foreach (var attachment in psmove.Attachments)
                    {
                        if (attachment == null) return Json(-11);
                        if (attachment.File == null || attachment.File.Length == 0) continue;
                        if (attachment.File.Length > MAX_FILE_SIZE_BYTES) return Json(-10);

                        var fileExt = Path.GetExtension(attachment.File.FileName)?.ToLowerInvariant();
                        if (string.IsNullOrEmpty(fileExt) || !allowedExtensions.Contains(fileExt))
                            return Json(-11);

                        if (!await HasValidFileSignatureAsync(attachment.File, fileExt))
                            return Json(-12);
                    }
                }

                // ── 7. Whitelist update logic ──────────────────────────────────────
                if (psmove.StatusActionsMappingId == 88)
                {
                    var projname = _dbContext.Projects.Find(psmove.ProjId);
                    if (projname == null) return Json(-999);

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
                    if (projname == null) return Json(-999);

                    var Whitelist = _dbContext.trnWhiteListed
                        .FirstOrDefault(x => x.ProjName == projname.ProjName);

                    if (Whitelist != null)
                    {
                        Whitelist.Clearence = DateTime.Now;
                        _dbContext.trnWhiteListed.UpdateRange(Whitelist);
                        _dbContext.SaveChanges();
                    }
                }

                // ── 8. Check ToUnit != CC Unit ─────────────────────────────────────
                bool toUnitAlsoInCc = psmove.CcId != null && psmove.CcId.Contains(psmove.ToUnitId);

                int psmid = currentPsmId;

                // ── 9. Get current movement record ─────────────────────────────────
                var currentMove = _dbContext.ProjStakeHolderMov
                    .FirstOrDefault(x => x.PsmId == psmid);

                if (currentMove == null) return Json(-4);

                var getprojidbypsmid = currentMove.ProjId;

                // ── 10. Authorise current user on this movement ────────────────────
                var latst = _dbContext.ProjStakeHolderMov
                    .Where(r => r.PsmId == psmid
                             && r.ToUnitId == Logins.unitid
                             && r.IsComplete == false
                             && r.IsComment == false)
                    .FirstOrDefault();

                if (latst == null) return Json(-4);

                // ── 11. Forward if ToUnit != CC ────────────────────────────────────
                if (!toUnitAlsoInCc)
                {
                    var legacy_approval = _dbContext.LegacyHistory
                        .Where(x => x.ProjectId == getprojidbypsmid)
                        .OrderByDescending(x => x.HistoryId)
                        .FirstOrDefault();

                    psmove.ProjId = getprojidbypsmid;
                    psmove.FromUnitId = Logins.unitid ?? 0;

                    // Mark old movement complete
                    var updateiscomplete = await _projectsRepository.GettXNByPsmIdAsync(currentPsmId);
                    if (updateiscomplete == null) return Json(-4);

                    updateiscomplete.IsComplete = true;
                    await _projectsRepository.UpdateTxnAsync(updateiscomplete);

                    psmove.UserDetails = Helper.LoginDetails(Logins);
                    psmove.UpdatedByUserId = Logins.UserIntId;

                    if (legacy_approval?.ActionType == ActionTypeEnum.Approved)
                    {
                        psmove.DateTimeOfUpdate = psmove.TimeStamp;
                        psmove.EditDeleteDate = DateTime.Now;
                        // TimeStamp preserved from decrypted model
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
                        psmove.IsRead = true;

                    if (psmove.CcId != null && psmove.CcId.Length > 0)
                        psmove.IsCc = true;

                    _dbContext.SaveChanges();

                    // Mark reminders as read
                    var remainders = await _dbContext.TrnRemainders
                        .Where(r => r.Projid == getprojidbypsmid
                                 && r.ReadDate == null
                                 && r.ToUserDetails == null
                                 && r.Tounitid == Logins.unitid)
                        .ToListAsync();

                    if (remainders.Count > 0)
                        await _Remainder.UpdateReaminderRead(getprojidbypsmid, 0);

                    // Reset read status of project comments
                    var projectMovements = await _dbContext.ProjStakeHolderMov
                        .Where(x => x.ProjId == getprojidbypsmid
                                 && x.IsRead == true
                                 && x.IsComment == true)
                        .ToListAsync();

                    foreach (var item in projectMovements)
                        item.IsRead = false;

                    _dbContext.ProjStakeHolderMov.UpdateRange(projectMovements);
                    await _dbContext.SaveChangesAsync();

                    // Add new project movement
                    var Ret = await _psmRepository.AddWithReturn(psmove);
                    if (Ret == null) return Json(nmum.NotSave);

                    var latestpsmid = _projStakeHolderMovRepository.GetLastRecProjectMov(getprojidbypsmid);

                    // Save attachments
                    var errors = new List<int>();

                    if (psmove.Attachments != null && psmove.Attachments.Count > 0)
                    {
                        foreach (var attachment in psmove.Attachments)
                        {
                            if (attachment?.File == null || attachment.File.Length == 0)
                                continue;

                            var saveResult = await SaveAttachmentAsync(
                                attachment.File, attachment.Remarks, latestpsmid, Logins, psmove.TimeStamp);

                            if (saveResult is JsonResult jsonResult)
                            {
                                var resultValue = jsonResult.Value as int?;
                                if (resultValue != 1)
                                    errors.Add(resultValue.GetValueOrDefault());
                            }
                        }
                    }

                    if (errors.Any()) return Json(errors);

                    // Save CC movement records
                    if (psmove.CcId != null && psmove.CcId.Length > 0)
                    {
                        foreach (int ccId in psmove.CcId)
                        {
                            await _projStakeHolderCcMovRepository.AddWithReturn(new tbl_ProjStakeHolderCcMov
                            {
                                PsmId = Ret.PsmId,
                                ProjId = getprojidbypsmid,
                                ToCcUnitId = ccId,
                                IsActive = true,
                                IsDeleted = false,
                                IsRead = false,
                                UserDetails = "",
                                ReadDate = DateTime.Now
                            });
                        }
                    }

                    return Json(Ret);
                }
                else
                {
                    return Json(nmum.TounitEqualsCCUnitID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in FwdToProject.");
                return Json(new { type = 500, message = "Something went wrong." });
            }
        }

        // ── File signature validation ─────────────────────────────────────────────

        private async Task<bool> HasValidFileSignatureAsync(IFormFile file, string extension)
        {
            if (file == null || file.Length == 0) return false;

            var signatures = new Dictionary<string, byte[]>
            {
                { ".pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 } }   // %PDF
            };

            if (!signatures.TryGetValue(extension, out byte[]? expected) || expected == null)
                return true; // unknown type → allow

            using var stream = file.OpenReadStream();
            var header = new byte[expected.Length];
            int bytesRead = await stream.ReadAsync(header, 0, expected.Length);

            return bytesRead == expected.Length && header.SequenceEqual(expected);
        }

        // ── Save attachment (reusable) ────────────────────────────────────────────

        public async Task<IActionResult> SaveAttachmentAsync(
            IFormFile attdata,
            string remarks,
            int psmid,
            Login Logins,
            DateTime? TimeStamp)
        {
            const long MaxFileSizeBytes = 10 * 1024 * 1024;

            if (attdata == null || attdata.Length == 0) return Json(-10);
            if (attdata.Length > MaxFileSizeBytes) return Json(-3);

            var ext = Path.GetExtension(attdata.FileName?.Trim() ?? "").ToLowerInvariant();
            if (ext != ".pdf") return Json(-2);
            if (psmid <= 0) return Json(-1);

            var uploadsDir = Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/");
            Directory.CreateDirectory(uploadsDir);

            var uniqueFileName = $"Swas_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                await attdata.CopyToAsync(stream);

            var atthis = new tbl_AttHistory
            {
                ActionId = 0,
                AttPath = uniqueFileName,
                Reamarks = remarks ?? string.Empty,
                PsmId = psmid,
                UpdatedByUserId = Logins?.unitid,
                IsDeleted = false,
                IsActive = true,
                EditDeleteBy = Logins?.unitid,
                EditDeleteDate = DateTime.Now,
                TimeStamp = TimeStamp,
                ActFileName = attdata.FileName?.Trim() ?? ""
            };

            await _attHistoryRepository.AddAttHistoryAsync(atthis);
            _dbContext.SaveChanges();
            return Json(1);
        }

        // ── Project movement history ──────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectMovHistory(string ProjectId)
        {
            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                _logger.LogWarning("ProjectMovHistory called with empty ProjectId");
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired." });

            var cryptoKey = Logins.CryptoKey;
            if (string.IsNullOrWhiteSpace(cryptoKey))
            {
                _logger.LogError("Crypto key is missing from configuration");
                return StatusCode(500, new { success = false, message = "Server configuration error." });
            }

            int projectId;

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(ProjectId, cryptoKey);
                if (string.IsNullOrWhiteSpace(decrypted))
                {
                    _logger.LogWarning("Decryption returned empty result for ProjectId");
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                decrypted = decrypted.Trim().Trim('"');

                if (!int.TryParse(decrypted, out projectId) || projectId <= 0)
                {
                    _logger.LogWarning("Invalid decrypted ProjectId: {Value}", decrypted);
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "Cryptographic error while decrypting ProjectId");
                return BadRequest(new { success = false, message = "Invalid encrypted data." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProjectMovHistory");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }

            try
            {
                var result = await _psmRepository.ProjectMovHistory(projectId);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Database error while fetching ProjectMovHistory for ProjectId: {ProjectId}", projectId);
                return StatusCode(500, new { success = false, message = "Failed to fetch data." });
            }
        }

        // ── Undo project ──────────────────────────────────────────────────────────

        public async Task<IActionResult> UndoProject(int ProjectId, int PsmId, string Remarks, int StageId)
        {
            try
            {
                if (StageId == 1)
                    return Json(nmum.NotSave);

                var movent = await _psmRepository.GetByByte(PsmId);
                movent.IsRead = false;
                movent.UndoRemarks = Remarks;
                movent.IsComplete = true;
                movent.IsPullBack = true;
                await _psmRepository.UpdateWithReturn(movent);

                var Logins = GetSessionLogin();

                var psmidold = await _psmRepository.GetLastRecProjectMovForUnod(ProjectId, Logins?.unitid);
                var movent1 = await _psmRepository.GetByByte(psmidold);
                movent1.Remarks = "";
                movent1.IsComplete = false;
                movent1.IsRead = false;
                await _psmRepository.UpdateWithReturn(movent1);

                return Json(nmum.Update);
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "UndoProject");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while UndoProject in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(nmum.Exception);
            }
        }

        #endregion

        // ═════════════════════════════════════════════════════════════════════════
        // PULLBACK
        // ═════════════════════════════════════════════════════════════════════════

        #region PullBack

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PullBAckProject(string encrypted_data)
        {
            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired." });

            var cryptoKey = Logins.CryptoKey;

            var result = DecryptionHelper.DecryptRequest(encrypted_data, cryptoKey, _logger);
            if (!result.Success)
                return BadRequest(new { success = false, message = result.ErrorMessage });

            var data = result.Data as DecryptedRequest;
            if (data == null)
                return BadRequest(new { success = false, message = "Invalid decrypted data." });

            int projid = data.ProjectId;
            int psmId = data.PsmId;
            int stageId = data.StageId;
            string remarks = data.Remarks ?? "";

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Session == null)
                    return Unauthorized(new { success = false, message = "Session expired." });

                var movent = new tbl_ProjStakeHolderMov();

                var remainders = await _dbContext.TrnRemainders
                    .Where(r => r.Projid == projid && r.ReadDate == null && r.ToUserDetails == null)
                    .ToListAsync();

                if (remainders.Count > 0)
                    await _Remainder.UpdateReaminderRead(projid, 1);

                await _dbContext.SaveChangesAsync();

                int psmData = _psmRepository.GetLastRecProjectMov(projid);
                if (psmData == 0) return Json(nmum.NotSave);

                movent = await _psmRepository.GetByByte(psmData);
                movent.IsRead = true;
                movent.UndoRemarks = remarks;
                movent.IsComplete = true;
                movent.DateTimeOfUpdate = DateTime.Now;
                await _psmRepository.UpdateWithReturn(movent);

                UnitDtl? unitDetail = null;

                if (psmData != psmId)
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
                    var userdet = await _projectsRepository.GetUserByUnitId(unitDetail.unitid);
                    if (userdet != null)
                    {
                        var rankName = _dbContext.mRank.FirstOrDefault(x => x.Id == userdet.Rank);
                        movent.UserDetails = $"{rankName?.RankName ?? ""} {userdet.Offr_Name?.Trim() ?? ""} / {userdet.UserName?.Trim() ?? ""}";
                    }
                    else
                    {
                        movent.UserDetails = "";
                    }
                }

                if (psmData != psmId)
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
                movent.Remarks = Helper.LoginDetails(Logins) + "(" + (Logins.Unit ?? "") + ") 𝐑𝐞𝐦𝐚𝐫𝐤𝐬: " + remarks;
                movent.UpdatedByUserId = Logins.UserIntId;
                movent.DateTimeOfUpdate = DateTime.Now;
                movent.EditDeleteDate = DateTime.Now;
                movent.EditDeleteBy = Logins.UserIntId;
                movent.TimeStamp = DateTime.Now;
                movent.IsComment = false;
                movent.IsCc = false;

                await _psmRepository.AddWithReturn(movent);
                return Json(nmum.Update);
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "PullBAckProject");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while Pull Back Project in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(nmum.Exception);
            }
        }

        #endregion

        // ═════════════════════════════════════════════════════════════════════════
        // PROJECT COMMENTS
        // ═════════════════════════════════════════════════════════════════════════

        #region ProjComments
        [AuthorizePermission("StakeholderComments")]
        public async Task<IActionResult> ProjComments() => View();

        public async Task<IActionResult> GetProjCommentsByUnitId(int StatusId)
        {
            try
            {
                var Logins = GetSessionLogin();
                return Json(await _projComments.GetAllStkForComment(Convert.ToInt32(Logins?.unitid), StatusId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProjCommentsByUnitId");
                return Json(nmum.Exception);
            }
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 26_214_400)]
        public async Task<IActionResult> SendCommentonProject(
            IFormFile uploadfile,
            string Comments,
            string StkStatusId,
            string ProjectId,
            string psmid,
            string CommentDate)
        {
            var Logins = GetSessionLogin();
            if (Logins == null)
            {
                _logger.LogWarning("Session expired in SendCommentonProject");
                return Json(-401);
            }

            var cryptoKey = Logins.CryptoKey;

            try
            {
                Comments = string.IsNullOrEmpty(Comments) ? "" : CryptoHelper.SafeDecrypt(Comments, cryptoKey);
                StkStatusId = string.IsNullOrEmpty(StkStatusId) ? "0" : CryptoHelper.SafeDecrypt(StkStatusId, cryptoKey);
                ProjectId = string.IsNullOrEmpty(ProjectId) ? "0" : CryptoHelper.SafeDecrypt(ProjectId, cryptoKey);
                psmid = string.IsNullOrEmpty(psmid) ? "0" : CryptoHelper.SafeDecrypt(psmid, cryptoKey);
                CommentDate = string.IsNullOrEmpty(CommentDate) ? DateTime.Now.ToString() : CryptoHelper.SafeDecrypt(CommentDate, cryptoKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed in SendCommentonProject");
                return Json(-500);
            }


            if (!int.TryParse(StkStatusId.Trim('"'), out int stkStatusId) ||
                !int.TryParse(ProjectId.Trim('"'), out int projectId) ||
                !int.TryParse(psmid.Trim('"'), out int psmIdInt) ||
                !DateTime.TryParse(CommentDate.Trim('"'), out DateTime commentDateTime))
            {
                _logger.LogWarning("Invalid decrypted input values in SendCommentonProject");
                return Json(-400);
            }

            try
            {
                StkComment cmmets = new StkComment();
                string uniqueFileName = "";

                var proj = _dbContext.ProjStakeHolderMov
                    .Where(x => x.ProjId == projectId && x.IsComment == true)
                    .OrderByDescending(x => x.TimeStamp)
                    .FirstOrDefault();

                var initateddate = _dbContext.Projects.Find(projectId)?.InitiatedDate;

                if (proj == null)
                    return Json(new { success = false, message = "Project comment record not found." });

                if (commentDateTime < proj.TimeStamp || commentDateTime < initateddate)
                    return Json(new { success = false, message = "Comment date/time cannot be Less than project ProcessDate." });

                int psmove = await _stkCommentRepository.GetCommentStatusByPsmiId(psmIdInt);
                int allowForInfo = _stkCommentRepository.IsAllowForCommentByStkStatusId(stkStatusId);

                if (psmove != 1 || allowForInfo == 1)
                {
                    if (uploadfile != null)
                    {
                        if (uploadfile.Length <= 10_485_760)
                        {
                            uniqueFileName = $"Swas_{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
                            string filePath = Path.Combine(
                                _environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                                await uploadfile.CopyToAsync(stream);

                            cmmets.ActFileName = uploadfile.FileName;
                        }
                        else
                        {
                            return Json(nmum.PdfSizeEx);
                        }
                    }

                    var approval_legacy = _dbContext.LegacyHistory
                        .Where(x => x.ProjectId == projectId)
                        .OrderByDescending(x => x.HistoryId)
                        .FirstOrDefault();

                    cmmets.Attpath = uniqueFileName;
                    cmmets.Comments = Comments.Trim('"');
                    cmmets.PsmId = psmIdInt;
                    cmmets.ProjId = projectId;
                    cmmets.UpdatedByUserId = Logins.UserIntId;
                    cmmets.DateTimeOfUpdate = (approval_legacy?.ActionType == ActionTypeEnum.Approved)
                                             ? commentDateTime
                                             : DateTime.Now;
                    cmmets.EditDeleteDate = DateTime.Now;
                    cmmets.IsDeleted = false;
                    cmmets.IsActive = true;
                    cmmets.EditDeleteBy = Logins.unitid;
                    cmmets.StkStatusId = stkStatusId;
                    cmmets.UserDetails = Helper.LoginDetails(Logins);
                    cmmets.StakeHolderId = Logins.unitid;

                    var projectStkHolderMovementData =
                        await _projectsRepository.GetProjStkHolderMovmentByPsmiId(cmmets.PsmId);

                    if (projectStkHolderMovementData != null)
                    {
                        var projectMovements = await _dbContext.ProjStakeHolderMov
                            .Where(x => x.ProjId == projectStkHolderMovementData.ProjId
                                     && x.PsmId != psmIdInt
                                     && x.IsComment == true)
                            .ToListAsync();

                        foreach (var item in projectMovements)
                            item.IsRead = false;

                        var latestPsmId = await _dbContext.ProjStakeHolderMov
                            .Where(x => x.ProjId == projectStkHolderMovementData.ProjId
                                     && x.IsComplete == false)
                            .OrderByDescending(x => x.PsmId)
                            .Select(x => x.PsmId)
                            .FirstOrDefaultAsync();

                        if (latestPsmId != 0)
                        {
                            var latestMovement = await _dbContext.ProjStakeHolderMov
                                .FirstOrDefaultAsync(x => x.PsmId == latestPsmId);

                            if (latestMovement != null)
                                latestMovement.IsRead = false;
                        }

                        _dbContext.ProjStakeHolderMov.UpdateRange(projectMovements);
                        await _dbContext.SaveChangesAsync();

                        projectStkHolderMovementData.DateTimeOfUpdate = commentDateTime;
                        var rets = await _projectsRepository.UpdateProjectStkMovementAsync(projectStkHolderMovementData);

                        if (rets != null)
                        {
                            var ret = await _stkCommentRepository.AddWithReturn(cmmets);
                            return ret != null ? Json(nmum.Save) : Json(0);
                        }

                        return Json(0);
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
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "SendCommentonProject");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while Send Comment on Project in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(nmum.Exception);
            }
        }

        public async Task<IActionResult> GetAllCommentBypsmId_UnitId(string encrypted_ids)
        {
            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired." });

            var cryptoKey = Logins.CryptoKey;
            var result = DecryptionHelper.DecryptRequest(encrypted_ids, cryptoKey, _logger);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.ErrorMessage });

            if (result.Data == null)
                return BadRequest(new { success = false, message = "Invalid decrypted data." });

            int projid = result.Data.ProjId;
            int psmId = result.Data.PsmId;

            try
            {
                var stkComment = new StkComment { ProjId = projid, PsmId = psmId };
                var ret = await _stkCommentRepository.GetAllCommentBypsmId_UnitId(stkComment);
                return Json(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCommentBypsmId_UnitId");
                return StatusCode(500, new { success = false, message = "Something went wrong." });
            }
        }

        public async Task<IActionResult> GetCommentStatus(int UnitId)
        {
            var ret = await _projComments.GetCommentStatus(UnitId);
            return Json(ret);
        }

        #endregion

        // ═════════════════════════════════════════════════════════════════════════
        // PROJECT HISTORY
        // ═════════════════════════════════════════════════════════════════════════

        #region Project History

           [HttpGet]

        public async Task<IActionResult> ProjHistory(string userid, int? dataProjId, int? dtaProjID, string? AttPath, int? psmid, string? Projpin, string? EncyID, EncryModel? encryModel, string Type)
        {
            
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

                if (encryModel?.EncryItem != null)
                {
                    var UnprotectedValue = _dataProtector.Unprotect(encryModel.EncryItem.ToString() ?? "");
                    var originalData = JsonConvert.DeserializeObject<MyRequestModel>(UnprotectedValue);
                    dtaProjID = originalData?.DtaProjID;
                    if (dtaProjID == 0)
                    {
                        dtaProjID = null;
                    }

                    AttPath = originalData?.AttPath;
                    Projpin = originalData?.Projpin;
                    psmid = originalData?.PsmId;
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

        // ═════════════════════════════════════════════════════════════════════════
        // ATTACHMENT DOCUMENT PARTIALS
        // ═════════════════════════════════════════════════════════════════════════

        #region Attachment Document

        public async Task<IActionResult> AttDetails(int Id)
        {
            var Logins = GetSessionLogin();
            if (Logins == null) return Redirect("/Identity/Account/login");

            var atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(Id);
            return PartialView("_attachmetsview", atthis);
        }

        public async Task<IActionResult> AttDetailsRead(int Id)
        {
            var Logins = GetSessionLogin();
            if (Logins == null) return Redirect("/Identity/Account/login");

            var atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(Id);
            return PartialView("_attachmetsread", atthis);
        }

        #endregion

        // ═════════════════════════════════════════════════════════════════════════
        // WATERMARKED PDF DOWNLOAD
        // ═════════════════════════════════════════════════════════════════════════

        #region Watermark PDF for attachment

        // NOTE: filepathpdf is only used by OnTimer. Kept as instance field per original design.
        private string filepathpdf = "";

        public IActionResult WatermarkWithPdf(string id)
        {
            var Logins = GetSessionLogin();
            if (Logins == null) return Redirect("/Identity/Account/login");

            try
            {
                var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                var filePath = Path.Combine(_environment.WebRootPath, "Uploads", id);

                if (!System.IO.File.Exists(filePath))
                    return Content("PDF IS NOT IN FOLDER");

                var pdfBytes = generate2(filePath, ip);
                if (pdfBytes == null)
                    return Content("Error generating watermarked PDF.");

                var randomName = $"{new Random().Next(1, 1000)}.pdf";
                Response.Headers["Content-Disposition"] = $"inline; filename={randomName}";
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "WatermarkWithPdf");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while Watermark With Pdf in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(0);
            }
        }

        public void OnTimer(Object source, ElapsedEventArgs e)
        {
            try
            {
                var filePath1 = Path.Combine(
                    _environment.ContentRootPath, "wwwroot", "DownloadFile", filepathpdf + ".pdf");

                if (System.IO.File.Exists(filePath1))
                    System.IO.File.Delete(filePath1);
            }
            catch
            {
                // Timer cleanup — swallow to avoid crashing the timer thread
            }
        }

        #endregion

        // ═════════════════════════════════════════════════════════════════════════
        // IS-UN-READ INBOX / COMMENT
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsUnReadInbox(int PsmId)
        {
            try
            {
                if (PsmId <= 0)
                    ModelState.AddModelError(nameof(PsmId), "Invalid PsmId.");

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        type = 400,
                        message = "Invalid request data.",
                        errors = ModelState
                            .Where(x => x.Value.Errors.Count > 0)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value.Errors.Select(e => e.ErrorMessage).ToList())
                    });
                }

                var Logins = GetSessionLogin();
                if (Logins == null)
                    return Json(new { type = 401, message = "Session expired. Please login again." });

                try
                {
                    var psmove = await _projectsRepository.GettXNByPsmIdAsync(PsmId);
                    if (psmove == null)
                        return Json(new { type = 404, message = "Inbox record not found." });

                    psmove.IsRead = false;
                    await _projectsRepository.UpdateTxnAsync(psmove);
                    return Json(PsmId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in ProjectsController IsUnReadInbox.");
                    return StatusCode(500, new { success = false, message = "An unexpected error occurred. Please try again later." });
                }
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "IsUnReadInbox");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while on IsUnReadInbox in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        [HttpPost]
        public async Task<IActionResult> IsUnReadComment(int Projid, int PsmId)
        {
            var Logins = GetSessionLogin();
            if (Logins == null) return Redirect("/Identity/Account/login");

            try
            {
                var inboxComments = await _projectsRepository.GetCommentByExcludingPsmId(Projid, Logins.unitid);
                foreach (var comment in inboxComments)
                {
                    comment.IsRead = false;
                    await _projectsRepository.UpdateTxnAsync(comment);
                }
                return Json(Projid);
            }
            catch (Exception ex)
            {
                Error.ExceptionHandle(ex.Message);
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "IsUnReadComment");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while on IsUnReadComment in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(0);
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // NOTIFICATION COUNTS
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<JsonResult> GetProjectCommentCount()
        {
            try
            {
                int count = await _projectsRepository.GetNotificationCommentCount();
                return new JsonResult(count);
            }
            catch (Exception ex)
            {
                var errorId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
                _logger.LogError(ex, "Unhandled exception. ErrorId={ErrorId}", errorId);
                return new JsonResult(new { message = "Something went wrong. Please contact the administrator.", errorId })
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
                return new JsonResult(count);
            }
            catch (Exception ex)
            {
                var errorId = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
                _logger.LogError(ex, "Unhandled exception. ErrorId={ErrorId}", errorId);
                return new JsonResult(new { message = "Something went wrong. Please contact the administrator.", errorId })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // PROJECT MOVEMENT (ADMIN VIEW)
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        [AuthorizePermission("EditProjectMovement")]
        public async Task<IActionResult> ProjectMovement(string? ProjName) => View();

        public async Task<IActionResult> GetProjectMov(int Id)
        {
            try
            {
                var ret = await _projStakeHolderMovRepository.ProjectMovement(Id);
                return Json(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProjectMov for Id {Id}", Id);
                return Json(-1);
            }
        }

        public async Task<IActionResult> ProjectMovementUpdate(tbl_ProjStakeHolderMov psmove)
        {
            var Logins = GetSessionLogin();

            var data = await _projectsRepository.GettXNByPsmIdAsync(psmove.PsmId);
            data.ProjId = psmove.ProjId;
            data.StatusActionsMappingId = psmove.StatusActionsMappingId;
            data.Remarks = psmove.Remarks;
            data.ToUnitId = psmove.ToUnitId;
            data.UpdatedByUserId = Logins?.UserIntId;
            data.DateTimeOfUpdate = psmove.TimeStamp;
            data.IsActive = true;
            data.EditDeleteDate = psmove.TimeStamp;
            data.EditDeleteBy = Logins?.UserIntId;
            data.TimeStamp = psmove.TimeStamp;

            var Ret = await _psmRepository.UpdateWithReturn(data);
            if (Ret == null) return Json(nmum.NotSave);

            var nextPsmMove = await _projectsRepository.GetNextPsmMoveAsync(psmove.ProjId, psmove.PsmId);

            if (nextPsmMove != null)
            {
                var unitDetail = await _unitRepository.GetUnitDtl(psmove.ToUnitId);
                if (unitDetail != null)
                {
                    var userdet = await _userManager.FindByNameAsync(unitDetail.UnitName);
                    if (userdet != null)
                    {
                        var rankName = _dbContext.mRank.FirstOrDefault(x => x.Id == userdet.Rank);
                        if (rankName != null)
                            nextPsmMove.UserDetails = $"{rankName.RankName} {userdet.Offr_Name?.Trim()} / {userdet.UserName?.Trim()}";
                    }
                }

                nextPsmMove.FromUnitId = psmove.ToUnitId;
                await _psmRepository.UpdateWithReturn(nextPsmMove);
            }

            return Json(Ret);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // SEARCH
        // ═════════════════════════════════════════════════════════════════════════

        public async Task<IActionResult> GetALLByProjectName(string? ProjName)
        {
            var ProjectName = await _projectsRepository.GetALLByProjectName(ProjName);
            return Json(ProjectName);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // PROCESS NOTIFICATION
        // ═════════════════════════════════════════════════════════════════════════

        public async Task<IActionResult> ProcessNotification(int ProjId, int unitid, DateTime FwdDateForComment)
        {
            try
            {
                var Logins = GetSessionLogin();
                if (Logins == null) return Redirect("/Identity/Account/login");

                if (ProjId <= 0) return Json(0);

                var project = await _projectsRepository.GetProjectByIdAsync(ProjId);
                unitid = project.StakeHolderId;

                int[] unitids = unitid == 1
                    ? new[] { 4, 3, 5, 1 }
                    : new[] { 4, 3, 5, 1, unitid };

                foreach (var toUnit in unitids)
                {
                    await _psmRepository.AddNotificationCommentAsync(new Notification
                    {
                        ProjId = ProjId,
                        NotificationFrom = Logins.unitid ?? 0,
                        NotificationTo = toUnit,
                        IsRead = false,
                        ReadDateTime = FwdDateForComment,
                        NotificationType = 1
                    });
                }

                return Json(1);
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "ProcessNotification");
                _logger.Log(LogLevel.Error, eventId,
                    "An error occurred while on Process Notification in ProjectsController.",
                    ex, (s, e) => $"{s} - {e?.Message}");
                return Json(-1);
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // IS-READ COMMENT
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost]
        public async Task<IActionResult> IsReadComment(string encrypted_payload)
        {
            if (string.IsNullOrWhiteSpace(encrypted_payload))
                return BadRequest(new { success = false, message = "Invalid request." });

            int PsmId = 0;

            var Logins = GetSessionLogin();
            if (Logins == null) return Redirect("/Identity/Account/login");

            var cryptoKey = Logins.CryptoKey;
            if (string.IsNullOrWhiteSpace(cryptoKey))
                return StatusCode(500, new { success = false, message = "Server configuration error." });

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(encrypted_payload, cryptoKey);
                if (string.IsNullOrWhiteSpace(decrypted))
                    return BadRequest(new { success = false, message = "Invalid request data." });

                var obj = JsonConvert.DeserializeObject<dynamic>(decrypted.Trim('"'));
                if (obj == null || !int.TryParse((string?)obj.PsmId, out PsmId))
                    return BadRequest(new { success = false, message = "Invalid identifier." });
            }
            catch (CryptographicException)
            {
                return BadRequest(new { success = false, message = "Invalid encrypted data." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IsReadComment decryption");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }

            try
            {
                var inboxComments = await _projectsRepository.GettXNByPsmIdAsync(PsmId);
                if (inboxComments != null && !inboxComments.IsRead)
                {
                    inboxComments.IsRead = true;
                    await _projectsRepository.UpdateTxnAsync(inboxComments);
                }

                int getunreadComments = _dbContext.ProjStakeHolderMov
                    .Count(x => x.ToUnitId == Logins.unitid
                             && x.IsRead == false
                             && x.IsComment == true);

                return Json(getunreadComments);
            }
            catch (Exception ex)
            {
                var eventId = new EventId(DateTime.UtcNow.Ticks.GetHashCode(), "IsReadComment");
                _logger.LogError(eventId, ex, "Unhandled error in ProjectsController.IsReadComment.");
                Error.ExceptionHandle("Unhandled error in IsReadComment.");
                return Json(new { success = false, message = "Something went wrong." });
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // SESSION / CALENDAR
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost]
        public IActionResult SetCalendarModeInSession(int mode)
        {
            HttpContext.Session.SetInt32("CalendarMode", mode);
            return Json(new { success = true, message = "Calendar mode saved in session." });
        }

        // ═════════════════════════════════════════════════════════════════════════
        // DATE APPROVAL / LEGACY HISTORY
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost("Projects/LogDateApprovalWithRemarks")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogDateApproval(
            int ProjId,
            bool UserReq,
            int actiontype,
            string remarks)
        {
            var user = GetSessionLogin();

            if (ProjId <= 0) ModelState.AddModelError(nameof(ProjId), "Invalid Project Id.");
            if (actiontype <= 0) ModelState.AddModelError(nameof(actiontype), "Invalid action type.");
            if (string.IsNullOrWhiteSpace(remarks)) ModelState.AddModelError(nameof(remarks), "Remarks is required.");
            if (user == null) ModelState.AddModelError("User", "Session expired. Please login again.");

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input.",
                    errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(e => e.ErrorMessage).ToList())
                });
            }

            try
            {
                _dbContext.DateApproval.Add(new DateApproval
                {
                    ProjId = ProjId,
                    UnitId = user!.unitid,
                    Request_Date = DateTime.Now,
                    UserRequest = UserReq,
                    DDGIT_approval = false,
                    DDGIT_Approval_dat = null,
                    User = Helper1.LoginDetails(user),
                    IsRead = false,
                    RequestType = 1
                });
                await _dbContext.SaveChangesAsync();

                await _legacyHistoryRepository.AddHistoryAsync(new LegacyHistory
                {
                    ProjectId = ProjId,
                    UnitId = user.unitid,
                    FromUnit = user.unitid,
                    ActionBy = $"{user.Rank} {user.Offr_Name}",
                    ActionType = (ActionTypeEnum)actiontype,
                    Remarks = remarks,
                    ActionDate = DateTime.Now,
                    Userdetails = Helper1.LoginDetails(user)
                });

                return Json(new { success = true, message = "Request has been forward to admin for legacy project ingestion." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging date approval.");
                return StatusCode(500, new { success = false, message = "Error logging date approval." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveDateRequest(int id, int actiontype, string remarks)
        {
            var user = GetSessionLogin();

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

                await _legacyHistoryRepository.AddHistoryAsync(new LegacyHistory
                {
                    ProjectId = entry.ProjId ?? 0,
                    UnitId = user?.unitid,
                    FromUnit = user?.unitid,
                    ActionBy = $"{user?.Rank ?? ""} {user?.Offr_Name ?? ""}",
                    ActionType = (ActionTypeEnum)actiontype,
                    Remarks = remarks,
                    ActionDate = DateTime.Now,
                    Userdetails = Helper1.LoginDetails(user)
                });

                string message = entry.DDGIT_approval == true
                    ? "Request approved successfully."
                    : "Request Rejected.";

                return Json(new { success = true, message, currentStatus = entry.DDGIT_approval });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ApproveDateRequest for id {Id}", id);
                return Json(new { success = false, message = "An error occurred while updating." });
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // RE-VETTED PROJECT QUERIES
        // ═════════════════════════════════════════════════════════════════════════

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
                query = query.Where(x => x.ProjName.Contains(searchQuery));

            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectDetails([FromQuery] int projId)
        {
            var project = await _dbContext.Projects
                .Where(p => p.ProjId == projId)
                .FirstOrDefaultAsync();

            if (project == null) return NotFound();
            return Ok(project);
        }

        public async Task<string> GetReVettedProjectName(tbl_Projects project)
        {
            string originalName = project.ProjName.Trim();
            string baseName = originalName;
            int currentCount = 0;

            var reVettedPattern = new System.Text.RegularExpressions.Regex(
                @"(.*)\sRe-Vetted\s(\d+)$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var match = reVettedPattern.Match(originalName);
            if (match.Success)
            {
                baseName = match.Groups[1].Value.Trim();
                currentCount = int.Parse(match.Groups[2].Value);
            }

            var count = await _dbContext.Projects
                .Where(i => i.ProjName.Trim().ToUpper().StartsWith(baseName.ToUpper())
                         && i.ProjName.Contains("Re-Vetted"))
                .CountAsync();

            int newCount = Math.Max(count, currentCount) + 1;
            return $"{baseName} Re-Vetted {newCount}";
        }

        // ═════════════════════════════════════════════════════════════════════════
        // LEGACY HISTORY
        // ═════════════════════════════════════════════════════════════════════════

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

        // ═════════════════════════════════════════════════════════════════════════
        // TYPEAHEAD SEARCH
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> GetProjectByKeyup(string searchQuery)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                    return Ok(new List<object>());

                searchQuery = searchQuery.Trim();

                if (searchQuery.Length < 2)
                    return Ok(new List<object>());

                if (searchQuery.Length > 100)
                    return BadRequest(new { success = false, message = "Search text is too long." });

                var result = await _dbContext.Projects
                    .AsNoTracking()
                    .Where(x => x.ProjName != null && x.ProjName.Contains(searchQuery))
                    .Select(x => new { x.ProjId, x.ProjName })
                    .Take(20)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetProjectByKeyup.");
                return StatusCode(500, new { success = false, message = "Something went wrong." });
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // REMAINDER
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost]
        public async Task<IActionResult> SendRemainder(string ProjId, string Remarks)
        {
            var Logins = GetSessionLogin();
            if (Logins == null)
                return Json(0);

            var cryptoKey = Logins.CryptoKey;

            try
            {
                Remarks = CryptoHelper.SafeDecrypt(Remarks, cryptoKey).Trim('"');
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting Remarks in SendRemainder");
                return StatusCode(500, new { success = false, message = "Error SendRemainder request." });
            }

            string decryptedValue = _dataProtector.Unprotect(ProjId);
            int dataProjId = int.Parse(decryptedValue);

            var latestpsmid = _projStakeHolderMovRepository.GetLastRecProjectMov(dataProjId);
            var latestpsmiddata = _dbContext.ProjStakeHolderMov.Find(latestpsmid);

            if (latestpsmiddata == null) return Json(0);

            latestpsmiddata.IsRead = false;
            _dbContext.ProjStakeHolderMov.Update(latestpsmiddata);

            int fromUnitId = Logins.unitid ?? 0;
            int toUnitId = latestpsmiddata.ToUnitId;
            string userDetails = Helper1.LoginDetails(Logins);
            int result = await _Remainder.AddRemainder(dataProjId, latestpsmid, fromUnitId, toUnitId, Remarks, userDetails);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectRemainderHistory(string ProjectId,bool Encrypted)
        {
            try
            {
                int dataProjId = 0;
                if (Encrypted == true)
                {

                dataProjId = int.Parse(_dataProtector.Unprotect(ProjectId));
                }
                else
                {
                    dataProjId = int.Parse(ProjectId);
                }

                    _logger.LogInformation("Fetching remainder history for ProjectId: {ProjectId}", dataProjId);

                if (dataProjId <= 0)
                    return BadRequest(new { success = false, message = "Invalid project ID." });

                var history = await _Remainder.ProjectRemainderMovHistory(dataProjId);

                if (history == null || !history.Any())
                    return Json(new { success = false, message = "No legacy history found." });

                return Json(new { success = true, data = history });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching project remainder history for ProjectId: {ProjectId}",
                    ProjectId);

                return RedirectToAction("Home", "Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRemaRead(int ProjectId)
        {
            try
            {
                await _Remainder.UpdateReaminderRead(ProjectId, 0);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateRemaRead for ProjectId {ProjectId}", ProjectId);
                return StatusCode(500, new { success = false, message = "An error occurred while Update project remainder history." });
            }
        }

        public async Task<IActionResult> FindProjectForComment(string searchQuery)
        {
            try
            {
                var user = GetSessionLogin();
                var projects = await _projComments.FindForComment(user?.unitid, searchQuery);
                return Json(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FindProjectForComment");
                return StatusCode(500, new { success = false, message = "An error occurred while Find Project For Comment." });
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // UTILITY ENDPOINTS
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult GetDate()
        {
            var nowIst = DateTime.Now;
            var dateYmd = nowIst.ToString("yyyy-MM-dd");
            var dateTimeLocal = nowIst.ToString("yyyy-MM-ddTHH:mm:ss");
            var analy = nowIst.TimeOfDay.ToString();
            return Json(new { dateYmd, dateTimeLocal, analy });
        }

        [HttpGet]
        public async Task<IActionResult> GetStkCommentBystkId(int PsmId)
        {
            var psmove = await _stkCommentRepository.GetCommentByPsmid(PsmId);
            return Json(psmove);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStkcomments(
            int stkcommentid,
            string comments,
            int ddlstatus,
            DateTime? CommentDateFwd)
        {
            if (stkcommentid <= 0)
                return BadRequest("Invalid Comment ID");

            var commentEntity = await _dbContext.StkComment
                .FirstOrDefaultAsync(c => c.StkCommentId == stkcommentid);

            if (commentEntity == null)
                return NotFound("Comment not found");

            commentEntity.Comments = comments;
            commentEntity.StkStatusId = ddlstatus;

            if (CommentDateFwd.HasValue)
                commentEntity.DateTimeOfUpdate = CommentDateFwd.Value;

            await _dbContext.SaveChangesAsync();
            return Json(1);
        }

        // ═════════════════════════════════════════════════════════════════════════
        // CHECK PREVIOUS APPROVALS
        // ═════════════════════════════════════════════════════════════════════════

        [HttpPost]
        public IActionResult CheckPreviousApprovals(string encryptdata)
        {
            if (string.IsNullOrWhiteSpace(encryptdata))
            {
                _logger.LogWarning("CheckPreviousApprovals called with empty encryptdata");
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            int projid = 0, statusId = 0, actionsId = 0;

            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired." });

            var cryptoKey = Logins.CryptoKey;

            try
            {
                string decryptedJson = CryptoHelper.SafeDecrypt(encryptdata, cryptoKey);
                if (string.IsNullOrWhiteSpace(decryptedJson))
                {
                    _logger.LogWarning("Decryption returned empty result in CheckPreviousApprovals");
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                var obj = JsonConvert.DeserializeObject<dynamic>(decryptedJson.Trim('"'));
                if (obj == null
                    || !int.TryParse((string?)obj.ProjId, out projid)
                    || !int.TryParse((string?)obj.StatusId, out statusId)
                    || !int.TryParse((string?)obj.Actionsid, out actionsId))
                {
                    _logger.LogWarning("Invalid decrypted values in CheckPreviousApprovals: {Value}", decryptedJson);
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting encryptdata in CheckPreviousApprovals");
                return StatusCode(500, new { success = false, message = "Error processing request." });
            }

            try
            {
                var notapproved = _projStakeHolderMovRepository
                    .CheckPreviousApprovals(statusId, projid, actionsId);
                return Json(new { message = notapproved });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckPreviousApprovals repository call");
                return StatusCode(500, new { success = false, message = "Error processing request." });
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // PARKED PROJECTS
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> GetActParkedProject()
        {
            try
            {
                var list = await _projectsRepository.GetActParkedItemsAsync();
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetActParkedProject");
                return StatusCode(500, new { success = false, message = "An error occurred." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ParkedProject(string psmid)
        {
            if (string.IsNullOrWhiteSpace(psmid))
            {
                _logger.LogWarning("ParkedProject called with empty psmid");
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            var Logins = GetSessionLogin();
            if (Logins == null)
                return Unauthorized(new { success = false, message = "Session expired." });

            var cryptoKey = Logins.CryptoKey;
            int parsedPsmId;

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(psmid, cryptoKey);
                if (string.IsNullOrWhiteSpace(decrypted))
                {
                    _logger.LogWarning("Decryption returned empty result in ParkedProject");
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                decrypted = decrypted.Trim('"');
                if (!int.TryParse(decrypted, out parsedPsmId))
                {
                    _logger.LogWarning("Invalid decrypted psmid: {Value}", decrypted);
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting psmid in ParkedProject");
                return StatusCode(500, new { success = false, message = "Error processing request." });
            }

            try
            {
                var project = await _dbContext.ProjStakeHolderMov
                    .FirstOrDefaultAsync(x => x.PsmId == parsedPsmId);

                if (project == null)
                {
                    _logger.LogInformation("Project not found for PsmId: {PsmId}", parsedPsmId);
                    return NotFound(new { success = false, message = "Project not found." });
                }

                project.IsParked = !project.IsParked;
                _dbContext.ProjStakeHolderMov.Update(project);
                await _dbContext.SaveChangesAsync();

                string message = project.IsParked
                    ? "Project successfully parked"
                    : "Project successfully unparked";

                return Ok(new { success = true, message });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while updating project {PsmId}", parsedPsmId);
                return StatusCode(500, new { success = false, message = "Database error occurred. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in ParkedProject for PsmId {PsmId}", parsedPsmId);
                return StatusCode(500, new { success = false, message = "Something went wrong. Please contact support." });
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        // DOCUMENT TYPES
        // ═════════════════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> GetDocumentTypes()
        {
            var docs = await _dbContext.DocumentTypes
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new { id = x.Id, name = x.Name, isRequired = x.IsRequired })
                .ToListAsync();

            return Json(docs);
        }

        [HttpGet]
        public async Task<IActionResult> GetUploadedDocument(int projid, int DocumentTypeId)
        {
            var currentpsmid = _dbContext.Projects.Find(projid)?.CurrentPslmId;

            var doc = await _dbContext.AttHistory
                .Where(x => x.PsmId == currentpsmid && x.DocumentTypeId == DocumentTypeId)
                .OrderByDescending(x => x.TimeStamp)
                .FirstOrDefaultAsync();

            if (doc == null)
                return Json(new { success = false });

            return Json(new { success = true, filePath = doc.AttPath });
        }

        // ═════════════════════════════════════════════════════════════════════════
        // FORECLOSE ITEMS
        // ═════════════════════════════════════════════════════════════════════════
        #region start
        [HttpGet]
        public async Task<IActionResult> GetForecloseitems(string Type)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(
                    _httpContextAccessor.HttpContext.Session, "User");

                if (Logins == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Session expired. Please login again."
                    });
                }

                var cryptoKey = Logins.CryptoKey;

                var decryptedJson = CryptoHelper.SafeDecrypt(Type, cryptoKey);

                if (string.IsNullOrWhiteSpace(decryptedJson))
                {
                    _logger.LogWarning("Decryption returned empty result");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request data."
                    });
                }

                int closetype = 0;

               
                if (!int.TryParse(decryptedJson?.ToString(), out closetype))
                {
                    _logger.LogWarning("Invalid decrypted payload: {Payload}", decryptedJson);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid identifier."
                    });
                }

                var proj = await _projectsRepository.GetForecloseitems(closetype);

                return Json(new
                {
                    success = true,
                    data = proj
                });
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "JSON parsing failed in GetForecloseitems");

                return BadRequest(new
                {
                    success = false,
                    message = "Invalid data format."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting foreclose items.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching data."
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendForecloseToInbox(int Psmid)
        {
            try
            {
                var result = await _projectsRepository.SendForecloseToInbox(Psmid);

                return Json(new
                {
                    success = result,
                    message = result
                        ? "Project sent to Inbox successfully."
                        : "Unable to send project to Inbox."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending foreclosed project to inbox.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred."
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CloseProjectAgain(int psmId)
        {
            var result = await _projectsRepository.CloseProjectAgain(psmId);

            return Json(new
            {
                success = result,
                message = result
                    ? "Project closed again successfully."
                    : "Unable to close project again."
            });
        }
        #endregion

    }
}