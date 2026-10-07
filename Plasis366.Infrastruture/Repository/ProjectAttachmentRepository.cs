using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class ProjectAttachmentRepository : IProjectAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectAttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectAttachment>> GetAllAsync()
        {
            return await _context.ProjectAttachments
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<ProjectAttachment?> GetByIdAsync(long projectAttachmentId)
        {
            return await _context.ProjectAttachments
                .FirstOrDefaultAsync(x =>
                    x.ProjectAttachmentId == projectAttachmentId &&
                    x.IsActive);
        }

        public async Task<ProjectAttachment> CreateAsync(
            ProjectAttachment projectAttachment)
        {
            await _context.ProjectAttachments.AddAsync(projectAttachment);

            await _context.SaveChangesAsync();

            return projectAttachment;
        }

        public async Task<ProjectAttachment> UpdateAsync(
            ProjectAttachment projectAttachment)
        {
            var existingAttachment =
                await _context.ProjectAttachments
                    .FirstOrDefaultAsync(x =>
                        x.ProjectAttachmentId ==
                        projectAttachment.ProjectAttachmentId);

            if (existingAttachment == null)
                return projectAttachment;

            existingAttachment.ProjectId = projectAttachment.ProjectId;
            existingAttachment.FileName = projectAttachment.FileName;
            existingAttachment.FilePath = projectAttachment.FilePath;
            existingAttachment.FileType = projectAttachment.FileType;
            existingAttachment.FileSize = projectAttachment.FileSize;
            existingAttachment.Description = projectAttachment.Description;
            existingAttachment.IsActive = projectAttachment.IsActive;

            await _context.SaveChangesAsync();

            return existingAttachment;
        }

        public async Task<bool> DeleteAsync(long projectAttachmentId)
        {
            var attachment =
                await _context.ProjectAttachments
                    .FirstOrDefaultAsync(x =>
                        x.ProjectAttachmentId == projectAttachmentId);

            if (attachment == null)
                return false;

            // Soft delete
            attachment.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
