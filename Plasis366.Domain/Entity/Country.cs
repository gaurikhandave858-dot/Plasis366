using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Country : AuditEntity
    {
        public long CountryId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string CountryName { get; set; } = string.Empty;

        [StringLength(10)]
        public string? CountryCode { get; set; }

        [Range(1, long.MaxValue)]
        public long RegionId { get; set; }

        public Region? Region { get; set; }

        public ICollection<State> States { get; set; } = new List<State>();
    }
}
