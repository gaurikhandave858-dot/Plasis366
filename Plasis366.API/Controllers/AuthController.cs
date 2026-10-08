using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Plasis366.API;
using Plasis366.Domain;
using Plasis366.Infrastructure;
using static Plasis366.API.AuthDtos;

namespace Plasis366.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrEmpty(request.Password) ||
                request.Password.Length < 6)
            {
                return BadRequest(new { message = "First name, email and a password of at least 6 characters are required." });
            }

            // Only these two account types can be created from the public form.
            // Admin is deliberately not allowed here.
            var requestedRole = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role.Trim();
            if (requestedRole != "Customer" && requestedRole != "Designer")
                return BadRequest(new { message = "Invalid account type." });

            var email = request.Email.Trim();

            var tenantCode = _config["DefaultTenantCode"];
            var tenant = await _db.Tenants
                .FirstOrDefaultAsync(t => t.TenantCode == tenantCode && t.IsActive);
            if (tenant == null)
                return BadRequest(new { message = "Default studio was not found." });

            var emailTaken = await _db.Users
                .AnyAsync(u => u.TenantId == tenant.TenantId && u.Email == email);
            if (emailTaken)
                return Conflict(new { message = "This email is already registered." });

            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.TenantId == tenant.TenantId && r.RoleName == requestedRole && r.IsActive);
            if (role == null)
                return BadRequest(new { message = $"The {requestedRole} role was not found." });

            var user = new User
            {
                TenantId = tenant.TenantId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName?.Trim() ?? "",
                Email = email,
                PhoneNumber = request.PhoneNumber,
                UserName = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            user.UserRoles.Add(new UserRole { RoleId = role.RoleId });

            _db.Users.Add(user);

            // Only customers get a row in the Customers table
            if (requestedRole == "Customer")
            {
                var customer = new Customer
                {
                    TenantId = tenant.TenantId,
                    User = user,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = email,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address
                };
                _db.Customers.Add(customer);
            }

            await _db.SaveChangesAsync();   // saves user, role link and customer together

            return Ok(new { message = "Registration successful. Please log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var email = request.Email.Trim();

            var user = await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

            if (user == null || !PasswordMatches(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            var roles = user.UserRoles
                .Where(ur => ur.IsActive && ur.Role != null)
                .Select(ur => ur.Role!.RoleName)
                .ToList();

            // Null for designers and admins, because they have no Customers row
            var customerId = await _db.Customers
                .Where(c => c.UserId == user.UserId)
                .Select(c => (long?)c.CustomerId)
                .FirstOrDefaultAsync();

            var expires = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpiryMinutes"]!));

            var claims = new List<Claim>
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("email", user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim("tenantId", user.TenantId.ToString())
            };
            foreach (var role in roles)
                claims.Add(new Claim("role", role));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);

            return Ok(new AuthResponse
            {
                Token = token,
                ExpiresAt = expires,
                UserId = user.UserId,
                TenantId = user.TenantId,
                CustomerId = customerId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles
            });
        }

        private static bool PasswordMatches(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;   // the stored value is not a valid BCrypt hash
            }
        }
    }
}