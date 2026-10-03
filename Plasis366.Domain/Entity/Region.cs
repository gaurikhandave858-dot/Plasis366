using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Region :AuditEntity
    {
        public long RegionId { get; set; }

        public string RegionName { get; set; } = string.Empty;

        public string? RegionCode { get; set; }

        public ICollection<Country> Countries { get; set; } = new List<Country>();
    }
}
