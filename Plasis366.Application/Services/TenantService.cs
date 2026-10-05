using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantService(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _tenantRepository.GetAllAsync();
        }

        public async Task<Tenant?> GetByIdAsync(long tenantId)
        {
            return await _tenantRepository.GetByIdAsync(tenantId);
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            return await _tenantRepository.CreateAsync(tenant);
        }

        public async Task<Tenant> UpdateAsync(Tenant tenant)
        {
            return await _tenantRepository.UpdateAsync(tenant);
        }

        public async Task<bool> DeleteAsync(long tenantId)
        {
            return await _tenantRepository.DeleteAsync(tenantId);
        }
    }
}
