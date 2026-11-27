using AutoMapper;
using Evenex.Application.Events;
using Evenex.Application.EventSections;
using Evenex.Application.Venues;
using Evenex.Application.VenueSections;
using Evenex.Domain.Entities;

namespace Evenex.Application.Mappings;

/// <summary>
/// Entity ↔ DTO dönüşüm kuralları tek bir yerde.
/// İsimleri aynı olan alanları (Name, Title, ...) AutoMapper kendisi eşler,
/// burada sadece "özel" olanları yazıyoruz.
///
/// DTO'ların KENDİ id'leri artık burada hash'lenmiyor: entity.Id (int) -> dto.Id (int) isim aynı olduğu için
/// kendiliğinden kopyalanıyor, hash'leme ise BaseDto'nun init bloğunda otomatik oluyor.
/// Eskiden her map'te ForCtorParam(Id, Encode) satırı vardı; hepsi gitti.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // HashedId'yi AutoMapper doldurmasın: Id set edilince BaseDto kendisi hesaplıyor.
        // Global ignore = her map için tek tek .Ignore() yazmamak. Bu olmasa "HashedId map'lenmedi"
        // diye konfigürasyon kontrolü (AssertConfigurationIsValid) patlardı.
        AddGlobalIgnore("HashedId");

        // ---------- Entity -> Response DTO (veritabanından dışarıya) ----------
        // Hiçbir id burada hash'lenmiyor: kendi id'ler BaseDto'da, foreign key'ler (VenueId, EventId...)
        // DTO'daki [HashId] etiketiyle JSON'a yazılırken hash'leniyor. Burada hepsi int -> int, isimler aynı,
        // AutoMapper kendiliğinden kopyalıyor.

        CreateMap<Venue, VenueResponseDto>()
            // TODO: Kapasite henüz hesaplanmıyor; Section/Seat'ler tamamlanınca buradan toplanacak.
            .ForMember(d => d.Capacity, o => o.MapFrom(_ => 0));

        CreateMap<Event, EventResponseDto>();
        CreateMap<Section, SectionResponseDto>();
        CreateMap<EventSection, EventSectionResponseDto>();

        // ---------- Request DTO -> Entity (dışarıdan veritabanına) ----------
        // MemberList.Source: "DTO'daki her alan bir yere gitmeli" kontrolü yapılır.
        // Entity'deki Id, CreatedUser, IsDeleted gibi alanlar DTO'da olmadığı için
        // kontrol dışı kalır; Created* alanlarını AuditInterceptor dolduruyor.
        // VenueId artık DTO'da da gerçek int (JSON'dan gelirken [HashId] çözdü), o yüzden düz kopyalanıyor.
        // "Böyle bir mekân var mı?" kontrolünü servis yapıyor.

        CreateMap<CreateVenueDto, Venue>(MemberList.Source);
        CreateMap<UpdateVenueDto, Venue>(MemberList.Source);

        CreateMap<CreateEventDto, Event>(MemberList.Source);
        CreateMap<UpdateEventDto, Event>(MemberList.Source);

        CreateMap<CreateSectionDto, Section>(MemberList.Source);
    }
}
