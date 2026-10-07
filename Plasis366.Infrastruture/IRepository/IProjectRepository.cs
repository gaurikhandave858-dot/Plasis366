using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();

        Task<Project?> GetByIdAsync(long projectId);

        Task<Project> CreateAsync(Project project);

        Task<Project> UpdateAsync(Project project);

        Task<bool> DeleteAsync(long projectId);
    }
}
