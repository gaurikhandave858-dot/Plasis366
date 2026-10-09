using System.ComponentModel.DataAnnotations;

namespace Plasis366.Domain
{
    public class Tenant : AuditEntity
    {
        public long TenantId { get; set; }

        [Required, StringLength(150, MinimumLength = 2)]
        public string TenantName { get; set; } = string.Empty;

        [Required, StringLength(50, MinimumLength = 2)]
        public string TenantCode { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [EmailAddress, StringLength(254)]
        public string? Email { get; set; }

        [Phone, StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        [Range(1, long.MaxValue)]
        public long? CountryId { get; set; }

        public Country? Country { get; set; }
    }
}
