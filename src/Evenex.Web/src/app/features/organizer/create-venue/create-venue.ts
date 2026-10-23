import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { VenueService } from '../../../core/services/venue.service';
import { getErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-create-venue',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, UiInput, UiButton],
  templateUrl: './create-venue.html',
  styleUrl: './create-venue.scss'
})
export class CreateVenue {
  private readonly fb = inject(FormBuilder);
  private readonly venueService = inject(VenueService);
  private readonly router = inject(Router);

  readonly venueForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    address: ['', Validators.required],
    // Backend'de henüz karşılığı yok (Venue sadece Name + Address tutuyor).
    // Kapasite ileride Section'lardan hesaplanacak. Tasarım kaybolmasın diye alanlar kapalı duruyor;
    // disabled alanlar form.valid hesabına katılmaz.
    city: [{ value: '', disabled: true }],
    capacity: [{ value: '', disabled: true }]
  });

  readonly isSubmitting = signal(false);
  readonly errorMessage = signal('');

  onSubmit(): void {
    if (this.venueForm.invalid) {
      this.venueForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    // getRawValue disabled alanları da içerir; o yüzden sadece backend'in beklediği name + address'i alıyoruz
    const { name, address } = this.venueForm.getRawValue();

    this.venueService.create({ name, address }).subscribe({
      next: () => this.router.navigate(['/admin/venues']),
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isSubmitting.set(false);
      }
    });
  }

  fieldError(field: 'name' | 'address'): string {
    const control = this.venueForm.controls[field];
    return control.touched && control.hasError('required') ? 'Bu alan zorunludur.' : '';
  }
}
