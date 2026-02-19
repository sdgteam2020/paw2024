using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.BAL.Repository;
using swas.DAL.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;

namespace swas.UI.Controllers
{


    public class ProjStakeHolderMovController : Controller
    {
        private readonly IProjStakeHolderMovRepository _psmRepo;
        private readonly IProjectsRepository _projectsRepository;
        private readonly IAttHistoryRepository _attHistoryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDdlRepository _dlRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataProtector _dataProtector;
        private readonly ILogger<ProjStakeHolderMovController> _logger;

        public ProjStakeHolderMovController(IProjStakeHolderMovRepository psmRepo, IProjectsRepository projectsRepository, IAttHistoryRepository attHistoryRepository, IHttpContextAccessor httpContextAccessor, IDdlRepository dlRepository, IWebHostEnvironment ienvironments, IDataProtectionProvider DataProtector, ILogger<ProjStakeHolderMovController> logger)
        {
            _psmRepo = psmRepo;
            _projectsRepository = projectsRepository;
            _attHistoryRepository = attHistoryRepository;
            _httpContextAccessor = httpContextAccessor;
            _dlRepository = dlRepository;
            _environment = ienvironments;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var projStakeHolderMovList = await _psmRepo.GetAllProjStakeHolderMovAsync();
            return View(projStakeHolderMovList);
        }
        public async Task<IActionResult> Details(int id)
        {
            var projStakeHolderMov = await _psmRepo.GetProjStakeHolderMovByIdAsync(id);
            if (projStakeHolderMov == null)
            {
                return NotFound();
            }

            return View(projStakeHolderMov);
        }
        public async Task<IActionResult> Create()
        {
            var stackhold = await _psmRepo.GetAllProjStakeHolderMovAsync();
            return View(stackhold);
        }




        public async Task<IActionResult> Edit(int id)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                var projStakeHolderMov = await _psmRepo.GetProjStakeHolderMovByIdAsync(id);
                if (projStakeHolderMov == null)
                {
                    return NotFound();
                }
                return View(projStakeHolderMov);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_ProjStakeHolderMov projStakeHolderMov)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                try
                {
                    if (id != projStakeHolderMov.PsmId)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        await _psmRepo.UpdateProjStakeHolderMovAsync(projStakeHolderMov);
                        return RedirectToAction(nameof(Index));
                    }

                    return View(projStakeHolderMov);
                }
                catch (Exception ex)
                {
                    int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                    var eventId = new EventId(dynamicEventId, "Edit");
                    _logger.Log(LogLevel.Error, eventId, "An error occurred while on Edit in ProjStakeHolderMovController.", ex, (s, e) => $"{s} - {e?.Message}");

                    return Json(-1);
                }

            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                var projStakeHolderMov = await _psmRepo.GetProjStakeHolderMovByIdAsync(id);
                if (projStakeHolderMov == null)
                {
                    return NotFound();
                }
                return View(projStakeHolderMov);
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (Logins != null)
            {
                try
                {
                    await _psmRepo.DeleteProjStakeHolderMovAsync(id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                    var eventId = new EventId(dynamicEventId, "DeleteConfirmed");
                    _logger.Log(LogLevel.Error, eventId, "An error occurred while on DeleteConfirmed in ProjStakeHolderMovController.", ex, (s, e) => $"{s} - {e?.Message}");

                    return Json(-1);
                }
            }
            else
            {
                return Redirect("/Identity/Account/login");
            }
        }
    }

}
