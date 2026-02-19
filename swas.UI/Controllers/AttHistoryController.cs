using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using System.Threading.Tasks;

namespace swas.UI.Controllers
{
    public class AttHistoryController : Controller
    {
        private readonly IAttHistoryRepository _attHistoryRepository;
        private readonly ILogger<AttHistoryController> _logger;
        public AttHistoryController(IAttHistoryRepository attHistoryRepository, ILogger<AttHistoryController> logger)
        {
            _attHistoryRepository = attHistoryRepository;
            _logger = logger;
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var attHistories = await _attHistoryRepository.GetAllAttHistoriesAsync();
            return View(attHistories);
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var attHistory = await _attHistoryRepository.GetAttHistoryByIdAsync(id);
            if (attHistory == null)
            {
                return NotFound();
            }

            return View(attHistory);
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var atthist = await _attHistoryRepository.GetAllAttHistoriesAsync();
            return View(atthist);
            
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_AttHistory attHistory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _attHistoryRepository.AddAttHistoryAsync(attHistory);
                    return RedirectToAction(nameof(Index));
                }
                return View(attHistory);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "Create");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Get All Create on AttHistoryContoller.", ex, (s, e) => $"{s} - {e?.Message}");
                return RedirectToAction("Error", "Home");
            }
            
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var attHistory = await _attHistoryRepository.GetAttHistoryByIdAsync(id);
            if (attHistory == null)
            {
                return NotFound();
            }
            return View(attHistory);
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_AttHistory attHistory)
        {
            try
            {
                if (id != attHistory.AttId)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    await _attHistoryRepository.UpdateAttHistoryAsync(attHistory);
                    return RedirectToAction(nameof(Index));
                }
                return View(attHistory);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "Edit");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Get All Edit on AttHistoryContoller.", ex, (s, e) => $"{s} - {e?.Message}");
                return RedirectToAction("Error", "Home");
            }
            
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var attHistory = await _attHistoryRepository.GetAttHistoryByIdAsync(id);
            if (attHistory == null)
            {
                return NotFound();
            }
            return View(attHistory);
        }
        [Authorize(Policy = "StakeHolders")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _attHistoryRepository.DeleteAttHistoryAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "Create");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Get All DeleteConfirmed on AttHistoryContoller.", ex, (s, e) => $"{s} - {e?.Message}");
                return RedirectToAction("Error", "Home");
            }            
        }
    }
}
