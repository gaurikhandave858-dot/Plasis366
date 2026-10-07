using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(
            INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _notificationRepository.GetAllAsync();
        }

        public async Task<Notification?> GetByIdAsync(long notificationId)
        {
            return await _notificationRepository
                .GetByIdAsync(notificationId);
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            return await _notificationRepository
                .CreateAsync(notification);
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            return await _notificationRepository
                .UpdateAsync(notification);
        }

        public async Task<bool> DeleteAsync(long notificationId)
        {
            return await _notificationRepository
                .DeleteAsync(notificationId);
        }
    }
}

