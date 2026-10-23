import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Route guard = bir sayfaya girilmeden önce çalışan kontrol. Backend'deki [Authorize]'ın frontend karşılığı.
 * Giriş yapılmamışsa login sayfasına yönlendirir ve gidilmek istenen adresi returnUrl olarak taşır,
 * böylece login sonrası kullanıcı kaldığı yere döner.
 *
 * Önemli: Bu sadece kullanıcı deneyimi içindir, güvenlik değil. Tarayıcıdaki kod değiştirilebilir;
 * asıl koruma backend'deki [Authorize]'dır.
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) return true;

  // false yerine UrlTree döndürmek = "girme, şuraya git"
  return router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
};
