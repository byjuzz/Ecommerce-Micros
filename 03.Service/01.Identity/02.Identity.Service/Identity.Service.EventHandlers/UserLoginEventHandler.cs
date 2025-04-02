using Identity.Domain;
using Identity.Persistence.Database;
using Identity.Service.EventHandlers.Commands;
using Identity.Service.EventHandlers.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Service.EventHandlers
{
    public class UserLoginEventHandler : IRequestHandler<UserLoginCommand, IdentityAccess>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserLoginEventHandler(
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IdentityAccess> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var result = new IdentityAccess();
            var user = await _context.ApplicationUsers.SingleOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
                return result;

            var response = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (response.Succeeded)
            {
                result.Succeded = true;
                await GenerateToken(user, result);
            }

            return result;
        }

        private async Task GenerateToken(ApplicationUser user, IdentityAccess identityAccess)
        {
            var secretKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            Console.WriteLine($"🔐 secretKey: {secretKey}");
            Console.WriteLine($"🌍 issuer: {issuer}");
            Console.WriteLine($"👥 audience: {audience}");

            if (string.IsNullOrEmpty(secretKey))
            {
                Console.WriteLine("❌ Error: El secreto JWT está vacío o nulo.");
                return;
            }

            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? "N/A"),
        new Claim(ClaimTypes.NameIdentifier, user.Id ?? "N/A"),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Name  , user.FirstName   ?? string.Empty),
        new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
    };

            Console.WriteLine("📌 Claims generados:");
            foreach (var claim in claims)
            {
                Console.WriteLine($" - {claim.Type}: {claim.Value}");
            }

            // 🔐 Agregar roles
            var roles = await _context.ApplicationRoles
                .Where(x => x.UserRoles.Any(y => y.UserId == user.Id))
                .ToListAsync();

            foreach (var role in roles)
            {
                Console.WriteLine($"🛡️ Rol encontrado: {role.Name}");
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(createdToken);
            identityAccess.AccessToken = token;

            Console.WriteLine("✅ Token generado correctamente:");
            Console.WriteLine(token);
        }

    }
}
