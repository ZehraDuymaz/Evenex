using Microsoft.EntityFrameworkCore;
using Evenex.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Evenex.Application.Auth;
using Evenex.Domain.Repositories;
using Evenex.Infrastructure.Repositories;
using Evenex.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

// rsa => encryption için 
using var rsa = RSA.Create(2048);
var publicKey = new RsaSecurityKey(rsa.ExportParameters(false));
var privateKey = new RsaSecurityKey(rsa.ExportParameters(true));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(publicKey);
builder.Services.AddSingleton(privateKey);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<AuthService>();



//Authentication and Authorization Servisleri
builder.Services.AddAuthentication(JwtBearerDefaults.Authentication.AddJwtBearer(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true, 
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = publicKey,
    };    
}));

builder.Services.AddAuthorization();

// Database bağlama
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// authN ve authZ ekleme
app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();

app.Run();