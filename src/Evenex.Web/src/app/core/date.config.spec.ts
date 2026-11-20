import { toUtcIso } from './date.config';

describe('toUtcIso', () => {
  it('formdaki saati Türkiye saati kabul edip UTC\'ye çevirir (3 saat geri)', () => {
    expect(toUtcIso('2026-12-12', '20:00')).toBe('2026-12-12T17:00:00.000Z');
  });

  it('gece yarısını geçen dönüşümde günü de doğru değiştirir', () => {
    // Türkiye'de 13 Aralık 01:30 = UTC'de hâlâ 12 Aralık 22:30
    expect(toUtcIso('2026-12-13', '01:30')).toBe('2026-12-12T22:30:00.000Z');
  });
});
