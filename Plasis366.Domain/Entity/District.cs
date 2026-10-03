using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class District : AuditEntity
    {
        public long DistrictId { get; set; }

        public string DistrictName { get; set; } = string.Empty;

        public string? DistrictCode { get; set; }

        // Foreign Key
        public long StateId { get; set; }

        // Navigation Property
        public State State { get; set; } = null!;

        // Navigation Property
        public ICollection<Taluka> Talukas { get; set; } = new List<Taluka>();
    }
}
