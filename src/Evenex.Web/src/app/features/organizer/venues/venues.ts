import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { VenueService } from '../../../core/services/venue.service';
import { VenueResponse } from '../../../core/models/api.models';
import { getErrorMessage } from '../../../core/utils/api-error';

@Component({
  selector: 'app-venues',
  imports: [RouterModule, UiButton],
  templateUrl: './venues.html',
  styleUrl: './venues.scss',
})
export class Venues implements OnInit {
  private readonly venueService = inject(VenueService);

  // Sayfanın 3 olası durumu: yükleniyor / hata / liste (boş da olabilir)
  readonly venues = signal<VenueResponse[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal('');
  /** Hangi mekân şu an siliniyor (butonu kilitlemek için) */
  readonly deletingId = signal<string | null>(null);

  // ngOnInit: component ekrana ilk geldiğinde bir kez çalışır, veri çekmek için doğru yer
  ngOnInit(): void {
    this.loadVenues();
  }

  loadVenues(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.venueService.getAll().subscribe({
      next: (venues) => {
        this.venues.set(venues);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isLoading.set(false);
      }
    });
  }

  deleteVenue(venue: VenueResponse): void {
    if (!confirm(`"${venue.name}" mekanını silmek istediğinize emin misiniz?`)) return;

    this.deletingId.set(venue.id);
    this.venueService.delete(venue.id).subscribe({
      next: () => {
        // Listeyi baştan çekmek yerine silineni yerel listeden çıkarıyoruz (daha hızlı)
        this.venues.update((list) => list.filter((v) => v.id !== venue.id));
        this.deletingId.set(null);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.deletingId.set(null);
      }
    });
  }
}
