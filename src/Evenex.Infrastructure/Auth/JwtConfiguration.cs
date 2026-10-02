
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HashidsNet;

namespace Evenex.Infrastructure.Auth;

public static class JwtConfigurations
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        var rsa = RSA.Create(2048);
        var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));
        var privateKey = new RsaSecurityKey(rsa.ExportParameters(true));
        services.AddSingleton(privateKey);
        services.AddSingleton<IHashids>(new Hashids("EvenexSuperSecretDeliciousDesserts", 8));
        services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true, 
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt-Bearer"],
                ValidAudience = configuration["Jwt-Audience"],
                IssuerSigningKey = publicKey
            };
        });
        return services;
    }
}