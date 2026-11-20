using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

/// <summary>
/// Kullanıcı sorguları. Ortak CRUD Repository&lt;User&gt;'den gelir.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    // Silinmiş kullanıcı giriş yapamasın / aynı email ile tekrar kayıt engellenmesin diye
    // IsDeleted olanları da eliyoruz.
    public Task<User?> GetByEmailAsync(string email)
        => _set.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
}
