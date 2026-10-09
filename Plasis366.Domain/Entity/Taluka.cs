using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Taluka : AuditEntity
    {
        public long TalukaId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string TalukaName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? TalukaCode { get; set; }

        [Range(1, long.MaxValue)]
        public long DistrictId { get; set; }

        public District? District { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
