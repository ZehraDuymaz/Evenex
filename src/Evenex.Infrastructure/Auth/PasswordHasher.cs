using Evenex.Application.Auth;

namespace Evenex.Infrastructure.Auth;

/// <summary>
/// BCrypt ile şifre hash'leme. BCrypt her hash'e rastgele bir "salt" ekler ve bilerek yavaş çalışır;
/// böylece aynı şifre iki kez hash'lenince bile farklı sonuç çıkar ve kaba kuvvet (brute force) saldırısı zorlaşır.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    
}