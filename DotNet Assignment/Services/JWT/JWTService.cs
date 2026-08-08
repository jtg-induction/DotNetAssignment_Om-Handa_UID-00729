using DotNet_Assignment.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNet_Assignment.Services.JWT
{
    public class JWTService : IJWTService
    {
        public string GetAccessToken(User user )
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];

            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(issuer, issuer, claims, expires: DateTime.UtcNow.AddMinutes(15), signingCredentials: credentials);

            var jWTtoken = new JwtSecurityTokenHandler().WriteToken(token);

            return jWTtoken;
        }

        public string GenerateRefreshToken()
        {
            byte[] Byte = new byte[64];

            using (var rng = RandomNumberGenerator.
                Create()) {
                rng.GetBytes(Byte);
            }

            return Convert.ToBase64String(Byte);
 
        }
    }
}
