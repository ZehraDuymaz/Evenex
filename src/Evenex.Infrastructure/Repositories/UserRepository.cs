using Evenex.Domain.Entities;
using Evenex.Domain.Repositories;
using Evenex.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Evenex.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> GetEmailAsync(string email) => _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);
}