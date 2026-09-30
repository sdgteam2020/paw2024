using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using swas.UI.Helpers;
using static swas.DAL.Models.LegacyHistory;



namespace swas.BAL.Repository
{
    public class RemainderRepository : IRemainder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDataProtector _dataProtector;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjStakeHolderMovRepository _projStakeHolderMovRepository;
        public RemainderRepository(ApplicationDbContext context, IProjStakeHolderMovRepository projStakeHolderMovRepository, IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtector)
        {
            _dbContext = context;

            _httpContextAccessor = httpContextAccessor;
            _projStakeHolderMovRepository = projStakeHolderMovRepository;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");
        }
        public async Task<int> AddRemainder(int Projid, int Psmid, int fromUnitId, int toUnitId, string remarks, string userDetails)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            string domain = Logins.Unit +
                " "+ Logins.UserName.Trim() +
               "(" + Logins.Rank.Trim() +
               " " + Logins.Offr_Name.Trim()+ ")" ;


            var tblRemainder = new trnRemainder
            {
                Projid = Projid,
                Psmid = Psmid,
                FromUnitId = fromUnitId,
                Tounitid = toUnitId,
                Remarks = remarks,
                UserDetails = domain,
                SendDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsRemainder = true,
                IsRead = false
            };

                _dbContext.TrnRemainders.Add(tblRemainder);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Provides data for the GetAllAsync operation.
        /// <param name="projectSearchTypeId">The value used by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        /// </summary>
        public async Task<List<RemainderDisplayDto>> GetAllAsync()
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");

            try
            {
                int unitId = Logins.unitid ?? 0;

                var ret = await (from rem in _dbContext.TrnRemainders
                                 join proj in _dbContext.Projects
                                     on rem.Projid equals proj.ProjId

                                 join fromUnit in _dbContext.tbl_mUnitBranch
                                     on rem.FromUnitId equals fromUnit.unitid into fromUnitJoin
                                 from fromUnit in fromUnitJoin.DefaultIfEmpty()

                                 join toUnit in _dbContext.tbl_mUnitBranch
                                     on rem.Tounitid equals toUnit.unitid into toUnitJoin
                                 from toUnit in toUnitJoin.DefaultIfEmpty()

                                 join stakeholder in _dbContext.tbl_mUnitBranch
                                     on proj.StakeHolderId equals stakeholder.unitid

                                 where (rem.Tounitid == unitId || rem.FromUnitId == unitId)
                                    //&& (
                                    //       projectSearchTypeId == 1
                                    //    || (projectSearchTypeId == 2 && proj.Is_AI_ML != true)
                                    //    || (projectSearchTypeId == 3 && proj.Is_AI_ML == true)
                                    //   )

                                 select new
                                 {
                                     rem.ReadDate,
                                     rem.RemainderId,
                                     rem.Remarks,
                                     rem.Projid,
                                     rem.Psmid,
                                     ProjName = proj.ProjName,
                                     UnitName = stakeholder.UnitName,
                                     IsAI_ML = proj.Is_AI_ML,
                                     Sponsor = proj.Sponsor,
                                     FromUnitName = fromUnit != null ? fromUnit.UnitName : "N/A",
                                     ToUnitName = toUnit != null ? toUnit.UnitName : "N/A",
                                     rem.UserDetails,
                                     rem.SendDate
                                 })
                                .GroupBy(x => x.Projid)   // Group by Projid (safer than ProjName)
                                .Select(g => new RemainderDisplayDto
                                {
                                    // Take the latest reminder (by SendDate) for each project
                                    ReadOn = g.OrderByDescending(x => x.SendDate)
                                              .Select(x => x.ReadDate)
                                              .FirstOrDefault() ?? "-",

                                    Psmid = g.First().Psmid,
                                    projid = g.Key,                          // Projid
                                    aiml = g.First().IsAI_ML ?? false,
                                    ProjName = g.First().ProjName,
                                    unitName = g.First().UnitName,

                                    // Correct mapping
                                    Sponsor = g.First().Sponsor,
                                    Domain = g.First().UserDetails,          // if you still need Domain for something

                                    Remarks = g.OrderByDescending(x => x.SendDate)
                                               .Select(x => x.Remarks)
                                               .FirstOrDefault(),

                                    FromUnit = g.OrderByDescending(x => x.SendDate)
                                                .Select(x => x.FromUnitName)
                                                .FirstOrDefault(),

                                    ToUnit = g.OrderByDescending(x => x.SendDate)
                                              .Select(x => x.ToUnitName)
                                              .FirstOrDefault(),

                                    SentOn = g.Max(x => x.SendDate),

                                    EncyID = _dataProtector.Protect(g.Key.ToString())
                                })
                                .OrderByDescending(x => x.SentOn)
                                .ToListAsync();

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<List<RemainderDisplayDto>> ProjectRemainderMovHistory(int? ProjectId)
        {
            try
            {

                if (!ProjectId.HasValue)
                {
                    throw new ArgumentNullException(nameof(ProjectId), "ProjectId cannot be null.");
                }
                
                var history = await (from Rm in _dbContext.TrnRemainders
                                     join p in _dbContext.Projects on Rm.Projid equals p.ProjId
                                     join u in _dbContext.tbl_mUnitBranch on Rm.FromUnitId equals u.unitid
                                     join f in _dbContext.tbl_mUnitBranch on Rm.Tounitid equals f.unitid
                                     join stakeholder in _dbContext.tbl_mUnitBranch
                              on p.StakeHolderId equals stakeholder.unitid
                                     where Rm.Projid == ProjectId
                                     orderby Rm.SendDate descending
                                     select new RemainderDisplayDto
                                     {

                                         projid = Rm.Projid,
                                         ProjName = p.ProjName,
                                         SentOn = Rm.SendDate ,
                                         Remarks = Rm.Remarks ,
                                         userDetails = Rm.UserDetails ,
                                         ReadOn = Rm.ReadDate ,
                                         FromUnit = u.UnitName ,
                                         ToUnit = f.UnitName ,
                                         Sponsor = p.Sponsor,
                                         unitName = stakeholder.UnitName,
                                         TouserDetails = Rm.ToUserDetails
                                         
                                     }).ToListAsync();

                return history;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task<int> GetProjectById(string? ProjName)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateReaminderRead(int? projectId, int pullback)
        {
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            var latestpsmid = _projStakeHolderMovRepository.GetLastRecProjectMov(projectId);
            try
            {


                if(pullback ==1)
                {
					var latestpsmiddata = _dbContext.ProjStakeHolderMov.Find(latestpsmid);
					latestpsmiddata.IsRead = true;

					_dbContext.ProjStakeHolderMov.Update(latestpsmiddata);

					var remainders = await _dbContext.TrnRemainders
						.Where(r => r.Projid == projectId && r.ReadDate == null)
						.ToListAsync();




					string domain = Logins.Unit +
						" " + Logins.UserName.Trim() +
					   "(" + Logins.Rank.Trim() +
					   " " + Logins.Offr_Name.Trim() + ")";


					if (remainders == null || !remainders.Any())
					{
						return 0; // No records found
					}

					foreach (var remainder in remainders)
					{
						remainder.ReadDate = DateTime.Now.ToString();  // Assuming Readdate is DateTime
						remainder.IsRead = true;
						remainder.IsRemainder = false;
						remainder.ToUserDetails = "PullBack Before Read";
                        remainder.Remarks = "PullBack By : " + domain;
					}

					_dbContext.TrnRemainders.UpdateRange(remainders);
					return await _dbContext.SaveChangesAsync();
				}
                else
                {
				

					var remainders = await _dbContext.TrnRemainders
						.Where(r => r.Projid == projectId && r.ReadDate == null && r.Tounitid == Logins.unitid)
						.ToListAsync();




					string domain = Logins.Unit +
						" " + Logins.UserName.Trim() +
					   "(" + Logins.Rank.Trim() +
					   " " + Logins.Offr_Name.Trim() + ")";


					if (remainders == null || !remainders.Any())
					{
						return 0; // No records found
					}
                    var latestpsmiddata = _dbContext.ProjStakeHolderMov.Find(latestpsmid);
                    latestpsmiddata.IsRead = true;

                    _dbContext.ProjStakeHolderMov.Update(latestpsmiddata);

                    foreach (var remainder in remainders)
					{
						remainder.ReadDate = DateTime.Now.ToString();  // Assuming Readdate is DateTime
						remainder.IsRead = true;
						remainder.IsRemainder = false;
						remainder.ToUserDetails = domain;
					}

					_dbContext.TrnRemainders.UpdateRange(remainders);
					return await _dbContext.SaveChangesAsync();
				}
            }catch(Exception ex)
            {
                throw;
            }

           
        }

    }
}





















