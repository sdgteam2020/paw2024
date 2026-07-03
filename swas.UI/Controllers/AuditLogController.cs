using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
using swas.UI.Helpers;

namespace swas.UI.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly IAuditlogRepository _auditlogRepository;
        public AuditLogController(IAuditlogRepository auditlogRepository) 
        { 
            _auditlogRepository = auditlogRepository;
        }
        [AuthorizePermission("EditProjectMovement")]
        public async Task<IActionResult> Index(int ProjId)
        {
            var auditHistory = await _auditlogRepository.GetAllGetAuditLogHistory(ProjId);

            return PartialView("_AuditHistoryPartial", auditHistory);
            //return View(auditHistory);
           
        }
    }
}
