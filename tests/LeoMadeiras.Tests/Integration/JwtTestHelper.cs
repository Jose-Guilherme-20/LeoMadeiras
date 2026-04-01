using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LeoMadeiras.Tests.Integration
{
    public static class JwtTestHelper
    {
        private const string Key = "QbdJcCQALuk2Pwke8RWG5YPUCyawcZST";
        private const string Issuer = "leomadeiras-api";
        private const string Audience = "leomadeiras-client";

        public static string GerarToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "teste@email.com"),
                new Claim(ClaimTypes.Name, "Teste")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}