using Microsoft.AspNetCore.Mvc;

namespace swas.UI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Org.BouncyCastle.Utilities;
    using swas.BAL.DTO;
    using swas.BAL.Helpers;
    using swas.BAL.Interfaces;
    using swas.BAL.Repository;
    using swas.DAL.Models;
    using System.Threading.Tasks;
    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class ActionsController : Controller
    {
        private readonly IActionsRepository _actionsRepository;

        private readonly IStagesRepository _stagesRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActionsController(IActionsRepository actionsRepository, IHttpContextAccessor httpContextAccessor, IStagesRepository stagesRepository)
        {
            _actionsRepository = actionsRepository;
            _stagesRepository = stagesRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Actions
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";

            TempData["ipadd"] = watermarkText;
            var stages = _stagesRepository.GetAllStages();

            ViewBag.StagesList = new SelectList(stages, "StagesId", "Stages");

            var actions = await _actionsRepository.GetAllActionsAsync();
            return View(actions);
        }
        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Actions/Details/5
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var actions = await _actionsRepository.GetActionsByIdAsync(id);
            if (actions == null)
            {
                return NotFound();
            }

            return View(actions);
        }

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // GET: Actions/Create

        //public async Task<IActionResult> Create()
        //{
        //    var stages = _stagesRepository.GetAllStages(); 
        //    ViewBag.StagesList = new SelectList(stages, "StagesId", "Stages");
        //    return View();
        //}

        ///Created and Reviewed by : Sub Maj Sanal
        ///Reviewed Date : 31 Jul 23
        // POST: Actions/Create
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tbl_mActions model)
        {

            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            if (ModelState.IsValid)
            {
                model.IsDeleted = false;
                model.IsActive = true;
                model.UpdatedByUserId = Logins.ActualUserName + "(" + Logins.Unit + ")";
                model.DateTimeOfUpdate = DateTime.Now;
                model.EditDeleteDate = DateTime.Now;
                model.EditDeleteBy = Logins.ActualUserName + "(" + Logins.Unit + ")";


                await _actionsRepository.AddActionsAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }



        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var stages = _stagesRepository.GetAllStages();
            ViewBag.StagesList = new SelectList(stages, "StagesId", "Stages");

            var actions = await _actionsRepository.GetActionsByIdAsync(id);
            if (actions == null)
            {
                return NotFound();
            }
            return View(actions);
        }
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tbl_mActions model)
        {
            if (id != model.ActionsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _actionsRepository.UpdateActionsAsync(model);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var actions = await _actionsRepository.GetActionsByIdAsync(id);
            if (actions == null)
            {
                return NotFound();
            }


            TempData["DeleteMessage"] = $"Are you sure you want to delete action : {actions.Actions}?";
            TempData["DeleteId"] = id;
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _actionsRepository.DeleteActionsAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ActionSeqView()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var currentDatetime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var watermarkText = $" {ipAddress}\n  {currentDatetime}";
            TempData["ipadd"] = watermarkText;

            List<ActionsSeq> resultList =  await _actionsRepository.GetActionresp();

            string animatedView = "<div class=\"container\" id=\"container\">";

            for (int i = 0; i < resultList.Count; i++)
            {
                ActionsSeq item = resultList[i];

                animatedView += $"<div class=\"box\" id=\"box{i + 1}\" style=\"text-align: center;\">{item.ActionDesc}<br/>{item.UnitName}</div>";

                if (i < resultList.Count - 1)
                {
                    animatedView += "<div class=\"link-line\"></div>";
                }

            }

            animatedView += "</div>";

            ViewBag.AnimatedView = resultList.ToList();

            return View();
        }


    }

}
