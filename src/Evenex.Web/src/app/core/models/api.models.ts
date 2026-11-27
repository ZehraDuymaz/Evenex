// Backend'deki DTO'ların TypeScript karşılıkları.
// İsimler bilerek backend ile aynı tutuldu (VenueResponseDto -> VenueResponse) ki iki taraf kolayca eşleştirilsin.
// .NET JSON'u camelCase gönderdiği için alan adları küçük harfle başlıyor (Name -> name).
// Tüm id'ler Hashids ile hash'lenmiş string'dir ("wk1JaEWX"), asla sayı değil.

// ---------- Auth ----------
export interface LoginRequest {
  email: string;
  password: string;
}

export type RegisterRequest = LoginRequest;

export interface AuthResponse {
  token: string;
  email: string;
  role: string;
}

// ---------- Venue (Mekan) ----------
export interface VenueResponse {
  id: string;
  name: string;
  address: string;
  capacity: number;
}

export interface CreateVenueRequest {
  name: string;
  address: string;
}

export type UpdateVenueRequest = CreateVenueRequest;

// ---------- Event (Etkinlik) ----------
// Backend'deki EventStatus enum'ının karşılığı. Backend JSON'da isim gönderiyor ("Published").
// Union type: status sadece bu 3 metinden biri olabilir; "Publishd" yazarsan TypeScript hata verir.
export type EventStatus = 'Draft' | 'Published' | 'Cancelled';

export interface EventResponse {
  id: string;
  venueId: string;
  title: string;
  description: string;
  eventDate: string; // ISO formatında tarih, örn. "2030-01-01T20:00:00Z"
  status: EventStatus;
}

export interface CreateEventRequest {
  venueId: string;
  title: string;
  description: string;
  eventDate: string; // UTC ISO string gönderiyoruz (sonunda "Z" olan)
}

export type UpdateEventRequest = Omit<CreateEventRequest, 'venueId'>;

// ---------- Section (Mekânın bölümü: "Tribün A", "Sahne Önü") ----------
// Bölüm mekâna aittir ve kalıcıdır; fiyat/kontenjan burada YOK, onlar etkinliğe göre değişir (EventSection).
export type SectionType = 'Seated' | 'Standing';

export interface SectionResponse {
  id: string;
  venueId: string;
  name: string;
  type: SectionType;
}

export interface CreateSectionRequest {
  venueId: string;
  name: string;
  type: SectionType;
}

// ---------- EventSection (Bir etkinlikte satışa açılan bölüm) ----------
// "Bu konserde Tribün A: 500 TL, 200 kişi" gibi. Aynı bölüm başka bir etkinlikte başka fiyatla satılabilir.
export interface EventSectionResponse {
  id: string;
  eventId: string;
  sectionId: string;
  basePrice: number;
  capacity: number;
  remainingCapacity: number; // satıldıkça azalır (bilet satışı gelince)
}

export interface CreateEventSectionRequest {
  eventId: string;
  sectionId: string;
  basePrice: number;
  capacity: number;
}

// Etkinlik ve bölüm sonradan değiştirilemez; sadece fiyat ve kontenjan güncellenir
export type UpdateEventSectionRequest = Pick<CreateEventSectionRequest, 'basePrice' | 'capacity'>;
