#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using swas.UI.Controllers;
using swas.BAL.Helpers;
using swas.UI.Models;
using swas.BAL;
using swas.DAL;

using swas.BAL.Utility;
using swas.DAL.Models;
using swas.BAL.DTO;

namespace swas.Areas.Identity.Pages.Account
{
    [Authorize(Policy = "Admin")]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string UserName { get; set; }
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(HttpContext.Session, "User");

            if (Logins.IsNotNull())
            {

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _userManager.FindByEmailAsync(Input.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                    return Page();
                }

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userId, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    Input.UserName,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            else
                return Redirect("/Identity/Account/Login");
        }
    }
}
