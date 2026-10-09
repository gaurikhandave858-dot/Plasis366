using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Region : AuditEntity
    {
        public long RegionId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string RegionName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? RegionCode { get; set; }

        public ICollection<Country> Countries { get; set; } = new List<Country>();
    }
}
