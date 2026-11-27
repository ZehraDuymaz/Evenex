import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { SelectOption, UiSelect } from '../../../shared/components/ui-select/ui-select';
import { VenueService } from '../../../core/services/venue.service';
import { SectionService } from '../../../core/services/section.service';
import { SectionResponse, SectionType, VenueResponse } from '../../../core/models/api.models';
import { SECTION_TYPE_LABELS } from '../../../core/labels';
import { getErrorMessage } from '../../../core/utils/api-error';

/**
 * Bir mekânın bölümleri sayfası: /admin/venues/:id/sections
 *
 * Bölüm = mekânın kalıcı bir parçası ("Tribün A", "Sahne Önü"). Burada sadece ad ve tür var;
 * fiyat ve kontenjan etkinliğe göre değiştiği için etkinliğin bölümler sayfasında belirleniyor.
 */
@Component({
  selector: 'app-venue-sections',
  imports: [RouterModule, ReactiveFormsModule, UiButton, UiInput, UiSelect],
  templateUrl: './venue-sections.html',
  styleUrl: './venue-sections.scss'
})
export class VenueSections implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly venueService = inject(VenueService);
  private readonly sectionService = inject(SectionService);

  // Adresteki hash'li mekân id'si (/admin/venues/k1JppaeX/sections -> "k1JppaeX").
  // snapshot: sayfa açıldığı andaki adres; bu sayfada id değişmediği için yeterli.
  private readonly venueId = this.route.snapshot.paramMap.get('id') ?? '';

  readonly venue = signal<VenueResponse | null>(null);
  readonly sections = signal<SectionResponse[]>([]);
  readonly isLoading = signal(true);
  readonly isSubmitting = signal(false);
  readonly errorMessage = signal('');

  readonly sectionForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    // Tür seçimi; "as SectionType" ile TypeScript'e bu alanın sadece 'Seated' | 'Standing' olacağını söylüyoruz
    type: ['Seated' as SectionType, Validators.required]
  });

  // Açılır liste seçenekleri etiket sözlüğünden üretiliyor; yeni bir tür eklenirse burası kendiliğinden güncellenir
  readonly typeOptions: SelectOption[] = Object.entries(SECTION_TYPE_LABELS).map(([value, label]) => ({
    value,
    label
  }));

  ngOnInit(): void {
    // Mekân bilgisi (başlıkta adını göstermek için) ve bölümleri aynı anda çekiyoruz
    forkJoin({
      venue: this.venueService.getById(this.venueId),
      sections: this.sectionService.getByVenue(this.venueId)
    }).subscribe({
      next: ({ venue, sections }) => {
        this.venue.set(venue);
        this.sections.set(sections);
        this.isLoading.set(false);
      },
      error: (error) => {
        // Uydurma bir id ile gelinirse backend 404 döner, mesaj burada görünür
        this.errorMessage.set(getErrorMessage(error));
        this.isLoading.set(false);
      }
    });
  }

  typeLabel(type: SectionType): string {
    return SECTION_TYPE_LABELS[type];
  }

  addSection(): void {
    if (this.sectionForm.invalid) {
      this.sectionForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const { name, type } = this.sectionForm.getRawValue();
    this.sectionService.create({ venueId: this.venueId, name: name.trim(), type }).subscribe({
      next: (created) => {
        // Yeni bölümü listeye ekle, formu bir sonraki bölüm için temizle (tür seçimi kalsın)
        this.sections.update((list) => [...list, created]);
        this.sectionForm.reset({ name: '', type });
        this.isSubmitting.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isSubmitting.set(false);
      }
    });
  }

  nameError(): string {
    const control = this.sectionForm.controls.name;
    return control.touched && control.hasError('required') ? 'Bölüm adı zorunludur.' : '';
  }
}
