using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.API
{
    // Small helpers for creating notifications. They only add rows to the context,
    // so the caller's own SaveChangesAsync saves the notification together with its change.
    public static class NotificationHelper
    {
        private const string ChangedPrefix = "Changed: ";

        public static void Add(ApplicationDbContext db, long userId, long? projectId,
                               string title, string message, long? createdBy)
        {
            db.Set<Notification>().Add(new Notification
            {
                UserId = userId,
                ProjectId = projectId,
                Title = title.Length > 200 ? title[..200] : title,
                Message = message,
                IsRead = false,
                CreatedBy = createdBy
            });
        }

        // Tells the designer that the customer changed something on the project.
        // If the designer has not read the previous "updated" notification yet, the new
        // changes are merged into it, so one busy editing session is one notification.
        public static async Task NotifyProjectChangedAsync(ApplicationDbContext db, long designerUserId,
            long projectId, string projectCode, IEnumerable<string> labels, long? changedBy)
        {
            var title = $"{projectCode} was updated by the customer";
            var merged = labels.ToList();

            var existing = await db.Set<Notification>().FirstOrDefaultAsync(n =>
                n.UserId == designerUserId && n.ProjectId == projectId &&
                n.Title == title && !n.IsRead && n.IsActive);

            if (existing != null)
            {
                var earlier = existing.Message.StartsWith(ChangedPrefix)
                    ? existing.Message[ChangedPrefix.Length..].TrimEnd('.')
                          .Split(", ", StringSplitOptions.RemoveEmptyEntries)
                    : Array.Empty<string>();

                foreach (var label in earlier)
                {
                    if (!merged.Contains(label)) merged.Insert(0, label);
                }

                existing.Message = ChangedPrefix + string.Join(", ", merged) + ".";
                existing.ModifiedDate = DateTime.Now;
                existing.ModifiedBy = changedBy;
                return;
            }

            Add(db, designerUserId, projectId, title,
                ChangedPrefix + string.Join(", ", merged) + ".", changedBy);
        }
    }
}