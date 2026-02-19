#nullable disable

using System;
using System.Threading.Tasks;
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace swas.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }
        [TempData]
        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }

        [AllowAnonymous]
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);


            ReturnUrl = returnUrl;


        }


        [AllowAnonymous]
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {

                return LocalRedirect("/Identity/Account/Login");
            }
            else
            {
                return RedirectToPage("/Identity/Account/Logout");
            }
        }
    }
}

