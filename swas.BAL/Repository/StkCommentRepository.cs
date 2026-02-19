using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace swas.BAL.Repository
{
    public class StkCommentRepository : GenericRepositoryDL<StkComment>, IStkCommentRepository
    {
        public StkCommentRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<List<DTOProComments>> GetAllCommentBypsmId_UnitId(StkComment Data)
        {
            try
            {
                var query = await
(
    from comment in _context.StkComment
    join stakeholder in _context.tbl_mUnitBranch
        on comment.StakeHolderId equals stakeholder.unitid

    join status in _context.StkStatus
        on comment.StkStatusId equals status.StkStatusId into statusGroup
    from status in statusGroup.DefaultIfEmpty() // LEFT JOIN

    join project in _context.Projects
        on comment.ProjId equals project.ProjId

    where comment.ProjId == Data.ProjId
       || comment.PsmId == Data.PsmId

    orderby comment.DateTimeOfUpdate descending

    select new DTOProComments
    {
        Stakeholder = stakeholder.UnitName,
        Status = status != null ? status.Status : null,
        Comments = comment.Comments,
        ProjId = comment.ProjId,
        PsmId = project.CurrentPslmId,
        Date = comment.DateTimeOfUpdate,
        StkCommentId = comment.StkCommentId,
        UnitId = comment.StakeHolderId,
        Attpath = comment.Attpath,
        UserDetails = comment.UserDetails,

        ProjectName = project.ProjName,
        AdminApprovalStatus =
            _context.DateApproval.Any(d =>
                d.ProjId == comment.ProjId &&
                d.DDGIT_approval == true)
    }
).ToListAsync();
                query.ForEach(x =>
                {
                    x.UserDetails ??= "____";
                });




                return query.ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<int> GetCommentStatusByPsmiId(int psmid)
        {
            var ret = await _context.StkComment.Where(i => i.PsmId == psmid && i.StkStatusId==1).Select(i=>i.StkStatusId).FirstOrDefaultAsync();
          if(ret==null)
                return 0;
          else
            return 1;
        }

        public int IsAllowForCommentByStkStatusId(int stkStatusId)
        {
            var ret = _context.StkStatus.Where(i => i.StkStatusId == stkStatusId).FirstOrDefault().Status;
            if (ret == "Info")
                return 1;
            else
                return 0;
        }

        public async Task<DTOProComments> GetCommentByPsmid(int psmid)
        {
            var query = await (from comment in _context.StkComment
                               join stakeholder in _context.tbl_mUnitBranch on comment.StakeHolderId equals stakeholder.unitid
                               join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                               from status in statusGroup.DefaultIfEmpty() // Left Join
                               join project in _context.Projects on comment.ProjId equals project.ProjId // Assuming 'ProjId' is in the 'Stk_Comments' table
                               join legacy in _context.DateApproval on comment.ProjId equals legacy.ProjId into gj
                               from subLegacy in gj.DefaultIfEmpty() // Left Join
                               where comment.StkCommentId == psmid //&& comment.StakeHolderId==Data.StakeHolderId // && comment.StakeHolderId == stakeholderId
                               orderby comment.DateTimeOfUpdate descending
                               select new DTOProComments
                               {

                                   Stakeholder = stakeholder.UnitName,
                                   Status = status != null ? status.Status : null,
                                   StkStatusId = comment.StkStatusId,
                                   Comments = comment.Comments,
                                   ProjId = Convert.ToInt32(comment.ProjId),
                                   PsmId = comment.PsmId,
                                   Date = comment.DateTimeOfUpdate,
                                   StkCommentId = comment.StkCommentId,
                                   UnitId = comment.StakeHolderId,
                                   Attpath = comment.Attpath,
                                   UserDetails = comment.UserDetails != null ? comment.UserDetails.ToString() : "____",
                                   ProjectName = project.ProjName,
                                   AdminApprovalStatus = subLegacy != null && subLegacy.DDGIT_approval == true
                               }).FirstOrDefaultAsync();



            return query;


        }

    }
}
