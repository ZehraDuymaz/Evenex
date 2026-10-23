import { HttpClient } from '@angular/common/http';
import { Injectable, PLATFORM_ID, computed, inject, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { API_URL } from '../api.config';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/api.models';

/**
 * Giriş / kayıt / çıkış işlemleri ve oturumdaki kullanıcının bilgisi.
 *
 * Token localStorage'da saklanıyor, böylece sayfa yenilense de kullanıcı giriş yapmış kalır.
 * providedIn: 'root' -> uygulama boyunca tek bir AuthService nesnesi var (backend'deki Singleton gibi).
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Uygulama SSR ile sunucuda da çalışabiliyor. Sunucuda localStorage YOKTUR,
  // o yüzden ona dokunmadan önce tarayıcıda olup olmadığımızı kontrol ediyoruz.
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));
  private readonly storageKey = 'evenex.auth';

  // signal: değeri değişince onu kullanan tüm ekranlar otomatik güncellenir
  private readonly currentUser = signal<AuthResponse | null>(this.loadFromStorage());

  /** Giriş yapmış kullanıcı (yoksa null). Dışarıdan sadece okunabilir. */
  readonly user = this.currentUser.asReadonly();
  /** computed: user değiştikçe kendiliğinden yeniden hesaplanan türetilmiş signal */
  readonly isLoggedIn = computed(() => this.currentUser() !== null);

  /** Interceptor her isteğe eklemek için buradan okur */
  get token(): string | null {
    return this.currentUser()?.token ?? null;
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    // tap: cevabı değiştirmeden araya girip "yan etki" yapar (burada: token'ı kaydetmek)
    return this.http
      .post<AuthResponse>(`${API_URL}/auth/login`, request)
      .pipe(tap((response) => this.saveSession(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    // Backend kayıt sonrası direkt token döndüğü için kullanıcı ayrıca login olmak zorunda kalmıyor
    return this.http
      .post<AuthResponse>(`${API_URL}/auth/register`, request)
      .pipe(tap((response) => this.saveSession(response)));
  }

  /**
   * Oturumu kapatıp login sayfasına yönlendirir.
   * sessionExpired = true ise login sayfası "oturumunuzun süresi doldu" mesajı gösterir.
   */
  logout(sessionExpired = false): void {
    this.currentUser.set(null);
    this.storage?.removeItem(this.storageKey);

    this.router.navigate(['/auth/login'], {
      queryParams: sessionExpired ? { expired: 1 } : {},
    });
  }

  private saveSession(response: AuthResponse): void {
    this.currentUser.set(response);
    this.storage?.setItem(this.storageKey, JSON.stringify(response));
  }

  /**
   * localStorage'a güvenli erişim. Sunucuda (SSR) hiç yok; tarayıcıda da gizli sekme veya
   * engellenmiş site verisi yüzünden erişim hata fırlatabiliyor. Bu durumlarda null döner ve
   * uygulama çalışmaya devam eder (sadece sayfa yenilenince oturum hatırlanmaz).
   */
  private get storage(): Storage | null {
    if (!this.isBrowser) return null;
    try {
      return globalThis.localStorage ?? null;
    } catch {
      return null;
    }
  }

  private loadFromStorage(): AuthResponse | null {
    const storage = this.storage;
    if (!storage) return null;

    try {
      const raw = storage.getItem(this.storageKey);
      if (!raw) return null;

      const saved = JSON.parse(raw) as AuthResponse;

      // Süresi dolmuş token'ı hiç yüklemiyoruz; yoksa ilk istekte 401 alıp atılırdık
      if (isTokenExpired(saved.token)) {
        storage.removeItem(this.storageKey);
        return null;
      }
      return saved;
    } catch {
      // Bozuk veri varsa (elle değiştirilmiş vs.) temizleyip sıfırdan başla
      storage.removeItem(this.storageKey);
      return null;
    }
  }
}

/**
 * JWT'nin orta kısmı (payload) base64 ile kodlanmış bir JSON'dur ve içinde "exp" (bitiş zamanı,
 * saniye cinsinden) vardır. Şifreli değil, sadece imzalı; o yüzden tarayıcıda okunabilir.
 * Not: Bu sadece kullanıcı deneyimi için. Asıl doğrulamayı her istekte backend yapıyor.
 */
function isTokenExpired(token: string): boolean {
  try {
    // base64url -> base64: '-' ve '_' karakterlerini standart karşılıklarına çevir
    const payload = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const { exp } = JSON.parse(atob(payload)) as { exp?: number };
    return !exp || exp * 1000 < Date.now();
  } catch {
    return true;
  }
}
