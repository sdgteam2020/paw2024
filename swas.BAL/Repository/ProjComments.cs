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
        public ProjComments(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<List<DTOProComments>> GetAllStkForComment(int UnitId)
        {
            List<DTOProComments> lst = new List<DTOProComments>();
            var queryes = await (from proj in _context.Projects
                           join mov in _context.ProjStakeHolderMov on proj.ProjId equals mov.ProjId
                           join stakeholder in _context.tbl_mUnitBranch on proj.StakeHolderId equals stakeholder.unitid

                                 //join comment in _context.StkComment on mov.PsmId equals comment.PsmId into com
                                 //from comment in com.DefaultIfEmpty()
                                 // join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
                                 //from status in statusGroup.DefaultIfEmpty()
                                 let StkStatusId =
                            (from cr1 in _context.StkComment
                             where cr1.StakeHolderId== mov.ToUnitId && cr1.PsmId== mov.PsmId
                             select cr1.StkStatusId
                            ).Count()
                               
                                 where mov.ToUnitId==UnitId && mov.IsComment==true //&& mov.StatusId==5
                                 select new DTOProComments
                           {
                               ProjectName= proj.ProjName,
                               Stakeholder = stakeholder.UnitName,
                               Status = "",
                               StkStatusId = Convert.ToInt32(StkStatusId),
                               ProjId = proj.ProjId, 
                               PsmId = mov.PsmId,

                              
                           }).ToListAsync();

            //var queryes1 = await (from proj in _context.Projects
            //                     join stakeholder in _context.tbl_mUnitBranch on proj.StakeHolderId equals stakeholder.unitid
            //                     //join comment in _context.StkComment on mov.PsmId equals comment.PsmId into com
            //                     //from comment in com.DefaultIfEmpty()
            //                     // join status in _context.StkStatus on comment.StkStatusId equals status.StkStatusId into statusGroup
            //                     //from status in statusGroup.DefaultIfEmpty()
            //                     let StkStatusId =
            //                (from cr1 in _context.StkComment
            //                 where cr1.ProjId == proj.ProjId
            //                 select cr1.StkStatusId
            //                ).Count()

            //                     where proj.StakeHolderId==UnitId
            //                      select new DTOProComments
            //                     {
            //                         ProjectName = proj.ProjName,
            //                         Stakeholder = stakeholder.UnitName,
            //                         Status = "",
            //                         StkStatusId = Convert.ToInt32(StkStatusId),
            //                         ProjId = proj.ProjId,
            //                         PsmId = 0,


            //                     }).ToListAsync();

            lst.AddRange(queryes);
            //lst.AddRange(queryes1);
            return lst;

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
