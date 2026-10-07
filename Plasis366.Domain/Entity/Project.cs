using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Domain
{
    public class Project :AuditEntity
    {

        public long ProjectId { get; set; }

        public long TenantId { get; set; }

        public long CustomerId { get; set; }

        public long? DesignerUserId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string ProjectCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Draft";

        public decimal? BudgetMin { get; set; }

        public decimal? BudgetMax { get; set; }

        public DateTime? ExpectedStartDate { get; set; }

        public DateTime? ExpectedCompletionDate { get; set; }

        // Navigation Properties
        public Tenant? Tenant { get; set; }

        public Customer? Customer { get; set; }

        public User? DesignerUser { get; set; }

        public ICollection<Property> Properties { get; set; }
            = new List<Property>();

        public ICollection<DesignRequirement> DesignRequirements { get; set; }
            = new List<DesignRequirement>();

        public ICollection<ProjectAttachment> ProjectAttachments { get; set; }
            = new List<ProjectAttachment>();

        public ICollection<DesignProposal> DesignProposals { get; set; }
            = new List<DesignProposal>();
    }
}
