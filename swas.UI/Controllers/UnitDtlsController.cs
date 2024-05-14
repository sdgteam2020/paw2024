﻿    
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

    //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class UnitDtlsController : Controller
    {

        private readonly ApplicationDbContext _context;
               private readonly IDataProtector _dataProtector;
        private readonly IUnitRepository _unitRepository;
        private readonly IDdlRepository _DdlRepostory;

        public UnitDtlsController(ApplicationDbContext context, IDataProtectionProvider DataProtector, IUnitRepository unitRepository, IDdlRepository ddlRepository)
        
        {
            _context = context;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.UnitDtlsController");
            _unitRepository = unitRepository;
            _DdlRepostory = ddlRepository;
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
        public async Task<IActionResult> AddOrEdit(string? Id)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {

                ViewBag.corpsId = 0;
                List <mCommand> cl = new List<mCommand>();
                
                //---------------Getting Data From Database Using EntityFrameworkCore----------------------
                cl =  await _DdlRepostory.ddlCommand();

                //-------------------Inserting Select Item in List-------------------------
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                //--------------Assigning categorylist to ViewBag.ListofCategory --------------------------
                ViewBag.cl = cl.ToList();


                List<Types> ty = new List<Types>();

                //---------------Getting Data From Database Using EntityFrameworkCore----------------------
                ty = await _DdlRepostory.ddlType();

                //-------------------Inserting Select Item in List-------------------------
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                //--------------Assigning categorylist to ViewBag.ListofCategory --------------------------
                ViewBag.ty = ty.ToList();


                List<UnitDtl> udtl = new List<UnitDtl>();

                udtl = await _unitRepository.GetAllUnitAsync();

                return View(udtl);
              
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
        //

        //AddOrEdit Post Method

        
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
                    //return Redirect("AddOrEdit");

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

        //private int Decode(string input)
        //{
        //    StringBuilder decoded = new StringBuilder();
        //    foreach (char c in input)
        //    {
        //        int digitValue = c - 'A' + '0';
        //        decoded.Append((char)digitValue);
        //    }
        //    int decodedValue;
        //    int.TryParse(decoded.ToString(), out decodedValue);
        //    return decodedValue;
        //}


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

                //---------------Getting Data From Database Using EntityFrameworkCore----------------------
                cl = await _DdlRepostory.ddlCommand();

                //-------------------Inserting Select Item in List-------------------------
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                //--------------Assigning categorylist to ViewBag.ListofCategory --------------------------
                ViewBag.cl = cl.ToList();


                List<Types> ty = new List<Types>();

                //---------------Getting Data From Database Using EntityFrameworkCore----------------------
                ty = await _DdlRepostory.ddlType();

                //-------------------Inserting Select Item in List-------------------------
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                //--------------Assigning categorylist to ViewBag.ListofCategory --------------------------
                ViewBag.ty = ty.ToList();

                if (UnitData.Updatecde == 1)
                {
                    return Redirect("../Projects/Create");

                    //return View("../Projects/Create", UnitData);
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

                //---------------Getting Data From Database Using EntityFrameworkCore----------------------
                cl = await _DdlRepostory.ddlCommand();

                //-------------------Inserting Select Item in List-------------------------
                cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });
                //--------------Assigning categorylist to ViewBag.ListofCategory --------------------------
                ViewBag.cl = cl.ToList();


                List<Types> ty = new List<Types>();

                //---------------Getting Data From Database Using EntityFrameworkCore----------------------
                ty = await _DdlRepostory.ddlType();

                //-------------------Inserting Select Item in List-------------------------
                ty.Insert(0, new Types { Id = 0, Name = "--Select--" });
                //--------------Assigning categorylist to ViewBag.ListofCategory --------------------------
                ViewBag.ty = ty.ToList();

                if (UnitData.Updatecde == 1)
                {
                    return Redirect("../Home/NewProject");

                    //return View("../Projects/Create", UnitData);
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
            //var result = _context.tbl_mUnitBranch.Select(p => p.UnitName.ToUpper() == unit.UnitName.ToUpper() && p.Id != unit.Id).ToList();
            //if (result.Contains(true))
                return 1;
            //else
            //    return 0;
        }

        public int CheckNameExistID(UnitDtl unit)
        {
            //var result = _context.tbl_mUnitBranch.Select(p => p.UnitName.ToUpper() == unit.UnitName.ToUpper() && p.Id != unit.Id).ToList();
            //if (result.Contains(true))
                return 1;
            //else
            //    return 0;

        }
     
        [HttpPost]
        public async Task<IActionResult> Delete(string? DelID)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {
                //try
                //{
                //    if (DelID != null)
                //    {
                //        int Idd = int.Parse(_dataProtector.Unprotect(DelID.ToString()));

                //        var UnitDtl = await _context.tbl_mUnitBranch.FindAsync(Idd);
                //        _context.tbl_mUnitBranch.Remove(UnitDtl);
                //        await _context.SaveChangesAsync();
                //        TempData["Status"] = nmum.Delete;
                //        return RedirectToAction(nameof(AddOrEdit));
                //    }
                //    else
                //    {
                //        TempData["Status"] = nmum.Null;
                //        return RedirectToAction("AddOrEdit");
                //    }

                //}
                //catch (Exception ex)
                //{
                //    TempData["Status"] = nmum.Exception;
                //    return RedirectToAction("AddOrEdit");
                //}
                return RedirectToAction("AddOrEdit");
            }
            else
                return Redirect("/Identity/Account/Login");
        }
        [HttpPost]
        public async Task<IActionResult> GetAllStakeHolderComment(int Id)
        {
            return Json(await _unitRepository.GetAllStakeHolderComment());
        }

    }
}
