using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class RegionRepository : IRegionRepository
    {
        private readonly ApplicationDbContext _context;

        public RegionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await _context.Regions
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(long regionId)
        {
            return await _context.Regions
                .FirstOrDefaultAsync(x => x.RegionId == regionId && x.IsActive);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await _context.Regions.AddAsync(region);
            await _context.SaveChangesAsync();

            return region;
        }

        public async Task<Region> UpdateAsync(Region region)
        {
            _context.Regions.Update(region);
            await _context.SaveChangesAsync();

            return region;
        }

        public async Task<bool> DeleteAsync(long regionId)
        {
            var region = await _context.Regions
                .FirstOrDefaultAsync(x => x.RegionId == regionId);

            if (region == null)
                return false;

            region.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
