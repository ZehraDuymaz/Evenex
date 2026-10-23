import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { EventService } from '../../../core/services/event.service';
import { VenueService } from '../../../core/services/venue.service';
import { EventResponse, VenueResponse } from '../../../core/models/api.models';
import { getErrorMessage } from '../../../core/utils/api-error';

// Backend'deki durum kodlarının ekranda görünecek Türkçe karşılıkları
const STATUS_LABELS: Record<string, string> = {
  Draft: 'Taslak'
};

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

  statusLabel(status: string): string {
    return STATUS_LABELS[status] ?? status;
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
