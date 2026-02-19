using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;

namespace swas.UI.Controllers
{
    public class StakeHolderController : Controller
    {


        private readonly IStakeHolderRepository _stakeHolderRepository;
        private readonly ILogger<StakeHolderController> _logger;

        public StakeHolderController(IStakeHolderRepository stakeHolderRepository, ILogger<StakeHolderController> logger)
        {
            _stakeHolderRepository = stakeHolderRepository;
            _logger = logger;
        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            var stakeHolders = await _stakeHolderRepository.GetAllStakeHoldersAsync();
            return View(stakeHolders);
        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var stakeHolder = await _stakeHolderRepository.GetStakeHolderByIdAsync(id);
            if (stakeHolder == null)
            {
                return NotFound();
            }

            return View(stakeHolder);
        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create()
        {
            var stkhold = await _stakeHolderRepository.GetAllStakeHoldersAsync();
            return View(stkhold);

        }
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_mStakeHolder stakeHolder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _stakeHolderRepository.AddStakeHolderAsync(stakeHolder);
                    return RedirectToAction(nameof(Index));
                }
                return View(stakeHolder);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "Create");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Create in StakeHolderController.", ex, (s, e) => $"{s} - {e?.Message}");

                return RedirectToAction("Error", "Home");
            }

        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var stakeHolder = await _stakeHolderRepository.GetStakeHolderByIdAsync(id);
            if (stakeHolder == null)
            {
                return NotFound();
            }
            return View(stakeHolder);
        }
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_mStakeHolder stakeHolder)
        {
            try
            {
                if (id != stakeHolder.StakeHolderId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    await _stakeHolderRepository.UpdateStakeHolderAsync(stakeHolder);
                    return RedirectToAction(nameof(Index));
                }

                return View(stakeHolder);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "Edit");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Edit in StakeHolderController.", ex, (s, e) => $"{s} - {e?.Message}");

                return RedirectToAction("Error", "Home");
            }

        }
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var stakeHolder = await _stakeHolderRepository.GetStakeHolderByIdAsync(id);
            if (stakeHolder == null)
            {
                return NotFound();
            }
            return View(stakeHolder);
        }
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _stakeHolderRepository.DeleteStakeHolderAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "DeleteConfirmed");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Edit in DeleteConfirmed.", ex, (s, e) => $"{s} - {e?.Message}");

                return RedirectToAction("Error", "Home");
            }
        }
    }

}

