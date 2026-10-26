using Evenex.Domain.Common;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// IRepository&lt;T&gt;'nin EF Core ile yazılmış ortak implementasyonu. Eskiden her repository
/// (Venue, Event, Section...) aynı Add/Update/SoftDelete kodunu kopyalıyordu; şimdi bir kere burada.
///
/// abstract: tek başına "new Repository&lt;Venue&gt;()" yapılamaz, sadece miras alınır
///   (çıplak bir şasi gibi; gerçek repository'ler bunun üstüne kurulur).
/// public: VenueRepository public olmak zorunda (Program.cs başka projede, Evenex.Api'de kaydediyor)
///   ve public bir sınıf internal bir sınıftan miras alamaz (CS0060 hatası).
/// </summary>
public abstract class Repository<T> : IRepository<T> where T : BaseEntity
{
    // protected: sadece bu sınıf ve onu miras alan repository'ler görebilir.
    // Alt sınıflar özel sorgularını (örn. GetSectionsByVenueIdAsync) bunlarla yazar.
    protected readonly AppDbContext _db;

    // T'nin tablosu. _db.Set<Venue>() ile _db.Venues tamamen aynı şeydir;
    // Set<T>() sayesinde hangi tablo olduğunu bilmeden ortak kod yazabiliyoruz.
    protected readonly DbSet<T> _set;

    protected Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    // Not: AppDbContext'te şimdilik sadece Venue için global query filter var.
    // Diğer entity'ler için silinmişleri burada elle eliyoruz (Venue'da iki kere filtrelemek zararsız).
    public virtual Task<T?> GetByIdAsync(int id)
        => _set.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _set.Where(e => !e.IsDeleted).ToListAsync();

    public virtual async Task AddAsync(T entity)
        => await _set.AddAsync(entity);

    // Update sadece EF'ye "bu nesne değişti" der; asıl SQL UPDATE, UnitOfWork.SaveChangesAsync'te çalışır.
    // İçeride beklenecek bir iş olmadığı için async yerine hazır tamamlanmış Task dönüyoruz.
    public virtual Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task SoftDeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.ModifiedDate = DateTime.UtcNow;
        _set.Update(entity);
        return Task.CompletedTask;
    }
}
