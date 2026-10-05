using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IDistrictService
    {
        Task<IEnumerable<District>> GetAllAsync();

        Task<District?> GetByIdAsync(long districtId);

        Task<District> CreateAsync(District district);

        Task<District> UpdateAsync(District district);

        Task<bool> DeleteAsync(long districtId);
    }
}
