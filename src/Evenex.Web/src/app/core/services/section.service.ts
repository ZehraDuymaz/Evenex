import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { CreateSectionRequest, SectionResponse } from '../models/api.models';

/**
 * Backend'deki SectionController ile konuşan servis (api/section).
 * Backend şimdilik sadece "mekânın bölümlerini listele" ve "bölüm ekle" sunuyor;
 * bölüm düzenleme/silme henüz yok.
 */
@Injectable({ providedIn: 'root' })
export class SectionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_URL}/section`;

  // GET api/section/venue/{venueId}: venueId hash'li string, backend [HashId] ile çözüyor
  getByVenue(venueId: string): Observable<SectionResponse[]> {
    return this.http.get<SectionResponse[]>(`${this.baseUrl}/venue/${venueId}`);
  }

  create(request: CreateSectionRequest): Observable<SectionResponse> {
    return this.http.post<SectionResponse>(this.baseUrl, request);
  }
}
