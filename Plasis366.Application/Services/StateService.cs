using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;

        public StateService(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _stateRepository.GetAllAsync();
        }

        public async Task<State?> GetByIdAsync(long stateId)
        {
            return await _stateRepository.GetByIdAsync(stateId);
        }

        public async Task<State> CreateAsync(State state)
        {
            return await _stateRepository.CreateAsync(state);
        }

        public async Task<State> UpdateAsync(State state)
        {
            return await _stateRepository.UpdateAsync(state);
        }

        public async Task<bool> DeleteAsync(long stateId)
        {
            return await _stateRepository.DeleteAsync(stateId);
        }
    }
}
