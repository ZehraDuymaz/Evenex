using AutoMapper;
using Evenex.Application.Events;
using Evenex.Application.Mappings;
using Evenex.Application.Venues;
using Evenex.Domain.Entities;
using HashidsNet;
using Microsoft.Extensions.DependencyInjection;

namespace Evenex.Application.Tests.Mappings;

/// <summary>
/// AutoMapper kurallarının doğru çalıştığını veritabanına/API'ye ihtiyaç duymadan test eder.
/// Program.cs'teki kayıtların aynısını küçük bir DI container'da kurup IMapper alıyoruz.
/// </summary>
public class MappingProfileTests
{
    private readonly IHashids _hashids = new Hashids("test-salt", 8);
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_hashids);
        services.AddAutoMapper((sp, cfg) => cfg.AddProfile(new MappingProfile(sp.GetRequiredService<IHashids>())), Type.EmptyTypes);
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact]
    public void Configuration_IsValid()
    {
        // Eşleşmeyen / unutulmuş bir alan varsa bu test patlar; yeni map eklerken ilk koruma bu.
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Venue_To_VenueResponseDto_HashesId()
    {
        var venue = new Venue { Id = 42, Name = "Zorlu PSM", Address = "İstanbul" };

        var dto = _mapper.Map<VenueResponseDto>(venue);

        Assert.Equal(_hashids.Encode(42), dto.Id);
        Assert.NotEqual("42", dto.Id); // gerçek id asla dışarı çıkmamalı
        Assert.Equal("Zorlu PSM", dto.Name);
        Assert.Equal("İstanbul", dto.Address);
    }

    [Fact]
    public void Event_To_EventResponseDto_HashesIdAndVenueId()
    {
        var ev = new Event { Id = 7, VenueId = 42, Title = "Konser", Description = "Açıklama", EventDate = new DateTime(2030, 1, 1) };

        var dto = _mapper.Map<EventResponseDto>(ev);

        Assert.Equal(_hashids.Encode(7), dto.Id);
        Assert.Equal(_hashids.Encode(42), dto.VenueId);
        Assert.Equal("Konser", dto.Title);
        Assert.Equal("Draft", dto.Status);
    }

    [Fact]
    public void UpdateVenueDto_OntoExistingVenue_KeepsIdAndAuditFields()
    {
        var venue = new Venue { Id = 5, Name = "Eski", Address = "Eski adres", CreatedUser = "a@b.com" };

        _mapper.Map(new UpdateVenueDto("Yeni", "Yeni adres"), venue);

        Assert.Equal("Yeni", venue.Name);
        Assert.Equal("Yeni adres", venue.Address);
        Assert.Equal(5, venue.Id);                  // id'ye dokunulmamalı
        Assert.Equal("a@b.com", venue.CreatedUser); // audit alanlarına dokunulmamalı
    }
}
