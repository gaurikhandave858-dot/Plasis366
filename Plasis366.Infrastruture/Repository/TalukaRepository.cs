using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class TalukaRepository : ITalukaRepository
    {
        private readonly ApplicationDbContext _context;

        public TalukaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Taluka>> GetAllAsync()
        {
            return await _context.Talukas
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Taluka?> GetByIdAsync(long talukaId)
        {
            return await _context.Talukas
                .FirstOrDefaultAsync(x => x.TalukaId == talukaId && x.IsActive);
        }

        public async Task<Taluka> CreateAsync(Taluka taluka)
        {
            await _context.Talukas.AddAsync(taluka);

            await _context.SaveChangesAsync();

            return taluka;
        }

        public async Task<Taluka> UpdateAsync(Taluka taluka)
        {
            var existingTaluka = await _context.Talukas
                .FirstOrDefaultAsync(x => x.TalukaId == taluka.TalukaId);

            if (existingTaluka == null)
                return taluka;

            existingTaluka.TalukaName = taluka.TalukaName;
            existingTaluka.TalukaCode = taluka.TalukaCode;
            existingTaluka.DistrictId = taluka.DistrictId;
            existingTaluka.IsActive = taluka.IsActive;

            await _context.SaveChangesAsync();

            return existingTaluka;
        }

        public async Task<bool> DeleteAsync(long talukaId)
        {
            var taluka = await _context.Talukas
                .FirstOrDefaultAsync(x => x.TalukaId == talukaId);

            if (taluka == null)
                return false;

            taluka.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
