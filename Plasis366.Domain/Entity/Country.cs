using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Country: AuditEntity
    {
        public long CountryId { get; set; }

        public string CountryName { get; set; } = string.Empty;

        public string? CountryCode { get; set; }

        // Foreign Key
        public long RegionId { get; set; }

        // Navigation Property
        public Region? Region { get; set; }

        // Navigation Property
        public ICollection<State> States { get; set; } = new List<State>();
    }
}
