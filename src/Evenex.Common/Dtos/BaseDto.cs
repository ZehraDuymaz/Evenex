namespace Evenex.Common.Dtos;

/// <summary> 
/// Tüm Dto'ların miras aldığı ortak sınıf. 
/// Sadece her response'ta ortak olan alanları taşır.
/// </summary>

public abstract record BaseResponseDto(string Id);
