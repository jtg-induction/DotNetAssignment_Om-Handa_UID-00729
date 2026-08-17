using Microsoft.Owin;
using Owin;
using System.Configuration;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

[assembly:OwinStartup(typeof(DotNet_Assignment.Startup))]

namespace DotNet_Assignment
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureJwt(app);
        }

        public void ConfigureJwt(IAppBuilder app) {

            var key = ConfigurationManager.AppSettings["JwtKey"];

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,

                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), 
                    ValidateIssuer=false,
                    ValidateAudience=false,
                    ValidateLifetime=true,
                    RoleClaimType= ClaimTypes.Role
                }
            });
        }
    }
}
