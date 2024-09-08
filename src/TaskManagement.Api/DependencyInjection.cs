using Microsoft.AspNetCore.Authentication.JwtBearer;
using TaskManagement.Domain.Contracts.Auth;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaskManagement.Api
{
    public static class DependencyInjection
    {
        public static void AddApiAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            //Options
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<PasswordComplexityOptions>(configuration.GetSection(PasswordComplexityOptions.SectionName));

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ??
                 throw new InvalidOperationException("JWT options are not configured properly.");
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidateIssuerSigningKey = true,
                    };
                });
        } 
    }
}
