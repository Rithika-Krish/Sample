using SampleAuth_WebAPI.Data;
using SampleAuth_WebAPI.DTO;
using SampleAuth_WebAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SampleAuth_WebAPI.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ---------------- REGISTER ----------------
        public string Register(RegisterDTO dto)
        {
            var exists = _context.Users.Any(x => x.Email == dto.Email);

            if (exists)
                return "User already exists";

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return "User registered successfully";
        }

        // ---------------- LOGIN ----------------
        public string Login(LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
                return null;

            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }

        // ---------------- HASH ----------------
        private string HashPassword(string password)
        {
            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            return Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(password))
            );
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        // ---------------- JWT ----------------
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "SampleAuthAPI",
                audience: "SampleAuthAPI",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}