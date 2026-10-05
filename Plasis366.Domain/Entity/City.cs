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

        public long TalukaId { get; set; }

        public Taluka? Taluka { get; set; }


    }
}
