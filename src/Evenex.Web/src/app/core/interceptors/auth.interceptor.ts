import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { API_URL } from '../api.config';
import { AuthService } from '../services/auth.service';

/**
 * Interceptor = giden her HTTP isteğinin ve gelen her cevabın arasına giren bir ara katman.
 * Backend'deki middleware'lerin frontend versiyonu gibi düşünebilirsin.
 *
 * İki işi var:
 *  1. Giden isteğe "Authorization: Bearer <token>" header'ını eklemek (her serviste tek tek yazmamak için)
 *  2. Backend 401 dönerse (token süresi dolmuş / API yeniden başlatılmış) oturumu kapatıp login'e atmak
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const token = auth.token;

  // Token'ı sadece kendi API'mize gönderiyoruz, başka sitelere sızmasın
  const isOurApi = request.url.startsWith(API_URL);

  // HttpRequest değiştirilemez (immutable); clone ile header eklenmiş yeni bir kopya oluşturuyoruz
  const authorizedRequest =
    token && isOurApi ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : request;

  return next(authorizedRequest).pipe(
    catchError((error: unknown) => {
      // Login/register'daki 401 "şifre yanlış" demek, oturum düşmesi değil; onları hariç tutuyoruz
      const isAuthEndpoint = request.url.startsWith(`${API_URL}/auth/`);

      if (error instanceof HttpErrorResponse && error.status === 401 && token && !isAuthEndpoint) {
        auth.logout(true);
      }

      // Hatayı yutmuyoruz; isteği yapan component de kendi hata mesajını gösterebilsin
      return throwError(() => error);
    }),
  );
};
