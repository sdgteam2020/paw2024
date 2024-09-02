using Microsoft.EntityFrameworkCore;
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
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNotificationCountByType(int Type, int? ToUnitId)
        {
            int notificationCount = await _context.Notification
                .Where(n => n.NotificationType == Type && n.IsRead == false && n.NotificationTo == ToUnitId && n.IsDeleted == false)
                .CountAsync();
            return notificationCount;
        }

        public async Task<Notification> GetNotificationByProjId(int ProjId)
        {
            return await _context.Notification
               .FirstOrDefaultAsync(a => a.ProjId == ProjId);

        }
        public async Task<Notification> GetNotifByToUnitAndType(int type, int? toUnitId, int projId)
        {
            var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.NotificationType == type && n.IsRead == false && n.NotificationTo == toUnitId && n.ProjId == projId && n.IsDeleted == false);

            return notification;
        }


        public async Task<Notification> GetNotifByToAndFormId(int type, int toUnitId, int projId, int? fromUNitId)
        {
            try
            {
                var notification = await _context.Notification
                .Where(n => n.NotificationType == type
                // && n.IsRead == true   
                && n.NotificationTo == toUnitId
                && n.ProjId == projId
                && n.NotificationFrom == fromUNitId)
                .OrderByDescending(n => n.NotificationId) 
                .FirstOrDefaultAsync();
                return notification;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<List<Notification>> GetNotifExcludingToUnit(int? unitId, int projId)
        {
            var notifications = await _context.Notification
                .Where(n => n.NotificationTo != unitId && n.ProjId == projId && n.IsDeleted == false)
                .ToListAsync();

            return notifications;
        }

        public async Task<int> AddNotification(Notification notifications)
        {
            _context.Notification.Add(notifications);
            return await _context.SaveChangesAsync();

        }
        public async Task<bool> UpdateNotification(Notification notify)
        {
            _context.Notification.Update(notify);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
