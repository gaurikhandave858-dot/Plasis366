using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class DistrictRepository:IDistrictRepository
    {
        private readonly ApplicationDbContext _context;

        public DistrictRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<District>> GetAllAsync()
        {
            return await _context.Districts
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<District?> GetByIdAsync(long districtId)
        {
            return await _context.Districts
                .FirstOrDefaultAsync(x =>
                    x.DistrictId == districtId &&
                    x.IsActive);
        }

        public async Task<District> CreateAsync(District district)
        {
            await _context.Districts.AddAsync(district);

            await _context.SaveChangesAsync();

            return district;
        }

        public async Task<District> UpdateAsync(District district)
        {
            var existingDistrict = await _context.Districts
                .FirstOrDefaultAsync(x =>
                    x.DistrictId == district.DistrictId);

            if (existingDistrict == null)
                return district;

            existingDistrict.DistrictName = district.DistrictName;
            existingDistrict.DistrictCode = district.DistrictCode;
            existingDistrict.StateId = district.StateId;
            existingDistrict.IsActive = district.IsActive;

            await _context.SaveChangesAsync();

            return existingDistrict;
        }

        public async Task<bool> DeleteAsync(long districtId)
        {
            var district = await _context.Districts
                .FirstOrDefaultAsync(x => x.DistrictId == districtId);

            if (district == null)
                return false;

            district.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
