using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public interface IStateRepository
    {
        Task<IEnumerable<State>> GetAllAsync();

        Task<State?> GetByIdAsync(long stateId);

        Task<State> CreateAsync(State state);

        Task<State> UpdateAsync(State state);

        Task<bool> DeleteAsync(long stateId);
    }
}
