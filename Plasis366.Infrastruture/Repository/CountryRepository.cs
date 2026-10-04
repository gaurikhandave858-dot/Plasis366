using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class CountryRepository:ICountryRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _context.Countries
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Country?> GetByIdAsync(long countryId)
        {
            return await _context.Countries
                .FirstOrDefaultAsync(x => x.CountryId == countryId && x.IsActive);
        }

        public async Task<Country> CreateAsync(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<Country> UpdateAsync(Country country)
        {
            var existingCountry = await _context.Countries
                .FirstOrDefaultAsync(x => x.CountryId == country.CountryId);

            if (existingCountry == null)
                return country;

            existingCountry.CountryName = country.CountryName;
            existingCountry.CountryCode = country.CountryCode;
            existingCountry.RegionId = country.RegionId;
            existingCountry.IsActive = country.IsActive;

            await _context.SaveChangesAsync();

            return existingCountry;
        }

        public async Task<bool> DeleteAsync(long countryId)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(x => x.CountryId == countryId);

            if (country == null)
                return false;

            country.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
