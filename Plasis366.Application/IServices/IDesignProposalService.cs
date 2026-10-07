using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface IDesignProposalService
    {
        Task<IEnumerable<DesignProposal>> GetAllAsync();

        Task<DesignProposal?> GetByIdAsync(long designProposalId);

        Task<DesignProposal> CreateAsync(DesignProposal designProposal);

        Task<DesignProposal> UpdateAsync(DesignProposal designProposal);

        Task<bool> DeleteAsync(long designProposalId);
    }
}
