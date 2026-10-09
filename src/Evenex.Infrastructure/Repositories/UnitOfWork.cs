using Evenex.Domain.Repositories;
using Evenex.Infrastructure; 

namespace Evenex.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Notice we removed the CancellationToken here to match your interface
    public async Task<int> SaveChangesAsync() 
    {
        return await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}