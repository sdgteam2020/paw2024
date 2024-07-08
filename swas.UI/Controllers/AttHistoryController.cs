using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using System.Threading.Tasks;

namespace swas.UI.Controllers
{

    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class AttHistoryController : Controller
    {
        private readonly IAttHistoryRepository _attHistoryRepository;

        public AttHistoryController(IAttHistoryRepository attHistoryRepository)
        {
            _attHistoryRepository = attHistoryRepository;
        }

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: AttHistory
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var attHistories = await _attHistoryRepository.GetAllAttHistoriesAsync();
            return View(attHistories);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: AttHistory/Details/5
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
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: AttHistory/Create
        [Authorize(Policy = "StakeHolders")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var atthist = await _attHistoryRepository.GetAllAttHistoriesAsync();
            return View(atthist);
            
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // POST: AttHistory/Create
        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_AttHistory attHistory)
        {
            if (ModelState.IsValid)
            {
                await _attHistoryRepository.AddAttHistoryAsync(attHistory);
                return RedirectToAction(nameof(Index));
            }
            return View(attHistory);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: AttHistory/Edit/5
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
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // POST: AttHistory/Edit/5
        [Authorize(Policy = "StakeHolders")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_AttHistory attHistory)
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
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: AttHistory/Delete/5
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
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // POST: AttHistory/Delete/5
        [Authorize(Policy = "StakeHolders")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _attHistoryRepository.DeleteAttHistoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
