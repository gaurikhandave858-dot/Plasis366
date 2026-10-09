using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Project : AuditEntity
    {
        public long ProjectId { get; set; }

        [Range(1, long.MaxValue)]
        public long TenantId { get; set; }

        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        [Range(1, long.MaxValue)]
        public long? DesignerUserId { get; set; }

        [Required, StringLength(200, MinimumLength = 2)]
        public string ProjectName { get; set; } = string.Empty;

        [Required, StringLength(50, MinimumLength = 1)]
        public string ProjectCode { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string Status { get; set; } = "Draft";

        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal? BudgetMin { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal? BudgetMax { get; set; }

        public DateTime? ExpectedStartDate { get; set; }

        public DateTime? ExpectedCompletionDate { get; set; }

        public Tenant? Tenant { get; set; }

        public Customer? Customer { get; set; }

        public User? DesignerUser { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();

        public ICollection<DesignRequirement> DesignRequirements { get; set; } = new List<DesignRequirement>();

        public ICollection<ProjectAttachment> ProjectAttachments { get; set; } = new List<ProjectAttachment>();

        public ICollection<DesignProposal> DesignProposals { get; set; } = new List<DesignProposal>();
    }
}
