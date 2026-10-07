using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application.Services
{
    public class ProjectService :IProjectService
    {
        private readonly IProjectRepository _repository;

        public ProjectService(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Project?> GetByIdAsync(long projectId)
        {
            return await _repository.GetByIdAsync(projectId);
        }

        public async Task<Project> CreateAsync(Project project)
        {
            return await _repository.CreateAsync(project);
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            return await _repository.UpdateAsync(project);
        }

        public async Task<bool> DeleteAsync(long projectId)
        {
            return await _repository.DeleteAsync(projectId);
        }
    }
}
