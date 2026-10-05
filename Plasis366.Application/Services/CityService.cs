using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class CityService : ICityService
    {

        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _cityRepository.GetAllAsync();
        }

        public async Task<City?> GetByIdAsync(long cityId)
        {
            return await _cityRepository.GetByIdAsync(cityId);
        }

        public async Task<City> CreateAsync(City city)
        {
            return await _cityRepository.CreateAsync(city);
        }

        public async Task<City> UpdateAsync(City city)
        {
            return await _cityRepository.UpdateAsync(city);
        }

        public async Task<bool> DeleteAsync(long cityId)
        {
            return await _cityRepository.DeleteAsync(cityId);
        }
    }
}
