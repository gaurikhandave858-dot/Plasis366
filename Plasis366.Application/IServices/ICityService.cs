using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface ICityService
    {
        Task<IEnumerable<City>> GetAllAsync();

        Task<City?> GetByIdAsync(long cityId);

        Task<City> CreateAsync(City city);

        Task<City> UpdateAsync(City city);

        Task<bool> DeleteAsync(long cityId);
    }
}
