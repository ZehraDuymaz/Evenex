namespace Evenex.Domain.Repositories;

/// <summary>
/// Unit of Work: repository'ler değişiklikleri sadece hafızada biriktirir (Add, Update...),
/// SaveChangesAsync çağrılınca hepsi TEK bir transaction ile veritabanına yazılır.
/// Ya hepsi kaydedilir ya hiçbiri; yarım kalan işlem olmaz.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
