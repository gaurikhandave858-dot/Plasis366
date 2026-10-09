using System;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class DesignProposal : AuditEntity
    {
        public long DesignProposalId { get; set; }

        [Range(1, long.MaxValue)]
        public long ProjectId { get; set; }

        [Range(1, long.MaxValue)]
        public long SubmittedBy { get; set; }

        [Range(1, int.MaxValue)]
        public int ProposalVersion { get; set; } = 1;

        [StringLength(200)]
        public string? ProposalTitle { get; set; }

        [StringLength(4000)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? FilePath { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string Status { get; set; } = "Draft";

        public DateTime? SubmittedDate { get; set; }

        public Project? Project { get; set; }

        public User? SubmittedByUser { get; set; }
    }
}
