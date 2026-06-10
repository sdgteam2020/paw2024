using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL.Models;

namespace swas.UI.Controllers
{
    [Authorize(Roles ="Dte")]
    public class PermissionMasterController
        : Controller
    {
        private readonly
            IPermissionMasterRepository
            _permissionMasterRepository;

        private readonly
            ILogger<PermissionMasterController>
            _logger;

        public PermissionMasterController(
            IPermissionMasterRepository
            permissionMasterRepository,
            ILogger<PermissionMasterController>
            logger)
        {
            _permissionMasterRepository =
                permissionMasterRepository;

            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var data =
                    _permissionMasterRepository
                    .GetAll();

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading permission master");

                TempData["error"] =
                    "Something went wrong.";

                return View(
                    new List<PermissionMaster>());
            }
        }

        [HttpGet]
        public IActionResult AddEdit(
            int id = 0)
        {
            try
            {
                if (id == 0)
                {
                    return PartialView(
                        "_AddEdit",
                        new PermissionMaster());
                }

                var data =
                    _permissionMasterRepository
                    .GetById(id);

                return PartialView(
                    "_AddEdit",
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching permission");

                return Json(new
                {
                    success = false,
                    message =
                    "Unable to load data."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Save(
            PermissionMaster model)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    var errors =
                        ModelState.Values
                        .SelectMany(v =>
                        v.Errors)
                        .Select(e =>
                        e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message =
                        string.Join(
                            "<br/>",
                            errors)
                    });
                }

                var login =
                SessionHelper
                .GetObjectFromJson<Login>(
                    HttpContext.Session,
                    "User");

                string username =
                    login?.UserName
                    ?? "System";

                var result =
                    await
                    _permissionMasterRepository
                    .Save(
                        model,
                        username);

                return Json(new
                {
                    success =
                    result.success,

                    message =
                    result.message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Save error");

                return Json(new
                {
                    success = false,
                    message =
                    "Something went wrong."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                var login =
                SessionHelper
                .GetObjectFromJson<Login>(
                    HttpContext.Session,
                    "User");

                string username =
                    login?.UserName
                    ?? "System";

                var result =
                    await
                    _permissionMasterRepository
                    .Delete(
                        id,
                        username);

                return Json(new
                {
                    success =
                    result.success,

                    message =
                    result.message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Delete error");

                return Json(new
                {
                    success = false,
                    message =
                    "Something went wrong."
                });
            }
        }
    }
}
