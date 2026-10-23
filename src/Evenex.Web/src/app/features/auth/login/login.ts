import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { AuthService } from '../../../core/services/auth.service';
import { getErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, UiButton, UiInput],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  // nonNullable: form sıfırlanınca değerler null değil '' olur, tipler daha temiz kalır
  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  // Bu uygulama zoneless çalışıyor: ekranın güncellenmesi için değişen durumu signal'de tutuyoruz
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal('');

  // Interceptor 401 yüzünden buraya attıysa ?expired=1 ile gelir
  readonly sessionExpired = this.route.snapshot.queryParamMap.has('expired');

  onSubmit(): void {
    if (this.form.invalid) {
      // Kullanıcı hiçbir alana dokunmadan "Giriş Yap"a bastıysa tüm hataları göster
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => {
        // Guard bizi buraya yönlendirdiyse kullanıcı asıl gitmek istediği sayfaya dönsün
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/admin';
        this.router.navigateByUrl(returnUrl);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isSubmitting.set(false);
      }
    });
  }

  /** Alana dokunulduysa ve geçersizse ekranda gösterilecek hata mesajı */
  fieldError(field: 'email' | 'password'): string {
    const control = this.form.controls[field];
    if (!control.touched || control.valid) return '';
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (control.hasError('email')) return 'Geçerli bir e-posta adresi girin.';
    return '';
  }
}
