import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable, forkJoin } from 'rxjs';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { EventService } from '../../../core/services/event.service';
import { VenueService } from '../../../core/services/venue.service';
import { EventResponse, EventStatus, VenueResponse } from '../../../core/models/api.models';
import { getErrorMessage } from '../../../core/utils/api-error';
import { EVENT_STATUS_LABELS } from '../../../core/labels';

@Component({
  selector: 'app-events',
  imports: [RouterModule, UiButton, DatePipe],
  templateUrl: './events.html',
  styleUrl: './events.scss',
})
export class Events implements OnInit {
  private readonly eventService = inject(EventService);
  private readonly venueService = inject(VenueService);

  readonly events = signal<EventResponse[]>([]);
  private readonly venues = signal<VenueResponse[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal('');
  readonly deletingId = signal<string | null>(null);
  // Yayınla / iptal et isteği süren etkinlik (butonu kilitlemek için)
  readonly changingStatusId = signal<string | null>(null);

  // Etkinlik sadece venueId taşıyor ("wk1JaEWX"). Ekranda mekân adını göstermek için
  // id -> ad sözlüğü (Map) oluşturuyoruz; her etkinlik için listeyi tek tek aramaktan hızlı.
  private readonly venueNames = computed(
    () => new Map(this.venues().map((venue) => [venue.id, venue.name]))
  );

  ngOnInit(): void {
    // forkJoin: iki isteği AYNI ANDA gönderir, ikisi de bitince tek seferde sonucu verir
    // (JavaScript'teki Promise.all gibi)
    forkJoin({ events: this.eventService.getAll(), venues: this.venueService.getAll() }).subscribe({
      next: ({ events, venues }) => {
        // En yakın tarihli etkinlik en üstte
        this.events.set([...events].sort((a, b) => a.eventDate.localeCompare(b.eventDate)));
        this.venues.set(venues);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isLoading.set(false);
      }
    });
  }

  venueName(venueId: string): string {
    return this.venueNames().get(venueId) ?? 'Bilinmeyen mekan';
  }

  statusLabel(status: EventStatus): string {
    return EVENT_STATUS_LABELS[status];
  }

  publishEvent(event: EventResponse): void {
    this.changeStatus(event, this.eventService.publish(event.id));
  }

  cancelEvent(event: EventResponse): void {
    if (!confirm(`"${event.title}" etkinliğini iptal etmek istediğinize emin misiniz? Bu işlem geri alınamaz.`)) return;
    this.changeStatus(event, this.eventService.cancel(event.id));
  }

  // Yayınla ve iptal et aynı adımları izliyor: isteği gönder, gelen güncel etkinliği listede değiştir.
  private changeStatus(event: EventResponse, request: Observable<EventResponse>): void {
    this.changingStatusId.set(event.id);
    this.errorMessage.set('');
    request.subscribe({
      next: (updated) => {
        // Sadece değişen kartı yeni haliyle değiştiriyoruz; tüm listeyi yeniden yüklemeye gerek yok
        this.events.update((list) => list.map((e) => (e.id === updated.id ? updated : e)));
        this.changingStatusId.set(null);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.changingStatusId.set(null);
      }
    });
  }

  deleteEvent(event: EventResponse): void {
    if (!confirm(`"${event.title}" etkinliğini silmek istediğinize emin misiniz?`)) return;

    this.deletingId.set(event.id);
    this.eventService.delete(event.id).subscribe({
      next: () => {
        this.events.update((list) => list.filter((e) => e.id !== event.id));
        this.deletingId.set(null);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.deletingId.set(null);
      }
    });
  }
}
