using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface ITalukaRepository
    {
        Task<IEnumerable<Taluka>> GetAllAsync();

        Task<Taluka?> GetByIdAsync(long talukaId);

        Task<Taluka> CreateAsync(Taluka taluka);

        Task<Taluka> UpdateAsync(Taluka taluka);

        Task<bool> DeleteAsync(long talukaId);
    }
}
