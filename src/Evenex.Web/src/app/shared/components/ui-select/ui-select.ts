import { Component, Input, forwardRef, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

/** Açılır listedeki tek bir seçenek: value forma yazılan değer, label ekranda görünen yazı */
export interface SelectOption {
  value: string;
  label: string;
}

let nextId = 0;

/**
 * app-ui-input ile aynı görünümde açılır liste (select).
 * O da ControlValueAccessor, yani formControlName ile kullanılır:
 *   <app-ui-select label="Mekan" formControlName="venueId" [options]="venueOptions()" />
 */
@Component({
  selector: 'app-ui-select',
  standalone: true,
  templateUrl: './ui-select.html',
  styleUrl: './ui-select.scss',
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => UiSelect), multi: true }
  ]
})
export class UiSelect implements ControlValueAccessor {
  @Input() label = '';
  @Input() options: SelectOption[] = [];
  /** Hiçbir şey seçilmemişken görünen ilk satır */
  @Input() placeholder = 'Seçiniz';
  @Input() error = '';
  @Input() hint = '';

  readonly selectId = `ui-select-${nextId++}`;

  protected readonly currentValue = signal('');
  protected readonly isDisabled = signal(false);

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

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

  protected handleChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.currentValue.set(value);
    this.onChange(value);
  }

  protected handleBlur(): void {
    this.onTouched();
  }
}
