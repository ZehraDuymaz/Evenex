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

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
