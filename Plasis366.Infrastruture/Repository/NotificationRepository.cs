using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(long notificationId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId &&
                    x.IsActive);
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            var existingNotification = await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notification.NotificationId);

            if (existingNotification == null)
                return notification;

            existingNotification.UserId = notification.UserId;
            existingNotification.ProjectId = notification.ProjectId;
            existingNotification.Title = notification.Title;
            existingNotification.Message = notification.Message;
            existingNotification.IsRead = notification.IsRead;
            existingNotification.ReadDate = notification.ReadDate;
            existingNotification.IsActive = notification.IsActive;

            await _context.SaveChangesAsync();

            return existingNotification;
        }

        public async Task<bool> DeleteAsync(long notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId &&
                    x.IsActive);

            if (notification == null)
                return false;

            notification.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
