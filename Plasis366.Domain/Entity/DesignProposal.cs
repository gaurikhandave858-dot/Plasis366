using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class DesignProposal : AuditEntity
    {
        public long DesignProposalId { get; set; }

        public long ProjectId { get; set; }

        public long SubmittedBy { get; set; }

        public int ProposalVersion { get; set; }

        public string? ProposalTitle { get; set; }

        public string? Description { get; set; }

        public string? FilePath { get; set; }

        public string Status { get; set; } = "Draft";

        public DateTime? SubmittedDate { get; set; }

        // Navigation Properties
        public Project Project { get; set; } = null!;

        public User SubmittedByUser { get; set; } = null!;
    }
}
