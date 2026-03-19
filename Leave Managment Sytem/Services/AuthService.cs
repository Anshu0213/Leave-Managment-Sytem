using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Leave_Managment_Sytem.Models;
using Org.BouncyCastle.Crypto.Generators;

namespace Leave_Managment_Sytem.Services
{
        public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly LeaveManagmentContext _context;

        public AuthService(LeaveManagmentContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<int> RegisterAsync(RegisterRequest request)
        {
            var validRoles = new[] { "Employee", "Manager" };
            if (!validRoles.Contains(request.Role))
                throw new ArgumentException("Invalid role");

            var exists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (exists)
                throw new Exception("User already exists");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                throw new Exception("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new Exception("Invalid credentials");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
