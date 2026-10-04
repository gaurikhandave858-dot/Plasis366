using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _countryRepository.GetAllAsync();
        }

        public async Task<Country?> GetByIdAsync(long countryId)
        {
            return await _countryRepository.GetByIdAsync(countryId);
        }

        public async Task<Country> CreateAsync(Country country)
        {
            return await _countryRepository.CreateAsync(country);
        }

        public async Task<Country> UpdateAsync(Country country)
        {
            return await _countryRepository.UpdateAsync(country);
        }

        public async Task<bool> DeleteAsync(long countryId)
        {
            return await _countryRepository.DeleteAsync(countryId);
        }
    }
}
