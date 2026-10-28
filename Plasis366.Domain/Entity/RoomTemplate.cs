using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    // A ready-made room look that the studio offers. The customer picks one per room,
    // and the designer sees it on the project.
    public class RoomTemplate : AuditEntity
    {
        public long RoomTemplateId { get; set; }

        [Range(1, long.MaxValue)]
        public long TenantId { get; set; }

        [Required, StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // The same words the project form uses: Living room, Bedroom, Kitchen ...
        [Required, StringLength(50)]
        public string RoomType { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Style { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public Tenant? Tenant { get; set; }

        public ICollection<RoomTemplateImage> Images { get; set; } = new List<RoomTemplateImage>();
    }
}