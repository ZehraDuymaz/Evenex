import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, switchMap } from 'rxjs';
import { UiButton } from '../../../shared/components/ui-button/ui-button';
import { UiInput } from '../../../shared/components/ui-input/ui-input';
import { SelectOption, UiSelect } from '../../../shared/components/ui-select/ui-select';
import { EventService } from '../../../core/services/event.service';
import { SectionService } from '../../../core/services/section.service';
import { EventSectionService } from '../../../core/services/event-section.service';
import { EventResponse, EventSectionResponse, SectionResponse } from '../../../core/models/api.models';
import { EVENT_STATUS_LABELS, SECTION_TYPE_LABELS } from '../../../core/labels';
import { getErrorMessage } from '../../../core/utils/api-error';

// Fiyat 0 veya daha büyük, kontenjan en az 1 tam sayı olmalı (backend'deki EventSection.Validate ile aynı kural).
// Backend de kontrol ediyor; burada kontrol etmemizin sebebi kullanıcıya anında, istek atmadan hata göstermek.
const PRICE_VALIDATORS = [Validators.required, Validators.min(0)];
const CAPACITY_VALIDATORS = [Validators.required, Validators.min(1), Validators.pattern(/^\d+$/)];

/**
 * Bir etkinliğin satış bölümleri sayfası: /admin/events/:id/sections
 *
 * Mekânın bölümlerinden birini seçip bu etkinlik için fiyat ve kontenjan veriyoruz
 * ("Bu konserde Tribün A: 500 TL, 200 kişi"). Etkinlik ancak en az bir bölümü varsa yayınlanabilir.
 * Kural: bölümler sadece etkinlik Taslak iken değiştirilebilir (backend'de Event.EnsureSectionsEditable).
 */
@Component({
  selector: 'app-event-sections',
  imports: [RouterModule, ReactiveFormsModule, CurrencyPipe, DatePipe, UiButton, UiInput, UiSelect],
  templateUrl: './event-sections.html',
  styleUrl: './event-sections.scss'
})
export class EventSections implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly eventService = inject(EventService);
  private readonly sectionService = inject(SectionService);
  private readonly eventSectionService = inject(EventSectionService);

  private readonly eventId = this.route.snapshot.paramMap.get('id') ?? '';

  readonly event = signal<EventResponse | null>(null);
  /** Mekânın TÜM bölümleri (seçim listesi ve bölüm adlarını göstermek için) */
  readonly venueSections = signal<SectionResponse[]>([]);
  /** Bu etkinlikte satışa açılmış bölümler */
  readonly eventSections = signal<EventSectionResponse[]>([]);

  readonly isLoading = signal(true);
  readonly isSubmitting = signal(false);
  readonly isPublishing = signal(false);
  readonly errorMessage = signal('');
  /** Şu an düzenlenen satış bölümü (aynı anda sadece bir kart düzenlenebilir) */
  readonly editingId = signal<string | null>(null);
  /** İsteği süren kart (Kaydet/Kaldır butonlarını kilitlemek için) */
  readonly busyId = signal<string | null>(null);

  // Değerler string: ui-input metin döndürüyor, göndermeden önce Number() ile sayıya çeviriyoruz
  readonly addForm = this.fb.nonNullable.group({
    sectionId: ['', Validators.required],
    basePrice: ['', PRICE_VALIDATORS],
    capacity: ['', CAPACITY_VALIDATORS]
  });

  readonly editForm = this.fb.nonNullable.group({
    basePrice: ['', PRICE_VALIDATORS],
    capacity: ['', CAPACITY_VALIDATORS]
  });

  // Sadece Taslak etkinliğin bölümleri değiştirilebilir. Butonları/formu buna göre gösteriyoruz,
  // böylece kullanıcı backend'in 409 hatasıyla karşılaşmadan neyin mümkün olduğunu görüyor.
  readonly isEditable = computed(() => this.event()?.status === 'Draft');

  // Bölüm id -> bölüm sözlüğü: satış bölümü sadece sectionId taşıyor, adını buradan buluyoruz
  private readonly sectionsById = computed(
    () => new Map(this.venueSections().map((section) => [section.id, section]))
  );

  // Seçim listesinde sadece HENÜZ EKLENMEMİŞ bölümler görünsün
  // (aynı bölüm bir etkinliğe iki kez eklenemez; backend'de unique index + 409 var)
  readonly availableOptions = computed<SelectOption[]>(() => {
    const used = new Set(this.eventSections().map((es) => es.sectionId));
    return this.venueSections()
      .filter((section) => !used.has(section.id))
      .map((section) => ({ value: section.id, label: `${section.name} (${SECTION_TYPE_LABELS[section.type]})` }));
  });

  ngOnInit(): void {
    // Önce etkinliği çekmemiz gerekiyor çünkü mekânın bölümlerini istemek için event.venueId lazım.
    // switchMap: ilk istek bitince, sonucunu kullanarak sonraki istekleri başlatır (zincirleme istek).
    this.eventService
      .getById(this.eventId)
      .pipe(
        switchMap((event) => {
          this.event.set(event);
          return forkJoin({
            venueSections: this.sectionService.getByVenue(event.venueId),
            eventSections: this.eventSectionService.getByEvent(event.id)
          });
        })
      )
      .subscribe({
        next: ({ venueSections, eventSections }) => {
          this.venueSections.set(venueSections);
          this.eventSections.set(eventSections);
          this.isLoading.set(false);
        },
        error: (error) => {
          this.errorMessage.set(getErrorMessage(error));
          this.isLoading.set(false);
        }
      });
  }

  sectionName(sectionId: string): string {
    const section = this.sectionsById().get(sectionId);
    return section ? `${section.name} (${SECTION_TYPE_LABELS[section.type]})` : 'Bilinmeyen bölüm';
  }

  statusLabel(): string {
    const event = this.event();
    return event ? EVENT_STATUS_LABELS[event.status] : '';
  }

  // ---------- Ekle ----------
  addEventSection(): void {
    if (this.addForm.invalid) {
      this.addForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set('');

    const { sectionId, basePrice, capacity } = this.addForm.getRawValue();
    this.eventSectionService
      .create({ eventId: this.eventId, sectionId, basePrice: Number(basePrice), capacity: Number(capacity) })
      .subscribe({
        next: (created) => {
          this.eventSections.update((list) => [...list, created]);
          this.addForm.reset();
          this.isSubmitting.set(false);
        },
        error: (error) => {
          this.errorMessage.set(getErrorMessage(error));
          this.isSubmitting.set(false);
        }
      });
  }

  // ---------- Düzenle ----------
  startEdit(eventSection: EventSectionResponse): void {
    // Formu kartın mevcut değerleriyle doldur
    this.editForm.reset({ basePrice: String(eventSection.basePrice), capacity: String(eventSection.capacity) });
    this.editingId.set(eventSection.id);
  }

  cancelEdit(): void {
    this.editingId.set(null);
  }

  saveEdit(eventSection: EventSectionResponse): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.busyId.set(eventSection.id);
    this.errorMessage.set('');

    const { basePrice, capacity } = this.editForm.getRawValue();
    this.eventSectionService
      .update(eventSection.id, { basePrice: Number(basePrice), capacity: Number(capacity) })
      .subscribe({
        next: (updated) => {
          // Sadece değişen kartı yeni haliyle değiştir
          this.eventSections.update((list) => list.map((es) => (es.id === updated.id ? updated : es)));
          this.editingId.set(null);
          this.busyId.set(null);
        },
        error: (error) => {
          // Örn. kontenjanı satılan bilet sayısının altına düşürmek -> backend hatası burada görünür
          this.errorMessage.set(getErrorMessage(error));
          this.busyId.set(null);
        }
      });
  }

  // ---------- Kaldır ----------
  removeEventSection(eventSection: EventSectionResponse): void {
    if (!confirm(`"${this.sectionName(eventSection.sectionId)}" bölümünü bu etkinlikten kaldırmak istediğinize emin misiniz?`)) return;

    this.busyId.set(eventSection.id);
    this.errorMessage.set('');
    this.eventSectionService.delete(eventSection.id).subscribe({
      next: () => {
        this.eventSections.update((list) => list.filter((es) => es.id !== eventSection.id));
        this.busyId.set(null);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.busyId.set(null);
      }
    });
  }

  // ---------- Yayınla ----------
  // Bölümleri ayarlayan kişi doğal olarak bir sonraki adımda yayınlamak ister; listeye dönmesine gerek kalmasın.
  // Bölüm yoksa backend 409 "en az bir bölüm gerekli" döner ve mesaj ekranda görünür.
  publish(): void {
    this.isPublishing.set(true);
    this.errorMessage.set('');
    this.eventService.publish(this.eventId).subscribe({
      next: (updated) => {
        this.event.set(updated);
        this.editingId.set(null);
        this.isPublishing.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isPublishing.set(false);
      }
    });
  }

  // ---------- Hata mesajları ----------
  fieldError(form: 'add' | 'edit', field: 'sectionId' | 'basePrice' | 'capacity'): string {
    // İki formun tipi farklı olduğu için ortak AbstractControl tipiyle bakıyoruz (edit formunda sectionId yok -> null)
    const group: AbstractControl = form === 'add' ? this.addForm : this.editForm;
    const control = group.get(field);
    if (!control || !control.touched || control.valid) return '';
    if (control.hasError('required')) return 'Bu alan zorunludur.';
    if (field === 'basePrice') return 'Fiyat 0 veya daha büyük olmalı.';
    return 'Kontenjan en az 1 olan bir tam sayı olmalı.';
  }
}
