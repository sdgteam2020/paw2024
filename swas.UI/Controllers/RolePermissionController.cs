using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using swas.BAL.DTO;
using swas.BAL.Interfaces;

namespace swas.UI.Controllers
{
    public class RolePermissionController
        : Controller
    {
        private readonly
            IRolePermissionRepository
            _rolePermissionRepository;

        private readonly
            RoleManager<IdentityRole>
            _roleManager;

        private readonly
            ILogger<RolePermissionController>
            _logger;

        public RolePermissionController(
            IRolePermissionRepository
            rolePermissionRepository,
            RoleManager<IdentityRole>
            roleManager,
            ILogger<RolePermissionController>
            logger)
        {
            _rolePermissionRepository =
                rolePermissionRepository;

            _roleManager =
                roleManager;

            _logger =
                logger;
        }

        public IActionResult Index()
        {
            try
            {
                ViewBag.Roles =
                    _roleManager.Roles
                    .Select(x =>
                    new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Name
                    })
                    .ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Role Permission Index Error");

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult>
            GetRolePermissions(
            string roleId)
        {
            try
            {
                var data =
                    await
                    _rolePermissionRepository
                    .GetRolePermissions(
                    roleId);

                return PartialView(
                    "_PermissionList",
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "GetRolePermissions Error");

                return Json(new
                {
                    success = false
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dte")]
        public async Task<IActionResult> SaveRolePermissions([FromBody] RolePermissionDTO model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data."
                });
            }

            if (string.IsNullOrWhiteSpace(model.RoleId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Role is required."
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
                var result = await _rolePermissionRepository.SaveRolePermissions(model);

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
                _logger.LogError(ex, "Error while saving role permissions for RoleId: {RoleId}", model.RoleId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while saving role permissions."
                });
            }
        }
    }
}
