
#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ASPNetCoreIdentityCustomFields.Data;
using swas.UI.Controllers;
using swas.BAL.Helpers;
using swas.BAL.Utility;
using swas.DAL.Models;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using swas.DAL;

namespace swas.Areas.Identity.Pages.Account
{

    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IUnitRepository _unitRepository;

        private readonly IUserRepository _userRepository;
        public readonly ApplicationDbContext _context;
        private readonly LoginCryptoKeyService _loginCryptoKeyService;
        private readonly IConfiguration _configuration;
        public LoginModel(SignInManager<ApplicationUser> signInManager, LoginCryptoKeyService loginCryptoKeyService, ILogger<LoginModel> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitRepository unitRepository, IUserRepository userRepository, ApplicationDbContext context, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _context = context;
            _configuration = configuration;
            _loginCryptoKeyService = loginCryptoKeyService;

        }
        [BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public string LoginCryptoKey { get; private set; } = string.Empty;
        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
            
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; } = true;
        }
        [AllowAnonymous]
        public async Task OnGetAsync(string returnUrl = null)
        {


            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            LoginCryptoKey = _loginCryptoKeyService.EnsureLoginCryptoKey(HttpContext);

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

        }
        [AllowAnonymous]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                returnUrl ??= Url.Content("~/");

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                var currentDatetime = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n {currentDatetime}";

                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var cryptoKey = _loginCryptoKeyService.GetLoginCryptoKey(HttpContext);

                if (string.IsNullOrWhiteSpace(cryptoKey))
                {
                    Input.UserName = string.Empty;
                    Input.Password = string.Empty;

                    ModelState.Clear();
                    ModelState.AddModelError("", "Login session expired. Please refresh the page and try again.");

                    LoginCryptoKey = _loginCryptoKeyService.EnsureLoginCryptoKey(HttpContext);

                    return Page();
                }
                if (string.IsNullOrWhiteSpace(Input?.UserName) || string.IsNullOrWhiteSpace(Input?.Password))
                {
                    ModelState.AddModelError("", "Username and Password are required.");
                    return Page();
                }
                if (!string.IsNullOrEmpty(cryptoKey))
                {
                    Input.UserName = CryptoHelper.SafeDecrypt(Input.UserName, cryptoKey)?.Trim();
                    Input.Password = CryptoHelper.SafeDecrypt(Input.Password, cryptoKey)?.Trim();
                }

                // ❗ Validate again after decrypt
                if (string.IsNullOrWhiteSpace(Input?.UserName) || string.IsNullOrWhiteSpace(Input?.Password))
{
    Input.UserName = string.Empty;
    Input.Password = string.Empty;

    ModelState.AddModelError("", "Invalid credentials format.");
    return Page();
}

                // Try login with ASP.NET Identity
                var result = await _signInManager.PasswordSignInAsync(
                    Input.UserName,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true
                );

                if (result.Succeeded)
                {
                    ApplicationUser userdet = await _userManager.FindByNameAsync(Input.UserName);

                    if (userdet != null)
                    {
                        var unitdetl = await _unitRepository.GetUnitDtl(userdet.unitid);
                        int cla = await _unitRepository.GetIdCalendar();

                        if (unitdetl != null)
                        {
                            Login Db = new Login();
                            CommonHelper commonHelper = new CommonHelper(_context);
                            var userRank = commonHelper.UserRankDetail(userdet);

                            Db.UserName = userdet.UserName;
                            Db.Comdid = unitdetl.unitid;
                            Db.Corpsid = unitdetl.CorpsId;
                            Db.Iamuserid = userdet.UserName;
                            Db.Unit = unitdetl.UnitName;
                            Db.unitid = userdet.unitid;
                            Db.Appontment = userdet.appointment;
                            Db.UserIntId = userdet.unitid;
                            Db.Rank_id = Convert.ToInt32(userdet.Rank);
                            Db.Rank = userRank;
                            Db.IcNo = userdet.Icno;
                            Db.Offr_Name = userdet.Offr_Name;
                            Db.IpAddress = watermarkText;
                            Db.CryptoKey = cryptoKey;
                            Db.cla = cla;

                            var roles = await _userManager.GetRolesAsync(userdet);
                            Db.Role = roles.Any() ? roles[0] : "Unit";

                            if (Db.ActualUserName == null)
                            {
                                Db.ActualUserName = Input.UserName;
                            }

                            // Login Log
                            tbl_LoginLog logs = new tbl_LoginLog
                            {
                                UserId = userdet.UserIntId,
                                IP = ipAddress,
                                IsActive = true,
                                Updatedby = userdet.unitid,
                                UpdatedOn = DateTime.UtcNow,
                                logindate = DateTime.UtcNow,
                                userName = userdet.UserName,
                                unitid = userdet.unitid
                            };

                            await _userRepository.Add(logs);

                            SessionHelper.SetObjectAsJson(HttpContext.Session, "User", Db);
                            HttpContext.Session.SetString(
    "SessionLastActivity",
    DateTime.UtcNow.ToString("O")
);
                            HttpContext.Session.SetString("UserName", Input.UserName);

                            return RedirectToAction("Promo", "Home");
                        }
                    }
                }

                // Account locked
                if (result.IsLockedOut)
                {
                    Input.UserName = string.Empty;
                    Input.Password = string.Empty;

                    ModelState.AddModelError("", "The account is locked.");
                    return Page();
                }

                // Two factor authentication
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe = Input.RememberMe
                    });
                }

                // Check if user exists
                var existingUser = await _userManager.FindByNameAsync(Input.UserName);

                if (existingUser == null)
                {
                    // User not registered → redirect to Register
                    TempData["UserName"] = Input.UserName;
                    HttpContext.Session.SetString("UserName", Input.UserName);

                    return RedirectToAction("Register", "Account");
                }

                // Invalid password
                Input.UserName = Input.UserName;
                Input.Password = string.Empty;

                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Invalid username or password.");

                return Page();
            }
            catch (Exception ex)
            {
                swas.BAL.Utility.Error.ExceptionHandle(ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred.");
                return Page();
            }
        }

    }
}
