using AutoMapper;
using Evenex.Application.Events;
using Evenex.Application.EventSections;
using Evenex.Application.Venues;
using Evenex.Application.VenueSections;
using Evenex.Domain.Entities;
using HashidsNet;

namespace Evenex.Application.Mappings;

/// <summary>
/// Entity ↔ DTO dönüşüm kuralları tek bir yerde.
/// İsimleri aynı olan alanları (Name, Title, ...) AutoMapper kendisi eşler,
/// burada sadece "özel" olanları (id hash'leme, elle set edilecek alanlar) yazıyoruz.
///
/// Neden ForMember değil de ForCtorParam?
/// Response DTO'larımız positional record: (string Id, string Name, ...). Bunlar sadece
/// constructor ile oluşturulabiliyor, o yüzden AutoMapper değerleri constructor parametrelerine veriyor.
/// ForMember property'yi ayarlar ve constructor'a etki etmez; Id "7" gibi düz yazıya dönüşür.
/// ForCtorParam ise doğrudan constructor parametresini ayarlar.
///
/// IHashids constructor'dan geliyor; Program.cs'te profil DI container'dan alınan IHashids ile oluşturuluyor.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile(IHashids hashids)
    {
        // ---------- Entity -> Response DTO (veritabanından dışarıya) ----------
        // Gerçek int id'ler asla dışarı çıkmaz, hepsi hashids.Encode ile string'e çevrilir.

        CreateMap<Venue, VenueResponseDto>()
            .ForCtorParam(nameof(VenueResponseDto.Id), o => o.MapFrom(s => hashids.Encode(s.Id)))
            // TODO: Kapasite henüz hesaplanmıyor; Section/Seat'ler tamamlanınca buradan toplanacak.
            .ForCtorParam(nameof(VenueResponseDto.Capacity), o => o.MapFrom(_ => 0));

        CreateMap<Event, EventResponseDto>()
            .ForCtorParam(nameof(EventResponseDto.Id), o => o.MapFrom(s => hashids.Encode(s.Id)))
            .ForCtorParam(nameof(EventResponseDto.VenueId), o => o.MapFrom(s => hashids.Encode(s.VenueId)));

        CreateMap<Section, SectionResponseDto>()
            .ForCtorParam(nameof(SectionResponseDto.Id), o => o.MapFrom(s => hashids.Encode(s.Id)))
            .ForCtorParam(nameof(SectionResponseDto.VenueId), o => o.MapFrom(s => hashids.Encode(s.VenueId)));

        CreateMap<EventSection, EventSectionResponseDto>()
            .ForCtorParam(nameof(EventSectionResponseDto.Id), o => o.MapFrom(s => hashids.Encode(s.Id)))
            .ForCtorParam(nameof(EventSectionResponseDto.EventId), o => o.MapFrom(s => hashids.Encode(s.EventId)))
            .ForCtorParam(nameof(EventSectionResponseDto.SectionId), o => o.MapFrom(s => hashids.Encode(s.SectionId)));

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
