using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class DesignProposalService : IDesignProposalService
    {
        private readonly IDesignProposalRepository _designProposalRepository;

        public DesignProposalService(
            IDesignProposalRepository designProposalRepository)
        {
            _designProposalRepository = designProposalRepository;
        }

        public async Task<IEnumerable<DesignProposal>> GetAllAsync()
        {
            return await _designProposalRepository.GetAllAsync();
        }

        public async Task<DesignProposal?> GetByIdAsync(long designProposalId)
        {
            return await _designProposalRepository.GetByIdAsync(
                designProposalId);
        }

        public async Task<DesignProposal> CreateAsync(
            DesignProposal designProposal)
        {
            return await _designProposalRepository.CreateAsync(
                designProposal);
        }

        public async Task<DesignProposal> UpdateAsync(
            DesignProposal designProposal)
        {
            return await _designProposalRepository.UpdateAsync(
                designProposal);
        }

        public async Task<bool> DeleteAsync(long designProposalId)
        {
            return await _designProposalRepository.DeleteAsync(
                designProposalId);
        }
    }
}
