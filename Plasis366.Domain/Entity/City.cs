using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class City:AuditEntity
    {
        public long CityId { get; set; }

        public string CityName { get; set; } = string.Empty;

        public string? CityCode { get; set; }

        // Foreign Key
        public long TalukaId { get; set; }

        // Navigation Property
        public Taluka Taluka { get; set; } = null!;
    }
}
