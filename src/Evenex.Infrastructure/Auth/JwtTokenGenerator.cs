using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Evenex.Application.Auth;
using Evenex.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HashidsNet;

namespace Evenex.Infrastructure.Auth;

/// <summary>
/// Login/register sonrası kullanıcıya verilen JWT'yi üretir.
/// Token 3 parçadan oluşur: header.payload.signature. Payload'da aşağıdaki claim'ler var,
/// signature ise private key ile atılan imza; token'ın değiştirilmediğini bu kanıtlar.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;
    private readonly RsaSecurityKey _privateKey;
    private readonly IHashids _hashids;

    public JwtTokenGenerator(IConfiguration config, RsaSecurityKey privateKey, IHashids hashids) 
    {
        _config = config;
        _privateKey = privateKey;
        _hashids = hashids;
    }

    public string GenerateToken(User user)
    {   
        
        // Claim = token'ın içine yazılan "bilgi kartı". Token'ı çözen herkes bunları okuyabilir
        // (şifreli değil, sadece imzalı), o yüzden buraya asla şifre gibi gizli bilgi konmaz.
        // Id de bu yüzden hash'lenerek yazılıyor.
        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, _hashids.Encode(user.Id)),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        

        var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            // Issuer/Audience appsettings.json'daki "Jwt" bölümünden geliyor.
            // JwtConfiguration da doğrularken aynı key'leri kullanıyor, ikisi eşleşmek zorunda.
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // 2 saat sonra token geçersiz, tekrar login gerekir
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}