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
        private readonly IProjStakeHolderMovRepository _stkholdmove;
        private readonly IDataProtector _dataProtector;
        private readonly ICommentRepository _commentRepository;
        private readonly IActionsRepository _actionsRepository;
        private IWebHostEnvironment webHostEnvironment;
        private System.Timers.Timer aTimer;


        public ProjectsController(IProjectsRepository projectsRepository, IDdlRepository ddlRepository, IProjStakeHolderMovRepository psmRepository, IHttpContextAccessor httpContextAccessor, IDdlRepository DDLRepository, IAttHistoryRepository attHistoryRepository, IWebHostEnvironment environment, IProjStakeHolderMovRepository stkholdmove, IDataProtectionProvider DataProtector, IWebHostEnvironment _webHostEnvironment, ICommentRepository commentRepository, IActionsRepository actionsRepository)
        {
            _projectsRepository = projectsRepository;
            _dlRepository = ddlRepository;
            _psmRepository = psmRepository;
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = _webHostEnvironment;

            _DDLRepository = ddlRepository;

            _attHistoryRepository = attHistoryRepository;
            _environment = environment;
            _stkholdmove = stkholdmove;
            _commentRepository = commentRepository;
            _actionsRepository = actionsRepository;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");

        }


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Projects
        //[Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Index()

        {
            CommonDTO dto = new CommonDTO();
            //dto.Projects = projects;
            dto.Projects = await _projectsRepository.GetAllProjectsAsync();

            return View(dto);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23    not reqd  .....&   disabled
        // GET: Projects/Details/5
        //public async Task<IActionResult> Details(int id)
        //{
        //    var project = await _projectsRepository.GetProjectByIdAsync(id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(project);
        //}
        //[Authorize(Policy = "StakeHolders")]
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

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DelayedProj()
        {

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;

            List<TimeExceeds> tmexc = new List<TimeExceeds>();

            tmexc = await _projectsRepository.DelayedProj();

            return View(tmexc);
        }

        ///Created and Reviewed by : Sub Maj Sanal
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ProjComments()
        {

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;

            var projects = await _projectsRepository.GetProcProjAsync();
            ViewBag.projects = projects;

            return View();
        }





        ///Created and Reviewed by : Sub Maj M Sanal Kumar on 09 Nov 23
        // Reviewed Date : 10,11 & 12 Nov  23

        //[Authorize(Policy = "StakeHolders")]   
        public async Task<IActionResult> ValidateAction(int psmid, int ActionId, int proId, int ddlStag, int unitid)
        {
            Login Loginss = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            tbl_Projects tbproj = new tbl_Projects();
            tbl_ProjStakeHolderMov pstkm = new tbl_ProjStakeHolderMov();
            string result = null;
            try
            {
                if (proId <= 0 && psmid > 0)
                {

                    pstkm = await _stkholdmove.GetProjStakeHolderMovByIdAsync(psmid);
                    if (pstkm.ProjId > 0)
                        proId = pstkm.ProjId;

                }

                if (proId > 0)
                {

                    result = await _projectsRepository.ValidateActionSel(proId, ActionId, ddlStag);

                }

                if (result == "Ok" || result == " Pending")
                {


                    string res = await _actionsRepository.ValidateActionsAsync(ActionId, Loginss.unitid);

                    if (res != null)
                    {
                        result = res;
                    }
                    else
                    {

                        result = "Succeed";

                    }
                }


            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            if (result == "Succeed")
            {

                string rests = null;
                int comstageid = await _stkholdmove.ValStatusAsync(proId);
                // ** san
                if (comstageid == 25)  //  ahcc arch vetting and return to acg
                {
                    comstageid = 24;
                }
                if (comstageid > 0 && ddlStag < comstageid)
                {
                    rests = "Project Already Moved Out from this Stage";
                    result = rests;
                }
            }


            return Json(result);

        }


        //         try
        //            {

        //                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

        //                if (Logins != null)
        //                {
        //                    if (cmndto.Submitcde == false && cmndto.ProjMov.Comments != null)
        //                    {

        //                        cmndto.ProjMov.TimeStamp = DateTime.Now;
        //                        cmndto.ProjMov.EditDeleteBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //                        cmndto.ProjMov.EditDeleteDate = DateTime.Now;
        //                        cmndto.ProjMov.IsActive = true;
        //                        cmndto.ProjMov.IsDeleted = false;
        //                        cmndto.ProjMov.UpdatedByUserId = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //                        cmndto.ProjMov.DateTimeOfUpdate = DateTime.Now;
        //                        cmndto.ProjMov.TostackholderDt = DateTime.Now;

        //                        if (cmndto.ProjMov.StageId == 1 && cmndto.ProjMov.StatusId == 1 && cmndto.ProjMov.ActionId == 1)
        //                        {
        //                            cmndto.ProjMov.CurrentStakeHolderId = 1;
        //                        }
        //                        else
        //                        {
        //                            cmndto.ProjMov.CurrentStakeHolderId =  cmndto.ProjMov.ToStakeHolderId;
        //                        }
        //await _psmRepository.UpdateProjStakeHolderMovAsync(cmndto.ProjMov);


        //tbl_Comment cmt = new tbl_Comment();
        //cmt = await _commentRepository.GetCommentByPsmIdAsync(cmndto.ProjMov.PsmId);
        //cmt.Comment = cmndto.ProjMov.Comments;
        //await _commentRepository.UpdateCommentAsync(cmt);


        //if (uploadfile != null && uploadfile.Length > 0)
        //{

        //    string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

        //    string filePath = System.IO.Path.Combine("wwwroot/Uploads/", uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        uploadfile.CopyTo(stream);
        //    }

        //    tbl_AttHistory atthis = new tbl_AttHistory();
        //    atthis.AttPath = uniqueFileName;
        //    atthis.ActFileName = uploadfile.FileName;

        //    atthis.PsmId = cmndto.ProjMov.PsmId;
        //    atthis.UpdatedByUserId = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //    atthis.DateTimeOfUpdate = DateTime.Now;
        //    atthis.IsDeleted = false;
        //    atthis.IsActive = true;
        //    atthis.EditDeleteBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //    atthis.EditDeleteDate = DateTime.Now;
        //    atthis.ActionId = 0;
        //    atthis.TimeStamp = DateTime.Now;
        //    atthis.Reamarks = cmndto.ProjMov.AttRemarks ?? "File Attached";


        //    await _attHistoryRepository.AddAttHistoryAsync(atthis);


        //}



        //                    }

        //                    else
        //{
        //    if (uploadfile != null && uploadfile.Length > 0)
        //    {

        //        string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

        //        string filePath = System.IO.Path.Combine("wwwroot/Uploads/", uniqueFileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            uploadfile.CopyTo(stream);
        //        }

        //        tbl_AttHistory atthis = new tbl_AttHistory();
        //        atthis.AttPath = uniqueFileName;
        //        atthis.ActFileName = uploadfile.FileName;

        //        atthis.PsmId = cmndto.ProjMov.PsmId;
        //        atthis.UpdatedByUserId = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //        atthis.DateTimeOfUpdate = DateTime.Now;
        //        atthis.IsDeleted = false;
        //        atthis.IsActive = true;
        //        atthis.EditDeleteBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //        atthis.EditDeleteDate = DateTime.Now;
        //        atthis.ActionId = 0;
        //        atthis.TimeStamp = DateTime.Now;
        //        atthis.Reamarks = cmndto.ProjMov.AttRemarks ?? "File Attached";


        //        await _attHistoryRepository.AddAttHistoryAsync(atthis);


        //    }

        //}
        //                }
        //                else
        //{


        //    return Redirect("/Identity/Account/login");


        //}
        //cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(cmndto.ProjMov.PsmId);


        //ViewBag.tabshift = 12;


        //ViewBag.corpsId = 0;
        //List<mCommand> cl = new List<mCommand>();
        //cl = await _dlRepository.ddlCommand();
        //cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
        //ViewBag.cl = cl.ToList();
        //List<Types> ty = new List<Types>();
        //ty = await _dlRepository.ddlType();
        //ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
        //ViewBag.ty = ty.ToList();

        //List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
        //List<SelectListItem> mHostTyp = new List<SelectListItem>
        //                {
        //                new SelectListItem { Value = "0", Text = "--Select--" }
        //                };
        //mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


        //ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
        //List<mAppType> tapp = await _dlRepository.DdlAppType();
        //List<SelectListItem> selectList = new List<SelectListItem>
        //                {
        //                new SelectListItem { Value = "0", Text = "--Select--" }
        //                    };
        //selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
        //ViewBag.apptype = new SelectList(selectList, "Value", "Text");


        //List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
        //List<SelectListItem> selectLists = new List<SelectListItem>
        //                {
        //                new SelectListItem { Value = "0", Text = "--Select--" }
        //                };
        //selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
        //ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");
        //cmndto.Submitcde = true;

        //return View(cmndto);


        //            }
        //            catch (Exception ex)
        //            {
        //    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
        //    return Redirect("/Home/Error");
        //}
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 13 08 23
        // GET: Projects/Create

        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- incl comments
        // GET: Projects/Create

       // [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> ProjHistoryView(string? EncyID)
        {
            try
            {
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
                            return Redirect("~/Home/Error");
                        }
                        List<tbl_AttHistory> atthis = new List<tbl_AttHistory>();


                        atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(dataProjId);
                        tbl_Projects tbp = new tbl_Projects();
                        tbp = await _projectsRepository.GetProjectByPsmIdAsync(dataProjId);

                        if (tbp != null)
                            dataProjId = tbp.ProjId;

                        List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID(dataProjId);
                        tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(dataProjId);
                        projHist[0].ProjectDetl.Add(projects);
                        // var stholder = await _psmRepository.GetProjStakeHolderMovByIdAsync(projects.CurrentPslmId);

                        projHist[0].Atthistory = atthis;

                        return View(projHist);
                    }
                    return null;
                }

                else
                {
                    return Redirect("~/Identity/Account/Login");
                }
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }


        }


        //Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- flow merge
        // GET: Projects/Create

        [HttpGet]
        public async Task<IActionResult> ProjHistoryProcess(string? EncyID)
        {

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
                    List<tbl_AttHistory> atthis = new List<tbl_AttHistory>();

                    var dto3 = await _projectsRepository.GetCommentByStakeHolder(dataProjId);

                    ViewBag.CommentByStakeholderList = dto3;

                    atthis = await _attHistoryRepository.GetAttHistoryByIdAsync(dataProjId);
                    tbl_Projects tbp = new tbl_Projects();
                    tbp = await _projectsRepository.GetProjectByPsmIdAsync(dataProjId);

                    if (tbp != null)
                        dataProjId = tbp.ProjId;

                    List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID(dataProjId);
                    tbl_Projects projects = await _projectsRepository.GetProjectByIdAsync(dataProjId);
                    projHist[0].ProjectDetl.Add(projects);
                    // var stholder = await _psmRepository.GetProjStakeHolderMovByIdAsync(projects.CurrentPslmId);

                    projHist[0].Atthistory = atthis;

                    return View(projHist);
                }
                return null;
            }

            else
            {
                return Redirect("~/Identity/Account/Login");
            }

        }

        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 17 Nov 23  -- segrigation

        [HttpGet]
        //[Authorize(Policy = "StakeHolders")]
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

                    mbx.SendItems = await _projectsRepository.GetStatusProjAsync(dataProjId);

                    return View(mbx);
                }
                return null;
            }

            else
            {
                return Redirect("~/Identity/Account/Login");
            }



        }






        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge



       // [Authorize(Policy = "StakeHolders")]
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



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge


        //[Authorize(Policy = "StakeHolders")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProj(CommonDTO cmndto, IFormFile uploadfile)
        {
            try
            {
                if (uploadfile == null)
                {
                    if (cmndto.ProjEdit.ProjName == null || cmndto.ProjEdit.InitiatedDate == null || (cmndto.ProjEdit.CompletionDate < cmndto.ProjEdit.InitiatedDate) || cmndto.ProjEdit.IsWhitelisted == null ||
                        cmndto.ProjEdit.InitialRemark == null || cmndto.ProjEdit.StakeHolderId < 0 || cmndto.ProjEdit.AimScope == null || cmndto.ProjEdit.HQandITinfraReqd == null || cmndto.ProjEdit.HostTypeID == null ||
                        cmndto.ProjEdit.ContentofSWApp == null || cmndto.ProjEdit.ReqmtJustification == null || cmndto.ProjEdit.HostTypeID == 0
                        )
                    {
                        TempData["Tabshift"] = 5;
                        TempData["FailureMessage"] = "Required data field is empty....";
                        return RedirectToAction(nameof(Create));

                    }
                }



                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (TempData.ContainsKey("projpsmided"))
                    {

                    }
                    else
                    {
                        TempData["projpsmided"] = null;
                    }

                    if (TempData.ContainsKey("projpsmided") && TempData["projpsmided"] != null && uploadfile != null)
                    {

                        int psmid = (int)TempData["projpsmided"];

                        if (uploadfile != null && uploadfile.Length > 0)
                        {


                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            tbl_AttHistory atthis = new tbl_AttHistory();
                            atthis.AttPath = uniqueFileName;
                            atthis.PsmId = psmid;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.DateTimeOfUpdate = DateTime.Now;
                            atthis.IsDeleted = false;
                            atthis.ActFileName = uploadfile.FileName;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.ActionId = 0;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.Reamarks = cmndto.AttHisAdd.Reamarks ?? "File Attached";


                            await _attHistoryRepository.AddAttHistoryAsync(atthis);
                            TempData["SuccessMessage"] = "New Files Attached  !";

                        }
                        TempData["Tabshift"] = 12;
                        TempData["projpsmided"] = TempData["projpsmided"];
                        TempData.Keep("projpsmided");
                        return RedirectToAction(nameof(Create));
                    }

                    else
                    {
                        if (Logins.UserName != null)
                        {
                            TempData["projpsmided"] = cmndto.ProjEdit.CurrentPslmId;
                            TempData.Keep("projpsmided");
                            tbl_Projects project = new tbl_Projects();
                            project = cmndto.ProjEdit;
                            //project.CurrentPslmId = cmndto.ProjEdit.CurrentPslmId;
                            project.IsActive = true;
                            project.EditDeleteDate = DateTime.Now;
                            project.EditDeleteBy = Logins.unitid;
                            project.IsDeleted = false;
                            project.UpdatedByUserId = Logins.unitid;
                            project.Comments = project.InitialRemark;

                            await _projectsRepository.UpdateProjectAsync(project);

                            // TempData["SuccessMessage"] = "Project Edited!";


                            TempData["Tabshift"] = 12;

                            return RedirectToAction(nameof(Create));
                        }
                        else
                        {
                            return LocalRedirect("~/Identity/Account/Login");
                        }
                    }
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


        //[Authorize(Policy = "StakeHolders")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProjUndo(CommonDTO cmndto, IFormFile uploadfile)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (TempData.ContainsKey("projpsmided"))
                    {

                    }
                    else
                    {
                        TempData["projpsmided"] = null;
                    }

                    if (TempData.ContainsKey("projpsmided") && TempData["projpsmided"] != null && uploadfile != null)
                    {

                        int psmid = (int)TempData["projpsmided"];

                        if (uploadfile != null && uploadfile.Length > 0)
                        {


                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            tbl_AttHistory atthis = new tbl_AttHistory();
                            atthis.AttPath = uniqueFileName;
                            atthis.PsmId = psmid;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.DateTimeOfUpdate = DateTime.Now;
                            atthis.IsDeleted = false;
                            atthis.ActFileName = uploadfile.FileName;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.ActionId = 0;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.Reamarks = cmndto.AttHisAdd.Reamarks ?? "File Attached";


                            await _attHistoryRepository.AddAttHistoryAsync(atthis);
                            TempData["SuccessMessage"] = "New Files Attached  !";

                        }
                        TempData["Tabshift"] = 12;
                        TempData["projpsmided"] = TempData["projpsmided"];
                        TempData.Keep("projpsmided");
                        return RedirectToAction(nameof(ProjDraftUndo));
                    }

                    else
                    {
                        if (Logins.UserName != null)
                        {
                            TempData["projpsmided"] = cmndto.ProjEdit.CurrentPslmId;
                            TempData.Keep("projpsmided");
                            tbl_Projects project = new tbl_Projects();
                            project = cmndto.ProjEdit;
                            //project.CurrentPslmId = cmndto.ProjEdit.CurrentPslmId;
                            project.IsActive = true;
                            project.EditDeleteDate = DateTime.Now;
                            project.EditDeleteBy = Logins.unitid;
                            project.IsDeleted = false;
                            project.UpdatedByUserId = Logins.unitid;
                            project.Comments = project.InitialRemark;

                            await _projectsRepository.UpdateProjectAsync(project);

                            // TempData["SuccessMessage"] = "Project Edited!";


                            TempData["Tabshift"] = 12;

                            return RedirectToAction(nameof(Create));
                        }
                        else
                        {
                            return LocalRedirect("~/Identity/Account/Login");
                        }
                    }
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








        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge




    
        public async void ddlGen()
        {
            await Task.Delay(1000);

            ViewBag.corpsId = 0;
            List<mCommand> cl = new List<mCommand>();
            cl = await _DDLRepository.ddlCommand();
            cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
            ViewBag.cl = cl.ToList();
            List<Types> ty = new List<Types>();
            ty = await _DDLRepository.ddlType();

            ty.Insert(0, new Types { Id = 0, Name = "--Select--" });

            ViewBag.ty = ty.ToList();

            List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
            List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
            mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


            ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");

            List<mAppType> tapp = await _dlRepository.DdlAppType();
            List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
            selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
            ViewBag.apptype = new SelectList(selectList, "Value", "Text");

            List<UnitDtl> stkholder = await _dlRepository.ddlUnit();
            List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
            selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
            ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");


        }


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Projects/Edit/5
        //[Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    var project = await _projectsRepository.GetProjectByIdAsync(id);

                    if (project == null)
                    {
                        return NotFound();
                    }
                    return View(project);
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





        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // POST: Projects/Edit/5
        //[Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_Projects project)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {

                    if (id != project.ProjId)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        await _projectsRepository.UpdateProjectAsync(project);
                        return RedirectToAction(nameof(Index));
                    }

                    return View(project);
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
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Projects/Delete/5
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                if (Logins != null)
                {
                    var project = await _projectsRepository.GetProjectByIdAsync(id);
                    if (project == null)
                    {
                        return NotFound();
                    }
                    return View(project);
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
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // POST: Projects/Delete/5
        // Added by Ajay on 06 Sep 23
        //[Authorize(Policy = "StakeHolders")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                int result = 1;
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (TempData.ContainsKey("projpsmided"))
                    {
                        if (TempData["projpsmided"] != null)
                        {
                            if ((int)TempData["projpsmided"] > 0)
                            {
                                TempData["SuccessMessage"] = "Project Edited!";
                                TempData["Tabshift"] = 12;
                            }
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Deleted Successfully..!";
                        TempData["Tabshift"] = 3;
                    }


                    var attHistoryData = await _attHistoryRepository.GetAttHistDelIdAsync(id);
                    if (attHistoryData == null)
                    {
                        return NotFound();
                    }

                    if (!string.IsNullOrEmpty(attHistoryData[0].AttPath))
                    {
                        result = attHistoryData[0].PsmId;

                        //var filePathWithExtension = attHistoryData[0].AttPath;
                        //var filePath = Path.Combine(_environment.WebRootPath, "uploads\\", filePathWithExtension);

                        //string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{Path.GetExtension(attHistoryData[0].AttPath)}";

                        string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", attHistoryData[0].AttPath);

                        try
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);

                            }
                            else
                            {
                                TempData["FailureMessage"] = "File Does Not Exists..!";

                            }
                        }
                        catch (Exception ex)
                        {
                            swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                            TempData["FailureMessage"] = "File Reading Error..!";

                        }



                    }

                    TempData.Keep("Tabshift");
                    ViewBag.SubmitCde = true;
                    ViewBag.SubmitCde = true;
                    TempData["Psmiiddel"] = id;
                    TempData.Keep("Psmiiddel");


                    if (TempData.ContainsKey("projpsmided"))
                    {
                        if (TempData["projpsmided"] != null)
                        {
                            if ((int)TempData["projpsmided"] > 0)
                            {
                                TempData["projpsmided"] = TempData["projpsmided"];

                            }
                        }
                    }

                    else
                    {
                        TempData["projpsmid"] = TempData["projpsmid"];
                    }
                    await _attHistoryRepository.DeleteAttHistoryAsync(id);

                    return Json(result);
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


        //[Authorize(Policy = "StakeHolders")]
        [HttpPost, ActionName("DeleteConfDft")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfDft(int id)
        {
            try
            {
                int result = id;
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (TempData.ContainsKey("projpsmided"))
                    {
                        if (TempData["projpsmided"] != null)
                        {
                            if ((int)TempData["projpsmided"] > 0)
                            {
                                TempData["SuccessMessage"] = "Project Edited!";
                                TempData["Tabshift"] = 12;
                            }
                        }
                    }
                    else
                    {
                        //  TempData["SuccessMessage"] = "Deleted Successfully..!";
                        TempData["Tabshift"] = 3;
                    }


                    var attHistoryData = await _attHistoryRepository.GetAttHistDelIdAsync(id);
                    if (attHistoryData == null)
                    {
                        return NotFound();
                    }

                    if (!string.IsNullOrEmpty(attHistoryData[0].AttPath))
                    {
                        result = attHistoryData[0].PsmId;

                        //var filePathWithExtension = attHistoryData[0].AttPath;
                        //var filePath = Path.Combine(_environment.WebRootPath, "uploads\\", filePathWithExtension);

                        //string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{Path.GetExtension(attHistoryData[0].AttPath)}";

                        string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", attHistoryData[0].AttPath);

                        try
                        {
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);

                            }
                            else
                            {
                                TempData["FailureMessage"] = "File Does Not Exists..!";

                            }
                        }
                        catch (Exception ex)
                        {
                            swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                            TempData["FailureMessage"] = "File Reading Error..!";

                        }



                    }

                    TempData.Keep("Tabshift");
                    // ViewBag.SubmitCde = true;



                    if (TempData.ContainsKey("projpsmided"))
                    {
                        if (TempData["projpsmided"] != null)
                        {
                            if ((int)TempData["projpsmided"] > 0)
                            {
                                TempData["projpsmided"] = TempData["projpsmided"];

                            }
                        }
                    }

                    else
                    {
                        TempData["projpsmid"] = TempData["projpsmid"];
                    }
                    await _attHistoryRepository.DeleteAttHistoryAsync(id);
                    //ViewBag.SubmitCde = true;
                    TempData["Psmiiddel"] = id;
                    TempData.Keep("Psmiiddel");
                    return Json(result);
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

       // [Authorize(Policy = "StakeHolders")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAllStatus()
        {
            var ss = await _dlRepository.ddlStatus();
            return View(ss);

        }

       
        public async void UnitDdlGet()
        {
            await Task.Delay(1000);
            ViewBag.corpsId = 0;
            List<mCommand> cl = new List<mCommand>();
            cl = await _dlRepository.ddlCommand();
            cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
            ViewBag.cl = cl.ToList();
            List<Types> ty = new List<Types>();
            ty = await _dlRepository.ddlType();
            ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
            ViewBag.ty = ty.ToList();
            List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
            List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
            mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


            ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
            List<mAppType> tapp = await _dlRepository.DdlAppType();
            List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
            selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
            ViewBag.apptype = new SelectList(selectList, "Value", "Text");



        }



        //[HttpGet]
        //[Authorize(Policy = "StakeHolders")]
        //public async Task<IActionResult> ProjHistory(ProjmovDTO cmndtos, string? EncyID)
        //{

        //    try
        //    {
        //        ProjmovDTO cmndto = new ProjmovDTO();
        //        Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");


        //        var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //        var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        //        var watermarkText = $" {ipAddress}\n  {currentDatetime}";
        //        TempData["ipadd"] = watermarkText;

        //        if (EncyID != null)
        //        {
        //            if (cmndtos.Submitcde == true)
        //            {
        //                cmndto.Submitcde = true;
        //                ViewBag.Submitcde = true;
        //                return View(cmndto);
        //            }
        //            else
        //            {
        //                string decryptedValue = _dataProtector.Unprotect(EncyID);
        //                int dataProjId = int.Parse(decryptedValue);

        //                cmndto.ProjEdit = await _projectsRepository.GetProjectByIdAsync(dataProjId);
        //                cmndto.ProjHistory = await _projectsRepository.GetProjectHistorybyID(dataProjId);
        //                cmndto.ProjMov = await _psmRepository.GetProjStakeHolderMovByIdAsync(cmndto.ProjEdit.CurrentPslmId);
        //                var dto3 = await _projectsRepository.GetCommentByStakeHolder(dataProjId);
        //                ViewBag.CommentByStakeholderList = dto3;
        //                if (cmndto.ProjEdit == null && cmndto.ProjMov != null)
        //                {
        //                    cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(cmndto.ProjMov.PsmId);
        //                }
        //                else if (cmndto.Atthistory == null)
        //                {
        //                    cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(0);
        //                }
        //                cmndto.DataProjId = dataProjId;
        //                //cmndto.ProjMov.Psmidex = cmndto.ProjMov.PsmId;
        //                cmndto.Submitcde = false;
        //                ViewBag.Submitcde = false;




        //                return View(cmndto);
        //            }

        //        }
        //        else
        //        {
        //            return Redirect("/Home/Error");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
        //        return Redirect("/Home/Error");
        //    }



        //}






















        //[HttpPost]
        //[Authorize(Policy = "StakeHolders")]
        //public async Task<IActionResult> ProjHistory([ValidateNever] ProjmovDTO cmndtos, IFormFile uploadfile)
        //{
        //    try
        //    {

        //        Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

        //        if (Logins != null)
        //        {
        //            if (cmndtos.Submitcde == false && cmndtos.ProjMov.Comments != null)
        //            {

        //                cmndtos.ProjMov.TimeStamp = DateTime.Now;
        //                cmndtos.ProjMov.EditDeleteBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //                cmndtos.ProjMov.EditDeleteDate = DateTime.Now;
        //                cmndtos.ProjMov.IsActive = true;
        //                cmndtos.ProjMov.IsDeleted = false;
        //                cmndtos.ProjMov.UpdatedByUserId = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //                cmndtos.ProjMov.DateTimeOfUpdate = DateTime.Now;
        //                cmndtos.ProjMov.TostackholderDt = DateTime.Now;
        //                cmndtos.ProjMov.ProjId = cmndtos.DataProjId ?? 0;

        //                if (cmndtos.ProjMov.StageId == 1 && cmndtos.ProjMov.StatusId == 1 && cmndtos.ProjMov.ActionId == 1)
        //                {
        //                    cmndtos.ProjMov.CurrentStakeHolderId = 1;
        //                }
        //                else
        //                {
        //                    cmndtos.ProjMov.CurrentStakeHolderId = cmndtos.ProjMov.StakeHolderId;
        //                }


        //            }



        //            Projmove projStakeHolderMov = new Projmove();


        //            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

        //            if (uploadfile != null && uploadfile.Length > 0)
        //            {


        //                string filePath = System.IO.Path.Combine("wwwroot/Uploads/", uniqueFileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    uploadfile.CopyTo(stream);
        //                }
        //                projStakeHolderMov.Atthistory[0].AttPath = uniqueFileName;
        //                projStakeHolderMov.Atthistory[0].ActFileName = uploadfile.FileName;


        //            }


        //            projStakeHolderMov.ProjMov = cmndtos.ProjMov;
        //            projStakeHolderMov.ProjMov.ProjId = cmndtos.DataProjId ?? 0;
        //            projStakeHolderMov.DataProjId = cmndtos.DataProjId ?? 0;

        //            int psmid = 0;
        //            if (cmndtos.Submitcde == false)
        //            {
        //                cmndtos = new ProjmovDTO();
        //                psmid = await _psmRepository.AddProjStakeHolderMovAsync(projStakeHolderMov);
        //                cmndtos.ProjMov = new tbl_ProjStakeHolderMov();
        //                cmndtos.ProjMov = await _psmRepository.GetProjStakeHolderMovByIdAsync(psmid);
        //                cmndtos.ProjMov.Psmidex = psmid;
        //                cmndtos.DataProjId = projStakeHolderMov.DataProjId;
        //            }
        //            else
        //            {
        //                psmid = _projectsRepository.CurrentPsmGet(cmndtos.DataProjId ?? 0);

        //                if (projStakeHolderMov.Atthistory[0].AttPath !=null)
        //                {
        //                    tbl_AttHistory atthis = new tbl_AttHistory();
        //                    atthis.AttPath = projStakeHolderMov.Atthistory[0].AttPath;
        //                    atthis.ActFileName = projStakeHolderMov.Atthistory[0].ActFileName;
        //                    atthis.PsmId = psmid;
        //                    atthis.UpdatedByUserId = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //                    atthis.DateTimeOfUpdate = DateTime.Now;
        //                    atthis.IsDeleted = false;
        //                    atthis.IsActive = true;
        //                    atthis.EditDeleteBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
        //                    atthis.ActionId = 1;
        //                    atthis.TimeStamp = DateTime.Now;
        //                    atthis.ActionId = 0;
        //                    atthis.EditDeleteDate = DateTime.Now;
        //                    atthis.Reamarks = cmndtos.ProjMov.AttRemarks ?? "File Attached";
        //                    await _attHistoryRepository.AddAttHistoryAsync(atthis);


        //                }





        //                cmndtos.ProjMov = new tbl_ProjStakeHolderMov();
        //                cmndtos.ProjMov = await _psmRepository.GetProjStakeHolderMovByIdAsync(psmid);
        //                cmndtos.ProjMov.Psmidex = psmid;
        //            }

        //            if (cmndtos.ProjEdit != null)
        //            {


        //                cmndtos.ProjEdit.ProjId = cmndtos.DataProjId ?? 0;
        //            }




        //        int dataProjId = cmndtos.ProjMov.PsmId;
        //            psmid = _projectsRepository.CurrentPsmGet(cmndtos.DataProjId ?? 0);

        //            cmndtos.ProjMov = await _stkholdmove.GetProjStakeHolderMovByIdAsync(dataProjId);
        //        cmndtos.ProjEdit = await _projectsRepository.GetProjectByPsmIdAsync(dataProjId);
        //        cmndtos.ProjHistory = await _projectsRepository.GetProjectHistorybyID(dataProjId);
        //        var dto3 = await _projectsRepository.GetCommentByStakeHolder(cmndtos.DataProjId);
        //        cmndtos.ProjMov.Psmidex = dataProjId;
        //        ViewBag.CommentByStakeholderList = dto3;
        //        if (psmid > 0)
        //        {
        //            cmndtos.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(psmid);
        //        }
        //       else
        //            {
        //                cmndtos.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(0);
        //            }


        //            ViewBag.Submitcde = true;



        //            cmndtos.Submitcde = true;



        //            ViewBag.tabshift = 12;


        //            List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
        //            List<SelectListItem> selectLists = new List<SelectListItem>
        //        {
        //        new SelectListItem { Value = "0", Text = "--Select--" }
        //        };
        //            selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
        //            ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");


        //            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        //            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
        //            TempData["ipadd"] = watermarkText;
        //            TempData["psmid"] = psmid;




        //            cmndtos.ProjMov.PsmId = psmid;
        //            cmndtos.Submitcde = true;
        //            return View(cmndtos);

        //        }
        //        else
        //        {


        //            return Redirect("/Identity/Account/login");
        //        }




        //    }
        //    catch (Exception ex)
        //    {
        //        swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
        //        return Redirect("/Home/Error");
        //    }

        //}



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 13 08 23
        // GET: Projects/Create
        //[Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> ProjHistory(string userid, int? dataProjId, int? dtaProjID, string? AttPath, int? psmid, string? Projpin, string? EncyID, EncryModel? encryModel)
        {
            try
            {
                Login Loginss = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
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
                            userid = Loginss.UserName;
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
                var dto3 = await _projectsRepository.GetCommentByStakeHolder(dataProjId);

                ViewBag.CommentByStakeholderList = dto3;



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



























       // [Authorize(Policy = "StakeHolders")]
        [HttpPost]

        public async Task<IActionResult> FwdCreates(Projmove projStakeHolderMov, IFormFile uploadfile)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;

            if (Logins != null)
            {
                int psmid = 0;
                string webRootPath = _environment.WebRootPath;

                string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
                string host = _httpContextAccessor.HttpContext.Request.Host.Value;
                string fullUrl = $"{scheme}://{host}/Uploads/";


                try
                {


                    tbl_Projects Tbproj = new tbl_Projects();
                    Tbproj = await _projectsRepository.GetProjectByIdAsync(projStakeHolderMov.DataProjId ?? 0);
                    tbl_Projects pjt = new tbl_Projects();
                    pjt = await _projectsRepository.GetProjectByIdAsync(projStakeHolderMov.DataProjId ?? 0);
                    ProjIDRes PjIR = new ProjIDRes();

                    PjIR = swas.BAL.Utility.ExtensionMethods.FirstSecond(pjt.ProjName, projStakeHolderMov.DataProjId ?? 0, psmid);

                    if (Tbproj.ProjCode == null)
                    {
                        Tbproj.ProjCode = PjIR.PorjPin;
                        await _projectsRepository.UpdateProjectAsync(Tbproj);
                        ViewBag.PjIR = PjIR;
                    }
                    else
                    {
                        ViewBag.PjIR = Tbproj.ProjCode;
                    }


                    if (projStakeHolderMov.Submitcde != true)
                    //first submit
                    {
                        List<ProjHistory> projHist = await _projectsRepository.GetProjectHistorybyID(projStakeHolderMov.DataProjId);

                        tbl_AttHistory atthis = new tbl_AttHistory();
                        if (uploadfile != null && uploadfile.Length > 0)
                        {
                            // upload not null
                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            projStakeHolderMov.Atthistory[0].AttPath = uniqueFileName;




                            if (uploadfile.FileName != null && projStakeHolderMov.Atthistory[0].Reamarks != null)
                            // 
                            {
                                projStakeHolderMov.Atthistory[0].Reamarks = projStakeHolderMov.ProjMov.AttRemarks;
                                projStakeHolderMov.Atthistory[0].ActFileName = uploadfile.FileName;
                            }


                        }
                        else
                        // no uploads
                        {

                        }

                        //start //san
                        psmid = await _psmRepository.AddProjStakeHolderMovAsync(projStakeHolderMov);




                        MyRequestModel mrmodel = new MyRequestModel();
                        mrmodel.DtaProjID = projStakeHolderMov.DataProjId ?? 0;
                        if (atthis.AttPath != null)
                        {
                            mrmodel.AttPath = atthis.AttPath;
                            mrmodel.AttDocuDesc = atthis.Reamarks;

                            mrmodel.ActFileName = uploadfile.FileName;
                        }
                        mrmodel.PsmId = psmid;
                        mrmodel.Projpin = Tbproj.ProjCode;

                        var serializedData = JsonConvert.SerializeObject(mrmodel);
                        string ProtectedValue = _dataProtector.Protect(serializedData);
                        EncryModel encryModel = new EncryModel();
                        encryModel.EncryItem = ProtectedValue;

                        ViewBag.SubmitCde = true; // for subsequent submissions

                        ViewBag.PsmId = psmid;
                        ViewBag.DataProjId = projStakeHolderMov.DataProjId;

                        // TempData["SuccessMessage"] = "Please note down the Proj Code for Further Enquiries.. (Fwd & Ready for Attach more Docu)...";

                        // end

                        return RedirectToAction("ProjHistory", "Projects", encryModel);
                        // 1 time
                        // Process the form data



                    }
                    else
                    {
                        // second submit

                        // 2nd time
                        // Handle
                        // 
                        ViewBag.PjIR = projStakeHolderMov.ProjMov.ProjCode;
                        ViewBag.PsmId = projStakeHolderMov.ProjMov.PsmId;
                        tbl_AttHistory atthis = new tbl_AttHistory();
                        if (uploadfile != null && uploadfile.Length > 0)
                        {
                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";
                            //string filePath = Path.Combine(fullUrl, uniqueFileName);
                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            atthis.AttPath = uniqueFileName;
                            atthis.Reamarks = projStakeHolderMov.ProjMov.AttRemarks;
                            atthis.ActFileName = uploadfile.FileName;


                            MyRequestModel mrmodel = new MyRequestModel();
                            mrmodel.DtaProjID = projStakeHolderMov.DataProjId ?? 0;
                            mrmodel.AttPath = atthis.AttPath;
                            mrmodel.AttDocuDesc = atthis.Reamarks;
                            mrmodel.ActFileName = uploadfile.FileName;
                            mrmodel.PsmId = projStakeHolderMov.ProjMov.PsmId;
                            var serializedData = JsonConvert.SerializeObject(mrmodel);
                            string ProtectedValue = _dataProtector.Protect(serializedData);
                            EncryModel encryModel = new EncryModel();
                            encryModel.EncryItem = ProtectedValue;


                            tbl_AttHistory atthist = new tbl_AttHistory();
                            atthist.AttPath = atthis.AttPath;
                            atthist.Reamarks = atthis.Reamarks;
                            atthist.ActFileName = uploadfile.FileName;
                            atthist.PsmId = projStakeHolderMov.ProjMov.PsmId;
                            atthist.UpdatedByUserId = Logins.unitid;
                            atthist.DateTimeOfUpdate = DateTime.Now;
                            atthist.IsDeleted = false;
                            atthist.IsActive = true;
                            atthist.EditDeleteBy = Logins.unitid;
                            atthist.ActionId = projStakeHolderMov.ProjMov.ActionId;
                            atthist.TimeStamp = DateTime.Now;
                            atthist.EditDeleteDate = DateTime.Now;

                            await _attHistoryRepository.AddAttHistoryAsync(atthist);



                            TempData["SuccessMessage"] = "Docu Attached...";
                            ViewBag.SubmitCde = true;


                            return RedirectToAction("ProjHistory", "Projects", encryModel);

                        }

                        return RedirectToAction("Error", "Home");
                    }

                }
                catch (Exception ex)
                {
                    tbl_AttHistory atthist = new tbl_AttHistory();
                    atthist.AttPath = ex.Message;
                    atthist.ActFileName = ex.Message;
                    atthist.PsmId = 88;
                    atthist.UpdatedByUserId = 1;
                    atthist.DateTimeOfUpdate = DateTime.Now;
                    atthist.IsDeleted = false;
                    atthist.IsActive = true;
                    atthist.EditDeleteBy = 1;
                    atthist.ActionId = projStakeHolderMov.ProjMov.ActionId;
                    atthist.TimeStamp = DateTime.Now;
                    atthist.EditDeleteDate = DateTime.Now;

                    await _attHistoryRepository.AddAttHistoryAsync(atthist);

                    return RedirectToAction("ProjHistory", "Projects");
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }

            // return View("ProjHistory", projHist);
        }











































        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Projects/Create
        [HttpGet]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Create(CommonDTO cmndto)
        {
            try
            {
                TempData["SubCde"] = false;
                TempData.Keep("SubCde");
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    //UnitDdlGet(); projpsmided
                    var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                    TempData["ipadd"] = watermarkText;

                    ViewBag.SubmitCde = false;

                    TempData.Remove("TfrToNext");
                    if (ViewBag.SubmitCde = false)
                        TempData["projpsmid"] = null;


                    ViewBag.corpsId = 0;
                    List<mCommand> cl = new List<mCommand>();
                    cl = await _dlRepository.ddlCommand();
                    cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                    ViewBag.cl = cl.ToList();
                    List<Types> ty = new List<Types>();
                    ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();


                    List<mAppType> tapp = await _dlRepository.DdlAppType();
                    List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                    selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                    ViewBag.apptype = new SelectList(selectList, "Value", "Text");


                    List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                    List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                    ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");

                    List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                    List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));

                    ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");

                    ViewBag.ProjEdit = string.Empty;

                    if (TempData["projpsmid"] == null)
                        TempData["projpsmid"] = 0;

                    if (TempData["TfrToNext"] == null)
                        TempData["TfrToNext"] = 0;

                    if (TempData["ProjID"] == null)
                        TempData["ProjID"] = 0;
                    if (TempData["projpsmided"] == null)
                        TempData["projpsmided"] = 0;

                    int TfrToNext = (int)TempData["Tabshift"];
                    if ((int)TempData["projpsmided"] > 0)
                    {
                        TempData["projpsmided"] = TempData["projpsmided"];
                        cmndto.Atthistory = _projectsRepository.GetAttachmentsByPsmId((int)TempData["projpsmided"]);
                        TempData["Tabshift"] = 12;

                    }
                    else if (cmndto.ProjEdit != null)
                    {
                        cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(cmndto.ProjEdit.CurrentPslmId);

                    }

                    cmndto.Projects = await _projectsRepository.GetMyProjectsAsync();
                    //await _projectsRepository.GetActProjectsAsync();

                    if (cmndto.Projects.Count < 1)
                    {
                        TempData["Tabshift"] = 0;
                        ViewBag.SubmitCde = false;
                        TempData["projpsmid"] = 0;
                        TempData["projpsmided"] = 0;


                    }
                    if (cmndto.ProjMov != null)
                    {
                        if (cmndto.ProjMov.PsmId == null)
                        {
                            cmndto.ProjMov.PsmId = 0;
                        }
                    }
                    return View(cmndto);
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



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // POST: Projects/Create
        [HttpPost]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Create(CommonDTO cmndto, IFormFile? uploadfile, [FromForm] int? id)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    if (id > 1)
                    {
                        ViewBag.SubmitCde = true;
                        ViewBag.corpsId = 0;
                        List<mCommand> cl = await _dlRepository.ddlCommand();
                        cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                        ViewBag.cl = cl.ToList();
                        List<Types> ty = await _dlRepository.ddlType();

                        ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                        ViewBag.ty = ty.ToList();

                        List<mAppType> tapp = await _dlRepository.DdlAppType();
                        List<SelectListItem> selectList = new List<SelectListItem>
                         {
                              new SelectListItem { Value = "0", Text = "--Select--" }
                         };
                        selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                        ViewBag.apptype = new SelectList(selectList, "Value", "Text");

                        List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                        List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                        selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                        ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");
                        List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                        List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                        mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                        ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");


                        cmndto.Projects = await _projectsRepository.GetMyProjectsAsync();
                        cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(id);
                        ////   WORK     TAKE ALL DATA ACCORDING TO ID AS OLD ITEMS AND PASS TO VIEW

                        if (TempData["SuccessMessage"] as string == "Project Edited!")
                        {

                            TempData["projpsmided"] = TempData["projpsmided"];
                            TempData["Tabshift"] = 12;
                        }
                        else
                        {
                            TempData["projpsmid"] = TempData["projpsmid"];
                            TempData["Tabshift"] = 3;
                        }
                        // TempData["SuccessMessage"] = "**********   ********";
                        cmndto.ProjEdit = await _projectsRepository.GetProjectByPsmIdAsync(id ?? 0);
                        return View(cmndto);
                    }


                    // NEW PROJECT SUBMIT CREATION
                    // xx

                    if (cmndto.AttHisAdd == null || cmndto.AttHisAdd.Reamarks == null)
                    {
                        if (cmndto.ProjEdit.CurrentStakeHolderId < 0)
                        {
                            TempData["FailureMessage"] = "Unit Not Selected....!";
                            return RedirectToAction(nameof(Create));
                        }

                        if (Logins != null)
                        {

                            tbl_Projects project = new tbl_Projects();
                            project = cmndto.ProjEdit;

                            project.DateTimeOfUpdate = DateTime.Now;
                            project.StakeHolderId = Logins.unitid ?? 0;
                            project.IsActive = true;
                            project.EditDeleteDate = DateTime.Now;
                            project.EditDeleteBy = Logins.unitid;
                            project.IsDeleted = false;
                            project.CurrentPslmId = 0;
                            project.UpdatedByUserId = Logins.unitid;
                            project.Comments = project.InitialRemark;

                            try
                            {
                                if (uploadfile == null)
                                {
                                    ModelState.ClearValidationState("UploadedFile");
                                    ModelState.ClearValidationState("uploadfile");
                                    ModelState.Remove("uploadfile");
                                }

                                if (!_projectsRepository.VerifyProjectNameAsync(project.ProjName))

                                {

                                    if (uploadfile != null && uploadfile.Length > 0)
                                    {
                                        string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                                        string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                                        using (var stream = new FileStream(filePath, FileMode.Create))
                                        {
                                            uploadfile.CopyTo(stream);
                                        }

                                        project.UploadedFile = uniqueFileName;
                                        project.ActFileName = uploadfile.FileName;
                                    }


                                    ViewBag.SubmitCde = true;
                                    int projid = await _projectsRepository.AddProjectAsync(project);
                                    project = await _projectsRepository.GetProjectByIdAsync(projid);
                                    TempData["ProjCde"] = project.ProjCode;


                                    TempData["Tabshift"] = 3;
                                    //TempData["SuccessMessage"] = "****  ****";

                                    TempData["ProjID"] = project.ProjId;
                                    TempData["projpsmid"] = project.CurrentPslmId;
                                    TempData.Keep("projpsmid");


                                    List<mCommand> clA = new List<mCommand>();
                                    clA = await _dlRepository.ddlCommand();
                                    clA.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                                    ViewBag.cl = clA.ToList();
                                    List<Types> tyA = new List<Types>();
                                    tyA = await _dlRepository.ddlType();
                                    tyA.Insert(0, new Types { Id = 0, Name = "--Select--" });
                                    ViewBag.ty = tyA.ToList();

                                    List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                                    List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                                    mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));
                                    ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
                                    List<mAppType> tapp = await _dlRepository.DdlAppType();
                                    List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                                    selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                                    ViewBag.apptype = new SelectList(selectList, "Value", "Text");
                                    List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                                    List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                                    selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                                    ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");


                                    cmndto.Projects = await _projectsRepository.GetMyProjectsAsync();
                                    //await _projectsRepository.GetActProjectsAsync();
                                    cmndto.ProjEdit = project;

                                    // TempData["SuccessMessage"] = "*****  ******";
                                    return View(cmndto);


                                }
                                else
                                {
                                    // PROJECT ALREADY EXISTS

                                    ViewBag.corpsId = 0;
                                    List<mCommand> cl = new List<mCommand>();
                                    cl = await _dlRepository.ddlCommand();
                                    cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                                    ViewBag.cl = cl.ToList();
                                    List<Types> ty = new List<Types>();
                                    ty = await _dlRepository.ddlType();
                                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                                    ViewBag.ty = ty.ToList();
                                    List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                                    List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                                    mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                                    ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
                                    List<mAppType> tapp = await _dlRepository.DdlAppType();
                                    List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                                    selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                                    ViewBag.apptype = new SelectList(selectList, "Value", "Text");



                                    List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                                    List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                                    selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                                    ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");


                                    TempData["projpsmid"] = TempData["projpsmid"];


                                    cmndto.Projects = await _projectsRepository.GetMyProjectsAsync();
                                    //await _projectsRepository.GetActProjectsAsync();


                                    TempData["FailureMessage"] = "Poject Already Exists....!";
                                    return View(cmndto);

                                }
                            }
                            catch (Exception ex)
                            {
                                TempData["FailureMessage"] = "Network Error....!";
                                string ss = ex.Message;
                                return RedirectToAction(nameof(Create));

                            }
                        }

                        else
                        {
                            TempData["FailureMessage"] = "Invalid access... Permission Denied..!";
                            return RedirectToAction(nameof(Create));
                        }



                    }
                    else
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

                        List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                        List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                        mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                        ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
                        List<mAppType> tapp = await _dlRepository.DdlAppType();
                        List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                        selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                        ViewBag.apptype = new SelectList(selectList, "Value", "Text");


                        List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                        List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                        selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                        ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");


                        cmndto.Projects = await _projectsRepository.GetMyProjectsAsync();//
                                                                                         //await _projectsRepository.GetActProjectsAsync();

                        TempData["Tabshift"] = 3;
                        int projidre = 0;
                        projidre = (int)TempData["projpsmid"];

                        tbl_AttHistory atthis = new tbl_AttHistory();
                        if (uploadfile != null && uploadfile.Length > 0)
                        {
                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }
                            atthis.ActionId = 0;
                            atthis.AttPath = uniqueFileName;

                            atthis.Reamarks = cmndto.AttHisAdd.Reamarks;
                            atthis.PsmId = projidre;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.DateTimeOfUpdate = DateTime.Now;
                            atthis.IsDeleted = false;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.ActFileName = uploadfile.FileName;

                            await _attHistoryRepository.AddAttHistoryAsync(atthis);
                            TempData["SuccessMessage"] = "Docu Attached...";



                            if (TempData.ContainsKey("projpsmided"))
                            {
                                if (TempData["projpsmided"] != null)
                                {
                                    if ((int)TempData["projpsmided"] > 0)
                                        TempData["Tabshift"] = 12;
                                }
                            }
                        }


                        cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(projidre);
                        cmndto.ProjEdit.CurrentPslmId = projidre;
                        return View(cmndto);
                    }
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




        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge

        //[Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> FwdProjConfirm(int projid)
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
                    tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                    psmove = await _projectsRepository.GettXNByPsmIdAsync(projid);
                    psmove.ActionCde = 1;
                    psmove.DateTimeOfUpdate = DateTime.Now;
                    psmove.TimeStamp = DateTime.Now;
                    psmove.EditDeleteDate = DateTime.Now;
                    psmove.EditDeleteBy = Logins.unitid;
                    await _projectsRepository.UpdateTxnAsync(psmove);


                    SessionHelper.ClaerObjectAsJson(HttpContext.Session, "Encid");
                    SessionHelper.ClaerObjectAsJson(HttpContext.Session, "tabshift");
                    SessionHelper.ClaerObjectAsJson(HttpContext.Session, "psmid");
                    SessionHelper.ClaerObjectAsJson(HttpContext.Session, "submitcde");

                    //TempData["SuccessMessage"] = "Project Successfully Fwd..!";
                    return Json(projid);
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



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge

        //[Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> FwdProjConfirmnew(int projid)
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

                    tbl_ProjStakeHolderMov psmove = new tbl_ProjStakeHolderMov();
                    psmove = await _projectsRepository.GettXNByPsmIdAsync(projid);
                    psmove.ActionCde = 1;
                    await _projectsRepository.UpdateTxnAsync(psmove);
                    CommonDTO dtoc = new CommonDTO();
                    dtoc.ProjHistory = await _projectsRepository.GetProjectHistorybyID(projid);
                    // TempData["SuccessMessage"] = "Project Successfully Fwd..!";

                    return PartialView("_projfwdupload", dtoc);
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
        //FwdProjConfirm


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  

        //[Authorize(Policy = "StakeHolders")]
        public IActionResult ClearTempDataAndRedirect()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                try
                {
                    TempData["Tabshift"] = 0;
                    TempData.Remove("projpsmided");
                    TempData.Remove("SuccessMessage");
                    TempData.Remove("projpsmid");
                    TempData.Remove("FailureMessage");
                    ViewBag.SubmitCde = false;
                    return RedirectToAction("Create", "Projects"); // Redirect to the desired action and controller
                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    return Redirect("/Home/Error");
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge


        //[Authorize(Policy = "StakeHolders")]
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


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        /// <summary>


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




        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- bugs pedning

        //[Authorize(Policy = "StakeHolders")]

        [HttpPost]
        public IActionResult ProjectUndo(string id)
        {
            try
            {
                int ids = 0;
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {
                    try
                    {
                        if (id != null)
                        {
                            string decryptedValue = _dataProtector.Unprotect(id);
                            ids = int.Parse(decryptedValue);
                        }
                        var success = _projectsRepository.UndoChanges(ids).Result;
                        if (success)
                        {
                            return Json(new { success = true, message = "Record updated successfully." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "No records were updated." });
                        }
                    }
                    catch (Exception ex)
                    {
                        swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                        return Json(new { success = false, message = "Error updating record: " + ex.Message });
                    }
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



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised after merge  //   bypassed



        //[Authorize(Policy = "StakeHolders")]
        //EditProj
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DraftEditProj(CommonDTO cmndto, IFormFile uploadfile)
        {



            try
            {
                int cde = 0;
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                int psmid = int.Parse(SessionHelper.GetObjectFromJson<string>(_httpContextAccessor.HttpContext.Session, "psmid"));

                if (Logins != null)
                {
                    if (TempData.ContainsKey("projpsmided"))
                    {

                    }
                    else
                    {
                        TempData["projpsmided"] = null;
                    }

                    if (TempData.ContainsKey("projpsmided") && TempData["projpsmided"] != null && uploadfile != null)
                    {



                        if (uploadfile != null && uploadfile.Length > 0)
                        {
                            cde = 1;
                            //string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
                            //var filePath = Path.Combine(_environment.WebRootPath, "Uploads\\", uniqueFileName);


                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            tbl_AttHistory atthis = new tbl_AttHistory();
                            atthis.AttPath = uniqueFileName;
                            atthis.ActFileName = uploadfile.FileName;

                            atthis.PsmId = psmid;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.DateTimeOfUpdate = DateTime.Now;
                            atthis.IsDeleted = false;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.ActionId = 0;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.Reamarks = cmndto.ProjMov.AttRemarks ?? "File Attached";


                            await _attHistoryRepository.AddAttHistoryAsync(atthis);
                            TempData["SuccessMessage"] = "New Files Attached  !";

                        }
                        TempData["Tabshift"] = 12;
                        TempData["projpsmided"] = TempData["projpsmided"];
                        TempData.Keep("projpsmided");

                        if (cmndto.ProjMov.StageId == -1 || cmndto.ProjMov.StakeHolderId == -1)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "submitcde", false);


                        }
                        else
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "submitcde", true);
                        }
                        return RedirectToAction(nameof(ProjDraftUndo));
                    }

                    else
                    {
                        if (Logins.UserName != null)
                        {
                            TempData["projpsmided"] = cmndto.ProjEdit.CurrentPslmId;
                            TempData.Keep("projpsmided");
                            tbl_Projects project = new tbl_Projects();
                            project = cmndto.ProjEdit;
                            //project.CurrentPslmId = cmndto.ProjEdit.CurrentPslmId;
                            project.IsActive = true;
                            project.EditDeleteDate = DateTime.Now;
                            project.EditDeleteBy = Logins.unitid;
                            project.IsDeleted = false;
                            project.UpdatedByUserId = Logins.unitid;
                            project.Comments = project.InitialRemark;

                            //   add attachment here 

                            if (uploadfile != null && psmid > 0 && cde == 0)
                            {

                                string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                                string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    uploadfile.CopyTo(stream);
                                }

                                tbl_AttHistory atthis = new tbl_AttHistory();
                                atthis.AttPath = uniqueFileName;
                                atthis.ActFileName = uploadfile.FileName;

                                atthis.PsmId = psmid;
                                atthis.UpdatedByUserId = Logins.unitid;
                                atthis.DateTimeOfUpdate = DateTime.Now;
                                atthis.IsDeleted = false;
                                atthis.IsActive = true;
                                atthis.EditDeleteBy = Logins.unitid;
                                atthis.EditDeleteDate = DateTime.Now;
                                atthis.ActionId = 0;
                                atthis.TimeStamp = DateTime.Now;
                                atthis.Reamarks = cmndto.ProjMov.AttRemarks ?? "File Attached";


                                await _attHistoryRepository.AddAttHistoryAsync(atthis);


                            }


                            //  end attachement here




                            await _projectsRepository.UpdateProjectAsync(project);

                            TempData["SuccessMessage"] = "Project Edited!";

                            //  SessionHelper.SetObjectAsJson(HttpContext.Session, "submitcde", true);

                            TempData["Tabshift"] = 12;
                            return Redirect("/Projects/ProjDraftUndo");
                            //return RedirectToAction(nameof(Create));
                        }
                        else
                        {
                            return LocalRedirect("~/Identity/Account/Login");
                        }
                    }
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



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- Bugs Pending  //   used for edit draft proj
        // second tab next clk

        [HttpPost]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> ProjDraftUndo(CommonDTO cmndto, IFormFile uploadfile)
        {
            try
            {

                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                if (Logins != null)
                {
                    if (cmndto.Submitcde == false && cmndto.ProjMov.Comments != null)
                    {

                        cmndto.ProjMov.TimeStamp = DateTime.Now;
                        cmndto.ProjMov.EditDeleteBy = Logins.unitid;
                        cmndto.ProjMov.EditDeleteDate = DateTime.Now;
                        cmndto.ProjMov.IsActive = true;
                        cmndto.ProjMov.IsDeleted = false;
                        cmndto.ProjMov.UpdatedByUserId = Logins.unitid;
                        cmndto.ProjMov.DateTimeOfUpdate = DateTime.Now;
                        cmndto.ProjMov.TostackholderDt = DateTime.Now;

                        if (cmndto.ProjMov.StageId == 1 && cmndto.ProjMov.StatusId == 1 && cmndto.ProjMov.ActionId == 1)
                        {
                            cmndto.ProjMov.CurrentStakeHolderId = 1;
                        }
                        else
                        {
                            cmndto.ProjMov.CurrentStakeHolderId = cmndto.ProjMov.ToStakeHolderId;
                        }
                        await _psmRepository.UpdateProjStakeHolderMovAsync(cmndto.ProjMov);


                        tbl_Comment cmt = new tbl_Comment();
                        cmt = await _commentRepository.GetCommentByPsmIdAsync(cmndto.ProjMov.PsmId);
                        if (cmt != null)
                        {
                            cmt.Comment = cmndto.ProjMov.Comments;
                            await _commentRepository.UpdateCommentAsync(cmt);

                        }



                        if (uploadfile != null && uploadfile.Length > 0)
                        {

                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";
                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            tbl_AttHistory atthis = new tbl_AttHistory();
                            atthis.AttPath = uniqueFileName;
                            atthis.ActFileName = uploadfile.FileName;
                            atthis.PsmId = cmndto.ProjMov.PsmId;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.DateTimeOfUpdate = DateTime.Now;
                            atthis.IsDeleted = false;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.ActionId = 0;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.Reamarks = cmndto.ProjMov.AttRemarks ?? "File Attached";
                            await _attHistoryRepository.AddAttHistoryAsync(atthis);


                        }



                    }

                    else
                    {
                        if (uploadfile != null && uploadfile.Length > 0)
                        {

                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{System.IO.Path.GetExtension(uploadfile.FileName)}";

                            string filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                uploadfile.CopyTo(stream);
                            }

                            tbl_AttHistory atthis = new tbl_AttHistory();
                            atthis.AttPath = uniqueFileName;
                            atthis.ActFileName = uploadfile.FileName;

                            atthis.PsmId = cmndto.ProjMov.PsmId;
                            atthis.UpdatedByUserId = Logins.unitid;
                            atthis.DateTimeOfUpdate = DateTime.Now;
                            atthis.IsDeleted = false;
                            atthis.IsActive = true;
                            atthis.EditDeleteBy = Logins.unitid;
                            atthis.EditDeleteDate = DateTime.Now;
                            atthis.ActionId = 0;
                            atthis.TimeStamp = DateTime.Now;
                            atthis.Reamarks = cmndto.ProjMov.AttRemarks ?? "File Attached";


                            await _attHistoryRepository.AddAttHistoryAsync(atthis);


                        }

                    }
                }
                else
                {


                    return Redirect("/Identity/Account/login");


                }
                cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(cmndto.ProjMov.PsmId);


                ViewBag.tabshift = 12;


                ViewBag.corpsId = 0;
                List<mCommand> cl = new List<mCommand>();
                cl = await _dlRepository.ddlCommand();
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                ViewBag.cl = cl.ToList();
                List<Types> ty = new List<Types>();
                ty = await _dlRepository.ddlType();
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                ViewBag.ty = ty.ToList();

                List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
                List<mAppType> tapp = await _dlRepository.DdlAppType();
                List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                ViewBag.apptype = new SelectList(selectList, "Value", "Text");


                List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");
                cmndto.Submitcde = true;
                cmndto.NextActionDetl = await _projectsRepository.NextActionGet(cmndto.ProjMov.ProjId);

                return View(cmndto);


            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                return Redirect("/Home/Error");
            }

        }





        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- bugs pending  // used for edit draft edit
        // from the grid hit

        [HttpGet]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> ProjDraftUndo(string id, string idx)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                ViewBag.tabshift = 5;

                if (Logins != null)
                {

                    int ids = 0;
                    if (id != null)
                    {
                        string decryptedValue = _dataProtector.Unprotect(id);
                        ids = int.Parse(decryptedValue);
                    }

                    CommonDTO cmdto = new CommonDTO();


                    cmdto.ProjEdit = await _projectsRepository.GetProjectByPsmIdAsync(ids);
                    cmdto.NextActionDetl = await _projectsRepository.NextActionGet(ids);

                    if (cmdto.ProjEdit.ProjId != null)
                    {
                        cmdto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(ids);
                    }
                    tbl_Comment cmt = new tbl_Comment();
                    cmdto.DataProjId = cmdto.ProjEdit.ProjId;

                    if (cmdto.Submitcde == null)
                        cmdto.Submitcde = false;

                    cmdto.ProjMov = await _projectsRepository.GettXNByPsmIdAsync(ids);
                    cmt = await _commentRepository.GetCommentByPsmIdAsync(ids);
                    if (cmt != null)
                    {
                        if (cmt.Comment != null)
                        {
                            cmdto.ProjMov.Comments = cmt.Comment;
                        }
                    }

                    ViewBag.SubmitCde = false;

                    ViewBag.corpsId = 0;
                    List<mCommand> cl = new List<mCommand>();
                    cl = await _dlRepository.ddlCommand();
                    cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                    ViewBag.cl = cl.ToList();
                    List<Types> ty = new List<Types>();
                    ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();

                    List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                    List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                    ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
                    List<mAppType> tapp = await _dlRepository.DdlAppType();
                    List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                    selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                    ViewBag.apptype = new SelectList(selectList, "Value", "Text");


                    List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                    List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                    ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");

                    // return Redirect("/Home/Index");

                    return View(cmdto);
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









        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- Bugs pending

        [HttpGet]

        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Draftedit(string data)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                if (Logins != null)
                {

                    string decodedData = HttpUtility.UrlDecode(data);



                    CommonDTO cmndto = JsonConvert.DeserializeObject<CommonDTO>(decodedData);


                    //CommonDTO cmndto =  new CommonDTO();
                    //cmndto = jsondata;


                    CommonDTO cmndtos = TempData["cmndto"] as CommonDTO;


                    //UnitDdlGet(); projpsmided
                    var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    var watermarkText = $" {ipAddress}\n  {currentDatetime}";
                    TempData["ipadd"] = watermarkText;

                    ViewBag.SubmitCde = false;

                    TempData.Remove("TfrToNext");
                    if (ViewBag.SubmitCde = false)
                        TempData["projpsmid"] = null;


                    ViewBag.corpsId = 0;
                    List<mCommand> cl = new List<mCommand>();
                    cl = await _dlRepository.ddlCommand();
                    cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                    ViewBag.cl = cl.ToList();
                    List<Types> ty = new List<Types>();
                    ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();


                    List<mAppType> tapp = await _dlRepository.DdlAppType();
                    List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                    selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                    ViewBag.apptype = new SelectList(selectList, "Value", "Text");


                    List<UnitDtl> stkholder = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);
                    List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                    ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");


                    List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                    List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                    ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");


                    ViewBag.ProjEdit = string.Empty;

                    if (TempData["projpsmid"] == null)
                        TempData["projpsmid"] = 0;

                    if (TempData["TfrToNext"] == null)
                        TempData["TfrToNext"] = 0;

                    if (TempData["ProjID"] == null)
                        TempData["ProjID"] = 0;
                    if (TempData["projpsmided"] == null)
                        TempData["projpsmided"] = 0;

                    int TfrToNext = (int)TempData["Tabshift"];
                    if ((int)TempData["projpsmided"] > 0)
                    {
                        TempData["projpsmided"] = TempData["projpsmided"];
                        cmndto.Atthistory = _projectsRepository.GetAttachmentsByPsmId((int)TempData["projpsmided"]);
                        TempData["Tabshift"] = 12;

                    }
                    else if (cmndto.ProjEdit != null)
                    {
                        cmndto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(cmndto.ProjEdit.CurrentPslmId);

                    }

                    //cmndto.Projects = await _projectsRepository.GetActSendItemsAsync();
                    //await _projectsRepository.GetActProjectsAsync();

                    TempData["Tabshift"] = 12;
                    ViewBag.Tabshift = 12;
                    ViewBag.SubmitCde = false;
                    TempData["projpsmid"] = 0;
                    TempData["projpsmided"] = cmndto.ProjEdit.CurrentPslmId;

                    //ViewBag.cmndto = cmndto;




                    return View(cmndto);
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



        ///Created by Mr Ajay  and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- revised Bugs Pending


        string filepathpdf = "";
        //[Authorize(Policy = "StakeHolders")]
        public IActionResult WatermarkWithPdf(string id)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {

                //var stream = new FileStream(@"path\to\file", FileMode.Open);
                //return new FileStreamResult(stream, "application/pdf");
                try
                {
                    var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                    //var filePath = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/" + id + "");
                    var filePath = System.IO.Path.Combine(_environment.WebRootPath, "Uploads\\" + id + "");
                    if (System.IO.File.Exists(filePath))
                    {
                        filepathpdf = generate2(filePath, ip);
                    }
                    else
                    {
                        return Content("Something Went Wrong");
                    }


                    aTimer = new System.Timers.Timer(60000);
                    // Hook up the Elapsed event for the timer.
                    aTimer.Elapsed += OnTimer;

                    aTimer.Enabled = true;
                    return Redirect("../../DownloadFile/" + filepathpdf + ".pdf");
                }
                catch (Exception ex)
                {
                    swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                    //Comman.ExceptionHandle(ex.Message);
                    return Json(0);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }


        ///Created by Mr Ajay  
        // 

        public void OnTimer(Object source, ElapsedEventArgs e)
        {

            try
            {
                var filePath1 = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot\\DownloadFile\\" + filepathpdf + ".pdf");

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

        //[Authorize(Policy = "StakeHolders")]
        public string generate2(string Path, string ip)
        {

            try
            {


                Random rnd = new Random();
                string Dfilename = rnd.Next(1, 1000).ToString();
                var filePath1 = System.IO.Path.Combine(_environment.ContentRootPath, "wwwroot\\DownloadFile\\" + Dfilename + ".pdf");
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




        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- Bugs Pending

        [HttpPost]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> ProjCallBack(string id)
        {
            try
            {

                int ids = 0;
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                if (Logins != null)
                {
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "Encid", id);

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "tabshift", 12);

                    if (id != null)
                    {
                        string decryptedValue = _dataProtector.Unprotect(id);
                        ids = int.Parse(decryptedValue);
                    }
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "psmid", ids);
                    //CommonDTO cmdto = new CommonDTO();

                    //   cmdto.ProjEdit = await _projectsRepository.GetProjectByPsmIdAsync(ids);

                    ViewBag.SubmitCde = true;
                    TempData["ProjID"] = null;
                    return Json(new { success = true, message = "Success." });
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


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 18 Nov 23  -- bugs pending

        [HttpGet]
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> ProjCallBack(string idx, string ss)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

                if (idx == null)
                {
                    idx = SessionHelper.GetObjectFromJson<string>(_httpContextAccessor.HttpContext.Session, "Encid").ToString();

                }

                if (Logins != null)
                {

                    int ids = 0;
                    if (idx != null)
                    {
                        string decryptedValue = _dataProtector.Unprotect(idx);
                        ids = int.Parse(decryptedValue);
                    }

                    CommonDTO cmdto = new CommonDTO();

                    cmdto.ProjEdit = await _projectsRepository.GetProjectByPsmIdAsync(ids);


                    if (cmdto.ProjEdit.ProjId != null)
                    {
                        cmdto.Atthistory = await _attHistoryRepository.GetAttHistoryByIdAsync(ids);
                    }
                    tbl_Comment cmt = new tbl_Comment();
                    cmdto.DataProjId = cmdto.ProjEdit.ProjId;

                    cmdto.Submitcde = false;
                    cmdto.ProjMov = await _projectsRepository.GettXNByPsmIdAsync(ids);
                    cmt = await _commentRepository.GetCommentByPsmIdAsync(ids);
                    if (cmt != null)
                    {
                        if (cmt.Comment != null)
                        {
                            cmdto.ProjMov.Comments = cmt.Comment;
                        }
                    }

                    ViewBag.SubmitCde = false;

                    ViewBag.corpsId = 0;
                    List<mCommand> cl = new List<mCommand>();
                    cl = await _dlRepository.ddlCommand();
                    cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                    ViewBag.cl = cl.ToList();
                    List<Types> ty = new List<Types>();
                    ty = await _dlRepository.ddlType();
                    ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                    ViewBag.ty = ty.ToList();

                    List<mHostType> mHostTypes = await _dlRepository.ddlmHostType(0);
                    List<SelectListItem> mHostTyp = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    mHostTyp.AddRange(mHostTypes.Select(item => new SelectListItem { Value = item.HostTypeID.ToString(), Text = item.HostingDesc }));


                    ViewBag.Hostedon = new SelectList(mHostTyp, "Value", "Text");
                    List<mAppType> tapp = await _dlRepository.DdlAppType();
                    List<SelectListItem> selectList = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                    };
                    selectList.AddRange(tapp.Select(item => new SelectListItem { Value = item.Apptype.ToString(), Text = item.AppDesc }));
                    ViewBag.apptype = new SelectList(selectList, "Value", "Text");


                    List<UnitDtl> stkholder = await _dlRepository.ddlUnit();
                    List<SelectListItem> selectLists = new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "--Select--" }
                };
                    selectLists.AddRange(stkholder.Select(item => new SelectListItem { Value = item.unitid.ToString(), Text = item.UnitName }));
                    ViewBag.stkhold = new SelectList(selectLists, "Value", "Text");

                    // return Redirect("/Home/Index");

                    return View(cmdto);
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




    }

}
