namespace Evenex.Domain.Enums;

/// <summary>
/// Seated (0): numaralı koltuklu bölüm, Seat kayıtları vardır.
/// Standing (1): ayakta bölüm, koltuk yok; sadece EventSection.RemainingCapacity sayılır.
/// </summary>
public enum SectionType
{
    Seated,
    Standing
}
