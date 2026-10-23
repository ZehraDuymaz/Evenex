using Evenex.Domain.Repositories;
using Evenex.Infrastructure; 

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// IUnitOfWork implementasyonu. Tüm repository'ler aynı (scoped) AppDbContext'i paylaştığı için
/// tek bir SaveChangesAsync çağrısı hepsinin değişikliklerini birlikte kaydeder.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync() 
    {
        return await _context.SaveChangesAsync();
    }
    
    // Not: DbContext'in ömrünü DI container yönetiyor (istek bitince kendisi dispose ediyor)
    public void Dispose()
    {
        _context.Dispose();
    }
}