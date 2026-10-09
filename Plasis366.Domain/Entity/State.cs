using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class State : AuditEntity
    {
        public long StateId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string StateName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? StateCode { get; set; }

        [Range(1, long.MaxValue)]
        public long CountryId { get; set; }

        public Country? Country { get; set; }

        public ICollection<District> Districts { get; set; } = new List<District>();
    }
}
