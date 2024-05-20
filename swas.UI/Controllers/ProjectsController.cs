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

namespace swas.UI.Controllers
{
    public class ProjectsController : Controller
    {

        private readonly IProjectsRepository _projectsRepository;

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

        private IWebHostEnvironment webHostEnvironment;
        private System.Timers.Timer aTimer;
        private readonly IStkCommentRepository _stkCommentRepository;
        private readonly IProjStakeHolderMovRepository _stkholdmove;

        public ProjectsController(IProjectsRepository projectsRepository, IDdlRepository ddlRepository,
            IProjStakeHolderMovRepository psmRepository, IHttpContextAccessor httpContextAccessor,
            IDdlRepository DDLRepository, IAttHistoryRepository attHistoryRepository,
            IWebHostEnvironment environment, IProjStakeHolderMovRepository stkholdmove,
            IDataProtectionProvider DataProtector, IWebHostEnvironment _webHostEnvironment,
            ICommentRepository commentRepository, IActionsRepository actionsRepository,
            IProjComments projComments, IStkCommentRepository stkCommentRepository,
            IProjStakeHolderMovRepository projStakeHolderMovRepository


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
        }


        [HttpGet]
        public async Task<IActionResult> Index()

        {
            CommonDTO dto = new CommonDTO();
            //dto.Projects = projects;
            dto.Projects = await _projectsRepository.GetAllProjectsAsync();

            return View(dto);
        }

        public async Task<IActionResult> Details(int id)
        {
            //TempData["Tabshift"] = 12;
            //ViewBag.Tabshift = 12;
            var project = await _projectsRepository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return Json(new { success = true, project });
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

                    // sanal
                    MailBox mbx = new MailBox();

                    mbx.SendItems = null;//await _projectsRepository.GetStatusProjAsync(dataProjId);

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
                    ViewBag.SubmitCde = false;

                    MailBox mbx = new MailBox();

                    ViewBag.unitid = Logins.unitid;

                    mbx.InBox = await _projectsRepository.GetActInboxAsync();

                    mbx.Draft = await _projectsRepository.GetActDraftItemsAsync();


                    mbx.SendItems = await _projectsRepository.GetActSendItemsAsync();
                    mbx.CompletedItems = await _projectsRepository.GetActComplettemsAsync();


                    return View(mbx);


                    //var projects = await _projectsRepository.GetActProjectsAsync();
                    //return View(projects);
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



        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllStatus()
        {
            var ss = await _dlRepository.ddlStatus();
            return View(ss);

        }

        #region CreateProject
        [HttpGet]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Create(string id)
        {
            try
            {
                int ids = 0;
                if (id != null)
                {
                    string decryptedValue = _dataProtector.Unprotect(id);
                    ids = int.Parse(decryptedValue);
                    tbl_Projects tbl_Projects = new tbl_Projects();
                    tbl_Projects = await _projectsRepository.GetProjectByPsmIdAsync(ids);
                    ViewBag.Projects = await _projectsRepository.GetMyProjectsAsync();
                    return View(tbl_Projects);

                }
                TempData["SubCde"] = false;
                TempData.Keep("SubCde");
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    //UnitDdlGet(); projpsmided
                    var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    var watermarkText = $" {ipAddress}\n  {currentDatetime}";

                    ViewBag.Projects = await _projectsRepository.GetMyProjectsAsync();
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
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> UploadMultiFile(IFormFile uploadfile, string Reamarks, int PsmId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            if (uploadfile != null && uploadfile.Length > 0)
            {
                string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadfile.CopyTo(stream);
                }

                // var project = await _projectsRepository.GetProjectByIdAsync(ProjectId);
                if (PsmId != null && PsmId != 0)
                {
                    tbl_AttHistory atthis = new tbl_AttHistory();
                    atthis.ActionId = 0;
                    atthis.AttPath = uniqueFileName;

                    atthis.Reamarks = Reamarks;
                    atthis.PsmId = PsmId;
                    atthis.UpdatedByUserId = Logins.unitid;
                    atthis.DateTimeOfUpdate = DateTime.Now;
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
            return Json(1);

        }
        public async Task<IActionResult> AddProject(tbl_Projects Data)
        {

            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                int projid = 0;
                Data.DateTimeOfUpdate = DateTime.Now;
                Data.StakeHolderId = Logins.unitid ?? 0;
                Data.IsActive = true;
                Data.EditDeleteDate = DateTime.Now;
                Data.EditDeleteBy = 0;
                Data.IsDeleted = false;
                Data.IsSubmited = false;
                Data.UpdatedByUserId = Logins.UserIntId;
                Data.Comments = Data.InitialRemark;
                if (Data.ProjId == 0)
                {
                    Data.CurrentPslmId = 0;
                    projid = await _projectsRepository.AddProjectAsync(Data);
                    Data = await _projectsRepository.GetProjectByIdAsync(projid);
                }
                else
                {

                    Data.EditDeleteDate = DateTime.Now;



                    await _projectsRepository.UpdateProjectAsync(Data);
                    Data = await _projectsRepository.GetProjectByIdAsync(Data.ProjId);

                }

                return Json(Data);
            }
            catch (Exception ex)
            {
                return Json(5);
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
        public async Task<IActionResult> ProjectSubmited(int projid)
        {
            var project = await _projectsRepository.GetProjectByIdAsync(projid);
            project.IsSubmited = true;
            await _projectsRepository.UpdateProjectAsync(project);
            return Json(project.ProjId);
        }
        [HttpPost]
        public async Task<IActionResult> FwdProjConfirm(int PslmId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
          

            if (Logins != null)
            {

                try
                {
                    tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                    // var project = await _projectsRepository.GetProjectByIdAsync(projid);
                    psmove = await _projectsRepository.GettXNByPsmIdAsync(PslmId);
                    psmove.DateTimeOfUpdate = DateTime.Now;
                    psmove.IsComplete = true;
                    await _projectsRepository.UpdateTxnAsync(psmove);



                    return Json(PslmId);
                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> IsReadInbox(int PsmId)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");


            if (Logins != null)
            {

                try
                {

                    tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                    // var project = await _projectsRepository.GetProjectByIdAsync(projid);
                    psmove = await _projectsRepository.GettXNByPsmIdAsync(PsmId);
                    psmove.DateTimeOfUpdate = DateTime.Now;
                    psmove.IsRead = true;
                    await _projectsRepository.UpdateTxnAsync(psmove);



                    return Json(PsmId);



                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
        [HttpPost]
        public async Task<IActionResult> IsProcessProjConfirm(int ProjId)
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
                    // var project = await _projectsRepository.GetProjectByIdAsync(projid);
                    proj = await _projectsRepository.GetProjectByIdAsync(ProjId);
                    proj.DateTimeOfUpdate = DateTime.Now;
                    proj.IsProcess = true;
                    await _projectsRepository.UpdateProjectAsync(proj);



                    return Json(ProjId);
                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
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
        public string generate2(string Path, string ip)
        {

            try
            {


                Random rnd = new Random();
                string Dfilename = rnd.Next(1, 1000).ToString();
                var filePath1 = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot\\DownloadFile\\" + Dfilename + ".pdf");
                PdfDocument pdfDoc = new PdfDocument(new PdfReader(Path), new PdfWriter(filePath1));
                iText.Layout.Document doc = new iText.Layout.Document(pdfDoc);
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



        #region Project Movment For PROcess For Comment

        public async Task<IActionResult> ProcessMail(int ProjId, int unitid)
        {
            //**
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (ProjId != null)
                    {
                        var project = await _projectsRepository.GetProjectByIdAsync(ProjId);
                        unitid = project.StakeHolderId;
                        int[] stausid = {23, 22, 25, 27, 27 };
                        int[] unitids = {4,3,5 ,1, unitid };
                        for(int i = 0; i < stausid.Length; i++) 
                        {
                            tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();

                            psmove.ProjId = ProjId;
                            psmove.StatusActionsMappingId = stausid[i];
                            //psmove.ActionId = 1;
                            psmove.Remarks = "";
                            psmove.FromUnitId = Logins.unitid ?? 0;

                            //psmove.TostackholderDt = DateTime.Now;  

                            psmove.UpdatedByUserId = Logins.unitid; // change with userid
                            psmove.DateTimeOfUpdate = DateTime.Now;
                            psmove.IsActive = true;

                            psmove.EditDeleteDate = DateTime.Now;
                            psmove.EditDeleteBy = Logins.unitid;
                            psmove.TimeStamp = DateTime.Now;
                            psmove.IsComplete = false;
                            psmove.ToUnitId = unitids[i];
                            psmove.IsComment = true;

                            await _psmRepository.AddProjStakeHolderMovAsync(psmove);



                            
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
            catch (Exception ex) { return Json(-1); }
        }


        public async Task<IActionResult> FwdToProject(tbl_ProjStakeHolderMov psmove)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            psmove.ProjId = psmove.ProjId;
            psmove.StatusActionsMappingId = psmove.StatusActionsMappingId;
           // psmove.ActionId = psmove.ActionId;
            psmove.Remarks = psmove.Remarks;
            psmove.FromUnitId = Logins.unitid ?? 0;
            psmove.ToUnitId = psmove.ToUnitId; //  
            //psmove.TostackholderDt = DateTime.Now;  
            psmove.UserDetails = Helper.LoginDetails(Logins);
            psmove.UpdatedByUserId = Logins.UserIntId; // change with userid
            psmove.DateTimeOfUpdate = DateTime.Now;
            psmove.IsActive = true;

            psmove.EditDeleteDate = DateTime.Now;
            psmove.EditDeleteBy = Logins.UserIntId;
            psmove.TimeStamp = DateTime.Now;
            psmove.IsComplete = false;
            psmove.IsComment = false;
            var Ret= await _psmRepository.AddWithReturn(psmove);
           if(Ret!=null)
            {
                return Json(Ret);
            }
            else
            {
                return Json(nmum.NotSave);
            }

        }

        public async Task<IActionResult> ProjectMovHistory(int ProjectId)
        {
            //  var Ret1 = await _psmRepository.UndoProjectMov(ProjectId);
            var Ret = await _psmRepository.ProjectMovHistory(ProjectId);
            return Json(Ret);
        }
        public async Task<IActionResult> UndoProject(int ProjectId, int PsmId, string Remarks)
        {
            try
            {
                var movent = await _psmRepository.GetByByte(PsmId);
                movent.IsActive = false;
                movent.IsDeleted = true;
                movent.UndoRemarks = Remarks;
                var Ret = await _psmRepository.UpdateWithReturn(movent);

                var psmidold = _psmRepository.GetLastRecProjectMov(ProjectId);
                var movent1 = await _psmRepository.GetByByte(psmidold);
                movent1.Remarks = "";
                movent1.IsComplete = false;
                var Ret1 = await _psmRepository.UpdateWithReturn(movent);
                return Json(nmum.Update);
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }
        #endregion
        #region ProjComments
        public async Task<IActionResult> ProjComments()
        {

            return View();
        }
        public async Task<IActionResult> GetProjCommentsByUnitId(int Id)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                return Json(await _projComments.GetAllStkForComment(Convert.ToInt32(Logins.unitid)));
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }
        [HttpPost]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> SendCommentonProject(IFormFile uploadfile, string Comments, int StkStatusId, int ProjectId, int psmid)
        {
            try
            {
                StkComment cmmets = new StkComment();
                string uniqueFileName = "";
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
               var psmove = await _projectsRepository.GettXNByPsmIdAsync(psmid);
                if(psmove.IsComplete==false)
                {
                    if (uploadfile != null && uploadfile.Length > 0)
                    {
                        uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                        string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadfile.CopyTo(stream);
                        }


                        cmmets.ActFileName = uploadfile.FileName;
                    }

                    cmmets.Attpath = uniqueFileName;
                    cmmets.Comments = Comments;
                    cmmets.PsmId = psmid;
                    cmmets.ProjId = ProjectId;
                    cmmets.UpdatedByUserId = Logins.UserIntId;
                    cmmets.DateTimeOfUpdate = DateTime.Now;
                    cmmets.IsDeleted = false;
                    cmmets.IsActive = true;
                    cmmets.EditDeleteBy = Logins.unitid;
                    cmmets.EditDeleteDate = DateTime.Now;
                    cmmets.StkStatusId = StkStatusId;

                    cmmets.StakeHolderId = Logins.unitid; ;

                    var ret = await _stkCommentRepository.AddWithReturn(cmmets);

                    if (ret != null)
                        return Json(nmum.Save);
                    else
                        return Json(0);
                }
                else
                {
                    return Json(nmum.NotSave);
                }

            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }


        }


        public async Task<IActionResult> GetAllCommentBypsmId_UnitId(int psmId, int stakeholderId, int projId)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
                StkComment stkComment = new StkComment();
                stkComment.ProjId = projId;
                //stkComment.StakeHolderId = Logins.unitid;

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

        public async Task<IActionResult> ProjHistory(string userid, int? dataProjId, int? dtaProjID, string? AttPath, int? psmid, string? Projpin, string? EncyID, EncryModel? encryModel)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                string actufilename = "";
                string AttDocuDescs = "";

                if (EncyID != null)
                {
                    ViewBag.SubmitCde = true;
                    ViewBag.EncyID = EncyID;

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
                    // Assuming your model type is MyModelClass
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
                    //ViewBag.SubmitCde = false;
                    EncyID = TempData["EncyID"].ToString() ?? null;
                    TempData["EncyID"] = EncyID;
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
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.Message;
                        swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    }

                }
                int statgeIDMAx = await _stkholdmove.GetlaststageId(dataProjId);
                ViewBag.stageid = statgeIDMAx;

                var projdetails = await _projectsRepository.GetProjectByIdAsync1(dataProjId);



                var dto3 = await _commentRepository.GetCommentByPsmIdAsync(projdetails.CurrentPslmId);

                ViewBag.CommentByStakeholderList = dto3;

                var ProjMovementHist = await _projStakeHolderMovRepository.ProjectMovHistory(dataProjId);
                ViewBag.ProjMovementHist = ProjMovementHist;





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
                        //prohis[0].ActFileName = uplo
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





    }

}
