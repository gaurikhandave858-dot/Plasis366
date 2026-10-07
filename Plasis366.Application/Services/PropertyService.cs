using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _propertyRepository.GetAllAsync();
        }

        public async Task<Property?> GetByIdAsync(long propertyId)
        {
            return await _propertyRepository.GetByIdAsync(propertyId);
        }

        public async Task<Property> CreateAsync(Property property)
        {
            return await _propertyRepository.CreateAsync(property);
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            return await _propertyRepository.UpdateAsync(property);
        }

        public async Task<bool> DeleteAsync(long propertyId)
        {
            return await _propertyRepository.DeleteAsync(propertyId);
        }
    }
}
