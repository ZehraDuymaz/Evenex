import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import {
  CreateEventSectionRequest,
  EventSectionResponse,
  UpdateEventSectionRequest
} from '../models/api.models';

/**
 * Backend'deki EventSectionController ile konuşan servis (api/eventsection).
 * Ekleme/güncelleme/silme sadece etkinlik "Taslak" iken çalışır; değilse backend 409 döner.
 */
@Injectable({ providedIn: 'root' })
export class EventSectionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/eventsection`;

  getByEvent(eventId: string): Observable<EventSectionResponse[]> {
    return this.http.get<EventSectionResponse[]>(`${this.baseUrl}/event/${eventId}`);
  }

  create(request: CreateEventSectionRequest): Observable<EventSectionResponse> {
    return this.http.post<EventSectionResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateEventSectionRequest): Observable<EventSectionResponse> {
    return this.http.put<EventSectionResponse>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
