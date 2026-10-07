using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllAsync();

        Task<Notification?> GetByIdAsync(long notificationId);

        Task<Notification> CreateAsync(Notification notification);

        Task<Notification> UpdateAsync(Notification notification);

        Task<bool> DeleteAsync(long notificationId);
    }
}
