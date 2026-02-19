using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.BAL.Utility;
using swas.DAL.Models;
using System.Security.Claims;

namespace swas.UI.Controllers
{
    public class ChatSignalRController : Controller
    {

        private readonly IUserMapChatRepository _userMapChatRepository;
        private readonly ITrnChatMsgRepository _trnChatMsg;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatSignalRController(
            UserManager<ApplicationUser> userManager,
            IUserMapChatRepository userMapChatRepository,
            IHttpContextAccessor httpContextAccessor,
            ITrnChatMsgRepository trnChatMsg,
            IHubContext<ChatHub> hubContext
            )
        {
            _userMapChatRepository = userMapChatRepository;
            _trnChatMsg = trnChatMsg;
            _hubContext = hubContext;
        }
    }
}
