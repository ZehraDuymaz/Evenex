using System.Text.Json;
using AutoMapper;
using Evenex.Application.Events;
using Evenex.Application.Mappings;
using Evenex.Application.Venues;
using Evenex.Common.Hashing;
using Evenex.Domain.Entities;
using HashidsNet;
using Microsoft.Extensions.DependencyInjection;

namespace Evenex.Application.Tests.Mappings;

/// <summary>
/// AutoMapper kurallarının ve BaseDto'nun otomatik id hash'lemesinin doğru çalıştığını
/// veritabanına/API'ye ihtiyaç duymadan test eder.
/// Program.cs'teki kayıtların aynısını küçük bir DI container'da kurup IMapper alıyoruz.
/// </summary>
public class MappingProfileTests
{
    private readonly IHashids _hashids = new Hashids("test-salt", 8);
    private readonly IMapper _mapper;

    // API'nin kullandığı JSON ayarlarının aynısı (camelCase, büyük/küçük harf duyarsız)
    private static readonly JsonSerializerOptions WebJson = new(JsonSerializerDefaults.Web);

    public MappingProfileTests()
    {
        // Program.cs'te olduğu gibi: BaseDto'nun kullandığı statik yardımcıyı ayarla
        HashidsHelper.Configure(_hashids);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), Type.EmptyTypes);
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

        Assert.Equal(42, dto.Id);                         // içeride gerçek id duruyor
        Assert.Equal(_hashids.Encode(42), dto.HashedId);  // BaseDto kendisi hash'ledi
        Assert.Equal("Zorlu PSM", dto.Name);
        Assert.Equal("İstanbul", dto.Address);
    }

    [Fact]
    public void Event_To_EventResponseDto_HashesIdAndVenueId()
    {
        var ev = new Event { Id = 7, VenueId = 42, Title = "Konser", Description = "Açıklama", EventDate = new DateTime(2030, 1, 1) };

        var dto = _mapper.Map<EventResponseDto>(ev);

        Assert.Equal(_hashids.Encode(7), dto.HashedId);
        Assert.Equal(_hashids.Encode(42), dto.VenueId);   // foreign key MappingProfile'da hash'lendi
        Assert.Equal("Konser", dto.Title);
        Assert.Equal("Draft", dto.Status);
    }

    [Fact]
    public void ResponseDto_Json_ShowsOnlyHashedIdAsId()
    {
        var dto = _mapper.Map<VenueResponseDto>(new Venue { Id = 42, Name = "Zorlu PSM", Address = "İstanbul" });

        var json = JsonSerializer.Serialize(dto, WebJson);

        // Frontend eskisi gibi "id" alanında hash'i görmeli; gerçek 42 hiçbir yerde olmamalı
        Assert.Contains($"\"id\":\"{_hashids.Encode(42)}\"", json);
        Assert.DoesNotContain("42", json);
    }

    [Fact]
    public void ResponseDto_FromJson_DecodesHashedIdBackToRealId()
    {
        // İçeri gelen yön: JSON'daki hash, BaseDto'nun init bloğunda gerçek id'ye çözülür
        var json = $"{{\"id\":\"{_hashids.Encode(42)}\",\"name\":\"Zorlu PSM\",\"address\":\"İstanbul\",\"capacity\":0}}";

        var dto = JsonSerializer.Deserialize<VenueResponseDto>(json, WebJson)!;

        Assert.Equal(42, dto.Id);
    }

    [Fact]
    public void ResponseDto_FromJson_WithFakeId_ThrowsFormatException()
    {
        // Uydurulmuş id -> FormatException. API'de bu, 500 değil 400 Bad Request olarak döner.
        var json = "{\"id\":\"uydurma!!\",\"name\":\"x\",\"address\":\"y\",\"capacity\":0}";

        Assert.Throws<FormatException>(() => JsonSerializer.Deserialize<VenueResponseDto>(json, WebJson));
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
