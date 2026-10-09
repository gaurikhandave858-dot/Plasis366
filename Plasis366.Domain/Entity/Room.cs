using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Room : AuditEntity
    {
        public long RoomId { get; set; }

        [Range(1, long.MaxValue)]
        public long PropertyId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        public string RoomType { get; set; } = string.Empty;

        [StringLength(100)]
        public string? RoomName { get; set; }

        [Range(typeof(decimal), "0.01", "10000")]
        public decimal? Length { get; set; }

        [Range(typeof(decimal), "0.01", "10000")]
        public decimal? Width { get; set; }

        [Range(typeof(decimal), "0.01", "10000")]
        public decimal? Height { get; set; }

        [Range(0, 1000)]
        public int? DoorCount { get; set; }

        [Range(0, 1000)]
        public int? WindowCount { get; set; }

        [StringLength(2000)]
        public string? Details { get; set; }

        public Property? Property { get; set; }

        public ICollection<DesignRequirement> DesignRequirements { get; set; } = new List<DesignRequirement>();
    }
}
