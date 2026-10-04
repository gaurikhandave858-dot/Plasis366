using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class StateRepository : IStateRepository
    {
        private readonly ApplicationDbContext _context;

        public StateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.States
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<State?> GetByIdAsync(long stateId)
        {
            return await _context.States
                .FirstOrDefaultAsync(x => x.StateId == stateId && x.IsActive);
        }

        public async Task<State> CreateAsync(State state)
        {
            await _context.States.AddAsync(state);

            await _context.SaveChangesAsync();

            return state;
        }

        public async Task<State> UpdateAsync(State state)
        {
            var existingState = await _context.States
                .FirstOrDefaultAsync(x => x.StateId == state.StateId);

            if (existingState == null)
                return state;

            existingState.StateName = state.StateName;
            existingState.StateCode = state.StateCode;
            existingState.CountryId = state.CountryId;
            existingState.IsActive = state.IsActive;

            await _context.SaveChangesAsync();

            return existingState;
        }

        public async Task<bool> DeleteAsync(long stateId)
        {
            var state = await _context.States
                .FirstOrDefaultAsync(x => x.StateId == stateId);

            if (state == null)
                return false;

            state.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
