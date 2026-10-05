using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class CityRepository:ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<City?> GetByIdAsync(long cityId)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(x => x.CityId == cityId && x.IsActive);
        }

        public async Task<City> CreateAsync(City city)
        {
            await _context.Cities.AddAsync(city);

            await _context.SaveChangesAsync();

            return city;
        }

        public async Task<City> UpdateAsync(City city)
        {
            var existingCity = await _context.Cities
                .FirstOrDefaultAsync(x => x.CityId == city.CityId);

            if (existingCity == null)
                return city;

            existingCity.CityName = city.CityName;
            existingCity.CityCode = city.CityCode;
            existingCity.TalukaId = city.TalukaId;
            existingCity.IsActive = city.IsActive;

            await _context.SaveChangesAsync();

            return existingCity;
        }

        public async Task<bool> DeleteAsync(long cityId)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(x => x.CityId == cityId);

            if (city == null)
                return false;

            city.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
