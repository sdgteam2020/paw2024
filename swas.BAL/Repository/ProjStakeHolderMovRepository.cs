using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace swas.BAL.Repository
{

    ///Created and Reviewed by : Sub Maj M Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class ProjStakeHolderMovRepository : IProjStakeHolderMovRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtector _dataProtector;

        public ProjStakeHolderMovRepository(ApplicationDbContext dbContext, IDataProtectionProvider DataProtector, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _dataProtector = DataProtector.CreateProtector("swas.UI.Controllers.ProjectsController");

        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 15,16 & 31 Jul 23
        // Reviewed Date : 27 Aug 23, 02,03, 09 & 10 Oct 23  ---   Flags Error Rectified
        //  Data Transfer Error Rectified...   Get Request error by remove antifurg....  carried out
        //  outside user access error rectified
        //   model validation removed

        public async Task<int> AddProjStakeHolderMovAsync(Projmove psmove)
        {

            int lastpsmid = 0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            tbl_ProjStakeHolderMov projmove = new tbl_ProjStakeHolderMov();
            if (psmove.ProjMov.AddRemarks == null)
            {
                psmove.ProjMov.AddRemarks = "Proj Out from : " + Logins.Unit;
            }
            if (psmove.ProjMov.Comments == null)
            {
                psmove.ProjMov.Comments = "Proj Out from : " + Logins.Unit;
            }
            projmove = psmove.ProjMov;

            projmove.CurrentStakeHolderId = psmove.ProjMov.StakeHolderId;
            //projmove.ToStakeHolderId = psmove.ProjMov.StakeHolderId;

            projmove.FromStakeHolderId = Logins.unitid ?? 0;
            projmove.ProjId = psmove.DataProjId ?? 0;
            projmove.PsmId = psmove.PsmId ?? 0;
            tbl_Comment cmt = new tbl_Comment();
            projmove.UpdatedByUserId = Logins.unitid;
            projmove.DateTimeOfUpdate = DateTime.Now;
            projmove.IsActive = true;
            projmove.EditDeleteDate = DateTime.Now;
            projmove.EditDeleteBy = Logins.unitid;
            projmove.TimeStamp = DateTime.Now;
            // projmove.ActionCde = 0;
            // _dbContext.ProjStakeHolderMov.Add(projmove);

            if (projmove.ActionCde == 2)
            {

                projmove.CurrentStakeHolderId = Logins.unitid ?? 0;

            }
            else
            {

                projmove.ActionCde = 0;
            }


            tbl_Projects? pjct = await _dbContext.Projects.FirstOrDefaultAsync(a => a.ProjId == psmove.DataProjId);
            if (pjct != null)
            {
                lastpsmid = pjct.CurrentPslmId;

                tbl_ProjStakeHolderMov? psmovelast = await _dbContext.ProjStakeHolderMov.FirstOrDefaultAsync(a => a.PsmId == lastpsmid);
                if (psmovelast != null)
                {
                    psmovelast.TostackholderDt = DateTime.Now;
                    psmovelast.ToStakeHolderId = psmove.ProjMov.CurrentStakeHolderId;
                    projmove.EditDeleteDate = psmovelast.TimeStamp;
                    _dbContext.ProjStakeHolderMov.Update(psmovelast);
                    await _dbContext.SaveChangesAsync();
                }

                _dbContext.ProjStakeHolderMov.Add(projmove);
                await _dbContext.SaveChangesAsync();


                pjct.CurrentPslmId = projmove.PsmId;
                _dbContext.Projects.Update(pjct);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _dbContext.ProjStakeHolderMov.Add(projmove);
                await _dbContext.SaveChangesAsync();

            }

            cmt.Comment = projmove.Comments;
            cmt.EditDeleteDate = DateTime.Now;
            cmt.IsDeleted = false;
            cmt.IsActive = true;
            cmt.DateTimeOfUpdate = DateTime.Now;
            cmt.EditDeleteBy = projmove.FromStakeHolderId; /*+ "(" + Logins.unitid + ")"*/
            cmt.PsmId = projmove.PsmId;
            cmt.UpdatedByUserId = Logins.unitid;
            _dbContext.Comment.Add(cmt);
            await _dbContext.SaveChangesAsync();
            if (psmove.Atthistory[0].AttPath != null)
            {
                tbl_AttHistory atthis = new tbl_AttHistory();
                atthis.AttPath = psmove.Atthistory[0].AttPath;
                atthis.ActFileName = psmove.Atthistory[0].ActFileName;
                atthis.PsmId = projmove.PsmId;
                atthis.UpdatedByUserId = Logins.unitid;
                atthis.DateTimeOfUpdate = DateTime.Now;
                atthis.IsDeleted = false;
                atthis.IsActive = true;
                atthis.EditDeleteBy = Logins.unitid;
                atthis.ActionId = 1;
                atthis.TimeStamp = DateTime.Now;
                atthis.ActionId = 0;
                atthis.EditDeleteDate = DateTime.Now;
                atthis.Reamarks = psmove.ProjMov.AttRemarks ?? "File Attached";
                _dbContext.AttHistory.Add(atthis);
                await _dbContext.SaveChangesAsync();
            }
            psmove.PsmId = projmove.PsmId;
            return projmove.PsmId;
        }







        public async Task<int> RetWithObsnMovAsync(Projmove psmove)
        {

            int lastpsmid = 0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            tbl_ProjStakeHolderMov projmove = new tbl_ProjStakeHolderMov();
            if (psmove.ProjMov.AddRemarks == null)
            {
                psmove.ProjMov.AddRemarks = "Proj Out from : " + Logins.Unit;
            }
            if (psmove.ProjMov.Comments == null)
            {
                psmove.ProjMov.Comments = "Proj Out from : " + Logins.Unit;
            }
            projmove = psmove.ProjMov;

            projmove.CurrentStakeHolderId = psmove.ProjMov.StakeHolderId;
            //projmove.ToStakeHolderId = psmove.ProjMov.StakeHolderId;

            projmove.FromStakeHolderId = Logins.unitid ?? 0;
            projmove.ProjId = psmove.DataProjId ?? 0;
            tbl_Comment cmt = new tbl_Comment();
            projmove.UpdatedByUserId = Logins.unitid;
            projmove.DateTimeOfUpdate = DateTime.Now;
            projmove.IsActive = true;
            projmove.EditDeleteDate = DateTime.Now;
            projmove.EditDeleteBy = Logins.unitid;
            projmove.TimeStamp = DateTime.Now;
            projmove.ActionCde = 1;



            tbl_Projects? pjct = await _dbContext.Projects.FirstOrDefaultAsync(a => a.ProjId == psmove.DataProjId);
            if (pjct != null)
            {
                lastpsmid = pjct.CurrentPslmId;

                tbl_ProjStakeHolderMov? psmovelast = await _dbContext.ProjStakeHolderMov.FirstOrDefaultAsync(a => a.PsmId == lastpsmid);
                if (psmovelast != null)
                {
                    psmovelast.TostackholderDt = DateTime.Now;
                    psmovelast.CurrentStakeHolderId = psmove.ProjMov.CurrentStakeHolderId;
                    projmove.EditDeleteDate = psmovelast.TimeStamp;
                    _dbContext.ProjStakeHolderMov.Update(psmovelast);
                    await _dbContext.SaveChangesAsync();
                }

                _dbContext.ProjStakeHolderMov.Add(projmove);
                await _dbContext.SaveChangesAsync();


                pjct.CurrentPslmId = projmove.PsmId;
                _dbContext.Projects.Update(pjct);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                _dbContext.ProjStakeHolderMov.Add(projmove);
                await _dbContext.SaveChangesAsync();

            }

            cmt.Comment = projmove.Comments;
            cmt.EditDeleteDate = DateTime.Now;
            cmt.IsDeleted = false;
            cmt.IsActive = true;
            cmt.DateTimeOfUpdate = DateTime.Now;
            cmt.EditDeleteBy = Logins.unitid;
            cmt.PsmId = projmove.PsmId;
            cmt.UpdatedByUserId = Logins.unitid;
            _dbContext.Comment.Add(cmt);
            await _dbContext.SaveChangesAsync();
            if (psmove.Atthistory[0].AttPath != null)
            {
                tbl_AttHistory atthis = new tbl_AttHistory();
                atthis.AttPath = psmove.Atthistory[0].AttPath;
                atthis.ActFileName = psmove.Atthistory[0].ActFileName;
                atthis.PsmId = projmove.PsmId;
                atthis.UpdatedByUserId = Logins.unitid;
                atthis.DateTimeOfUpdate = DateTime.Now;
                atthis.IsDeleted = false;
                atthis.IsActive = true;
                atthis.EditDeleteBy = Logins.unitid;
                atthis.ActionId = 1;
                atthis.TimeStamp = DateTime.Now;
                atthis.ActionId = 0;
                atthis.EditDeleteDate = DateTime.Now;
                atthis.Reamarks = psmove.ProjMov.AttRemarks ?? "File Attached";
                _dbContext.AttHistory.Add(atthis);
                await _dbContext.SaveChangesAsync();
            }
            return projmove.PsmId;
        }



        public async Task<int> ReturnDuplProjMovAsync(Projmove psmove)
        {

            int lastpsmid = 0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            tbl_ProjStakeHolderMov projmove = new tbl_ProjStakeHolderMov();
            if (psmove.ProjMov.AddRemarks == null)
            {
                psmove.ProjMov.AddRemarks = "Proj Return & Closed by : " + Logins.Unit;
            }
            if (psmove.ProjMov.Comments == null)
            {
                psmove.ProjMov.Comments = "Found Duplicate Project  : " + Logins.Unit;
            }
            projmove = psmove.ProjMov;

            projmove.CurrentStakeHolderId = psmove.ProjMov.StakeHolderId;

            projmove.FromStakeHolderId = Logins.unitid ?? 0;
            projmove.ProjId = psmove.DataProjId ?? 0;
            tbl_Comment cmt = new tbl_Comment();
            projmove.UpdatedByUserId = Logins.unitid;
            projmove.DateTimeOfUpdate = DateTime.Now;
            projmove.IsActive = true;
            projmove.EditDeleteDate = DateTime.Now;
            projmove.EditDeleteBy = Logins.unitid;
            projmove.TimeStamp = DateTime.Now;
            // projmove.ActionCde = 0;
            // _dbContext.ProjStakeHolderMov.Add(projmove);

            projmove.ActionCde = 999;


            tbl_Projects? pjct = await _dbContext.Projects.FirstOrDefaultAsync(a => a.ProjId == psmove.DataProjId);
            if (pjct != null)
            {
                lastpsmid = pjct.CurrentPslmId;

                tbl_ProjStakeHolderMov? psmovelast = await _dbContext.ProjStakeHolderMov.FirstOrDefaultAsync(a => a.PsmId == lastpsmid);
                if (psmovelast != null)
                {
                    psmovelast.TostackholderDt = DateTime.Now;
                    psmovelast.ToStakeHolderId = pjct.StakeHolderId; // psmove.ProjMov.CurrentStakeHolderId;
                    projmove.EditDeleteDate = psmovelast.TimeStamp;
                    projmove.CurrentStakeHolderId = pjct.StakeHolderId;
                    projmove.ToStakeHolderId = pjct.StakeHolderId;
                    projmove.ActionCde = 999;
                    _dbContext.ProjStakeHolderMov.Update(psmovelast);
                    await _dbContext.SaveChangesAsync();
                }

                _dbContext.ProjStakeHolderMov.Add(projmove);
                await _dbContext.SaveChangesAsync();


                pjct.CurrentPslmId = projmove.PsmId;
                _dbContext.Projects.Update(pjct);
                await _dbContext.SaveChangesAsync();

                //else
                //{
                //    _dbContext.ProjStakeHolderMov.Add(projmove);
                //    await _dbContext.SaveChangesAsync();

                //}

                cmt.Comment = projmove.Comments;
                cmt.EditDeleteDate = DateTime.Now;
                cmt.IsDeleted = false;
                cmt.IsActive = true;
                cmt.DateTimeOfUpdate = DateTime.Now;
                cmt.EditDeleteBy = Logins.unitid;
                cmt.PsmId = projmove.PsmId;
                cmt.UpdatedByUserId = Logins.unitid;
                _dbContext.Comment.Add(cmt);
                await _dbContext.SaveChangesAsync();
                if (psmove.Atthistory[0].AttPath != null)
                {
                    tbl_AttHistory atthis = new tbl_AttHistory();
                    atthis.AttPath = psmove.Atthistory[0].AttPath;
                    atthis.ActFileName = psmove.Atthistory[0].ActFileName;
                    atthis.PsmId = projmove.PsmId;
                    atthis.UpdatedByUserId = Logins.unitid;
                    atthis.DateTimeOfUpdate = DateTime.Now;
                    atthis.IsDeleted = false;
                    atthis.IsActive = true;
                    atthis.EditDeleteBy = Logins.unitid;
                    atthis.ActionId = 1;
                    atthis.TimeStamp = DateTime.Now;
                    atthis.ActionId = 0;
                    atthis.EditDeleteDate = DateTime.Now;
                    atthis.Reamarks = psmove.ProjMov.AttRemarks ?? "File Attached";
                    _dbContext.AttHistory.Add(atthis);
                    await _dbContext.SaveChangesAsync();
                }
            }
            return projmove.PsmId;
        }


        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 17 Nov 23


        public async Task<int> AddProStkMovBlogAsync(Projmove psmove)
        {

            int lastpsmid = 0;
            Login Logins = SessionHelper.GetObjectFromJson<Login>(_httpContextAccessor.HttpContext.Session, "User");
            tbl_ProjStakeHolderMov projmove = new tbl_ProjStakeHolderMov();
            if (psmove.ProjMov.Comments == null)
            {
                psmove.ProjMov.Comments = psmove.ProjMov.AddRemarks + " by : " + Logins.Unit;

                psmove.ProjMov.AddRemarks = "Auto Reply Generated by PAW on behalf of : " + Logins.Unit;
            }

            projmove = psmove.ProjMov;

            projmove.CurrentStakeHolderId = 1;
            projmove.ToStakeHolderId = 1;

            projmove.FromStakeHolderId = Logins.unitid ?? 0;
            projmove.ProjId = psmove.DataProjId ?? 0;
            tbl_Comment cmt = new tbl_Comment();
            projmove.UpdatedByUserId = Logins.unitid;
            projmove.DateTimeOfUpdate = DateTime.Now;
            projmove.IsActive = true;
            projmove.EditDeleteDate = DateTime.Now;
            projmove.EditDeleteBy = Logins.unitid;
            projmove.TimeStamp = DateTime.Now;
            projmove.ActionCde = -1;
            _dbContext.ProjStakeHolderMov.Add(projmove);
            await _dbContext.SaveChangesAsync();




            //tbl_Projects? pjct = await _dbContext.Projects.FirstOrDefaultAsync(a => a.ProjId == psmove.DataProjId);
            //if (pjct != null)
            //{
            //    lastpsmid = pjct.CurrentPslmId;

            //    tbl_ProjStakeHolderMov? psmovelast = await _dbContext.ProjStakeHolderMov.FirstOrDefaultAsync(a => a.PsmId == lastpsmid);
            //    if (psmovelast != null)
            //    {
            //        psmovelast.TostackholderDt = DateTime.Now;
            //        psmovelast.ToStakeHolderId = 1;
            //        projmove.EditDeleteDate = psmovelast.TimeStamp;
            //        _dbContext.ProjStakeHolderMov.Update(psmovelast);
            //        await _dbContext.SaveChangesAsync();
            //    }

            //    _dbContext.ProjStakeHolderMov.Add(projmove);
            //    await _dbContext.SaveChangesAsync();

            //}
            //else
            //{
            //    _dbContext.ProjStakeHolderMov.Add(projmove);
            //    await _dbContext.SaveChangesAsync();

            //}

            cmt.Comment = projmove.Comments;
            cmt.EditDeleteDate = DateTime.Now;
            cmt.IsDeleted = false;
            cmt.IsActive = true;
            cmt.DateTimeOfUpdate = DateTime.Now;
            cmt.EditDeleteBy = Logins.unitid;
            cmt.PsmId = projmove.PsmId;
            cmt.UpdatedByUserId = Logins.unitid;
            _dbContext.Comment.Add(cmt);
            await _dbContext.SaveChangesAsync();
            if (psmove.Atthistory[0].AttPath != null)
            {
                tbl_AttHistory atthis = new tbl_AttHistory();
                atthis.AttPath = psmove.Atthistory[0].AttPath;
                atthis.ActFileName = psmove.Atthistory[0].ActFileName;
                atthis.PsmId = projmove.PsmId;
                atthis.UpdatedByUserId = Logins.unitid;
                atthis.DateTimeOfUpdate = DateTime.Now;
                atthis.IsDeleted = false;
                atthis.IsActive = true;
                atthis.EditDeleteBy = Logins.unitid;

                atthis.TimeStamp = DateTime.Now;
                atthis.ActionId = 0;
                atthis.EditDeleteDate = DateTime.Now;
                atthis.Reamarks = psmove.ProjMov.AttRemarks ?? "File Attached";
                _dbContext.AttHistory.Add(atthis);
                await _dbContext.SaveChangesAsync();
            }
            return projmove.PsmId;
        }



        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<tbl_ProjStakeHolderMov> GetProjStakeHolderMovByIdAsync(int psmId)
        {
            return await _dbContext.ProjStakeHolderMov.FindAsync(psmId);
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<List<tbl_ProjStakeHolderMov>> GetAllProjStakeHolderMovAsync()
        {
            return await _dbContext.ProjStakeHolderMov.ToListAsync();
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<bool> UpdateProjStakeHolderMovAsync(tbl_ProjStakeHolderMov projStakeHolderMov)
        {
            _dbContext.Entry(projStakeHolderMov).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        ///Created and Reviewed by : Sub Maj M Sanal Kumar
        // Reviewed Date : 31 Jul 23
        public async Task<bool> DeleteProjStakeHolderMovAsync(int psmId)
        {
            var projStakeHolderMov = await _dbContext.ProjStakeHolderMov.FindAsync(psmId);
            if (projStakeHolderMov == null)
                return false;

            _dbContext.ProjStakeHolderMov.Remove(projStakeHolderMov);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<int> CountinboxAsync(int stkhol)
        {
            var totalCount = _dbContext.ProjStakeHolderMov
           .Where(psm => psm.CurrentStakeHolderId == stkhol && psm.ActionCde == 1 && psm.ActionId != 48 && _dbContext.Projects.Any(proj => proj.IsActive == true && proj.CurrentPslmId == psm.PsmId))
           .Count();
            return totalCount;
        }

     
        // added by Sub Maj Sanal on 21 Nov 23
        public async Task<int> ValStatusAsync(int? ProjId)
        {
            int? maxStatusId = _dbContext.ProjStakeHolderMov
    .Where(p => p.ProjId == ProjId)
    .Select(p => (int?)p.StatusId)
    .Max();
           
            int result = maxStatusId ?? 0;

            return result;
        }
        public async Task<int> GetlaststageId(int? ProjId)
        {
            int? maxStatusId = _dbContext.ProjStakeHolderMov
    .Where(p => p.ProjId == ProjId)
    .Select(p => (int?)p.StageId)
    .Max();

            int result = maxStatusId ?? 0;

            return result;
        }



        public async Task<List<ProjLogView>> GetProjLogviewAsync(string startDate, string endDate)
        {
            List<ProjLogView> plvew;

            try
            {
                var result = from b in _dbContext.ProjStakeHolderMov
                             join c in _dbContext.Projects on b.ProjId equals c.ProjId
                             join d in _dbContext.mStages on b.StageId equals d.StagesId
                             join e in _dbContext.mStatus on b.StatusId equals e.StatusId
                             join f in _dbContext.mActions on b.ActionId equals f.ActionsId
                             join g in _dbContext.tbl_mUnitBranch on b.CurrentStakeHolderId equals g.unitid
                             join h in _dbContext.tbl_mUnitBranch on b.FromStakeHolderId equals h.unitid
                             join j in _dbContext.Comment on b.PsmId equals j.PsmId into commentGroup
                             from j in commentGroup.DefaultIfEmpty()
                             join k in _dbContext.tbl_mUnitBranch on c.StakeHolderId equals k.unitid
                             where b.EditDeleteDate >= DateTime.Parse(startDate) &&
                                   b.EditDeleteDate <= DateTime.Parse(endDate) && b.ActionCde >0
                             orderby b.ProjId, b.EditDeleteDate
                             select new
                             {
                                 b.PsmId,
                                 b.ProjId,
                                 c.ProjName,
                                 k.UnitName,
                                 d.Stages,
                                 e.Status,
                                 f.Actions,
                                 b.TimeStamp,
                                 FwdBy = h.UnitName,
                                 FwdTo = g.UnitName,
                                 j.Comment,
                                 AttDocu = string.Join(", ", _dbContext.AttHistory
                                                            .Where(a => a.PsmId == b.PsmId)
                                                            .Select(a => $"Desc: {a.Reamarks} : Docu: {a.ActFileName}")),
                                 Comments = string.Join(", ", _dbContext.StkComment
                                                            .Where(sc => sc.PsmId == b.PsmId)
                                                            .Select(sc => $"{k.UnitName} Desc: {sc.Comments} : Docu: {sc.ActFileName}")),
                                 b.AddRemarks,
                                 b.UpdatedByUserId,
                                 EncyId = _dataProtector.Protect(b.PsmId.ToString())
                             };

                // Assuming ProjLogView has the necessary properties
                var projLogViews = result.ToList().Select(x => new ProjLogView
                {
                    PsmId = x.PsmId,
                    ProjId = x.ProjId,
                    ProjName = x.ProjName,
                    UnitName = x.UnitName,
                    Stages = x.Stages,
                    Status = x.Status,
                    Actions = x.Actions,
                    TimeStamp = x.TimeStamp,
                    FwdBy = x.FwdBy,
                    FwdTo = x.FwdTo,
                    Comment = x.Comment,
                    AttDocu = x.AttDocu,
                    Comments = x.Comments,
                    AddRemarks = x.AddRemarks,
                    ActionByUser = (int)x.UpdatedByUserId,
                    EncyId = x.EncyId
                }).ToList();

                return projLogViews;
            }
            catch (Exception ex)
            {
                
               
                plvew = new List<ProjLogView>();
            }

            return plvew;
        }


    }
}
