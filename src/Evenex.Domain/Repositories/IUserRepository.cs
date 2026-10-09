using Evenex.Domain.Entities;

namespace Evenex.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetEmailAsync(string email);
    Task AddAsync(User user);    
     
}