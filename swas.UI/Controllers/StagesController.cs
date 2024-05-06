using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using swas.BAL.Interfaces;
using swas.DAL.Models;
using System.Threading.Tasks;


namespace swas.UI.Controllers
{
    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
   
    public class StagesController : Controller
    {
        private readonly IStagesRepository _stagesRepository;

        public StagesController(IStagesRepository stagesRepository)
        {
            _stagesRepository = stagesRepository;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: Stages
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            var stages = await _stagesRepository.GetAllStagesAsync();
            return View(stages);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: Stages/Details/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var stage = await _stagesRepository.GetStageByIdAsync(id);
            if (stage == null)
            {
                return NotFound();
            }

            return View(stage);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: Stages/Create
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create()
        {
            var stages = await _stagesRepository.GetAllStagesAsync();
            return View(stages);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // POST: Stages/Create
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_mStages stage)
        {
            if (ModelState.IsValid)
            {
                await _stagesRepository.AddStageAsync(stage);
                return RedirectToAction(nameof(Index));
            }
            return View(stage);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: Stages/Edit/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var stage = await _stagesRepository.GetStageByIdAsync(id);
            if (stage == null)
            {
                return NotFound();
            }
            return View(stage);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // POST: Stages/Edit/5
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_mStages stage)
        {
            if (id != stage.StagesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _stagesRepository.UpdateStageAsync(stage);
                return RedirectToAction(nameof(Index));
            }

            return View(stage);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: Stages/Delete/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var stage = await _stagesRepository.GetStageByIdAsync(id);
            if (stage == null)
            {
                return NotFound();
            }
            return View(stage);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // POST: Stages/Delete/5
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _stagesRepository.DeleteStageAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
