using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using swas.BAL.Interfaces;
 

namespace swas.UI.Controllers
{
    public class SoftwareTypeController : Controller
    {
        private readonly ISoftwareTypeRepository _SoftwareRepo ;

        public SoftwareTypeController(ISoftwareTypeRepository softwareTypeRepository)
        {
            _SoftwareRepo = softwareTypeRepository;
        }

    }
}
