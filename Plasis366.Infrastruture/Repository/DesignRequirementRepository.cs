using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class DesignRequirementRepository : IDesignRequirementRepository
    {
        private readonly ApplicationDbContext _context;

        public DesignRequirementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DesignRequirement>> GetAllAsync()
        {
            return await _context.DesignRequirements
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<DesignRequirement?> GetByIdAsync(long designRequirementId)
        {
            return await _context.DesignRequirements
                .FirstOrDefaultAsync(x =>
                    x.DesignRequirementId == designRequirementId &&
                    x.IsActive);
        }

        public async Task<DesignRequirement> CreateAsync(
            DesignRequirement designRequirement)
        {
            await _context.DesignRequirements.AddAsync(designRequirement);

            await _context.SaveChangesAsync();

            return designRequirement;
        }

        public async Task<DesignRequirement> UpdateAsync(
            DesignRequirement designRequirement)
        {
            var existingRequirement =
                await _context.DesignRequirements
                    .FirstOrDefaultAsync(x =>
                        x.DesignRequirementId ==
                        designRequirement.DesignRequirementId);

            if (existingRequirement == null)
                return designRequirement;

            existingRequirement.ProjectId =
                designRequirement.ProjectId;

            existingRequirement.RoomId =
                designRequirement.RoomId;

            existingRequirement.PreferredStyle =
                designRequirement.PreferredStyle;

            existingRequirement.PreferredColors =
                designRequirement.PreferredColors;

            existingRequirement.Budget =
                designRequirement.Budget;

            existingRequirement.FurnitureRequirements =
                designRequirement.FurnitureRequirements;

            existingRequirement.StorageRequirements =
                designRequirement.StorageRequirements;

            existingRequirement.LightingRequirements =
                designRequirement.LightingRequirements;

            existingRequirement.SpecialRequirements =
                designRequirement.SpecialRequirements;

            existingRequirement.AdditionalNotes =
                designRequirement.AdditionalNotes;

            existingRequirement.IsActive =
                designRequirement.IsActive;

            await _context.SaveChangesAsync();

            return existingRequirement;
        }

        public async Task<bool> DeleteAsync(long designRequirementId)
        {
            var requirement =
                await _context.DesignRequirements
                    .FirstOrDefaultAsync(x =>
                        x.DesignRequirementId == designRequirementId);

            if (requirement == null)
                return false;

            requirement.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
