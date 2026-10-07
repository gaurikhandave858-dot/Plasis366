using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;


namespace Plasis366.Infrastructure
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(long projectId)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == projectId &&
                    x.IsActive);
        }

        public async Task<Project> CreateAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            var existingProject =
                await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == project.ProjectId);

            if (existingProject == null)
                return project;

            existingProject.TenantId = project.TenantId;
            existingProject.CustomerId = project.CustomerId;
            existingProject.DesignerUserId = project.DesignerUserId;
            existingProject.ProjectName = project.ProjectName;
            existingProject.ProjectCode = project.ProjectCode;
            existingProject.Description = project.Description;
            existingProject.Status = project.Status;
            existingProject.BudgetMin = project.BudgetMin;
            existingProject.BudgetMax = project.BudgetMax;
            existingProject.ExpectedStartDate = project.ExpectedStartDate;
            existingProject.ExpectedCompletionDate =
                project.ExpectedCompletionDate;
            existingProject.IsActive = project.IsActive;

            await _context.SaveChangesAsync();

            return existingProject;
        }

        public async Task<bool> DeleteAsync(long projectId)
        {
            var project =
                await _context.Projects
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == projectId);

            if (project == null)
                return false;

            project.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
