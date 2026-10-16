
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
            [StringLength(1000, ErrorMessage = "Username cannot exceed 1000 characters.")]
            
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(1000, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 1000 characters.")]
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

                var ipAddress = HttpContext.Connection.RemoteIpAddress?
                    .MapToIPv4().ToString();

                var currentDatetime = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss");
                var watermarkText = $" {ipAddress}\n {currentDatetime}";

                ExternalLogins = (await _signInManager
                    .GetExternalAuthenticationSchemesAsync()).ToList();

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Get login crypto key
                var cryptoKey = _loginCryptoKeyService.GetLoginCryptoKey(HttpContext);

                if (string.IsNullOrWhiteSpace(cryptoKey))
                {
                    Input.UserName = string.Empty;
                    Input.Password = string.Empty;

                    ModelState.Clear();
                    ModelState.AddModelError(
                        "",
                        "Login session expired. Please refresh the page and try again."
                    );

                    LoginCryptoKey = _loginCryptoKeyService
                        .EnsureLoginCryptoKey(HttpContext);

                    return Page();
                }

                // Validate encrypted inputs
                if (string.IsNullOrWhiteSpace(Input?.UserName) ||
                    string.IsNullOrWhiteSpace(Input?.Password))
                {
                    ModelState.AddModelError(
                        "",
                        "Username and Password are required."
                    );

                    return Page();
                }

                // Decrypt credentials
                Input.UserName = CryptoHelper
                    .SafeDecrypt(Input.UserName, cryptoKey)?.Trim();

                Input.Password = CryptoHelper
                    .SafeDecrypt(Input.Password, cryptoKey)?.Trim();

                // Validate decrypted inputs
                if (string.IsNullOrWhiteSpace(Input.UserName) ||
                    string.IsNullOrWhiteSpace(Input.Password))
                {
                    Input.UserName = string.Empty;
                    Input.Password = string.Empty;

                    ModelState.Clear();
                    ModelState.AddModelError(
                        "",
                        "Invalid credentials format."
                    );

                    return Page();
                }

                // Authenticate using ASP.NET Core Identity
                _logger.LogWarning("STEP 1: Before PasswordSignInAsync");

                var result = await _signInManager.PasswordSignInAsync(
                    Input.UserName,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true
                );

                _logger.LogWarning(
                    "STEP 2: SignIn completed. Succeeded={Succeeded}",
                    result.Succeeded
                );

                if (result.Succeeded)
                {
                    _logger.LogWarning("STEP 3: Authentication successful");
                    ApplicationUser userdet =
                        await _userManager.FindByNameAsync(Input.UserName);

                    if (userdet == null)
                    {
                        ModelState.AddModelError(
                            "",
                            "User details could not be loaded."
                        );

                        Input.Password = string.Empty;
                        return Page();
                    }

                    var unitdetl = await _unitRepository
                        .GetUnitDtl(userdet.unitid);

                    if (unitdetl == null)
                    {
                        ModelState.AddModelError(
                            "",
                            "Unit details could not be loaded."
                        );

                        Input.Password = string.Empty;
                        return Page();
                    }

                    int cla = await _unitRepository.GetIdCalendar();

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

                    // Save login log
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

                    // Initialize user session
                    SessionHelper.SetObjectAsJson(
                        HttpContext.Session,
                        "User",
                        Db
                    );

                    HttpContext.Session.SetString(
                        "SessionLastActivity",
                        DateTime.UtcNow.ToString("O")
                    );

                    HttpContext.Session.SetString(
                        "UserName",
                        Input.UserName
                    );

                    // Redirect after successful login
                    return RedirectToAction("Promo", "Home");
                }

                // Account locked
                if (result.IsLockedOut)
                {
                    Input.UserName = string.Empty;
                    Input.Password = string.Empty;

                    ModelState.AddModelError(
                        "",
                        "The account is locked."
                    );

                    return Page();
                }

                // Two-factor authentication
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new
                    {
                        ReturnUrl = returnUrl,
                        RememberMe = Input.RememberMe
                    });
                }

                // Sign-in not allowed
                if (result.IsNotAllowed)
                {
                    Input.Password = string.Empty;

                    ModelState.AddModelError(
                        "",
                        "Sign-in is not allowed for this account."
                    );

                    return Page();
                }

                // Check whether user exists
                var existingUser = await _userManager
                    .FindByNameAsync(Input.UserName);

                if (existingUser == null)
                {
                    TempData["UserName"] = Input.UserName;

                    HttpContext.Session.SetString(
                        "UserName",
                        Input.UserName
                    );

                    return RedirectToAction("Register", "Account");
                }

                // Invalid password
                Input.Password = string.Empty;

                ModelState.Clear();
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password."
                );

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login processing failed");

                try
                {
                    var logDirectory = Path.Combine(
                        AppContext.BaseDirectory,
                        "logs"
                    );

                    Directory.CreateDirectory(logDirectory);

                    var logFile = Path.Combine(
                        logDirectory,
                        "login-error.txt"
                    );

                    await System.IO.File.AppendAllTextAsync(
                        logFile,
                        $"Time: {DateTime.UtcNow:O}{Environment.NewLine}" +
                        $"Exception: {ex}{Environment.NewLine}" +
                        $"------------------------{Environment.NewLine}"
                    );
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Could not write login diagnostic file");
                }

                ModelState.AddModelError(
                    "",
                    "An unexpected error occurred during login."
                );

                return Page();
            }
        }


    }
}
