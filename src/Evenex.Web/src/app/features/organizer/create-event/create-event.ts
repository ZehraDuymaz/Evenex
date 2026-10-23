import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { SelectOption, UiSelect } from '../../../shared/components/ui-select/ui-select';
import { EventService } from '../../../core/services/event.service';
import { VenueService } from '../../../core/services/venue.service';
import { VenueResponse } from '../../../core/models/api.models';
import { getErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, UiInput, UiButton, UiSelect],
  templateUrl: './create-event.html',
  styleUrl: './create-event.scss'
})
export class CreateEvent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly eventService = inject(EventService);
  private readonly venueService = inject(VenueService);
  private readonly router = inject(Router);

  readonly eventForm = this.fb.nonNullable.group({
    venueId: ['', Validators.required],
    title: ['', Validators.required],
    description: ['', Validators.required],
    date: ['', Validators.required],
    time: ['', Validators.required],
    // Fiyat backend'de etkinliğe değil EventSection'a (bölüme) ait; bilet satış tarihi ise henüz yok.
    // Backend desteklediğinde açılacak.
    price: [{ value: '', disabled: true }],
    ticketSaleStartDate: [{ value: '', disabled: true }]
  });

  readonly venues = signal<VenueResponse[]>([]);
  readonly isLoadingVenues = signal(true);
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal('');

  // Mekan listesini açılır listenin anlayacağı { value, label } şekline çeviriyoruz.
  // computed olduğu için venues değişince kendiliğinden yeniden hesaplanır.
  readonly venueOptions = computed<SelectOption[]>(() =>
    this.venues().map((venue) => ({ value: venue.id, label: `${venue.name} (${venue.address})` }))
  );

  ngOnInit(): void {
    // Etkinlik bir mekâna bağlı olmak zorunda; seçim yapılabilsin diye önce mekânları çekiyoruz
    this.venueService.getAll().subscribe({
      next: (venues) => {
        this.venues.set(venues);
        this.isLoadingVenues.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isLoadingVenues.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    const { venueId, title, description, date, time } = this.eventForm.getRawValue();

    // Tarih ("2026-12-12") ve saat ("20:00") ayrı alanlardan geliyor; birleştirip tek bir Date yapıyoruz.
    // new Date("2026-12-12T20:00") kullanıcının KENDİ saat dilimine göre yorumlanır (Türkiye: UTC+3),
    // toISOString() ise bunu UTC'ye çevirir: "2026-12-12T17:00:00.000Z". Backend de UTC ile karşılaştırıyor.
    const eventDate = new Date(`${date}T${time}`);

    // Backend de kontrol ediyor ama boşuna istek atmamak için burada da bakıyoruz
    if (eventDate.getTime() < Date.now()) {
      this.errorMessage.set('Sadece gelecek tarihler için etkinlik oluşturulabilir.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    this.eventService
      .create({ venueId, title, description, eventDate: eventDate.toISOString() })
      .subscribe({
        next: () => this.router.navigate(['/admin/events']),
        error: (error) => {
          this.errorMessage.set(getErrorMessage(error));
          this.isSubmitting.set(false);
        }
      });
  }

  fieldError(field: 'venueId' | 'title' | 'description' | 'date' | 'time'): string {
    const control = this.eventForm.controls[field];
    return control.touched && control.hasError('required') ? 'Bu alan zorunludur.' : '';
  }
}
