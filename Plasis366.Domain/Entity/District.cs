using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class District : AuditEntity
    {
        public long DistrictId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string DistrictName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? DistrictCode { get; set; }

        [Range(1, long.MaxValue)]
        public long StateId { get; set; }

        public State? State { get; set; }

        public ICollection<Taluka> Talukas { get; set; } = new List<Taluka>();
    }
}
