import { Component, Input, forwardRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

let nextId = 0;

/**
 * Uygulama genelinde kullanılan input bileşeni.
 *
 * ControlValueAccessor'ı implemente ettiği için Reactive Forms ile doğrudan kullanılabilir:
 *   <app-ui-input label="E-posta" formControlName="email" />
 * Angular form değerini writeValue ile içeri yazar; kullanıcı yazdıkça biz de onChange ile dışarı bildiririz.
 * Formsuz kullanım da hâlâ çalışır: <app-ui-input label="..." [value]="..." [readonly]="true" />
 */
@Component({
  selector: 'app-ui-input',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ui-input.html',
  styleUrl: './ui-input.scss',
  providers: [
    // Angular'a "bu bileşen bir form kontrolü gibi davranabilir" diyoruz
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => UiInput), multi: true }
  ]
})
export class UiInput implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() readonly = false;
  @Input() mono = false;
  @Input() hasIcon = false;
  @Input() placeholder = '';
  @Input() autocomplete = '';
  /** Alanın altında gösterilen kırmızı hata mesajı (boşsa gösterilmez) */
  @Input() error = '';
  /** Alanın altında gösterilen gri açıklama (örn. "Yakında") */
  @Input() hint = '';

  @Input() set value(value: string | number | null) {
    this.currentValue.set(value == null ? '' : String(value));
  }

  @Input() set disabled(isDisabled: boolean) {
    this.isDisabled.set(isDisabled);
  }

  // label'a tıklayınca input'a odaklanması için her bileşene benzersiz bir id
  readonly inputId = `ui-input-${nextId++}`;

  protected readonly currentValue = signal('');
  protected readonly isDisabled = signal(false);

  // Angular bu fonksiyonları registerOnChange/registerOnTouched ile verene kadar boş birer fonksiyon
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  // ---------- ControlValueAccessor: form -> bileşen ----------
  writeValue(value: string | null): void {
    this.currentValue.set(value ?? '');
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }

  // ---------- bileşen -> form ----------
  protected handleInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.currentValue.set(value);
    this.onChange(value);
  }

  protected handleBlur(): void {
    // "touched" = kullanıcı alana girip çıktı. Hata mesajlarını ancak bundan sonra gösteriyoruz,
    // yoksa sayfa açılır açılmaz her yer kırmızı olurdu.
    this.onTouched();
  }
}
