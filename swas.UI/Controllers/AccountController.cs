using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swas.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Mvc.Rendering;
using swas.DAL.Models;
using swas.DAL;
using swas.BAL.Helpers;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.BAL;
using Newtonsoft.Json;


using swas.BAL.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using iText.Commons.Actions.Contexts;
using iText.Layout.Renderer;
using OneLogin.Saml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Options;
using System.Configuration;
using Newtonsoft.Json;

namespace swas.UI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        public readonly ApplicationDbContext _context;
        private readonly IUnitRepository _unitRepository;
        private readonly ILogger<LoginModel> _logger;
        private readonly Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser> _userStore;
        private readonly Microsoft.AspNetCore.Identity.IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IRankRepository _rankRepository;

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
       
        public AccountController(Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser> userStore, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IUnitRepository unitRepository, ILogger<LoginModel> logger, IRankRepository rankRepository, IUserRepository userRepository,
        IConfiguration configuration)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _unitRepository = unitRepository;
            _logger = logger;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _rankRepository = rankRepository;
                _userRepository = userRepository;
                _configuration = configuration;
            
        }


        private Microsoft.AspNetCore.Identity.IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (Microsoft.AspNetCore.Identity.IUserEmailStore<ApplicationUser>)_userStore;
        }
        public IActionResult Register()
        {


            return Redirect("/Identity/Account/Register");


        }

        public IActionResult PasswordChange()
        {


            return Redirect("/Identity/Account/PasswordChange");


        }

        [Authorize(Policy = "Admin")]
        public IActionResult ResetPassword()
        {


            return Redirect("/Identity/Account/ResetPassword");


        }


        [Authorize(Policy = "Admin")]

        public async Task<IActionResult> GetAllUsers()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;
            ApplicationUser inputModel = new ApplicationUser();

            var users = await userManager.Users.ToListAsync();
            foreach (var v in users)
            {
                inputModel = new ApplicationUser();
                var roles = await userManager.GetRolesAsync(v);

                var list = (from a in userManager.Users
                            join b in _roleManager.Roles on a.RoleName equals b.Id


                            select new ApplicationUser()
                            {

                                UserName = a.UserName,
                                appointment = a.appointment,
                                RoleName = b.Name,
                                CreatedDate = DateTime.Now,

                            }
              ).ToList();

                ViewBag.data = list;

            }

            return View(inputModel);

        }
        public async Task<IActionResult> SetValue(string selectedValue)
        {
            TempData["SelectedValue"] = selectedValue; // Store the value in TempData or ViewBag as per your requirement
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            string psskey = _configuration.GetValue<string>("TestCredentials")??"";

            var result = await signInManager.PasswordSignInAsync(selectedValue, psskey, true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                ApplicationUser userdet = new ApplicationUser();
                IdentityRole ss = new IdentityRole();
                int cla = await _unitRepository.GetIdCalendar();
                var userId = userManager.GetUserId(User);
                userdet = await userManager.FindByNameAsync(selectedValue);
                var unitget = await _unitRepository.GetUnitDtl(userdet.unitid);
                var usrole = await userManager.GetRolesAsync(userdet);
                await Task.Delay(1000);

                Login Dbs = new Login();
                Dbs.Unit = unitget.UnitName;
                Dbs.Comdid = unitget.unitid;
                Dbs.Corpsid = unitget.CorpsId;
                Dbs.UserName = userdet.UserName;
                Dbs.UserName = selectedValue;
                Dbs.Role = usrole[0].ToString();
                Dbs.Iamuserid = userdet.domain_iam;
                Dbs.cla = cla;
                Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");
                Dbs.ActualUserName = Logins.ActualUserName;

                HttpContext.Session.Clear();
                HttpContext.Session.CommitAsync().GetAwaiter().GetResult();

                Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";


                SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Dbs);
                return RedirectToAction("Index", "Home");
            }


            else
            {


                return RedirectToAction("Index", "Home");


            }





        }
        [AllowAnonymous]
        public IActionResult Logins()
        {


            return Redirect("/Identity/Account/login");

        }
        public class Log
        {
            public string NameId { get; set; }
            public string SAMLRole { get; set; }
        }

       

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            String EncryptedResponse = "";
            EncryptedResponse = Request.Form["SAMLResponse"];
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            if (!string.IsNullOrEmpty(EncryptedResponse))

            {
                 string psskey = _configuration.GetValue<string>("CertCredentials") ?? "";

                string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\applwhitelisting.army.mil.p12", psskey);

                AccountSettings accountSettings = new AccountSettings();
                OneLogin.Saml.Response samlResponse = new Response(accountSettings);

                samlResponse.LoadXmlFromBase64(decryptedsamlresponse);

                if (samlResponse.IsValid_sign())
                {
                    Log log = new Log();
                    log.NameId = samlResponse.GetNameID();
                    log.SAMLRole = samlResponse.GetSAMLRole();

                    if (log.NameId != null)
                    {
                        HttpContext.Session.SetString("NameId", log.NameId);
                        HttpContext.Session.SetString("SAMLRole", log.SAMLRole);

                        TempData["NameId"] = log.NameId;
                        TempData["RoleId"] = log.SAMLRole;
                       
                        var result = await signInManager.PasswordSignInAsync(log.NameId.ToLower(), psskey, false, lockoutOnFailure: true);
                        if (result.Succeeded)  ///   registered user
                        {
                            ApplicationUser userdet = new ApplicationUser();
                            IdentityRole ss = new IdentityRole();
                            _logger.LogInformation("User logged in.");

                            userdet = await userManager.FindByNameAsync(log.NameId);
                            var unitdetl = await _unitRepository.GetUnitDtl(userdet.unitid);
                            CommonHelper commonHelper = new CommonHelper(_context);
                            var userRank = commonHelper.UserRankDetail(userdet);

                            Login Db = new Login();
                            if (userdet.domain_iam != null)   //   domain_iam available after registration
                            {
                                Db.UserName = userdet.UserName;
                                Db.Comdid = unitdetl.unitid;
                                Db.Corpsid = unitdetl.CorpsId;
                                Db.Iamuserid = userdet.domain_iam;
                                Db.Unit = unitdetl.UnitName;
                                Db.unitid = userdet.unitid;
                                Db.UserIntId = userdet.UserIntId;
                                Db.Rank = userRank;
                                Db.IcNo = userdet.Icno;
                                Db.Offr_Name = userdet.Offr_Name;
                                Db.IpAddress = watermarkText;
                                var usrole = await userManager.GetRolesAsync(userdet);
                                Db.Role = usrole[0].ToString();

                                if (Db.ActualUserName == null)
                                {
                                    Db.ActualUserName = log.NameId;
                                }

                                try
                                {
                                    tbl_LoginLog logs = new tbl_LoginLog();
                                    var Role = await userManager.GetRolesAsync(userdet);
                                    logs.UserId = userdet.UserIntId;
                                    logs.IP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                                    logs.IsActive = true;
                                    logs.Updatedby = userdet.unitid;
                                    logs.UpdatedOn = DateTime.Now;
                                    logs.logindate = DateTime.Now;
                                    logs.userName = userdet.UserName;
                                    logs.unitid = userdet.unitid;
                                    await _userRepository.Add(logs);
                                }

                                catch (Exception ex)
                                {
                                    Console.WriteLine("THE Print is 1");
                                }


                            }


                            else  //   without registration users
                            {

                                Db.UserName = log.NameId;   // IAM ID
                                Db.ActualUserName = log.NameId; //  IAM ID
                                Db.Role = "Guest";

                            }
                            int cla = await _unitRepository.GetIdCalendar();
                            Db.cla = cla;
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Db);



                            if (Db.Role == "Dte")
                            {
                                return RedirectToAction("NewProject", "Home");
                            }
                            else
                            {
                                return RedirectToAction("NewProject", "Home");
                            }
                        }
                        else
                        {
                            if (log.NameId != null)
                            {
                                TempData["UserName"] = log.NameId;
                                HttpContext.Session.SetString("UserName", log.NameId);
                                return RedirectToAction("NewProject", "Home");
                            }

                        }
                    }
                    else
                    {

                        return RedirectToAction("NewProject", "Home");
                    }
                }
                else
                {
                    Response.Redirect("https://iam2.army.mil/IAM/User", true);
                }
            }
            else
            {
                Response.Redirect("https://iam2.army.mil/IAM/User", true);
            }
            return RedirectToAction("UnAuthUser", "Account");
        }



        [AllowAnonymous]
        public void LogoutRequesttoIAM(String role, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();
            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

            string ReuestXML = req.SingleLogoutRequest(AuthRequest.AuthRequestFormat.Base64, entityid, role, usernam);
            Response.Redirect("https://iam2.army.mil/IAM/singleAppLogout?SAMLRequest=" + HttpUtility.UrlEncode(ReuestXML), true);

        }


        [AllowAnonymous]
        public string DecryptSAmlResponseNew(string Encryptedtext, string certificatepath, string password)
        {

            string result = "True";
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("alpha");

                String[] spearator = { Convert.ToBase64String(plainTextBytes) };
                String[] newstring = Encryptedtext.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                string key = newstring[1].ToString();
                string plain = newstring[0].ToString();
                #region decryptkeyusingprivatekey
                try
                {
                    byte[] byteData = Convert.FromBase64String(key);
                    byte[] decryptedkey = new byte[16];
                    X509Certificate2 myCert2 = null;
                    RSACryptoServiceProvider rsa = null;

                    try
                    {
                        string psskey = _configuration.GetValue<string>("CertCredentials") ?? "";


                        myCert2 = new X509Certificate2(@"C:\\Cert\\App Certificate\\acms.army.mil.pfx", psskey);
                        #region test
                        using (RSA rs = myCert2.GetRSAPrivateKey())
                        {
                            decryptedkey = rs.Decrypt(byteData, RSAEncryptionPadding.Pkcs1);

                        }
                        #endregion
                    }
                    catch (Exception e)
                    {

                    }
                    byte[] iv = new byte[16];


                    byte[] iv1 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                    result = DecryptString0705222_Final(plain, decryptedkey, iv1);
                }
                catch (Exception exxx)
                {
                    result = exxx.Message;
                }
                #endregion

            }
            catch (Exception exx)
            {
                result = exx.Message;
            }

            return result;
        }

        [AllowAnonymous]
        private string DecryptString0705222_Final(string cipherText, byte[] key, byte[] iv)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return string.Empty;

            if (key == null || key.Length < 16)
                throw new ArgumentException("Key must be at least 16 bytes.", nameof(key));

            if (iv == null || iv.Length != 16)
                throw new ArgumentException("IV must be exactly 16 bytes (AES block size).", nameof(iv));

            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // AES supports 16/24/32 byte keys. If you only have more/less, normalize safely.
            byte[] aesKey;
            if (key.Length == 16 || key.Length == 24 || key.Length == 32)
                aesKey = key;
            else
            {
                aesKey = new byte[16];
                Buffer.BlockCopy(key, 0, aesKey, 0, 16); // keep your original behavior
            }

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;              // ✅ NOT ECB
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = aesKey;
            aes.IV = iv;

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            byte[] plainBytes = memoryStream.ToArray();

            // ✅ Use UTF8 unless you KNOW plaintext is strictly ASCII
            return Encoding.UTF8.GetString(plainBytes);
        }

        [AllowAnonymous]
        public IActionResult FinalLogout()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult RoleNotAuth()
        {
            return View();
        }

        public IActionResult UnAuthUser()
        {
            return View();
        }
        [AllowAnonymous]
        public void SendResponseToIAM(String issueurl, string entityid, string usernam)
        {
            AccountSettings accountSettings = new AccountSettings();

            OneLogin.Saml.AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);
            string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");


            Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);


        }
        [HttpPost]
        public async Task<IActionResult> AddlTask(string UserName, string UserName2)
        {

            var user = await this.userManager.FindByNameAsync(UserName2);


            if (user != null)
            {
                user.domain_iam = UserName;
            }


            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Console.WriteLine("Successfully updated ");
            }
            return RedirectToAction("AddlTask");
        }

        public IActionResult AddlTask()
        {

            List<ApplicationUser> users = userManager.Users.ToList();

            List<SelectListItem> dropdownOptions = users.Select(u => new SelectListItem
            {
                Value = u.UserName,
                Text = u.UserName
            }).ToList();
            ViewBag.DropdownOptions = dropdownOptions;
            ViewBag.DropdownOptions1 = dropdownOptions;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetAddlTask(string UserName, string UserName2)
        {


            var user = await this.userManager.FindByNameAsync(UserName2);


            if (user != null)
            {
                user.domain_iam = null;
            }


            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Console.WriteLine("Successfully updated ");
            }
            else
            {
                return Json("Failed");
            }

            return Json("Success");
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            foreach (var cookie in Request.Cookies.Keys)      // clear all cookies
            {
                Response.Cookies.Delete(cookie);
            }
            Login Db = new Login();
            SessionHelper.ClaerObjectAsJson(HttpContext.Session, "User");

            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            else
            {
                return RedirectToAction("FinalLogout", "Account");
            }
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetsAllUsers()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;
            var users = await userManager.Users.ToListAsync();

            Users inputModel = new Users();
            try
            {
                foreach (var v in users)
                {
                    inputModel = new Users();
                    var roles = await userManager.GetRolesAsync(v);

                    var list = (from a in userManager.Users
                                join d in _context.mRank on a.Rank equals d.Id
                                join c in _context.UserRoles on a.Id equals c.UserId
                                join b in _roleManager.Roles on c.RoleId equals b.Id
                                orderby a.CreatedDate descending
                                select new Users
                                {
                                    RankId = d.Id,
                                    UserName = a.UserName,
                                    UpdatedBy = a.unitid,
                                    UnitId = a.unitid,
                                    RoleName = b.Name,
                                    CreatedDate = a.CreatedDate,
                                    Flag = a.Flag,
                                }).ToList();

                    ViewBag.data = list;
                    ViewBag.Watermark = watermarkText;
                }
            }
            catch (Exception ex)
            {
                string ss = ex.InnerException.Message;
            }
            return View(inputModel);
        }



        [HttpPost]
        public async Task<IActionResult> EditUser(Users model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles.FindAsync();
            if (roles == null)
            {
                return NotFound();
            }

            await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
            await userManager.AddToRoleAsync(user, roles.Name);


            return RedirectToAction("Index", "Home");
        }



        public async Task<IActionResult> UserDelete(string UserName, string RoleName)
        {
            try
            {
                var user = await userManager.FindByNameAsync(UserName);

                if (user == null)
                {
                    return NotFound();
                }

                var result1 = await userManager.RemoveFromRoleAsync(user, RoleName);

                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {

                    TempData["SuccessMessage"] = "User Delete successfully!";
                    return RedirectToAction("GetsAllUsers", "Account");
                }
                else
                {
                    TempData["FailureMessage"] = "Not Deleted";
                    return RedirectToAction("GetsAllUsers", "Account");
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "UserDelete");

                // Log full exception details on server
                _logger.LogError(eventId, ex, "Error in UserDelete (AccountConfirmed).");

                // Provide a non-sensitive id the user/admin can reference
                var errorId = HttpContext?.TraceIdentifier ?? dynamicEventId.ToString();

                // Return generic error (no ex.Message)
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred. Please try again or contact the administrator.",
                    errorId
                });
            }

        }


        public async Task<IActionResult> AllUsersEdit(string UserName, string RoleName, int RankId)
        {
            var user = userManager.Users.FirstOrDefault(u => u.UserName == UserName);

            List<mRank> ranks = new List<mRank>();

            ranks = (List<mRank>)await _rankRepository.GetAll();

            ViewData["Rank"] = ranks.ToList();

            var UserRank = ranks.FirstOrDefault(r => r.Id == RankId);

            var UserUnitRank = UserRank?.RankName;

            if (user == null)
            {
                return LocalRedirect("/Identity/Account/Login");

            }

            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault();

                ViewData["ty"] = RoleName;

                var type = _context.tbl_Type.ToList();
                ViewBag.Type = type;
            }

            List<mCommand> cl = new List<mCommand>();

            cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });

            ViewData["cl"] = _context.mCommand.ToList();


            List<UnitDtl> udtl = new List<UnitDtl>();

            udtl = await _unitRepository.GetAllUnitAsync();

            var userunit = udtl.FirstOrDefault(d => d.unitid == user?.unitid);

            var userunitname = userunit?.UnitName;

            ViewData["Unit"] = udtl.ToList();

            var users = new InputModel
            {
                Id = user.Id,
                UserName = user.UserName,
                OfficerName = user.Offr_Name,
                appointment = user.appointment,
                RoleName = user.RoleName,
                Tele_Army = user.Tele_Army,
                RankId = RankId,
                unitId = user.unitid,
                RankName = UserUnitRank

            };
            users.UserName = user.UserName.Trim();
            users.OfficerName = user.Offr_Name.Trim();
            users.appointment = user.appointment.Trim();
            users.Tele_Army = user.Tele_Army.Trim();
            return View(users);
        }



        public async Task<IActionResult> GetUserEditPartial(string UserName, string RoleName, int RankId)
         {
            var user = userManager.Users.FirstOrDefault(u => u.UserName == UserName);
            if (user == null)
            {
                return NotFound();
            }

            List<mRank> ranks = (List<mRank>)await _rankRepository.GetAll();
            ViewData["Rank"] = ranks.ToList();

            var UserRank = ranks.FirstOrDefault(r => r.Id == RankId);
            var UserUnitRank = UserRank?.RankName;

            ViewData["ty"] = RoleName;
            ViewBag.Type = _context.tbl_Type.ToList();
            ViewBag.SelectedType = _context.tbl_Type
    .Select(t => new { t.Id, t.Name })
    .ToList();
            List<UnitDtl> udtl = await _unitRepository.GetAllUnitAsync();
            ViewData["Unit"] = udtl.ToList();

            var users = new InputModel
            {
                Id = user.Id,
                UserName = user.UserName.Trim(),
                OfficerName = user.Offr_Name.Trim(),
                appointment = user.appointment.Trim(),
                RoleName = user.RoleName,
                Tele_Army = user.Tele_Army.Trim(),
                RankId = RankId,
                unitId = user.unitid,
                RankName = UserUnitRank,
              
            };

            return PartialView("_AllUsersEditPartial", users);
        }


        public async Task<IActionResult> UpdateUserEdit(InputModel input)
        {
          
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            input.UserName = input.UserName.Trim();
            input.OfficerName = input.OfficerName.Trim();
            input.appointment = input.appointment.Trim();
            input.Tele_Army = input.Tele_Army.Trim();

            if (input.RoleName == "1")
            {
                input.RoleName = "bc74ba2f-6cee-4936-800d-337b6e39d01a";
                input.Password = "Dte@123";
            }
            else
            {
                input.RoleName = "1789a675-9951-42a7-b064-8d7da156521f";
                input.Password = "Dte@123";
            }

            List<mRank> ranks = new List<mRank>();

            ranks = (List<mRank>)await _rankRepository.GetAll();
            ViewData["Rank"] = ranks.ToList();

            var UserRank = ranks.FirstOrDefault(r => r.Id == input.RankId);

            input.RankName = UserRank?.RankName;



            if (input.RoleName != null && input.Password != null && input.unitId > 0 && input.UserName != null && input.RankName != null && input.OfficerName != null && input.Tele_Army != null)
            {
                ApplicationUser userToUpdate = await userManager.FindByIdAsync(input.Id);

                if (userToUpdate != null)
                {
                    userToUpdate.RoleName = input.RoleName;
                    userToUpdate.domain_iam = input.UserName;
                    userToUpdate.appointment = input.appointment;
                    userToUpdate.unitid = input.unitId;
                    userToUpdate.Rank = input.RankId;
                    userToUpdate.Offr_Name = input.OfficerName;
                    userToUpdate.Tele_Army = input.Tele_Army;
                    userToUpdate.UserName = input.UserName;


                    var result = await userManager.UpdateAsync(userToUpdate);

                    var existingUserRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userToUpdate.Id);


                    if (existingUserRole != null)
                    {

                        _context.UserRoles.Remove(existingUserRole);

                        await _context.SaveChangesAsync();
                    }


                    _context.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = userToUpdate.Id,
                        RoleId = input.RoleName
                    });


                    await _context.SaveChangesAsync();


                    if (result.Succeeded && Logins.unitid == 1)
                    {

                        TempData["SuccessMessage"] = "User successfully updated!";
                        return RedirectToAction("GetsAllUsers", "Account");
                    }
                    else if (result.Succeeded && Logins.unitid != 1)
                    {
                        TempData["SuccessMessage"] = "User successfully updated!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["FailureMessage"] = "Failed to update user!";
                        return RedirectToAction("AllUsersEdit", "Account");
                    }
                }
                else
                {
                    TempData["FailureMessage"] = "User not found!";
                    return RedirectToAction("AllUsersEdit", "Account");
                }
            }
            else
            {
                TempData["FailureMessage"] = "One of the important inputs is missing!";
                return RedirectToAction("AllUsersEdit", "Account");
            }
        }



        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public async Task<IActionResult> UpdateFlag(string username, string rolename, bool flag)
        {
            ApplicationUser userToUpdate = await userManager.FindByNameAsync(username);

            if (userToUpdate != null)
            {

                userToUpdate.Flag = flag;

                var existingUserRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userToUpdate.Id);
                if (existingUserRole != null)
                {
                    _context.UserRoles.UpdateRange(existingUserRole);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("GetsAllUsers");
        }
    }

}
