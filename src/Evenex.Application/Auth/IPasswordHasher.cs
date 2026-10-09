namespace Evenex.Application.Auth;

/// <summary>
/// Kullanıcı şifrelerinin güvenli bir şekilde karma (hash) işleminden geçirilmesi ve giriş sırasında doğrulanması için gereken bilgiler.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}