
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swas.BAL;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.BAL.Utility;
using swas.DAL;
using swas.DAL.Models;
using System.Security.Cryptography;
using System.Text;

namespace swas.UI.Controllers
{
    [Authorize]
    public class UnitDtlsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _dataProtector;
        private readonly IUnitRepository _unitRepository;
        private readonly IDdlRepository _DdlRepostory;
        private readonly IUnitStatusMapping _unitStatusMapping;
        private readonly IStatusActionsMapping _statusActionsMapping;

        public UnitDtlsController(ApplicationDbContext context, IDataProtectionProvider DataProtector,
            IUnitRepository unitRepository, IDdlRepository ddlRepository,
            IUnitStatusMapping unitStatusMapping, IStatusActionsMapping statusActionsMapping)

        {
            _context = context;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.UnitDtlsController");
            _unitRepository = unitRepository;
            _DdlRepostory = ddlRepository;
            _unitStatusMapping = unitStatusMapping;
            _statusActionsMapping = statusActionsMapping;
        }



        public async Task<IActionResult> Index()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {

                return RedirectToAction("AddOrEdit");
            }

            else
                return Redirect("/Identity/Account/Login");
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int result)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {

                ViewBag.corpsId = 0;
                List<mCommand> cl = new List<mCommand>();
                cl = await _DdlRepostory.ddlCommand();
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                ViewBag.cl = cl.ToList();


                List<Types> ty = new List<Types>();
                ty = await _DdlRepostory.ddlType();
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                ViewBag.ty = ty.ToList();
                List<tbl_mStatus> status = new List<tbl_mStatus>();
                var test = from st in _context.mStatus
                           join s in _context.mStages on st.StageId equals s.StagesId
                           select new StatusStageDTO
                           {
                               StatusId = st.StatusId,
                               StageId = st.StageId,
                               Status = st.Status,
                               Stages = s.Stages
                           };

                var resultList = await test.ToListAsync();

                ViewBag.StatusData=resultList;




                return View();
              
            }

            else
                return Redirect("/Identity/Account/Login");

        }

        public async Task<List<mCorps>> GetCorpsOptions(int commandId)
        {
            List<mCorps> udtl = new List<mCorps>();
            udtl = await _DdlRepostory.ddlCorps(commandId);

            return udtl.ToList();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(UnitDtl UnitData)
        {

            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            List<UnitDtl> udtl = new List<UnitDtl>();

            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            if (Logins.IsNotNull())
            {
                UnitData.UpdatedBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
                UnitData.UpdatedDate = DateTime.Now;


                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] hashedBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(UnitData.UnitSusNo));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hashedBytes.Length; i++)
                    {
                        builder.Append(hashedBytes[i].ToString("x2"));
                    }
                    UnitData.UnitSusNo = builder.ToString();
                }

                int result = await _unitRepository.Save(UnitData);

                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Unit successfully created!";
                }
                else
                {
                    TempData["FailureMessage"] = "Unit Already Exist... Check Unit Name!";
                }

                udtl = await _unitRepository.GetAllUnitAsync();

                ViewBag.corpsId = 0;
                List<mCommand> cl = new List<mCommand>();
                cl = await _DdlRepostory.ddlCommand();
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                ViewBag.cl = cl.ToList();

                List<Types> ty = new List<Types>();
                ty = await _DdlRepostory.ddlType();
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                ViewBag.ty = ty.ToList();

                if (UnitData.Updatecde == 1)
                {

                    return View(udtl);
                }
                else
                {
                    return View(udtl);
                }
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
        }

        private string Encode(string input)
        {
            string inputString = input.ToString();
            StringBuilder encoded = new StringBuilder();

            foreach (char c in inputString)
            {
                char alphabeticalChar = (char)(c + 'A' - '0'); // Convert digit character to alphabetical character
                encoded.Append(alphabeticalChar);
            }

            return encoded.ToString();
        }


        public async Task<IActionResult> AddOrEditCREATEPROJ(UnitDtl UnitData)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            List<UnitDtl> udtl = new List<UnitDtl>();

            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            if (Logins.IsNotNull())

            {

                UnitData.UpdatedBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
                UnitData.UpdatedDate = DateTime.Now;
                int result = await _unitRepository.Save(UnitData);

                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Unit successfully created!";
                }
                else
                {

                    TempData["FailureMessage"] = "Unit Already Exist... Check SUS No or Unit Name!";
                }
                udtl = await _unitRepository.GetAllUnitAsync();

                ViewBag.corpsId = 0;
                List<mCommand> cl = new List<mCommand>();
                cl = await _DdlRepostory.ddlCommand();
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                ViewBag.cl = cl.ToList();


                List<Types> ty = new List<Types>();
                ty = await _DdlRepostory.ddlType();
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                ViewBag.ty = ty.ToList();

                if (UnitData.Updatecde == 1)
                {
                    return Redirect("../Projects/Create");
                }
                else
                {

                    return View(udtl);
                }

            }

            else
            {
                return Redirect("/Identity/Account/Login");
            }

        }



        public async Task<IActionResult> AddOrEdit2FORUNIT(UnitDtl UnitData)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            List<UnitDtl> udtl = new List<UnitDtl>();

            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
            if (Logins.IsNotNull())

            {

                UnitData.UpdatedBy = Logins.ActualUserName + "(" + Logins.Unit + ")";
                UnitData.UpdatedDate = DateTime.Now;
                int result = await _unitRepository.Save(UnitData);

                if (result == 1)
                {
                    TempData["SuccessMessage"] = "Unit successfully created!";
                }
                else
                {

                    TempData["FailureMessage"] = "Unit Already Exist... Check SUS No or Unit Name!";
                }
                udtl = await _unitRepository.GetAllUnitAsync();

                ViewBag.corpsId = 0;
                List<mCommand> cl = new List<mCommand>();
                cl = await _DdlRepostory.ddlCommand();
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                ViewBag.cl = cl.ToList();


                List<Types> ty = new List<Types>();
                ty = await _DdlRepostory.ddlType();
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                ViewBag.ty = ty.ToList();

                if (UnitData.Updatecde == 1)
                {
                    return Redirect("../Home/NewProject");
                }
                else
                {

                    return View(udtl);
                }

            }

            else
            {
                return Redirect("/Identity/Account/Login");
            }

        }

        public int CheckNameExist(UnitDtl unit)
        {
            return 1;
        }

        public int CheckNameExistID(UnitDtl unit)
        {
            return 1;

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string? DelID)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {
                return RedirectToAction("AddOrEdit");
            }
            else
                return Redirect("/Identity/Account/Login");
        }
        [HttpPost]
        public async Task<IActionResult> GetMappingByUnitId(int StageId,int StatusId)
        {


            var unitmap = await _unitRepository.GetallUnitwithmap1(StageId , StatusId);

            ViewBag.unitmapww = unitmap;


            return Json(unitmap);
        }
        #region  unitmapping for unitstatus ans action mapping
        public async Task<IActionResult> AddMapping(DTOUnitMapping data)
        {
            try
            {
                TrnStatusActionsMapping statusActionsMapping = new TrnStatusActionsMapping
                {
                    ActionsId = data.ActionsId,
                    StatusId = data.SubStagesId,
                    StatusActionsMappingId = data.StatusActionsMappingId
                };


                var existingStatusActionsMapping = await _statusActionsMapping.GetByActionsAndStatusAsync(data.StatusActionsMappingId, data.ActionsId, data.SubStagesId);
                if (existingStatusActionsMapping != null)
                {
                    return Json(new { message = "StatusActionsMapping data is already in the table" });
                }

                
                if (statusActionsMapping.StatusActionsMappingId == 0)
                {
                    var ret1 = await _statusActionsMapping.AddWithReturn(statusActionsMapping);
                }
                else
                {
                    var ret1 = await _statusActionsMapping.UpdateWithReturn(statusActionsMapping);
                }

                return Json(nmum.Save);
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }


        #endregion

        #region Delete For UnitMapping and Action Mapping

        public async Task<IActionResult> DeleteMapping(DTOUnitMapping data)
        {
            try
            {
                if (data.StatusActionsMappingId != null)
                {
                    var ret1 = await _statusActionsMapping.Delete(data.StatusActionsMappingId);
                }
                else
                {
                    Console.WriteLine("StatusAction is not in the table.");
                }


                if (data.UnitStatusMappingId != null)
                {
                    var ret1 = await _unitStatusMapping.Delete(data.UnitStatusMappingId);
                }
                else
                {
                    Console.WriteLine("UnitStatus is not in the table.");
                }

                return Json(nmum.Save);
            }
            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }

        #endregion
    }
}
