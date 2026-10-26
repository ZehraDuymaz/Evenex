using Evenex.Domain.Entities;
using Evenex.Domain.Enums;
using Evenex.Domain.Repositories;

namespace Evenex.Application.Auth;

/// <summary>
/// Kayıt (register) ve giriş (login) işlemleri. İkisi de başarılı olursa kullanıcıya bir JWT döner;
/// frontend sonraki isteklerde bu token'ı "Authorization: Bearer ..." header'ı ile gönderir.
/// </summary>
public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork
    )
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        
        // email kayıtlıysa hata ver
        if (existing is not null)
        {
            throw new InvalidOperationException("Bu email zaten kayıtlı. Giriş yapmayı deneyiniz.");
        } 
        
        // email kayıtlı değilse yeni user oluştur
        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.User,
            CreatedIP = "127.0.0.1",       // TODO: gerçek IP controller'dan alınıp buraya verilmeli (şimdilik test için sabit)
            CreatedUser = request.Email,   // Kullanıcı kendi kendini kaydediyor, yani oluşturan da kendisi
            CreatedDate = DateTime.UtcNow, // Kayıt olduğu an (her zaman UTC tutuyoruz)
            IsDeleted = false 

        };

        // user ekle 
        await _userRepo.AddAsync(user);
        
        // efcore ile veritabanına kaydet
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Role.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Güvenlik: "email yanlış" ve "şifre yanlış" için AYNI mesajı veriyoruz.
        // Farklı mesaj verirsek saldırgan hangi email'lerin sistemde kayıtlı olduğunu öğrenebilir.
        const string invalidCredentials = "Email veya şifre hatalı.";

        var user = await _userRepo.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException(invalidCredentials);
        
        // Şifreyi asla düz metin karşılaştırmıyoruz; BCrypt girilen şifreyi hash'leyip kayıtlı hash ile kıyaslar
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException(invalidCredentials);
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Role.ToString());
    }  
}