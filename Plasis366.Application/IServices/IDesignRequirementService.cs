using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IDesignRequirementService
    {
        Task<IEnumerable<DesignRequirement>> GetAllAsync();

        Task<DesignRequirement?> GetByIdAsync(long designRequirementId);

        Task<DesignRequirement> CreateAsync(
            DesignRequirement designRequirement);

        Task<DesignRequirement> UpdateAsync(
            DesignRequirement designRequirement);

        Task<bool> DeleteAsync(long designRequirementId);

    }
}
