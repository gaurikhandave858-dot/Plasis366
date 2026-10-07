using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class DesignRequirement:AuditEntity
    {
        public long DesignRequirementId { get; set; }

        public long ProjectId { get; set; }

        public long? RoomId { get; set; }

        public string? PreferredStyle { get; set; }

        public string? PreferredColors { get; set; }

        public decimal? Budget { get; set; }

        public string? FurnitureRequirements { get; set; }

        public string? StorageRequirements { get; set; }

        public string? LightingRequirements { get; set; }

        public string? SpecialRequirements { get; set; }

        public string? AdditionalNotes { get; set; }

        // Navigation Properties
        public Project? Project { get; set; }

        public Room? Room { get; set; }
    }
}
