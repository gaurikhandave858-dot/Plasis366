using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Plasis366.Domain;

namespace Plasis366.Infrastructure
{
    public class DesignProposalRepository : IDesignProposalRepository
    {
        private readonly ApplicationDbContext _context;

        public DesignProposalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DesignProposal>> GetAllAsync()
        {
            return await _context.DesignProposals
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<DesignProposal?> GetByIdAsync(long designProposalId)
        {
            return await _context.DesignProposals
                .FirstOrDefaultAsync(x =>
                    x.DesignProposalId == designProposalId &&
                    x.IsActive);
        }

        public async Task<DesignProposal> CreateAsync(DesignProposal designProposal)
        {
            await _context.DesignProposals.AddAsync(designProposal);
            await _context.SaveChangesAsync();

            return designProposal;
        }

        public async Task<DesignProposal> UpdateAsync(DesignProposal designProposal)
        {
            var existingProposal = await _context.DesignProposals
                .FirstOrDefaultAsync(x =>
                    x.DesignProposalId == designProposal.DesignProposalId);

            if (existingProposal == null)
                return designProposal;

            existingProposal.ProjectId = designProposal.ProjectId;
            existingProposal.SubmittedBy = designProposal.SubmittedBy;
            existingProposal.ProposalVersion = designProposal.ProposalVersion;
            existingProposal.ProposalTitle = designProposal.ProposalTitle;
            existingProposal.Description = designProposal.Description;
            existingProposal.FilePath = designProposal.FilePath;
            existingProposal.Status = designProposal.Status;
            existingProposal.SubmittedDate = designProposal.SubmittedDate;
            existingProposal.IsActive = designProposal.IsActive;

            await _context.SaveChangesAsync();

            return existingProposal;
        }

        public async Task<bool> DeleteAsync(long designProposalId)
        {
            var proposal = await _context.DesignProposals
                .FirstOrDefaultAsync(x =>
                    x.DesignProposalId == designProposalId &&
                    x.IsActive);

            if (proposal == null)
                return false;

            proposal.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
