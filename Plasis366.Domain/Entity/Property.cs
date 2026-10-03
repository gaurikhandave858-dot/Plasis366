using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Property :AuditEntity
    {
        public long PropertyId { get; set; }

        public long ProjectId { get; set; }

        public string PropertyType { get; set; } = string.Empty;

        public string? PropertyName { get; set; }

        public string? Address { get; set; }

        // Location
        public long? CountryId { get; set; }

        public long? RegionId { get; set; }

        public long? StateId { get; set; }

        public long? DistrictId { get; set; }

        public long? TalukaId { get; set; }

        public long? CityId { get; set; }

        public decimal? TotalArea { get; set; }

        public int? NumberOfFloors { get; set; }

        public int? NumberOfRooms { get; set; }

        public int? NumberOfBathrooms { get; set; }

        // Navigation Properties
        public Project Project { get; set; } = null!;

        public Country? Country { get; set; }

        public Region? Region { get; set; }

        public State? State { get; set; }

        public District? District { get; set; }

        public Taluka? Taluka { get; set; }

        public City? City { get; set; }

        public ICollection<Room> Rooms { get; set; }
            = new List<Room>();
    }
}
