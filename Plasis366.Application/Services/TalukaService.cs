using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class TalukaService : ITalukaService
    {
        private readonly ITalukaRepository _talukaRepository;

        public TalukaService(ITalukaRepository talukaRepository)
        {
            _talukaRepository = talukaRepository;
        }

        public async Task<IEnumerable<Taluka>> GetAllAsync()
        {
            return await _talukaRepository.GetAllAsync();
        }

        public async Task<Taluka?> GetByIdAsync(long talukaId)
        {
            return await _talukaRepository.GetByIdAsync(talukaId);
        }

        public async Task<Taluka> CreateAsync(Taluka taluka)
        {
            return await _talukaRepository.CreateAsync(taluka);
        }

        public async Task<Taluka> UpdateAsync(Taluka taluka)
        {
            return await _talukaRepository.UpdateAsync(taluka);
        }

        public async Task<bool> DeleteAsync(long talukaId)
        {
            return await _talukaRepository.DeleteAsync(talukaId);
        }
    }
}
