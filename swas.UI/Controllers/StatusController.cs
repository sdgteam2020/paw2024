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

    public class StatusController : Controller
    {
        private readonly IStatusRepository _statusRepository;

        public StatusController(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Status
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            var statusList = await _statusRepository.GetAllStatusAsync();
            return View(statusList);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Status/Details/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var status = await _statusRepository.GetStatusByIdAsync(id);
            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Status/Create
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create()
        {
            var Status = await _statusRepository.GetAllStatusAsync();
            return View(Status);
            
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // POST: Status/Create
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_mStatus status)
        {
            if (ModelState.IsValid)
            {
                await _statusRepository.AddStatusAsync(status);
                return RedirectToAction(nameof(Index));
            }
            return View(status);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Status/Edit/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var status = await _statusRepository.GetStatusByIdAsync(id);
            if (status == null)
            {
                return NotFound();
            }
            return View(status);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // POST: Status/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, tbl_mStatus status)
        {
            if (id != status.StatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _statusRepository.UpdateStatusAsync(status);
                return RedirectToAction(nameof(Index));
            }

            return View(status);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // GET: Status/Delete/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _statusRepository.GetStatusByIdAsync(id);
            if (status == null)
            {
                return NotFound();
            }
            return View(status);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        // POST: Status/Delete/5
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _statusRepository.DeleteStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
