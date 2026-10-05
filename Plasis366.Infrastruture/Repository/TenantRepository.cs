using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _context;

        public TenantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _context.Tenants
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Tenant?> GetByIdAsync(long tenantId)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive);
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            await _context.Tenants.AddAsync(tenant);

            await _context.SaveChangesAsync();

            return tenant;
        }

        public async Task<Tenant> UpdateAsync(Tenant tenant)
        {
            var existingTenant = await _context.Tenants
                .FirstOrDefaultAsync(x => x.TenantId == tenant.TenantId);

            if (existingTenant == null)
                return tenant;

            existingTenant.TenantName = tenant.TenantName;
            existingTenant.TenantCode = tenant.TenantCode;
            existingTenant.Description = tenant.Description;
            existingTenant.Email = tenant.Email;
            existingTenant.PhoneNumber = tenant.PhoneNumber;
            existingTenant.Address = tenant.Address;
            existingTenant.CountryId = tenant.CountryId;
            existingTenant.IsActive = tenant.IsActive;

            await _context.SaveChangesAsync();

            return existingTenant;
        }

        public async Task<bool> DeleteAsync(long tenantId)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(x => x.TenantId == tenantId);

            if (tenant == null)
                return false;

            tenant.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
