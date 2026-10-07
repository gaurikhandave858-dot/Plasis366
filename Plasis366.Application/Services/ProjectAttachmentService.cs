using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class ProjectAttachmentService :IProjectAttachmentService
    {
        private readonly IProjectAttachmentRepository
           _projectAttachmentRepository;

        public ProjectAttachmentService(
            IProjectAttachmentRepository projectAttachmentRepository)
        {
            _projectAttachmentRepository = projectAttachmentRepository;
        }

        public async Task<IEnumerable<ProjectAttachment>> GetAllAsync()
        {
            return await _projectAttachmentRepository.GetAllAsync();
        }

        public async Task<ProjectAttachment?> GetByIdAsync(
            long projectAttachmentId)
        {
            return await _projectAttachmentRepository
                .GetByIdAsync(projectAttachmentId);
        }

        public async Task<ProjectAttachment> CreateAsync(
            ProjectAttachment projectAttachment)
        {
            return await _projectAttachmentRepository
                .CreateAsync(projectAttachment);
        }

        public async Task<ProjectAttachment> UpdateAsync(
            ProjectAttachment projectAttachment)
        {
            return await _projectAttachmentRepository
                .UpdateAsync(projectAttachment);
        }

        public async Task<bool> DeleteAsync(long projectAttachmentId)
        {
            return await _projectAttachmentRepository
                .DeleteAsync(projectAttachmentId);
        }
    }
}
