using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using bloggit.Models;
using Microsoft.IdentityModel.Tokens;

namespace bloggit.Services.Service_Implements
{
    public class TokenService : ITokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        public TokenService(IConfiguration configuration)
        {
            _key = configuration.GetSection("JWT:AccessTokenKey").Value!;
            _issuer = configuration.GetSection("JWT:Issuer").Value!;
            _audience = configuration.GetSection("JWT:Audience").Value!;
        }
        public string GenerateToken(ApplicationUser user, IList<string> userRoles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);
            
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }


            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: authClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
