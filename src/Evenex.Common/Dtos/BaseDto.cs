namespace Evenex.Common.Dtos;

/// <summary> 
/// Tüm Dto'ların miras aldığı ortak sınıf. 
/// Sadece her response'ta ortak olan alanları taşır.
/// Id burada her zaman Hashids ile hash'lenmiş string'dir, gerçek int id değil.
/// Audit alanları (CreatedUser vb.) bilerek yok: her response'ta gereksiz veri ve bilgi sızıntısı olurdu.
/// </summary>

public abstract record BaseDto(string Id);

