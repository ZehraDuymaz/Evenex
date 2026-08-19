using Evenex.Domain.Entities;
using Evenex.Domain.Enums;
using Evenex.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Evenex.Application.Auth;

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
        var existing = await _userRepo.GetEmailAsync(request.Email);
        
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
            CreatedIP = "127.0.0.1", // get dynamic user ip (this is only for testing)
            CreatedUser = request.Email,   // They are registering themselves, so they are the creator
            CreatedDate = DateTime.UtcNow, // The exact moment they registered
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
        var user = await _userRepo.GetEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Email hatalı");
        
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Şifre Hatalı");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Role.ToString());
    }  
}