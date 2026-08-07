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
            var Key = ConfigurationManager.AppSettings["JwtKey"];

            var Issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

            var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

            var Claims = new List<Claim>() {
                new Claim("UserID", user.UserId.ToString()),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role.ToString())
            };

            var token = new JwtSecurityToken(Issuer, Issuer, Claims, expires: DateTime.UtcNow.AddMinutes(15), signingCredentials: Credentials);

            var JWTtoken = new JwtSecurityTokenHandler().WriteToken(token);

            return JWTtoken;
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
