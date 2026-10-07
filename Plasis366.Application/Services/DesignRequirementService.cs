using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class DesignRequirementService : IDesignRequirementService
    {
        private readonly IDesignRequirementRepository
           _designRequirementRepository;

        public DesignRequirementService(
            IDesignRequirementRepository designRequirementRepository)
        {
            _designRequirementRepository =
                designRequirementRepository;
        }

        public async Task<IEnumerable<DesignRequirement>> GetAllAsync()
        {
            return await _designRequirementRepository.GetAllAsync();
        }

        public async Task<DesignRequirement?> GetByIdAsync(
            long designRequirementId)
        {
            return await _designRequirementRepository
                .GetByIdAsync(designRequirementId);
        }

        public async Task<DesignRequirement> CreateAsync(
            DesignRequirement designRequirement)
        {
            return await _designRequirementRepository
                .CreateAsync(designRequirement);
        }

        public async Task<DesignRequirement> UpdateAsync(
            DesignRequirement designRequirement)
        {
            return await _designRequirementRepository
                .UpdateAsync(designRequirement);
        }

        public async Task<bool> DeleteAsync(long designRequirementId)
        {
            return await _designRequirementRepository
                .DeleteAsync(designRequirementId);
        }
    }
}
