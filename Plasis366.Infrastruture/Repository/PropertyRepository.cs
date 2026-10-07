using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;


namespace Plasis366.Infrastructure
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(long propertyId)
        {
            return await _context.Properties
                .FirstOrDefaultAsync(x =>
                    x.PropertyId == propertyId &&
                    x.IsActive);
        }

        public async Task<Property> CreateAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();

            return property;
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            var existingProperty = await _context.Properties
                .FirstOrDefaultAsync(x =>
                    x.PropertyId == property.PropertyId);

            if (existingProperty == null)
            {
                return property;
            }

            existingProperty.ProjectId = property.ProjectId;
            existingProperty.PropertyType = property.PropertyType;
            existingProperty.PropertyName = property.PropertyName;
            existingProperty.Address = property.Address;

            existingProperty.CountryId = property.CountryId;
            existingProperty.RegionId = property.RegionId;
            existingProperty.StateId = property.StateId;
            existingProperty.DistrictId = property.DistrictId;
            existingProperty.TalukaId = property.TalukaId;
            existingProperty.CityId = property.CityId;

            existingProperty.TotalArea = property.TotalArea;
            existingProperty.NumberOfFloors = property.NumberOfFloors;
            existingProperty.NumberOfRooms = property.NumberOfRooms;
            existingProperty.NumberOfBathrooms = property.NumberOfBathrooms;

            existingProperty.IsActive = property.IsActive;

            await _context.SaveChangesAsync();

            return existingProperty;
        }

        public async Task<bool> DeleteAsync(long propertyId)
        {
            var property = await _context.Properties
                .FirstOrDefaultAsync(x =>
                    x.PropertyId == propertyId);

            if (property == null)
            {
                return false;
            }

            property.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
