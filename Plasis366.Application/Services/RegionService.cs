using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;


namespace Plasis366.Application
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;

        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            return await _regionRepository.GetAllAsync();
        }

        public async Task<Region?> GetByIdAsync(long regionId)
        {
            return await _regionRepository.GetByIdAsync(regionId);
        }

        public async Task<Region> CreateAsync(Region region)
        {
            return await _regionRepository.CreateAsync(region);
        }

        public async Task<Region> UpdateAsync(Region region)
        {
            return await _regionRepository.UpdateAsync(region);
        }

        public async Task<bool> DeleteAsync(long regionId)
        {
            return await _regionRepository.DeleteAsync(regionId);
        }
    }
}
