using ASPNetCoreIdentityCustomFields.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL.Models;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;

namespace swas.UI.Controllers
{
    public class ChatController : Controller
    {

        private readonly IUserMapChatRepository _userMapChatRepository;
        private readonly ITrnChatMsgRepository _trnChatMsg;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatController> _logger;
        public ChatController(
            UserManager<ApplicationUser> userManager,
            IUserMapChatRepository userMapChatRepository,
            IHttpContextAccessor httpContextAccessor,
            ITrnChatMsgRepository trnChatMsg, IConfiguration configuration,
            ILogger<ChatController> logger
            )
        {

            _userManager = userManager;
            _userMapChatRepository = userMapChatRepository;
            _httpContextAccessor = httpContextAccessor;
            _trnChatMsg = trnChatMsg;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllUsers(int Id)
        {
            try
            {
                string id = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userdet = await _trnChatMsg.GetAllUsers();
                var ischat = await _trnChatMsg.GetIsChat(id);
                List<DTOApplicationUserWithChatRead> lstuser = new List<DTOApplicationUserWithChatRead>();
                if (userdet.Count > 0)
                {
                    foreach (var user in userdet)
                    {
                        if (id != user.Id)
                        {
                            DTOApplicationUserWithChatRead db = new DTOApplicationUserWithChatRead();
                            db.Id = user.Id;
                            db.RankName = user.RankName;
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
                            db.CreatedDate = user.CreatedDate;
                            lstuser.Add(db);
                        }
                    }
                }
                return Json(lstuser);
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "GetAllUsers");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Get All Users in ChatController.", ex, (s, e) => $"{s} - {e?.Message}");

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
                if (getret == null)
                {
                    var ret = await _userMapChatRepository.AddWithReturn(mUserMapChat);
                    return Json(ret);
                }
                else
                {
                    return Json(getret);
                }
            }
            catch (Exception ex)
            {
                int dynamicEventId = DateTime.UtcNow.Ticks.GetHashCode();
                var eventId = new EventId(dynamicEventId, "SaveUserMapChat");
                _logger.Log(LogLevel.Error, eventId, "An error occurred while on Get All SaveUserMapChat in ChatController.", ex, (s, e) => $"{s} - {e?.Message}");

                return Json(nmum.Exception);
            }
        }
        public async Task<IActionResult> SaveChat(
     TrnChatMsg trnChatMsg,
     [FromForm] string encrypted_data)
        {
            if (string.IsNullOrWhiteSpace(encrypted_data))
                return BadRequest(new { message = "Encrypted data is required." });

            var cryptoKey = _configuration["CryptoSettings:LoginKey"];

            if (string.IsNullOrWhiteSpace(cryptoKey))
                return StatusCode(500, new { message = "Encryption key not configured." });

            try
            {
                // 🔐 Decrypt
                var decryptedJson = CryptoHelper.SafeDecrypt(encrypted_data, cryptoKey);

                if (string.IsNullOrWhiteSpace(decryptedJson))
                    return BadRequest(new { message = "Invalid encrypted payload." });

                var decryptedModel = JsonConvert.DeserializeObject<TrnChatMsg>(decryptedJson);

                if (decryptedModel == null)
                    return BadRequest(new { message = "Failed to parse decrypted data." });

                // ✅ Validate required fields
                if (string.IsNullOrWhiteSpace(decryptedModel.Msg) || decryptedModel.UserMapChatId <= 0)
                    return BadRequest(new { message = "Invalid chat data." });

                // ✅ Map only allowed fields (avoid overposting)
                var chat = new TrnChatMsg
                {
                    ChatId = 0,
                    CreatedOn = DateTime.UtcNow,
                    IsRead = false,
                    Msg = decryptedModel.Msg.Trim(),
                    UserMapChatId = decryptedModel.UserMapChatId
                };

                var result = await _trnChatMsg.AddWithReturn(chat);

                return Ok(new { success = true, data = result });
            }
            catch (JsonException ex)
            {
                // JSON parsing error
                // TODO: log ex
                return BadRequest(new { message = "Invalid data format." });
            }
            catch (CryptographicException ex)
            {
                // Decryption error
                // TODO: log ex
                return BadRequest(new { message = "Decryption failed." });
            }
            catch (Exception ex)
            {
                // Unexpected error
                // TODO: log ex properly using ILogger
                return StatusCode(500, new { message = "Something went wrong." });
            }
        }
        public async Task<IActionResult> GetUserMapChat(int UserMapChatId, string FromUserId)
        {
            string ToUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Json(await _trnChatMsg.GetChat(UserMapChatId, FromUserId, ToUserId));
        }
    }
}
