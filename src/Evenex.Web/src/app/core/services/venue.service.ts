import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { CreateVenueRequest, UpdateVenueRequest, VenueResponse } from '../models/api.models';

/**
 * Backend'deki VenueController ile konuşan servis (api/venue).
 * Token eklemeyi burada düşünmüyoruz; authInterceptor her isteğe otomatik ekliyor.
 *
 * Metotlar Observable döner: istek, biri .subscribe() çağırana kadar GÖNDERİLMEZ.
 */
@Injectable({ providedIn: 'root' })
export class VenueService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/venue`;

  getAll(): Observable<VenueResponse[]> {
    return this.http.get<VenueResponse[]>(this.baseUrl);
  }

  getById(id: string): Observable<VenueResponse> {
    return this.http.get<VenueResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateVenueRequest): Observable<VenueResponse> {
    return this.http.post<VenueResponse>(this.baseUrl, request);
  }

  update(id: string, request: UpdateVenueRequest): Observable<VenueResponse> {
    return this.http.put<VenueResponse>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
