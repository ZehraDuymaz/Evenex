import { EventStatus, SectionType } from './models/api.models';

// Backend'den gelen kodların (enum isimlerinin) ekranda görünecek Türkçe karşılıkları.
// Birden fazla sayfa kullandığı için tek yerde duruyor.
// Record<X, string>: X'in HER değeri yazılmak zorunda; backend'e yeni bir değer eklenip
// burada unutulursa TypeScript hata verir.

export const EVENT_STATUS_LABELS: Record<EventStatus, string> = {
  Draft: 'Taslak',
  Published: 'Yayında',
  Cancelled: 'İptal edildi'
};

export const SECTION_TYPE_LABELS: Record<SectionType, string> = {
  Seated: 'Koltuklu',
  Standing: 'Ayakta'
};
