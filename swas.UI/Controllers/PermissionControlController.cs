using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.DTO;
using swas.BAL.Interfaces;

namespace swas.UI.Controllers
{
    [Authorize(Policy = "Admin")]
    public class PermissionControlController : Controller
    {
        private readonly IPermissionControlRepository _repository;
        private readonly ILogger<PermissionControlController> _logger;

        public PermissionControlController(
            IPermissionControlRepository repository,
            ILogger<PermissionControlController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTargets(string permissionFor)
        {
            if (string.IsNullOrWhiteSpace(permissionFor))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Permission type is required."
                });
            }

            try
            {
                var data = await _repository.GetTargetsAsync(permissionFor);

                return Json(new
                {
                    success = true,
                    data = data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading targets for {PermissionFor}", permissionFor);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to load records."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions(
            string permissionFor,
            string targetId)
        {
            try
            {
                var result = await _repository.GetPermissionsAsync(permissionFor, targetId);

                if (!result.success || result.data == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message
                    });
                }

                return PartialView("_PermissionControlList", result.data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while loading permissions. Type: {PermissionFor}, TargetId: {TargetId}",
                    permissionFor,
                    targetId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Unable to load permissions."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePermissions(
            [FromBody] PermissionControlSaveDTO model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request."
                });
            }

            if (string.IsNullOrWhiteSpace(model.PermissionFor))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Permission type is required."
                });
            }

            if (string.IsNullOrWhiteSpace(model.TargetId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please select role/user/unit."
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
                var result = await _repository.SavePermissionsAsync(model);

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
                _logger.LogError(ex, "Error while saving centralized permissions.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while saving permissions."
                });
            }
        }
    }
}
