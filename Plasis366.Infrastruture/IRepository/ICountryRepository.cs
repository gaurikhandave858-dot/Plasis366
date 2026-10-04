using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();

        Task<Country?> GetByIdAsync(long countryId);

        Task<Country> CreateAsync(Country country);

        Task<Country> UpdateAsync(Country country);

        Task<bool> DeleteAsync(long countryId);
    }
}
