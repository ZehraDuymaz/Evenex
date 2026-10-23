using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Kullanıcı verisine erişim sözleşmesi (interface). Domain katmanı sadece "ne yapılacağını" bilir;
/// "nasıl" yapılacağı (EF Core, SQL) Infrastructure katmanındaki implementasyondadır.
/// </summary>
public interface IUserRepository
{
    // Email'e göre kullanıcı arar, yoksa null döner
    Task<User?> GetEmailAsync(string email);
    Task AddAsync(User user);    
     
}