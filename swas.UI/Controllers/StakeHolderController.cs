﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.DAL.Models;

namespace swas.UI.Controllers
{

    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class StakeHolderController : Controller
    {
        

        private readonly IStakeHolderRepository _stakeHolderRepository;

        public StakeHolderController(IStakeHolderRepository stakeHolderRepository)
        {
            _stakeHolderRepository = stakeHolderRepository;
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: StakeHolder
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            var stakeHolders = await _stakeHolderRepository.GetAllStakeHoldersAsync();
            return View(stakeHolders);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: StakeHolder/Details/5
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
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: StakeHolder/Create
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create()
        {
            var stkhold = await _stakeHolderRepository.GetAllStakeHoldersAsync();
            return View(stkhold);
            
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // POST: StakeHolder/Create
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_mStakeHolder stakeHolder)
        {
            if (ModelState.IsValid)
            {
                await _stakeHolderRepository.AddStakeHolderAsync(stakeHolder);
                return RedirectToAction(nameof(Index));
            }
            return View(stakeHolder);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: StakeHolder/Edit/5
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
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // POST: StakeHolder/Edit/5
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_mStakeHolder stakeHolder)
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
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // GET: StakeHolder/Delete/5
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
        ///Created and Reviewed by : Sub Maj Sanal
        //Reviewed Date : 31 Jul 23
        // POST: StakeHolder/Delete/5
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _stakeHolderRepository.DeleteStakeHolderAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}

