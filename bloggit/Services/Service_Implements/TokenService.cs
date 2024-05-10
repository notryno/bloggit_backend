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
        public string GenerateToken(ApplicationUser user, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            // // Perform null check on user.Id
            // var idClaim = user.Id != null ? new Claim(ClaimTypes.NameIdentifier, user.Id) : null;
            // var emailClaim = user.Email != null ? new Claim(ClaimTypes.Email, user.Email) : null;
            // var roleClaim = role != null ? new Claim(ClaimTypes.Role, role) : null;
            //
            // var claims = new List<Claim>();
            // if (idClaim != null) claims.Add(idClaim);
            // if (emailClaim != null) claims.Add(emailClaim);
            // if (roleClaim != null) claims.Add(roleClaim);
            
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.Id))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
