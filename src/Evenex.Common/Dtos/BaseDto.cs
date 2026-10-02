namespace Evenex.Common.Dtos;

/// <summary> 
/// Tüm Dto'ların miras aldığı ortak sınıf. 
/// Sadece her response'ta ortak olan alanları taşır.
/// </summary>

public abstract class BaseDto
{
    public int Id { get; set; }
    // hımmm? 
    public string? EId { get; set; } 
    public string CreatedUser { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public  string CreatedIP { get; set; } = default!;
    public string? ModifiedUser { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedIP { get; set; }
    public bool IsDeleted { get; set; }

}