using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(long regionId);
        Task<Region> CreateAsync(Region region);
        Task<Region> UpdateAsync(Region region);
        Task<bool> DeleteAsync(long regionId);
    }
}
