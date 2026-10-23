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
export interface EventResponse {
  id: string;
  venueId: string;
  title: string;
  description: string;
  eventDate: string; // ISO formatında tarih, örn. "2030-01-01T20:00:00Z"
  status: string;    // şimdilik hep "Draft"
}

export interface CreateEventRequest {
  venueId: string;
  title: string;
  description: string;
  eventDate: string; // UTC ISO string gönderiyoruz (sonunda "Z" olan)
}

export type UpdateEventRequest = Omit<CreateEventRequest, 'venueId'>;
