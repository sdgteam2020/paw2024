using Microsoft.EntityFrameworkCore;
using swas.BAL.DTO;
using swas.BAL.Interfaces;
using swas.DAL;
using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace swas.BAL.Repository
{
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
         
          
            && _dbContext.Projects.Any(proj => proj.IsActive == true
            && proj.CurrentPslmId == psm.PsmId))
           .Join(_dbContext.ProjStakeHolderMov,
            comment => comment.PsmId,
            proj => proj.PsmId,
            (comment, proj) => new { Comment = comment, Proj = proj })
          
           .GroupBy(joined => joined.Comment.PsmId)
           .Where(group => group.Any())
           .CountAsync();
           return totalCount;
         }
        

      

        public async Task<int> GetNotificationInboxCount()
        {
            int notificationCount = await _dbContext.Notification
                .Where(n => n.NotificationType == 2 && n.IsRead == false)
                .CountAsync();
            return notificationCount;
        }

        
  

        public async Task<List<Notification>> GetNotificationInbox(int ProjId)
        {
            var commentData = await (from projMov in _dbContext.ProjStakeHolderMov
                                     join project in _dbContext.Projects on projMov.ProjId equals project.ProjId
                                     where projMov.ProjId == ProjId
                                     orderby projMov.PsmId descending
                                     select new
                                     {
                                         projMov.ProjId,
                                         NotificationFrom = projMov.FromUnitId,
                                         NotificationTo = projMov.ToUnitId,
                                         projMov.IsRead
                                     })
                                    .Take(1)
                                    .FirstOrDefaultAsync(); // Use async version
            if (commentData == null)
            {
                return new List<Notification>(); // Return an empty list if no data is found
            }
            var notifications = new List<Notification>
    {
        new Notification
        {
            ProjId = commentData.ProjId,
            NotificationFrom = commentData.NotificationFrom,
            NotificationTo = commentData.NotificationTo,
            IsRead = commentData.IsRead,
            ReadDateTime = DateTime.Now,
            NotificationType = 2

        }
    };
            await _dbContext.Notification.AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();

            return notifications;
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
