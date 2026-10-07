using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class ProjectStatusHistoryService : IProjectStatusHistoryService
    {
        private readonly IProjectStatusHistoryRepository
           _projectStatusHistoryRepository;

        public ProjectStatusHistoryService(
            IProjectStatusHistoryRepository projectStatusHistoryRepository)
        {
            _projectStatusHistoryRepository =
                projectStatusHistoryRepository;
        }

        public async Task<IEnumerable<ProjectStatusHistory>> GetAllAsync()
        {
            return await _projectStatusHistoryRepository.GetAllAsync();
        }

        public async Task<ProjectStatusHistory?> GetByIdAsync(
            long projectStatusHistoryId)
        {
            return await _projectStatusHistoryRepository
                .GetByIdAsync(projectStatusHistoryId);
        }

        public async Task<ProjectStatusHistory> CreateAsync(
            ProjectStatusHistory projectStatusHistory)
        {
            return await _projectStatusHistoryRepository
                .CreateAsync(projectStatusHistory);
        }

        public async Task<ProjectStatusHistory> UpdateAsync(
            ProjectStatusHistory projectStatusHistory)
        {
            return await _projectStatusHistoryRepository
                .UpdateAsync(projectStatusHistory);
        }

        public async Task<bool> DeleteAsync(long projectStatusHistoryId)
        {
            return await _projectStatusHistoryRepository
                .DeleteAsync(projectStatusHistoryId);
        }
    }
}
