using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Evenex.Infrastructure;
using Evenex.Application.Auth;
using Evenex.Domain.Repositories;
using Evenex.Infrastructure.Repositories;
using Evenex.Infrastructure.Auth;
using Scalar.AspNetCore;
using HashidsNet;
using Evenex.Application.Events;
using Evenex.Application.EventSections;
using Evenex.Application.Venues;
using Evenex.Application.VenueSections;
using Evenex.Application.Abstractions;
using Evenex.Application.Mappings;
using Evenex.Api.Middleware;
using Evenex.Api.ModelBinding;
using Evenex.Infrastructure.Interceptors;
using Evenex.Common.Hashing;

var builder = WebApplication.CreateBuilder(args);

// Database bağlama
// AuditInterceptor: her SaveChanges'tan önce Created*/Modified* alanlarını otomatik doldurur.
// Scoped, çünkü o anki isteğin kullanıcısını (ICurrentUserService) bilmesi gerekiyor.
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) => options
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Hashids: int id'leri dışarıya "wk1JaEWX" gibi tahmin edilemeyen string'ler olarak gösterir.
// Salt değişirse daha önce verilmiş tüm id'ler geçersiz olur, o yüzden sabit kalmalı.
// Singleton: uygulama boyunca tek bir nesne yeterli, durumu (state) yok.
var hashids = new Hashids(
    builder.Configuration["Hashids:Salt"] ?? throw new InvalidOperationException("Hashids:Salt ayarı eksik."),
    minHashLength: 8);
builder.Services.AddSingleton<IHashids>(hashids);
// Aynı nesneyi statik yardımcıya da veriyoruz: BaseDto (DI'dan bir şey alamayan DTO'lar) id'leri bununla
// hash'liyor. Tek nesne = servisler, JWT ve DTO'lar hep aynı salt ile aynı hash'i üretir.
HashidsHelper.Configure(hashids);

// AutoMapper: IMapper'ı DI'a kaydeder ve MappingProfile'ı ekler
builder.Services.AddAutoMapper(cfg =>
{
    // Boş string verilirse "geçersiz anahtar" hatası loglanıyor, o yüzden sadece doluysa set ediyoruz
    var licenseKey = builder.Configuration["AutoMapper:LicenseKey"];
    if (!string.IsNullOrWhiteSpace(licenseKey)) cfg.LicenseKey = licenseKey;
    cfg.AddProfile<MappingProfile>();
}, Type.EmptyTypes); // Assembly taraması yapma, profili zaten elle ekledik

// Scoped: her HTTP isteği için yeni bir nesne, istek bitince atılır (DbContext ile aynı ömür)
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<IEventSectionRepository, EventSectionRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<VenueService>();
builder.Services.AddScoped<SectionService>();
builder.Services.AddScoped<EventSectionService>();

// Yakalanmayan exception'ları doğru HTTP koduna çeviren handler (bkz. Middleware/GlobalExceptionHandler.cs)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// authN için Configuration
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Controllers
// JsonStringEnumConverter: enum'lar JSON'da sayı yerine İSİM olarak yazılır/okunur
// ("status": "Published", 1 değil). Frontend'in "1 neydi?" diye ezber yapmasına gerek kalmaz.
// Okurken hem "Published" hem eski usul sayı (1) kabul edilir.
// ModelBinderProviders.Insert(0, ...): URL'deki [HashId] int parametreleri bizim binder'ımız çözsün.
// En başa ekliyoruz ki ASP.NET'in hazır int binder'ından önce sorulsun.
builder.Services.AddControllers(options => options.ModelBinderProviders.Insert(0, new HashIdModelBinderProvider()))
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi(options =>
{
    // Scalar arayüzünde "Authorize" alanı çıksın ve token girilerek korumalı endpoint'ler test edilebilsin diye
    // OpenAPI dokümanına Bearer (JWT) güvenlik şemasını ekliyoruz.
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new();

        var securityScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Tokeni gir: "
        };

        document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = securityScheme;

        document.Security ??= [];
        document.Security.Add(new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Middleware sırası önemli: istek bunlardan yukarıdan aşağıya geçer.
// Exception handler en başta olmalı ki altındaki her şeyin hatasını yakalayabilsin.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title  = "Evenex API";
        options.Theme  = ScalarTheme.DeepSpace;
    });
}

app.UseHttpsRedirection();
app.UseCors("AngularApp");

// authN ve authZ ekleme. Sıra: önce "kimsin?" (Authentication), sonra "yetkin var mı?" (Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
