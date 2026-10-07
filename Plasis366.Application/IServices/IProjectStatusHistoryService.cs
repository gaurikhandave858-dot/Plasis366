using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IProjectStatusHistoryService
    {
        Task<IEnumerable<ProjectStatusHistory>> GetAllAsync();

        Task<ProjectStatusHistory?> GetByIdAsync(long projectStatusHistoryId);

        Task<ProjectStatusHistory> CreateAsync(
            ProjectStatusHistory projectStatusHistory);

        Task<ProjectStatusHistory> UpdateAsync(
            ProjectStatusHistory projectStatusHistory);

        Task<bool> DeleteAsync(long projectStatusHistoryId);
    }
}
