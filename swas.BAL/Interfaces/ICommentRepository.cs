using swas.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Interfaces
{
    public interface ICommentRepository
    {
        Task<int> AddCommentAsync(tbl_Comment comment);
        Task<tbl_Comment> GetCommentByIdAsync(int commentId);
        Task<List<tbl_Comment>> GetAllCommentsAsync();
        Task<bool> UpdateCommentAsync(tbl_Comment comment);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<tbl_Comment> GetCommentByPsmIdAsync(int? PsmId);

        Task<int> CountCommentAsync(int stkhol);
        Task<List<Notification>> GetNotificationInbox (int ProjId);
        Task<int> GetNotificationInboxCount();
    }

}
