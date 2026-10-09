using System.ComponentModel.DataAnnotations;

namespace Plasis366.API.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name can be at most 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name can be at most 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        // 100, not 150: the email is also saved as the user name, which allows 100
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email can be at most 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^\+?[0-9\s\-]{7,15}$", ErrorMessage = "Please enter a valid phone number.")]
        public string? PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Address can be at most 500 characters.")]
        public string? Address { get; set; }

        // BCrypt only uses the first 72 bytes, so a longer password would be silently cut
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Password must be 8 to 72 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Password must include at least one letter and one number.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = string.Empty;

        // No strength rules at login, only at registration
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public long UserId { get; set; }
        public long TenantId { get; set; }
        public long? CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}