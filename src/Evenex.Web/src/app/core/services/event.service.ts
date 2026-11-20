import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { CreateEventRequest, EventResponse, UpdateEventRequest } from '../models/api.models';

/**
 * Backend'deki EventController ile konuşan servis (api/event).
 */
@Injectable({ providedIn: 'root' })
export class EventService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/event`;

  getAll(): Observable<EventResponse[]> {
    return this.http.get<EventResponse[]>(this.baseUrl);
  }

  getById(id: string): Observable<EventResponse> {
    return this.http.get<EventResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateEventRequest): Observable<EventResponse> {
    return this.http.post<EventResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateEventRequest): Observable<EventResponse> {
    return this.http.put<EventResponse>(`${this.baseUrl}/${id}`, request);
  }

  // Durum değiştirme: body yok (null), sadece hangi etkinlik olduğu (id) adreste.
  // Kural ihlalinde (örn. iptal edilmiş etkinliği yayınlamak) backend 409 döner.
  publish(id: string): Observable<EventResponse> {
    return this.http.post<EventResponse>(`${this.baseUrl}/${id}/publish`, null);
  }

  cancel(id: string): Observable<EventResponse> {
    return this.http.post<EventResponse>(`${this.baseUrl}/${id}/cancel`, null);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
