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
using Microsoft.AspNet.Identity;

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

namespace swas.UI.Controllers
{
    //  Customised by Sub Maj Sanal on 29 Jul  
    [Authorize]
    public class AccountController : Controller
    {

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start

        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        public readonly ApplicationDbContext _context;
        //  private readonly HomeController _homeController;
        private readonly IUnitRepository _unitRepository;
        private readonly ILogger<LoginModel> _logger;
        private readonly Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser> _userStore;
        private readonly Microsoft.AspNetCore.Identity.IUserEmailStore<ApplicationUser> _emailStore;
        public AccountController(Microsoft.AspNetCore.Identity.IUserStore<ApplicationUser> userStore, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IUnitRepository unitRepository, ILogger<LoginModel> logger)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _unitRepository = unitRepository;
            _logger = logger;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }


        private Microsoft.AspNetCore.Identity.IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (Microsoft.AspNetCore.Identity.IUserEmailStore<ApplicationUser>)_userStore;
        }

        // [Authorize(Policy = "Admin")] 
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


        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 30 Jul 23


        [Authorize(Policy = "Admin")]

        public async Task<IActionResult> GetAllUsers()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;
            ApplicationUser inputModel = new ApplicationUser();

            var users = await userManager.Users.ToListAsync();

            //Users inputModel = new Users();
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

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start
        public async Task<IActionResult> SetValue(string selectedValue)
        {
            TempData["SelectedValue"] = selectedValue; // Store the value in TempData or ViewBag as per your requirement
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();

            var result = await signInManager.PasswordSignInAsync(selectedValue, "Dte@123", true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                ApplicationUser userdet = new ApplicationUser();
                IdentityRole ss = new IdentityRole();

                var userId = userManager.GetUserId(User);
                userdet = await userManager.FindByNameAsync(selectedValue);
                var unitget = await _unitRepository.GetUnitDtl(userdet.unitid);
                var usrole = await userManager.GetRolesAsync(userdet);
                // string rolename = ssd.Name;
                await Task.Delay(1000);

                Login Dbs = new Login();
                Dbs.Unit = unitget.UnitName;
                Dbs.Comdid = unitget.unitid;
                Dbs.Corpsid = unitget.CorpsId;
                Dbs.UserName = userdet.UserName;
                Dbs.UserName = selectedValue;
                Dbs.Role = usrole[0].ToString();
                Dbs.Iamuserid = userdet.domain_iam;

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
        ///Created and Reviewed by : Sub Maj M   Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start
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




            //********************IAM COMMENTED CODE START**********************************
            String EncryptedResponse = Request.Form["SAMLResponse"];

            if (!string.IsNullOrEmpty(EncryptedResponse))
            {
                string decryptedsamlresponse = DecryptSAmlResponseNew(EncryptedResponse, "C:\\Cert\\App Certificate\\applwhitelisting.army.mil.pfx", "Abc@2022");

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

                        var result = await signInManager.PasswordSignInAsync(log.NameId, "Dte@123", false, lockoutOnFailure: true);
                        if (result.Succeeded)  ///   registered user
                        {


                            ApplicationUser userdet = new ApplicationUser();
                            IdentityRole ss = new IdentityRole();
                            _logger.LogInformation("User logged in.");

                            userdet = await userManager.FindByNameAsync(log.NameId);
                            var unitdetl = await _unitRepository.GetUnitDtl(userdet.unitid);


                            Login Db = new Login();
                            if (userdet.domain_iam != null)   //   domain_iam available after registration
                            {
                                Db.UserName = userdet.UserName;
                                Db.Comdid = unitdetl.unitid;
                                Db.Corpsid = unitdetl.CorpsId;
                                Db.Iamuserid = userdet.domain_iam;
                                Db.Unit = unitdetl.UnitName;
                                Db.unitid = userdet.unitid;

                                var usrole = await userManager.GetRolesAsync(userdet);
                                Db.Role = usrole[0].ToString();

                                if (Db.ActualUserName == null)
                                {
                                    Db.ActualUserName = log.NameId;
                                }
                            }


                            else  //   without registration users
                            {

                                Db.UserName = log.NameId;   // IAM ID
                                Db.ActualUserName = log.NameId; //  IAM ID
                                Db.Role = "Guest";

                            }

                            SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Db);

                            if (Db.Role == "Dte")
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                return RedirectToAction("NewProject", "Home");
                            }
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
            }
            else
            {
                Response.Redirect("https://iam2.army.mil/IAM/User", true);
            }
            return RedirectToAction("UnAuthUser", "Account");
            //********************IAM COMMENTED CODE END**********************************
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

                // using the method
                String[] newstring = Encryptedtext.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                //string[] newstring = encryptedvalue.Split();
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
                        myCert2 = new X509Certificate2(@"C:\\Cert\\App Certificate\\acms.army.mil.pfx", "Abc@2022");
                        // rsa = (RSACryptoServiceProvider)myCert2.PrivateKey;
                        #region test
                        using (RSA rs = myCert2.GetRSAPrivateKey())
                        {
                            // rs.KeySize = 16;
                            decryptedkey = rs.Decrypt(byteData, RSAEncryptionPadding.Pkcs1);

                        }
                        #endregion
                    }
                    catch (Exception e)
                    {

                    }
                    byte[] iv = new byte[16];


                    byte[] iv1 = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

                    // result = DecryptString0705222_Final(plain, rsa.Decrypt(byteData, RSAEncryptionPadding.Pkcs1), iv1);
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
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.ECB;

            // Set key and IV
            byte[] aesKey = new byte[16];
            Array.Copy(key, 0, aesKey, 0, 16);
            encryptor.Key = aesKey;
            encryptor.IV = iv;
            encryptor.Padding = PaddingMode.PKCS7;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            catch (Exception exx)
            {

            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                memoryStream.Close();
                cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;

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

            //string ReuestXML = req.GetRequest(AuthRequest.AuthRequestFormat.Base64);
            string ReuestXML = req.GetLogOutRequest(AuthRequest.AuthRequestFormat.Base64, issueurl, "https://iam2.army.mil/IAM/logout");


            Response.Redirect("https://iam2.army.mil/IAM/logout?SAMLResponse=" + ReuestXML);


        }
        ///Created and Reviewed by : Sub Maj M  Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start
        [HttpPost]
        public async Task<IActionResult> AddlTask(string UserName, string UserName2)
        {

            var user = await this.userManager.FindByNameAsync(UserName2);


            if (user != null)
            {
                user.domain_iam = UserName;
                //user.UserName = UserName2;
            }


            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Console.WriteLine("Successfully updated ");
                // Failed to update the user, handle the error or redirect to an appropriate page
            }

            // User updated successfully, perform any additional actions or redirect to a success page
            return RedirectToAction("AddlTask");
        }


        ///Created and Reviewed by : Sub Maj  M Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start

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


        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 30 Jul 23
        ///Tested By :- 
        ///Tested Date : 
        ///Start

        [HttpPost]
        public async Task<IActionResult> ResetAddlTask(string UserName, string UserName2)
        {


            var user = await this.userManager.FindByNameAsync(UserName2);


            if (user != null)
            {
                user.domain_iam = null;
                //user.UserName = UserName2;
            }


            var result = await this.userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Console.WriteLine("Successfully updated ");
                // Failed to update the user, handle the error or redirect to an appropriate page
            }
            else
            {
                return Json("Failed");
            }

            //return RedirectToAction("AddlTask");

            return Json("Success");
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            Login Db = new Login();
            SessionHelper.ClaerObjectAsJson(HttpContext.Session, "User");

            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                //return RedirectToAction(nameof("/Identity/Account/Login"));

                return RedirectToAction("/Identity/Account/Login");
                //  return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage("/Identity/Account/Login");
            }
        }
        //[AllowAnonymous]
        //public void Logout()
        //{

        //    if (HttpContext.Request.Query.Count > 0)
        //    {
        //        if (!string.IsNullOrEmpty(Convert.ToString(HttpContext.Request.Query["SAMLRequest"])))
        //        {
        //            AccountSettings accountSettings = new AccountSettings();
        //            Response samlResponse = new Response(accountSettings);

        //            samlResponse.LoadXmlFromBase64(HttpContext.Request.Query["SAMLRequest"]);
        //            string nameid = string.Empty;
        //            string issuer = string.Empty;
        //            samlResponse.GetLogoutParameter(out nameid, out issuer);
        //            HttpContext.Session.Clear();
        //            try
        //            {
        //                SendResponseToIAM("https://applwhitelisting.army.mil/Account/Logout", accountSettings.entityId, nameid);
        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //        else if (!string.IsNullOrEmpty(Convert.ToString(HttpContext.Request.Query["SAMLResponse"])))
        //        {
        //            HttpContext.Session.Clear();

        //            Response.Redirect("https://applwhitelisting.army.mil/Account/FinalLogout");
        //        }
        //        else
        //        {
        //            AccountSettings acs = new AccountSettings();

        //            string NameId = HttpContext.Session.GetString("NameID");
        //            string userRole = HttpContext.Session.GetString("SAMLRole");


        //            signInManager.SignOutAsync();
        //            HttpContext.SignOutAsync();
        //            Login Db = new Login();
        //            SessionHelper.ClaerObjectAsJson(HttpContext.Session, "User");

        //            _logger.LogInformation("User logged out.");

        //            Response.Cookies.Delete(Request.Cookies.ToString());
        //            //HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

        //            HttpContext.Session.Clear();
        //            LogoutRequesttoIAM(userRole, acs.entityId, NameId);
        //        }
        //    }
        //    else
        //    {
        //        AccountSettings acs = new AccountSettings();
        //        string NameId = HttpContext.Session.GetString("NameID");
        //        string role = HttpContext.Session.GetString("SAMLRole");



        //        //await HttpContext.SignOutAsync();
        //        HttpContext.Session.Clear();
        //        //HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);



        //        LogoutRequesttoIAM(role, acs.entityId, NameId);
        //    }
        //}
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
                                join c in _context.UserRoles on a.Id equals c.UserId
                                join b in _roleManager.Roles on c.RoleId equals b.Id
                                orderby a.CreatedDate descending
                                select new Users
                                {
                                    UserName = a.UserName,
                                    UpdatedBy = a.unitid,
                                    UnitId = a.unitid,
                                    RoleName = b.Name,                                    
                                    CreatedDate = a.CreatedDate,
                                    Flag = a.Flag,
                                }).ToList();

                    ViewBag.data = list;

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
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<IActionResult> AllUsersEdit(string UserName, string RoleName)
        {
            var user = userManager.Users.FirstOrDefault(u => u.UserName == UserName);
            if (user == null)
            {
                return LocalRedirect("/Identity/Account/Login");
            }

            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault();

                ViewData["ty"] = userRole;

                var type = _context.tbl_Type.ToList();

                List<Types> Name = new List<Types>();

                type.Insert(0, new Types { Id = 0, Name = userRole });

                ViewData["TypesList"] = new SelectList(type, "Id", "Name");
            }

            List<mCommand> cl = new List<mCommand>();

            cl.Insert(0, new mCommand { comdid = 0, Command_Name = "--Select--" });

            ViewData["cl"] = _context.mCommand.ToList();

            ViewData["Rank"] = _context.mRank.ToList();

            var rank = _context.mRank.FirstOrDefault(r => r.RankName == user.Rank);
            user.Rank = rank?.RankName;

            List<UnitDtl> udtl = new List<UnitDtl>();

            udtl = await _unitRepository.GetAllUnitAsync();

            var userunit = udtl.FirstOrDefault(d => d.unitid == user?.unitid);

            var userunitname = userunit?.UnitName;

            ViewData["Unit"] = udtl.ToList();

            var users = new InputModel
            {
                UserName = user.UserName,
                OfficerName = user.Offr_Name,
                appointment = user.appointment,
                RoleName = user.RoleName,
                Tele_Army = user.Tele_Army,
                Rank = user.Rank,
                unitId = user.unitid
            };

            return View(users);
        }

        public async Task<IActionResult> UpdateUserEdit(InputModel input, Users user)
        {
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

            if (input.RoleName != null && input.Password != null && input.unitId > 0 && input.UserName != null && input.Rank != null && input.OfficerName != null && input.Tele_Army != null)
            {
                ApplicationUser userToUpdate = await userManager.FindByNameAsync(user.UserName);

                if (userToUpdate != null)
                {
                    // Update user properties
                    userToUpdate.RoleName = input.RoleName;
                    userToUpdate.domain_iam = input.domain_iam;
                    userToUpdate.appointment = input.appointment;
                    userToUpdate.unitid = input.unitId;
                    userToUpdate.Rank = input.Rank;
                    userToUpdate.Offr_Name = input.OfficerName;
                    userToUpdate.Tele_Army = input.Tele_Army;

                    var existingUserRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userToUpdate.Id);
                    if (existingUserRole != null)
                    {
                        _context.UserRoles.Remove(existingUserRole);
                        await _context.SaveChangesAsync();
                    }

                    // Add new UserRole with updated RoleId
                    _context.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = userToUpdate.Id,
                        RoleId = input.RoleName
                    });

                    await _context.SaveChangesAsync();

                    var result = await userManager.UpdateAsync(userToUpdate);

                    if (result.Succeeded)
                    {

                        TempData["SuccessMessage"] = "User successfully updated!";
                        return RedirectToAction("GetsAllUsers", "Account");
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

        public async Task<IActionResult> UpdateFlag(string username , string rolename , bool flag)
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

                return View();
        }
    }

}
