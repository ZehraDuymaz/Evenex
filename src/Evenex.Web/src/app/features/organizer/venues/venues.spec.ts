import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Venues } from './venues';

describe('Venues', () => {
  let component: Venues;
  let fixture: ComponentFixture<Venues>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Venues],
    }).compileComponents();

    fixture = TestBed.createComponent(Venues);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
