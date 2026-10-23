using Evenex.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Evenex.Api.Middleware;

/// <summary>
/// Uygulamanın herhangi bir yerinde yakalanmayan exception'lar en son buraya düşer.
/// Exception türüne göre doğru HTTP status kodunu seçip standart bir JSON (ProblemDetails) döner.
/// Bu sayede servisler sadece "throw new KeyNotFoundException(...)" der, controller'larda
/// her metoda try/catch yazmak gerekmez.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // switch expression: exception'ın türüne bakıp (status, başlık) ikilisini seçer.
        // Sıra önemli: ilk eşleşen kazanır, o yüzden en genel durum (_) en sonda.
        var (status, title) = exception switch
        {
            KeyNotFoundException        => (StatusCodes.Status404NotFound, "Kayıt bulunamadı"),
            ArgumentException           => (StatusCodes.Status400BadRequest, "Geçersiz istek"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Yetkisiz işlem"),
            DomainException             => (StatusCodes.Status409Conflict, "İş kuralı ihlali"),
            _                           => (StatusCodes.Status500InternalServerError, "Sunucu hatası")
        };

        // 500 bizim hatamızdır, loglanmalı. Diğerleri kullanıcının hatalı isteği, log kirletmesin.
        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Beklenmeyen hata: {Message}", exception.Message);

        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                // 500'de iç detayları (SQL hatası, stack trace vb.) dışarı sızdırmıyoruz
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Beklenmeyen bir hata oluştu."
                    : exception.Message
            }
        });
    }
}
