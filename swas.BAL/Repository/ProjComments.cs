using Microsoft.AspNetCore.DataProtection;
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

namespace swas.BAL
{
    public class ProjComments : IProjComments
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _dataProtector;
        public ProjComments(ApplicationDbContext applicationDbContext, IDataProtectionProvider dataProtector)
        {
            _context = applicationDbContext;
            _dataProtector = dataProtector.CreateProtector("swas.UI.Controllers.ProjectsController"); 
        }
        public async Task<List<DTOProComments>> GetAllStkForComment(int UnitId)
         {
            List<DTOProComments> lst = new List<DTOProComments>();
            var queryes = await (from proj in _context.Projects
                                 join mov in _context.ProjStakeHolderMov on proj.ProjId equals mov.ProjId
                                 join com in _context.StkComment on proj.ProjId equals com.ProjId
                                 join stakeholder in _context.tbl_mUnitBranch on proj.StakeHolderId equals stakeholder.unitid
                                 let StkStatusId =
                             (from cr1 in _context.StkComment
                              join Stdkst in _context.StkStatus on cr1.StkStatusId equals Stdkst.StkStatusId
                              where cr1.StakeHolderId == mov.ToUnitId && cr1.PsmId == mov.PsmId
                              orderby cr1.StkCommentId descending
                              select cr1.StkStatusId
                             ).FirstOrDefault()/*.Count()*/
                                 where mov.ToUnitId == UnitId && mov.IsComment == true //&& mov.StatusId==5
                                
                                 group new DTOProComments 
                                 {
                                     ProjectName = proj.ProjName,
                                     Stakeholder = stakeholder.UnitName,
                                     Status = "",
                                     StkStatusId = Convert.ToInt32(StkStatusId),
                                     //StkStatusId = comment.StkStatusId,
                                     ProjId = proj.ProjId,
                                     PsmId = mov.PsmId,
                                     EncyID = _dataProtector.Protect(proj.ProjId.ToString()),
                                     //TimeStamp = _context.StkComment.Where(i => i.StkStatusId == StkStatusId).Select(i => i.DateTimeOfUpdate).SingleOrDefault()?? mov.TimeStamp,
                                     TimeStamp= mov.TimeStamp,
                                     UnitId = mov.ToUnitId,
                                     IsComment = mov.IsRead ,
                                     StkCommentId = com.StkCommentId
                                 }
                                  by new { proj.ProjId, mov.ToUnitId } into g  // Group by both ProjId and ToUnitId
                                 select g.First()).ToListAsync();
            lst.AddRange(queryes);
            var data = lst.OrderByDescending(x => x.StkCommentId).ToList();
            //return lst.OrderByDescending(i=>i.TimeStamp).ToList(); change for the latest on top based on the discussion with Jasjeet singh
            return data;

        }



        public async Task<DTOProComments> GetCommentStatus(int UnitId)
        {
            var tempFileName = await (from comment in _context.StkComment
                                      join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                                      from status in statusGroup.DefaultIfEmpty()
                                      where comment.StakeHolderId == UnitId
                                      select new DTOProComments
                                      {
                                          Status = status.Status,
                                          StkStatusId = comment.StkStatusId,
                                      }).OrderByDescending(c => c.StkCommentId).SingleOrDefaultAsync();
            // Where(comment.StakeHolderId == UnitId)
            //.OrderByDescending(x => x.StkCommentId)
            //.Take(1)
            //.Select(x => x.StkStatusId)
            //.ToList()
            //.FirstOrDefault();
            return null;
        }
    }
}
