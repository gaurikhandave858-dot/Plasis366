using System;
using System.Collections.Generic;
using System.Text;

namespace Plasis366.Domain
{
    public class Room :AuditEntity
    {
        public long RoomId { get; set; }

        public long PropertyId { get; set; }

        public string RoomType { get; set; } = string.Empty;

        public string? RoomName { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public int? DoorCount { get; set; }

        public int? WindowCount { get; set; }

        public string? Details { get; set; }

        // Navigation Property
        public Property Property { get; set; } = null!;

        // Navigation Property
        public ICollection<DesignRequirement> DesignRequirements { get; set; }
            = new List<DesignRequirement>();
    }
}
