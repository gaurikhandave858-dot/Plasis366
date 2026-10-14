using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // What the designer estimates for this proposal
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.0, 100000000000.0)]
        public decimal? EstimatedCost { get; set; }

        [Range(1, 3650)]
        public int? EstimatedDays { get; set; }

        // The customer's answer: Approved, or Revision Requested with a comment
        [StringLength(2000)]
        public string? CustomerFeedback { get; set; }

        public DateTime? ReviewedDate { get; set; }

        public Project? Project { get; set; }

        public User? SubmittedByUser { get; set; }

        public ICollection<DesignProposalFile> Files { get; set; } = new List<DesignProposalFile>();
    }
}