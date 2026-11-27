import { HttpErrorResponse } from '@angular/common/http';

/**
 * Backend'den dönen herhangi bir hatayı kullanıcıya gösterilebilecek tek bir cümleye çevirir.
 *
 * Backend 3 farklı şekilde hata döndürebiliyor, hepsini burada ele alıyoruz:
 *  1. Düz metin           -> AuthController: "Email veya şifre hatalı."
 *  2. ProblemDetails      -> GlobalExceptionHandler: { title, detail, status }
 *  3. Validation hatası   -> [ApiController]: { errors: { "$.eventDate": ["..."] } }
 */
export function getErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Beklenmeyen bir hata oluştu.';
  }

  // status 0 = istek sunucuya hiç ulaşamadı (API kapalı, yanlış adres ya da CORS engeli)
  if (error.status === 0) {
    return 'Sunucuya ulaşılamadı. API çalışıyor mu? (dotnet run --project src/Evenex.Api)';
  }

  const body = error.error;

  // 1. Düz metin
  if (typeof body === 'string' && body.trim()) {
    return body;
  }

  // 3. Validation hatası: errors objesinden bir mesaj al.
  // Gövde okunamazsa ASP.NET asıl hatanın ("$.venueId": "Geçersiz id: ...") yanına genel bir
  // "The dto field is required." da ekliyor. Kullanıcıya asıl sebebi göstermek için önce belirli bir
  // JSON alanına ait ("$." ile başlayan) hatayı seçiyoruz; yoksa ilk hatayı.
  if (body?.errors && typeof body.errors === 'object') {
    const entries = Object.entries(body.errors);
    const [, messages] = entries.find(([key]) => key.startsWith('$.')) ?? entries[0] ?? [];
    if (Array.isArray(messages) && messages.length > 0) {
      return String(messages[0]);
    }
  }

  // 2. ProblemDetails: detail daha açıklayıcı, yoksa title
  if (body?.detail) return body.detail;
  if (body?.title) return body.title;

  if (error.status === 401) return 'Bu işlem için giriş yapmalısınız.';

  return `İstek başarısız oldu (HTTP ${error.status}).`;
}
