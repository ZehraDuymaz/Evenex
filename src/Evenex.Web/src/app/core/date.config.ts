/**
 * Uygulamanın TEK tarih/saat kuralı. Her yer buradan okur, böylece tarih her ekranda aynı görünür.
 *
 * Kural: veritabanı ve API her zaman UTC ("...Z") konuşur; KULLANICIYA ise her zaman Türkiye saati gösterilir
 * ve formda girilen saat de Türkiye saati kabul edilir. Etkinlikler Türkiye'de yapıldığı için kullanıcı
 * yurt dışında olsa bile "20:00 konser" her yerde 20:00 görünmeli.
 * Türkiye 2016'dan beri yaz/kış saati uygulamıyor, o yüzden sabit +03:00 güvenli.
 */

// DatePipe'ın beklediği biçim ("+0300", iki nokta yok)
export const APP_TIMEZONE = '+0300';

// ISO tarih metninin sonuna eklenen biçim ("+03:00"), new Date(...) için
export const APP_TIMEZONE_ISO = '+03:00';

// Ekranda gösterim biçimi, örn. "12 Ara 2026, 20:00" (Türkçe ay adları LOCALE_ID 'tr' sayesinde)
export const APP_DATE_FORMAT = 'd MMM y, HH:mm';

/**
 * Formdan gelen tarih ("2026-12-12") ve saati ("20:00") Türkiye saati olarak yorumlayıp
 * API'nin beklediği UTC ISO metnine çevirir: "2026-12-12T17:00:00.000Z".
 * Saat dilimini açıkça yazdığımız için kullanıcının bilgisayarı hangi saat diliminde olursa olsun sonuç aynı.
 */
export function toUtcIso(date: string, time: string): string {
  return new Date(`${date}T${time}:00${APP_TIMEZONE_ISO}`).toISOString();
}
