using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Taluka :AuditEntity
    {
        public long TalukaId { get; set; }

        public string TalukaName { get; set; } = string.Empty;

        public string? TalukaCode { get; set; }

        // Foreign Key
        public long DistrictId { get; set; }

        // Navigation Property
        public District? District { get; set; }

        // Navigation Property
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
