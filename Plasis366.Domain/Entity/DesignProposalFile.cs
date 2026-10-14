using System;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    // One picture or PDF that belongs to a design proposal
    public class DesignProposalFile : AuditEntity
    {
        public long DesignProposalFileId { get; set; }

        [Range(1, long.MaxValue)]
        public long DesignProposalId { get; set; }

        [Required, StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FileType { get; set; }

        [Range(0, long.MaxValue)]
        public long FileSize { get; set; }

        public DesignProposal? DesignProposal { get; set; }
    }
}