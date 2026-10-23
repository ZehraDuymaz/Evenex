import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { AuthService } from '../../../core/services/auth.service';
import { getErrorMessage } from '../../../core/utils/api-error';

/**
 * İki şifre alanının aynı olup olmadığını kontrol eden form grubu validator'ı.
 * Tek bir alana değil gruba bağlı, çünkü iki alanı birlikte görmesi gerekiyor.
 * Bu kontrol sadece frontend'de; backend'e zaten tek bir şifre gidiyor.
 */
function passwordsMatch(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password === confirm ? null : { passwordsMismatch: true };
}

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, UiButton, UiInput],
  templateUrl: './signup.html',
  styleUrl: './signup.scss'
})
export class Signup {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group(
    {
      // Backend'de User'ın henüz ad soyad alanı yok; tasarım kaybolmasın diye alan duruyor ama kapalı
      fullName: [{ value: '', disabled: true }],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    },
    { validators: passwordsMatch }
  );

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal('');

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    // Backend RegisterRequest sadece email + password bekliyor; fazlasını göndermiyoruz
    const { email, password } = this.form.getRawValue();

    this.auth.register({ email, password }).subscribe({
      // Kayıt başarılıysa backend token da döndü, yani kullanıcı artık giriş yapmış sayılır
      next: () => this.router.navigateByUrl('/admin'),
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isSubmitting.set(false);
      }
    });
  }

  fieldError(field: 'email' | 'password' | 'confirmPassword'): string {
    const control = this.form.controls[field];
    if (!control.touched) return '';
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (control.hasError('email')) return 'Geçerli bir e-posta adresi girin.';
    if (control.hasError('minlength')) return 'Şifre en az 6 karakter olmalı.';
    // Eşleşme hatası tek bir alanda değil grubun kendisinde duruyor
    if (field === 'confirmPassword' && this.form.hasError('passwordsMismatch')) return 'Şifreler eşleşmiyor.';
    return '';
  }
}
