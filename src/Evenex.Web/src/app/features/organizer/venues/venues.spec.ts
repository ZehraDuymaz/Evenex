import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { Venues } from './venues';
import { API_URL } from '../../../core/api.config';

describe('Venues', () => {
  let component: Venues;
  let fixture: ComponentFixture<Venues>;
  // Sahte backend: hangi isteklerin atıldığını görmemizi ve onlara elle cevap vermemizi sağlar
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Venues],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(Venues);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Beklenmeyen / cevapsız kalmış istek varsa test başarısız olsun
    httpMock.verify();
  });

  it('should create', () => {
    fixture.detectChanges();
    httpMock.expectOne(`${API_URL}/venue`).flush([]);
    expect(component).toBeTruthy();
  });

  it('API\'den gelen mekanları listeler', async () => {
    fixture.detectChanges(); // ngOnInit çalışır -> GET /api/venue isteği atılır

    // İsteği yakalayıp backend gibi cevap veriyoruz
    httpMock.expectOne(`${API_URL}/venue`).flush([
      { id: 'wk1JaEWX', name: 'Zorlu PSM', address: 'İstanbul', capacity: 0 },
    ]);
    await fixture.whenStable();

    const html = fixture.nativeElement as HTMLElement;
    expect(html.textContent).toContain('Zorlu PSM');
    expect(html.textContent).toContain('İstanbul');
  });

  it('API hata verirse hata mesajı gösterir', async () => {
    fixture.detectChanges();

    // status 0 = sunucuya hiç ulaşılamadı (API kapalı)
    httpMock.expectOne(`${API_URL}/venue`).error(new ProgressEvent('error'), { status: 0 });
    await fixture.whenStable();

    const html = fixture.nativeElement as HTMLElement;
    expect(html.textContent).toContain('Sunucuya ulaşılamadı');
  });
});
