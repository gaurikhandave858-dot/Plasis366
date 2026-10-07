using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IPropertyService
    {

        Task<IEnumerable<Property>> GetAllAsync();

        Task<Property?> GetByIdAsync(long propertyId);

        Task<Property> CreateAsync(Property property);

        Task<Property> UpdateAsync(Property property);

        Task<bool> DeleteAsync(long propertyId);
    }
}
