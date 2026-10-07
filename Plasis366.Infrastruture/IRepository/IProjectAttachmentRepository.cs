using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface IProjectAttachmentRepository
    {
        Task<IEnumerable<ProjectAttachment>> GetAllAsync();

        Task<ProjectAttachment?> GetByIdAsync(long projectAttachmentId);

        Task<ProjectAttachment> CreateAsync(ProjectAttachment projectAttachment);

        Task<ProjectAttachment> UpdateAsync(ProjectAttachment projectAttachment);

        Task<bool> DeleteAsync(long projectAttachmentId);
    }
}
