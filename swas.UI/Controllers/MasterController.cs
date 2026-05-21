using System.Configuration;
using System.Security.Cryptography;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Superpower.Model;
using swas.BAL;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL.Models;

namespace swas.UI.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDdlRepository _dlRepository;
        private readonly IStkStatusRepository _stkStatusRepository;
        private readonly IStagesRepository _stagesRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IActionsRepository _actionsRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IConfiguration _configuration;
        public MasterController(IHttpContextAccessor httpContextAccessor, IDdlRepository ddlRepository,
            IStkStatusRepository stkStatusRepository, IStagesRepository stagesRepository, IConfiguration configuration, IStatusRepository statusRepository, IActionsRepository actionsRepository, IUnitRepository unitRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _dlRepository = ddlRepository;
            _stkStatusRepository = stkStatusRepository;
            _stagesRepository = stagesRepository;
            _statusRepository = statusRepository;
            _actionsRepository = actionsRepository;
            _unitRepository = unitRepository;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region Master Table For DDL
        public async Task<IActionResult> GetStagebyStakeHolderId(string encrypted_Payload)
        {

            if (string.IsNullOrWhiteSpace(encrypted_Payload))
            {

                return BadRequest(new { success = false, message = "Invalid request." });
            }

            int id = 0;
            int ParentId = 0;
            int StakeHolderId = 0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(
                _httpContextAccessor.HttpContext.Session, "User");
            var cryptoKey = Logins.CryptoKey;

            if (string.IsNullOrWhiteSpace(cryptoKey))
            {

                return StatusCode(500, new { success = false, message = "Server configuration error." });
            }

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(encrypted_Payload, cryptoKey);

                if (string.IsNullOrWhiteSpace(decrypted))
                {

                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                var obj = JsonConvert.DeserializeObject<dynamic>(decrypted.Trim('"'));
                id = Convert.ToInt32(obj.id);
                ParentId = Convert.ToInt32(obj.ParentId);
                StakeHolderId = Convert.ToInt32(obj.StakeHolderId);

               
            }
            catch (CryptographicException ex)
            {

                return BadRequest(new { success = false, message = "Invalid encrypted data." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "Internal server error." });
            }


            try
            {

                if (StakeHolderId == 100)
                {
                    return Json(await _statusRepository.GetAllbyParentId(ParentId));
                }
                else if (StakeHolderId == Logins.unitid)
                    return Json(await _statusRepository.GetAllByStages_takeHolder(ParentId, Convert.ToInt32(Logins.unitid), true));
                else
                    return Json(await _statusRepository.GetAllByStages_takeHolder(ParentId, Convert.ToInt32(Logins.unitid), false));
            }
            catch(Exception ex)
            {
                Error.ExceptionHandle("GetStagebyStakeHolderId Controller" +ex.Message);
            }
            return null;

            
        }
        public async Task<IActionResult> GetFwdTo(string encrypted_payload)
        {
            if (string.IsNullOrWhiteSpace(encrypted_payload))
            {

                return BadRequest(new { success = false, message = "Invalid request." });
            }

            int StakeHolderId = 0;
            string Value ;
            int Type = 0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(
                _httpContextAccessor.HttpContext.Session, "User");
            var cryptoKey = Logins.CryptoKey;

            if (string.IsNullOrWhiteSpace(cryptoKey))
            {

                return StatusCode(500, new { success = false, message = "Server configuration error." });
            }

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(encrypted_payload, cryptoKey);

                if (string.IsNullOrWhiteSpace(decrypted))
                {

                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                var obj = JsonConvert.DeserializeObject<dynamic>(decrypted.Trim('"'));
                Value = obj.Value;
                if (obj == null || !int.TryParse((string?)obj.id, out StakeHolderId))
                {

                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (CryptographicException ex)
            {

                return BadRequest(new { success = false, message = "Invalid encrypted data." });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "Internal server error." });
            }



            return Json(await _dlRepository.GetFwdTo(StakeHolderId, (int)Logins.unitid, Value, Type));


        }
       
        public async Task<IActionResult> GetAllMasterTableforddl(string encrypte_data)
        {

            if (string.IsNullOrWhiteSpace(encrypte_data))
            {
              
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            int id=0;
            int ParentId=0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(
                   _httpContextAccessor.HttpContext.Session, "User");
            var cryptoKey = Logins.CryptoKey;

            if (string.IsNullOrWhiteSpace(cryptoKey))
            {
                
                return StatusCode(500, new { success = false, message = "Server configuration error." });
            }

            try
            {
                string decrypted = CryptoHelper.SafeDecrypt(encrypte_data, cryptoKey);

                if (string.IsNullOrWhiteSpace(decrypted))
                {
                   
                    return BadRequest(new { success = false, message = "Invalid request data." });
                }

                var obj = JsonConvert.DeserializeObject<dynamic>(decrypted.Trim('"'));
                if (obj == null ||!int.TryParse((string?)obj.id, out id) || !int.TryParse((string?)obj.ParentId, out ParentId))
                {
                  
                    return BadRequest(new { success = false, message = "Invalid identifier." });
                }
            }
            catch (CryptographicException ex)
            {

                return BadRequest(new { success = false, message = "Invalid encrypted data." });
            }
            catch (Exception ex)
            {
             
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
          
            try
            {
                List<DTODDLComman> lst = new List<DTODDLComman>();
                
                if (id == Mastertablenmumcs.Unit)
                {
                    var ret = await _dlRepository.ddlLimitUnit(Logins.unitid, 0);

                    foreach (var cmd in ret)
                    {

                        DTODDLComman db = new DTODDLComman();
                        db.Name = cmd.UnitName;
                        db.Id = cmd.unitid;
                        lst.Add(db);
                    }
                    return Json(lst);
                }
                else if (id == Mastertablenmumcs.HostType)
                {
                    var ret = await _dlRepository.ddlmHostType(0);
                    foreach (var cmd in ret)
                    {

                        DTODDLComman db = new DTODDLComman();
                        db.Name = cmd.HostingDesc;
                        db.Id = cmd.HostTypeID;
                        lst.Add(db);
                    }
                    return Json(lst);


                }
                else if (id == Mastertablenmumcs.AppType)
                {
                    var ret = await _dlRepository.DdlAppType();

                    foreach (var cmd in ret)
                    {

                        DTODDLComman db = new DTODDLComman();
                        db.Name = cmd.AppDesc;
                        db.Id = cmd.Apptype;
                        lst.Add(db);
                    }
                    return Json(lst);

                }
                else if (id == Mastertablenmumcs.stkStatus)
                {
                    var ret = await _stkStatusRepository.GetAll();

                    foreach (var cmd in ret)
                    {

                        DTODDLComman db = new DTODDLComman();
                        db.Name = cmd.Status;
                        db.Id = cmd.StkStatusId;
                        lst.Add(db);
                    }
                    return Json(lst);

                }
                else if (id == Mastertablenmumcs.mStages)
                {
                    var ret = await _stagesRepository.GetAll();

                    foreach (var cmd in ret)
                    {

                        DTODDLComman db = new DTODDLComman();
                        db.Name = cmd.Stages;
                        db.Id = cmd.StagesId;
                        lst.Add(db);
                    }
                    return Json(lst);

                }
                else if (id == Mastertablenmumcs.mStatus)
                {
                    if (ParentId == 0)
                    {
                        var ret = await _statusRepository.GetAll();


                        return Json(ret);
                    }
                    else
                    {
                        var ret = await _statusRepository.GetAllByStages_takeHolder(ParentId, Convert.ToInt32(Logins.unitid), false);


                        return Json(ret);

                    }


                }
                else if (id == Mastertablenmumcs.mActions)
                {
                    var ret = await _actionsRepository.GetActionByStatusIdlogin(ParentId, Convert.ToInt32(Logins.unitid));
                    return Json(ret);

                }
                else if (id == Mastertablenmumcs.ProjMovement_mActions)
                {
                    var ret = await _actionsRepository.ProjMovement_GetActionByStatusIdlogin(ParentId, Convert.ToInt32(Logins.unitid));
                    return Json(ret);

                }


                else if (id == Mastertablenmumcs.mMappingActionsException)
                {
                    var ret = await _actionsRepository.GetActionByStatusId(ParentId);
                    return Json(ret);
                }
                {
                    var ret = await _unitRepository.GetAllUnitNotDte();
                    return Json(ret);
                }
                return Json(null);
            }

            catch (Exception ex)
            {
                return Json(nmum.Exception);
            }
        }

        #endregion
    }
}
