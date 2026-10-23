import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { DashboardLayout } from './dashboard-layout';

describe('DashboardLayout', () => {
  let component: DashboardLayout;
  let fixture: ComponentFixture<DashboardLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardLayout],
      // Component router (routerLink) ve HttpClient (servisler) kullanıyor; testte ikisini de sağlıyoruz.
      // provideHttpClientTesting gerçek istek atmaz, sahte bir backend gibi davranır.
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
