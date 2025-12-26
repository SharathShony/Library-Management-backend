using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Libraray.Api.DTO.Users;
using Libraray.Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Libraray.Api.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
        {
    _configuration = configuration;
       }

        public string GenerateToken(UserClaimsDto userClaims)
        {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        var jwtExpiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userClaims.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, userClaims.Email),
        new Claim(ClaimTypes.Name, userClaims.Username),
        new Claim(ClaimTypes.Role, userClaims.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(jwtExpiryMinutes),
        signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
      }
    }
}
