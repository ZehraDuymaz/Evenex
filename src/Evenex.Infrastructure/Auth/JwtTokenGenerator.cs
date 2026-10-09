using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Evenex.Application.Auth;
using Evenex.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HashidsNet;

namespace Evenex.Infrastructure.Auth;

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
        
        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, _hashids.Encode(user.Id)),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        

        var creds = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            // add config !!!
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}