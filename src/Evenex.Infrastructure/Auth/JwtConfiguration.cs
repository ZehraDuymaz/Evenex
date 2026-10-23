using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Evenex.Infrastructure.Auth;

/// <summary>
/// JWT authentication ayarlarını tek bir yerde toplayan extension method.
/// Program.cs'te sadece <c>builder.Services.AddJwtAuthentication(...)</c> yazılarak çağrılır.
/// </summary>
public static class JwtConfigurations
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        // RS256 (asimetrik) imzalama: token private key ile imzalanır, public key ile doğrulanır.
        // DİKKAT: Anahtar her uygulama başlangıcında yeniden üretiliyor. Yani API yeniden
        // başlatılınca önceki tüm tokenlar geçersiz olur ve tekrar login olmak gerekir.
        // Production'a geçmeden önce anahtar bir dosyadan / secret store'dan okunmalı.
        var rsa = RSA.Create(2048);
        var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));
        var privateKey = new RsaSecurityKey(rsa.ExportParameters(true));

        // JwtTokenGenerator imzalamak için private key'e ihtiyaç duyuyor, DI üzerinden veriyoruz.
        services.AddSingleton(privateKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                // Buradaki key'ler JwtTokenGenerator'ın kullandığı key'lerle (Jwt:Issuer, Jwt:Audience)
                // birebir aynı olmak zorunda. Farklı olursa token üretilir ama hiçbir istekte kabul edilmez.
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = publicKey
            };
        });
        return services;
    }
}
