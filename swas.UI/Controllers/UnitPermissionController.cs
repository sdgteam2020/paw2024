using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using swas.BAL;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;

namespace swas.UI.Controllers
{
    [Authorize]
    public class UnitPermissionController : Controller
    {
        private readonly IUnitPermissionRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitPermissionController> _logger;

        public UnitPermissionController(
            IUnitPermissionRepository repository,
            ApplicationDbContext context,
            ILogger<UnitPermissionController> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Units = await _context.tbl_mUnitBranch
                .AsNoTracking()
                .OrderBy(x => x.UnitName)
                .Select(x => new SelectListItem
                {
                    Value = x.unitid.ToString(),
                    Text = x.UnitName
                })
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUnitPermissions(int unitId)
        {
            if (unitId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid unit selected."
                });
            }

            try
            {
                var unitExists = await _context.tbl_mUnitBranch
                    .AsNoTracking()
                    .AnyAsync(x => x.unitid == unitId);

                if (!unitExists)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Selected unit does not exist."
                    });
                }

                var data = await _repository.GetUnitPermissions(unitId);

                return PartialView("_UnitPermissionList", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading permissions for UnitId {UnitId}",
                    unitId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while loading permissions."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUnitPermissions(
            [FromBody] UnitPermissionDTO model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request."
                });
            }

            if (model.UnitId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid unit selected."
                });
            }

            if (model.Permissions == null || !model.Permissions.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No permissions received."
                });
            }

            model.Permissions = model.Permissions
                .Where(x => x.PermissionId > 0)
                .GroupBy(x => x.PermissionId)
                .Select(g => g.First())
                .ToList();

            if (!model.Permissions.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No valid permissions received."
                });
            }

            try
            {
                var unitExists = await _context.tbl_mUnitBranch
                    .AsNoTracking()
                    .AnyAsync(x => x.unitid == model.UnitId);

                if (!unitExists)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Selected unit does not exist."
                    });
                }

                
                var result = await _repository.SaveUnitPermissions(model);

                if (!result.success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.message ?? "Unable to save permissions."
                    });
                }

                return Json(new
                {
                    success = result.success,
                    message = result.message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error saving permissions for UnitId {UnitId}",
                    model.UnitId);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Something went wrong while saving permissions."
                });
            }
        }
    }
}
    