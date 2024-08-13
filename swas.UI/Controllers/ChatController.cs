using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using System.Security.Claims;

namespace swas.UI.Controllers
{
    public class ChatController : Controller
    {
       
        private readonly IUserMapChatRepository _userMapChatRepository;
        private readonly ITrnChatMsgRepository  _trnChatMsg;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatController(
            UserManager<ApplicationUser> userManager,
            IUserMapChatRepository userMapChatRepository,
            IHttpContextAccessor httpContextAccessor,
            ITrnChatMsgRepository trnChatMsg
            )
        {
          
            _userManager = userManager;
            _userMapChatRepository=userMapChatRepository;
            _httpContextAccessor = httpContextAccessor;
            _trnChatMsg=trnChatMsg;
        }
        public async Task<IActionResult> Index()
        {
			

			return View();
        }

        public async Task<IActionResult> GetAllUsers(int Id)
        {
            try
            {
				//var userdet = await _userManager.Users.ToListAsync();
				string id = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
				var userdet = await _userManager.Users.ToListAsync();
				var ischat = await _trnChatMsg.GetIsChat(id);
				List<DTOApplicationUserWithChatRead> lstuser = new List<DTOApplicationUserWithChatRead>();
				if (userdet.Count > 0)
				{
					foreach (var user in userdet)
					{
						DTOApplicationUserWithChatRead db = new DTOApplicationUserWithChatRead();
						db.Id = user.Id;
						db.Rank = user.Rank;
						db.Offr_Name = user.Offr_Name;
						db.UserName = user.UserName;
						if (ischat != null && ischat.Count > 0)
						{
							DTOIsChat dTOIsChat = new DTOIsChat();
							dTOIsChat = ischat.Where(i => i.FromUserID == user.Id).FirstOrDefault();
							if (dTOIsChat != null)
							{
								db.Total = dTOIsChat.Total;
								db.CreatedOn = dTOIsChat.CreatedOn;
							}
						}
						lstuser.Add(db);
					}
				}

				return Json(lstuser.OrderByDescending(i => i.Total).OrderByDescending(i => i.CreatedOn));
			}
            catch (Exception ex) {
                return Json(nmum.Exception);
            }
		}
        public async Task<IActionResult> SaveUserMapChat(mUserMapChat mUserMapChat)
        {
            try
            {
                Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
                string id = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                mUserMapChat.FromUserId = id.ToString().ToLower();
				mUserMapChat.ToUserId = mUserMapChat.ToUserId.ToString().ToLower();
				var getret = await _userMapChatRepository.GetMapDetails(mUserMapChat);
                if (getret==null)
                {
                    var ret = await _userMapChatRepository.AddWithReturn(mUserMapChat);
                    return Json(ret);
                }else
                {
                    return Json(getret);
                }
            }
            catch (Exception ex)
            {

                return Json(nmum.Exception);
            }
        }
        public async Task<IActionResult> SaveChat(TrnChatMsg trnChatMsg)
        {
            trnChatMsg.ChatId = 0;
			trnChatMsg.CreatedOn = DateTime.Now;
            trnChatMsg.IsRead = false;
			return Json(await _trnChatMsg.AddWithReturn(trnChatMsg));
        }
        public async Task<IActionResult> GetUserMapChat(int UserMapChatId,string FromUserId)
        {
            string ToUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Json(await _trnChatMsg.GetChat(UserMapChatId, FromUserId, ToUserId));
        }
       }
}
