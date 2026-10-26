using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Kullanıcı verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// Ortak CRUD IRepository&lt;User&gt;'den gelir.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    // Email'e göre kullanıcı arar, yoksa null döner
    Task<User?> GetByEmailAsync(string email);
}
