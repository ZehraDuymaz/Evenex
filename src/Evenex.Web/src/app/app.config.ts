import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { authInterceptor } from './core/interceptors/auth.interceptor';

// Tarih/sayı formatları Türkçe olsun diye ("12 Ara 2026", "1.250,00")
registerLocaleData(localeTr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), provideClientHydration(),
    // HttpClient'ı uygulamaya tanıtıyoruz. withFetch: SSR ile uyumlu olan fetch API'sini kullan.
    // withInterceptors: her istek authInterceptor'dan geçsin (token ekleme + 401 yakalama).
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
    { provide: LOCALE_ID, useValue: 'tr' }
  ]
};
