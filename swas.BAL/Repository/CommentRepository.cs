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

namespace swas.BAL.Repository
{

    ///Created and Reviewed by : Sub Maj Sanal
    ///Reviewed Date : 31 Jul 23
    ///Tested By :- 
    ///Tested Date : 
    ///Start
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddCommentAsync(tbl_Comment comment)
        {
            _dbContext.Comment.Add(comment);
            await _dbContext.SaveChangesAsync();
            return comment.CommentId;
        }

        public async Task<tbl_Comment> GetCommentByIdAsync(int commentId)
        {
            return await _dbContext.Comment.FindAsync(commentId);
        }
        public async Task<tbl_Comment> GetCommentByPsmIdAsync(int? PsmId)
        {
            return await _dbContext.Comment.FindAsync(PsmId);
        }


        public async Task<int> CountCommentAsync(int stkhol)
        {
            var totalCount = await _dbContext.ProjStakeHolderMov
           .Where(psm => psm.ToUnitId == stkhol
         
            && psm.ActionId != 48
            && _dbContext.Projects.Any(proj => proj.IsActive == true
            && proj.CurrentPslmId == psm.PsmId))
           .Join(_dbContext.ProjStakeHolderMov,
            comment => comment.PsmId,
            proj => proj.PsmId,
            (comment, proj) => new { Comment = comment, Proj = proj })
           .Where(joined => joined.Proj.ActionId == 5)
           .GroupBy(joined => joined.Comment.PsmId)
           .Where(group => group.Any())
           .CountAsync();
           return totalCount;
         }


        public async Task<List<Notification>> GetNotificationAsync(int stkhol)
        {
            //var Commentdata = from comment in _dbContext.StkComment
            //                  join projMov in _dbContext.ProjStakeHolderMov on comment.ProjId equals projMov.ProjId
            //                  where comment.ProjId == stkcom.ProjId
            //                  select new Notification
            //                  {
            //                      ProjId = projMov.ProjId,
            //                      NotificationFrom = projMov.FromStakeHolderId,
            //                      NotificationTo = projMov.ToStakeHolderId,

            //                  };

            //List<Notification> notifications = new List<Notification>();

            //foreach (var item in Commentdata)
            //{
            //    int projId = item.ProjId;

            //    var commentsForProj = await _dbContext.ProjStakeHolderMov.Where(sc => sc.ProjId == projId).ToListAsync();

            //    foreach (var comment in commentsForProj)
            //    {
            //        _dbContext.Notification.Add(new Notification
            //        {
            //            ProjId = projId,
            //            NotificationFrom = comment.FromStakeHolderId,
            //            NotificationTo = comment.ToStakeHolderId,
            //            ReadDateTime = DateTime.Now,

            //        });

            //        _dbContext.SaveChanges();
            //    }
            //}

            return null;
        }



        public async Task<List<tbl_Comment>> GetAllCommentsAsync()
        {
            return await _dbContext.Comment.ToListAsync();
        }

        public async Task<bool> UpdateCommentAsync(tbl_Comment comment)
        {
            _dbContext.Entry(comment).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _dbContext.Comment.FindAsync(commentId);
            if (comment == null)
                return false;

            _dbContext.Comment.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

     
    }


}
