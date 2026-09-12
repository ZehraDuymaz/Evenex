namespace Evenex.Application.Venues;

/// <summary>
/// Yeni mekan oluşturma ve güncelleme işlemleri 
/// Mekan bölümü ile Mekanı bağlamak için kullanılan VenueResponseDto mekanın id'sini çağırmak için kullanılır 
/// VenueResponseDto aynı zamanda Venue'yi güncellemek için kullanılır. 
/// </summary>

public record CreateVenueDto (string Name, string Address);
public record UpdateVenueDto (string Name, string Address);
public record VenueResponseDto(string Id, string Name, string Address);

