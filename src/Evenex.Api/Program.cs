using Microsoft.EntityFrameworkCore;
using Evenex.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Evenex.Application.Auth;
using Evenex.Domain.Repositories;
using Evenex.Infrastructure.Repositories;
using Evenex.Infrastructure.Auth;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database bağlama
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AuthService>();


//Authentication and Authorization Servisleri
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    
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
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ── Controller'lar + OpenAPI ─────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new();
        
        // Use the native OpenAPI components dictionary and scheme definition
        var securityScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter 'Bearer' [space] and then your token."
        };

        // Safely add the security scheme component
        document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = securityScheme;

        // Add the global security requirement so Scalar picks it up automatically
        document.Security ??= [];
        document.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                  // /openapi/v1.json endpoint'i
    app.MapScalarApiReference(options =>
    {
        options.Title  = "Evenex API";
        options.Theme  = ScalarTheme.DeepSpace;  // istersen Default, Moon, Purple vb.
    });
}

app.UseHttpsRedirection();
app.UseCors("AngularApp");

// authN ve authZ ekleme
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();