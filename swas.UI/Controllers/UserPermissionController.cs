using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;

namespace swas.UI.Controllers
{
    public class
  UserPermissionController
      : Controller
    {
        private readonly IUserPermissionRepository  _repository;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<UserPermissionDTO> _logger;
        public UserPermissionController(
    IUserPermissionRepository repository,
    UserManager<ApplicationUser> userManager, ILogger<UserPermissionDTO> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "Dte")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userManager.Users
                    .AsNoTracking()
                    .OrderBy(x => x.UserName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.UserName
                    })
                    .ToListAsync();

                ViewBag.Users = users;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading user permission page.");

                TempData["Error"] = "Unable to load users.";

                ViewBag.Users = new List<SelectListItem>();

                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Dte")]
        public async Task<IActionResult> GetUserPermissions(string userId)
        {
            try
            {
                var result = await _repository.GetUserPermissions(userId);

                if (!result.success || result.data == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message
                    });
                }

                return PartialView("_UserPermissionList", result.data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading permissions for UserId: {UserId}", userId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while loading permissions."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dte")]
        public async Task<IActionResult> SaveUserPermissions([FromBody] UserPermissionDTO model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data."
                });
            }

            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User is required."
                });
            }

            if (model.Permissions == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Permissions are required."
                });
            }

            try
            {
                var result = await _repository.SaveUserPermissions(model);

                if (!result.success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result.message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving user permissions for UserId: {UserId}", model.UserId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while saving permissions."
                });
            }
        }
    }
}
