using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;


namespace Plasis366.Infrastructure
{
    public class ProjectStatusHistoryRepository : IProjectStatusHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectStatusHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectStatusHistory>> GetAllAsync()
        {
            return await _context.ProjectStatusHistories
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<ProjectStatusHistory?> GetByIdAsync(long projectStatusHistoryId)
        {
            return await _context.ProjectStatusHistories
                .FirstOrDefaultAsync(x =>
                    x.ProjectStatusHistoryId == projectStatusHistoryId &&
                    x.IsActive);
        }

        public async Task<ProjectStatusHistory> CreateAsync(
            ProjectStatusHistory projectStatusHistory)
        {
            await _context.ProjectStatusHistories.AddAsync(projectStatusHistory);
            await _context.SaveChangesAsync();

            return projectStatusHistory;
        }

        public async Task<ProjectStatusHistory> UpdateAsync(
            ProjectStatusHistory projectStatusHistory)
        {
            var existingHistory = await _context.ProjectStatusHistories
                .FirstOrDefaultAsync(x =>
                    x.ProjectStatusHistoryId ==
                    projectStatusHistory.ProjectStatusHistoryId);

            if (existingHistory == null)
                return projectStatusHistory;

            existingHistory.ProjectId = projectStatusHistory.ProjectId;
            existingHistory.Status = projectStatusHistory.Status;
            existingHistory.Remarks = projectStatusHistory.Remarks;
            existingHistory.StatusDate = projectStatusHistory.StatusDate;
            existingHistory.ChangedBy = projectStatusHistory.ChangedBy;
            existingHistory.IsActive = projectStatusHistory.IsActive;

            await _context.SaveChangesAsync();

            return existingHistory;
        }

        public async Task<bool> DeleteAsync(long projectStatusHistoryId)
        {
            var history = await _context.ProjectStatusHistories
                .FirstOrDefaultAsync(x =>
                    x.ProjectStatusHistoryId == projectStatusHistoryId &&
                    x.IsActive);

            if (history == null)
                return false;

            history.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
