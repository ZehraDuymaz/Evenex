using AutoMapper;
using Evenex.Application.Events;
using Evenex.Application.EventSections;
using Evenex.Application.Venues;
using Evenex.Application.VenueSections;
using Evenex.Common.Hashing;
using Evenex.Domain.Entities;

namespace Evenex.Application.Mappings;

/// <summary>
/// Entity ↔ DTO dönüşüm kuralları tek bir yerde.
/// İsimleri aynı olan alanları (Name, Title, ...) AutoMapper kendisi eşler,
/// burada sadece "özel" olanları (foreign key hash'leme, elle set edilecek alanlar) yazıyoruz.
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
        // Kendi id'leri BaseDto'da hash'leniyor. Foreign key'ler (VenueId, EventId...) ise
        // BaseDto'nun kapsamında değil, onları burada HashidsHelper ile hash'liyoruz.

        CreateMap<Venue, VenueResponseDto>()
            // TODO: Kapasite henüz hesaplanmıyor; Section/Seat'ler tamamlanınca buradan toplanacak.
            .ForMember(d => d.Capacity, o => o.MapFrom(_ => 0));

        CreateMap<Event, EventResponseDto>()
            .ForMember(d => d.VenueId, o => o.MapFrom(s => HashidsHelper.Encode(s.VenueId)));

        CreateMap<Section, SectionResponseDto>()
            .ForMember(d => d.VenueId, o => o.MapFrom(s => HashidsHelper.Encode(s.VenueId)));

        CreateMap<EventSection, EventSectionResponseDto>()
            .ForMember(d => d.EventId, o => o.MapFrom(s => HashidsHelper.Encode(s.EventId)))
            .ForMember(d => d.SectionId, o => o.MapFrom(s => HashidsHelper.Encode(s.SectionId)));

        // ---------- Request DTO -> Entity (dışarıdan veritabanına) ----------
        // MemberList.Source: "DTO'daki her alan bir yere gitmeli" kontrolü yapılır.
        // Entity'deki Id, CreatedUser, IsDeleted gibi alanlar DTO'da olmadığı için
        // kontrol dışı kalır; onları servis elle dolduruyor.
        // Hash'li id'ler (VenueId gibi) burada map'lenmiyor çünkü decode hatalı olabilir,
        // o kontrolü servis yapıyor (bkz. HashidsExtensions.DecodeId).

        CreateMap<CreateVenueDto, Venue>(MemberList.Source);
        CreateMap<UpdateVenueDto, Venue>(MemberList.Source);

        CreateMap<CreateEventDto, Event>(MemberList.Source)
            .ForSourceMember(s => s.VenueId, o => o.DoNotValidate())
            .ForMember(d => d.VenueId, o => o.Ignore());
        CreateMap<UpdateEventDto, Event>(MemberList.Source);

        CreateMap<CreateSectionDto, Section>(MemberList.Source)
            .ForSourceMember(s => s.VenueId, o => o.DoNotValidate())
            .ForMember(d => d.VenueId, o => o.Ignore());
    }
}
