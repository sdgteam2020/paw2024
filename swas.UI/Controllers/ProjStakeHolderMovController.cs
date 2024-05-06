using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.BAL.Repository;
using swas.DAL.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;

namespace swas.UI.Controllers
{
  

    public class ProjStakeHolderMovController : Controller
    {
        private readonly IProjStakeHolderMovRepository _psmRepo;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAttHistoryRepository _attHistoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDdlRepository _dlRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataProtector _dataProtector;


        public ProjStakeHolderMovController(IProjStakeHolderMovRepository psmRepo, IProjectsRepository projectsRepository, IAttHistoryRepository attHistoryRepository, IHttpContextAccessor httpContextAccessor, IDdlRepository dlRepository, IWebHostEnvironment ienvironments, IDataProtectionProvider DataProtector)
        {
            _psmRepo = psmRepo;
            _projectsRepository = projectsRepository;
            _attHistoryRepository = attHistoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _dlRepository = dlRepository;
            _environment = ienvironments;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
        }
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // GET: ProjStakeHolderMov
        public async Task<IActionResult> Index()
        {
            var projStakeHolderMovList = await _psmRepo.GetAllProjStakeHolderMovAsync();
            return View(projStakeHolderMovList);
        }
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23

        // GET: ProjStakeHolderMov/Details/5
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Details(int id)
        {
            var projStakeHolderMov = await _psmRepo.GetProjStakeHolderMovByIdAsync(id);
            if (projStakeHolderMov == null)
            {
                return NotFound();
            }

            return View(projStakeHolderMov);
        }
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // GET: ProjStakeHolderMov/Create
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Create()
        {
            var stackhold = await _psmRepo.GetAllProjStakeHolderMovAsync();
            return View(stackhold);
        }

        ///Created by : Sub Maj M Sanal Kumar 11 Nov 23
        // Reviewed Date : 11 Nov 23
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ProcessMail(string Id)
        {
            //**
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                if (Id != null)
                {
                    string decryptedValue = _dataProtector.Unprotect(Id);
                    int ids = int.Parse(decryptedValue);
                    tbl_Projects tbproj = new tbl_Projects();
                    tbproj = await _projectsRepository.GetProjectByPsmIdAsync(ids);
                    //** san
                    Projmove psmove = new Projmove();
                    psmove.ProjMov.ProjId = tbproj.ProjId;
                    psmove.DataProjId = tbproj.ProjId;
                    psmove.ProjMov.StageId = 1;
                    psmove.ProjMov.StatusId = 4;
                    psmove.ProjMov.ActionId = 5;
                    psmove.ProjMov.AddRemarks = "File Accepted by : " + Logins.Unit;
                   
                    psmove.ProjMov.Comments = "Fwd to Other Stakeholders for Comments ";
                    psmove.ProjMov.CurrentStakeHolderId = Logins.unitid??0;
                    psmove.ProjMov.FromStakeHolderId = Logins.unitid ?? 0;
                    psmove.ProjMov.ToStakeHolderId = Logins.unitid ?? 0;
                    //  psmove.ProjMov.CurrentStakeHolderId = Logins.unitid ?? 0;
                   
                    psmove.ProjMov.TostackholderDt = DateTime.Now;
                    psmove.ProjMov.StakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.ActionCde = 2;
                    psmove.ProjMov.ActionDt = DateTime.Now;




                    int psmid = await _psmRepo.AddProjStakeHolderMovAsync(psmove);


                    var stholder = await _psmRepo.GetProjStakeHolderMovByIdAsync(ids);
                    if (stholder.ActionCde == 1)
                    {
                        stholder.ActionDt = DateTime.Now;
                        stholder.ActionCde = 2;


                        await _psmRepo.UpdateProjStakeHolderMovAsync(stholder);

                       
                    }
                    
                    
                    int cnt = await _psmRepo.CountinboxAsync(Logins.unitid ?? 0);

                    Logins.totmsgin = cnt;

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Logins);



                    if (psmid > 0)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
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


        ///Created by : Sub Maj M Sanal Kumar 11 Nov 23
        // Reviewed Date : 11 Nov 23
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> RetDuplicate(string Id)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                if (Id != null)
                {
                    //** san
                    string decryptedValue = _dataProtector.Unprotect(Id);
                    int ids = int.Parse(decryptedValue);
                    tbl_Projects tbproj = new tbl_Projects();
                    tbproj = await _projectsRepository.GetProjectByPsmIdAsync(ids);

                    Projmove psmove = new Projmove();
                    psmove.ProjMov.ProjId = tbproj.ProjId;
                    psmove.DataProjId = tbproj.ProjId;
                    psmove.ProjMov.ToStakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.CurrentStakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.StageId = 1;
                    psmove.ProjMov.StatusId = 31;
                    psmove.ProjMov.ActionId = 8;
                    psmove.ProjMov.AddRemarks = "Returned by : " + Logins.Unit;

                    psmove.ProjMov.Comments = "Not reqd.  Found Duplicate proj ";
                   // psmove.ProjMov.CurrentStakeHolderId = Logins.unitid ?? 0;
                    psmove.ProjMov.FromStakeHolderId = Logins.unitid ?? 0;
                  //  psmove.ProjMov.ToStakeHolderId = Logins.unitid ?? 0;
                    //  psmove.ProjMov.CurrentStakeHolderId = Logins.unitid ?? 0;

                    psmove.ProjMov.TostackholderDt = DateTime.Now;
                    psmove.ProjMov.StakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.ActionCde = 2;
                    psmove.ProjMov.ActionDt = DateTime.Now;




                    int psmid = await _psmRepo.ReturnDuplProjMovAsync(psmove);


                    var stholder = await _psmRepo.GetProjStakeHolderMovByIdAsync(ids);
                    
                        stholder.ActionDt = DateTime.Now;
                        stholder.ActionCde = 999;

                        await _psmRepo.UpdateProjStakeHolderMovAsync(stholder);


                    int cnt = await _psmRepo.CountinboxAsync(Logins.unitid ?? 0);

                    Logins.totmsgin = cnt;

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Logins);



                    if (psmid > 0)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
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




        ///Created by : Sub Maj M Sanal Kumar 11 Nov 23
        // Reviewed Date : 11 Nov 23
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> RetwithObsn(string Id,string? observationComments)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            if (Logins != null)
            {
                if (Id != null)
                {
                    //** san
                    string decryptedValue = _dataProtector.Unprotect(Id);
                    int ids = int.Parse(decryptedValue);
                    tbl_Projects tbproj = new tbl_Projects();
                    tbproj = await _projectsRepository.GetProjectByPsmIdAsync(ids);

                    Projmove psmove = new Projmove();
                    psmove.ProjMov.ProjId = tbproj.ProjId;
                    psmove.DataProjId = tbproj.ProjId;
                    //psmove.ProjMov.ToStakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.CurrentStakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.StageId = 1;
                    psmove.ProjMov.StatusId = 2;
                    psmove.ProjMov.ActionId = 2;
                    psmove.ProjMov.AddRemarks = observationComments + " & Returned by : " + Logins.Unit;

                    psmove.ProjMov.Comments = observationComments;
                    // psmove.ProjMov.CurrentStakeHolderId = Logins.unitid ?? 0;
                    psmove.ProjMov.FromStakeHolderId = Logins.unitid ?? 0;
                    //  psmove.ProjMov.ToStakeHolderId = Logins.unitid ?? 0;
                    //  psmove.ProjMov.CurrentStakeHolderId = Logins.unitid ?? 0;

                    //psmove.ProjMov.TostackholderDt = DateTime.Now;
                    psmove.ProjMov.StakeHolderId = tbproj.StakeHolderId;
                    psmove.ProjMov.ActionCde = 1;
                    psmove.ProjMov.ActionDt = DateTime.Now;



                    int psmid = await _psmRepo.RetWithObsnMovAsync(psmove);


                    var stholder = await _psmRepo.GetProjStakeHolderMovByIdAsync(ids);

                    stholder.ActionDt = DateTime.Now;
                    stholder.ActionCde = 2;

                    await _psmRepo.UpdateProjStakeHolderMovAsync(stholder);


                    int cnt = await _psmRepo.CountinboxAsync(Logins.unitid ?? 0);

                    Logins.totmsgin = cnt;

                    SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Logins);



                    if (psmid > 0)
                    {
                        return Json(1);
                    }
                    else
                    {
                        return Json(0);
                    }
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



        //[Authorize(Policy = "StakeHolders")]
        [HttpPost]

        public async Task<IActionResult> FwdCreate(Projmove projStakeHolderMov, IFormFile uploadfile)
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
                        _projectsRepository.UpdateProjectAsync(Tbproj);
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
                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
                            

                            string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


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

                        //start
                        psmid = await _psmRepo.AddProjStakeHolderMovAsync(projStakeHolderMov);




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


                        ViewBag.PsmId = psmid;
                        ViewBag.DataProjId = projStakeHolderMov.DataProjId;

                        // TempData["SuccessMessage"] = "Please note down the Proj Code for Further Enquiries.. (Fwd & Ready for Attach more Docu)...";

                        // end

                        return RedirectToAction("ProjHistory", "Projects", encryModel);
                        // 1 time
                        // Process the form data
                        ViewBag.SubmitCde = true; // for subsequent submissions

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
                            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
                            //string filePath = Path.Combine(fullUrl, uniqueFileName);
                            string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);


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

                            _attHistoryRepository.AddAttHistoryAsync(atthist);



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

                    Response.WriteAsJsonAsync(ex.Message.ToString());
                    await _attHistoryRepository.AddAttHistoryAsync(atthist);
                    Console.WriteLine(ex.Message);
                    return RedirectToAction("ProjHistory", "Projects");
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }

            // return View("ProjHistory", projHist);
        }


        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // POST: ProjStakeHolderMov/Create
        //[Authorize(Policy = "StakeHolders")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Projmove formData, [FromForm] IFormFile uploadfile)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            if (Logins != null)
            {
                string webRootPath = _environment.WebRootPath;
            //string fileName = "";
            //string virtualPath = "";

            string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;
            string fullUrl = $"{scheme}://{host}/Uploads/";
            //var uploadsFolderPath = Path.Combine(fullUrl);

            string uniqueFileName = $"{"Swas"}_{Guid.NewGuid()}{Path.GetExtension(uploadfile.FileName)}";
            //string filePath = Path.Combine(fullUrl, uniqueFileName);
            string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/Uploads/", uniqueFileName);

          
            if (Logins != null)
            {
               


                tbl_AttHistory atthis = new tbl_AttHistory();
                if (uploadfile != null && uploadfile.Length > 0)
                {

                  

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadfile.CopyTo(stream);
                    }
                    atthis.ActionId = 0;
                    atthis.AttPath = uniqueFileName;
                    atthis.ActFileName = uploadfile.FileName;
                    atthis.Reamarks = formData.ProjMov.AttRemarks;
                    atthis.PsmId = formData.ProjMov.PsmId;
                    atthis.UpdatedByUserId = Logins.unitid;
                    atthis.DateTimeOfUpdate = DateTime.Now;
                    atthis.IsDeleted = false;
                    atthis.IsActive = true;
                    atthis.EditDeleteBy = Logins.unitid;
                    atthis.EditDeleteDate = DateTime.Now;
                    atthis.TimeStamp = DateTime.Now;
                    //formData.Atthistory[0].AttPath = "Sanal";
                    await _attHistoryRepository.AddAttHistoryAsync(atthis);
                    TempData["SuccessMessage"] = "Docu Attached...";
                }






                //TempData["SuccessMessage"] = "Please note down the Proj Code for Further Enquiries.. (Fwd & Ready for Attach more Docu)...";
                ViewBag.SubmitCde = true;
                TempData["Tabshift"] = 3;
                // end
                ViewBag.ProjID = formData.DataProjId ?? 0;

               
                    CommonDTO dtoS = new CommonDTO();
                    tbl_Projects proedit = new tbl_Projects();
                    dtoS.ProjEdit = proedit;
                    //dtoS.Projects = projectss;
                    dtoS.ProjEdit.ProjId = formData.ProjMov.ProjId;
                    dtoS.ProjEdit.CurrentPslmId = formData.ProjMov.PsmId;
                    dtoS.ProjEdit.ProjCode = formData.ProjMov.ProjCode;
                

                    //List<tbl_AttHistory> atthistory = new List<tbl_AttHistory>();
                    Projmove pmove = new Projmove();

                   
                    
                    TempData["TfrToNext"] = formData.ProjMov.PsmId;
                    TempData["ProjID"] = formData.ProjMov.ProjId;
                    Thread.Sleep(3000);
                    TempData.Keep("TfrToNext");
                    TempData.Keep("ProjID");

                    return RedirectToAction("Create", "Projects", dtoS);

               

            }
            else
            {
                return RedirectToAction("NewProject", "Home");
            }


            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
            // return View("ProjHistory", projHist);
        }


        //   // return View("ProjHistory", projHist);
        //}
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // GET: ProjStakeHolderMov/Edit/5
        //[Authorize(Policy = "StakeHolders")]
        public async Task<IActionResult> Edit(int id)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                var projStakeHolderMov = await _psmRepo.GetProjStakeHolderMovByIdAsync(id);
                if (projStakeHolderMov == null)
                {
                    return NotFound();
                }
                return View(projStakeHolderMov);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // POST: ProjStakeHolderMov/Edit/5
        //[Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_ProjStakeHolderMov projStakeHolderMov)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                if (id != projStakeHolderMov.PsmId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _psmRepo.UpdateProjStakeHolderMovAsync(projStakeHolderMov);
                return RedirectToAction(nameof(Index));
            }

            return View(projStakeHolderMov);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // GET: ProjStakeHolderMov/Delete/5
        //[Authorize(Policy = "StakeHolders")]
        
        public async Task<IActionResult> Delete(int id)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                var projStakeHolderMov = await _psmRepo.GetProjStakeHolderMovByIdAsync(id);
            if (projStakeHolderMov == null)
            {
                return NotFound();
            }
            return View(projStakeHolderMov);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
        ///Created by : Sub Maj M Sanal Kumar
        // Reviewed Date : 30 Jul 23
        // POST: ProjStakeHolderMov/Delete/5
        //[Authorize(Policy = "StakeHolders")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {

                await _psmRepo.DeleteProjStakeHolderMovAsync(id);
            return RedirectToAction(nameof(Index));
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }















    }

}
