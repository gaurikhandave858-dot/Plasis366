using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class DesignRequirement : AuditEntity
    {
        public long DesignRequirementId { get; set; }

        [Range(1, long.MaxValue)]
        public long ProjectId { get; set; }

        [Range(1, long.MaxValue)]
        public long? RoomId { get; set; }

        [StringLength(100)]
        public string? PreferredStyle { get; set; }

        [StringLength(300)]
        public string? PreferredColors { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99")]
        public decimal? Budget { get; set; }

        [StringLength(2000)]
        public string? FurnitureRequirements { get; set; }

        [StringLength(2000)]
        public string? StorageRequirements { get; set; }

        [StringLength(2000)]
        public string? LightingRequirements { get; set; }

        [StringLength(3000)]
        public string? SpecialRequirements { get; set; }

        [StringLength(4000)]
        public string? AdditionalNotes { get; set; }

        public Project? Project { get; set; }

        public Room? Room { get; set; }
    }
}
