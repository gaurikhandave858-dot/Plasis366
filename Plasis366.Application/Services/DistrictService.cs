using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application.Services
{
    public class DistrictService :IDistrictService
    {
        private readonly IDistrictRepository _districtRepository;

        public DistrictService(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
        }

        public async Task<IEnumerable<District>> GetAllAsync()
        {
            return await _districtRepository.GetAllAsync();
        }

        public async Task<District?> GetByIdAsync(long districtId)
        {
            return await _districtRepository.GetByIdAsync(districtId);
        }

        public async Task<District> CreateAsync(District district)
        {
            return await _districtRepository.CreateAsync(district);
        }

        public async Task<District> UpdateAsync(District district)
        {
            return await _districtRepository.UpdateAsync(district);
        }

        public async Task<bool> DeleteAsync(long districtId)
        {
            return await _districtRepository.DeleteAsync(districtId);
        }
    }
}
