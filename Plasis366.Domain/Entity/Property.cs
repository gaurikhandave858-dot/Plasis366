using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Property : AuditEntity
    {
        public long PropertyId { get; set; }

        [Range(1, long.MaxValue)]
        public long ProjectId { get; set; }

        [Required, StringLength(50, MinimumLength = 2)]
        public string PropertyType { get; set; } = string.Empty;

        [StringLength(150)]
        public string? PropertyName { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [Range(1, long.MaxValue)]
        public long? CountryId { get; set; }

        [Range(1, long.MaxValue)]
        public long? RegionId { get; set; }

        [Range(1, long.MaxValue)]
        public long? StateId { get; set; }

        [Range(1, long.MaxValue)]
        public long? DistrictId { get; set; }

        [Range(1, long.MaxValue)]
        public long? TalukaId { get; set; }

        [Range(1, long.MaxValue)]
        public long? CityId { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal? TotalArea { get; set; }

        [Range(1, 1000)]
        public int? NumberOfFloors { get; set; }

        [Range(1, 1000)]
        public int? NumberOfRooms { get; set; }

        [Range(0, 1000)]
        public int? NumberOfBathrooms { get; set; }

        public Project? Project { get; set; }
        public Country? Country { get; set; }
        public Region? Region { get; set; }
        public State? State { get; set; }
        public District? District { get; set; }
        public Taluka? Taluka { get; set; }
        public City? City { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
