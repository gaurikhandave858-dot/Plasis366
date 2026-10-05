using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();

        Task<Tenant?> GetByIdAsync(long tenantId);

        Task<Tenant> CreateAsync(Tenant tenant);

        Task<Tenant> UpdateAsync(Tenant tenant);

        Task<bool> DeleteAsync(long tenantId);
    }
}
