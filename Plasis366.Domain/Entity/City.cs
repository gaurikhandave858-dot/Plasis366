using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class City : AuditEntity
    {
        public long CityId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string CityName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? CityCode { get; set; }

        [Range(1, long.MaxValue)]
        public long TalukaId { get; set; }

        public Taluka? Taluka { get; set; }
    }
}
