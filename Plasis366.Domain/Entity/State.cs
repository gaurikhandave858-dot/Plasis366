using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class State:AuditEntity
    {
        public long StateId { get; set; }

        public string StateName { get; set; } = string.Empty;

        public string? StateCode { get; set; }

        // Foreign Key
        public long CountryId { get; set; }

        // Navigation Property
        public Country Country { get; set; } = null!;

        // Navigation Property
        public ICollection<District> Districts { get; set; } = new List<District>();
    }
}
