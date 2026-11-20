using Evenex.Domain.Common;

namespace Evenex.Domain.Repositories;

/// <summary>
/// Tüm repository'lerin ortak CRUD sözleşmesi. Her entity için aynı olan işlemler
/// (id ile getir, listele, ekle, güncelle, sil) bir kere burada tanımlanır;
/// IVenueRepository gibi özel interface'ler bunu miras alıp sadece kendine özgü sorguları ekler.
///
/// T = "hangi entity olduğu sonra belirlenecek" boşluğu (Venue, Event, Section...).
/// where T : BaseEntity = sadece entity'ler kabul edilir. Bu kısıt sayesinde
/// her T'nin Id ve IsDeleted alanı olduğunu derleyici bilir ve kullanmamıza izin verir.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    // Silinmiş (IsDeleted = true) kayıtlar "yok" sayılır, null döner
    Task<T?> GetByIdAsync(int id);

    // Silinmemiş tüm kayıtlar
    Task<IEnumerable<T>> GetAllAsync();

    // Add/Update/SoftDelete veritabanına hemen yazmaz, sadece değişikliği hafızaya alır.
    // Asıl kayıt IUnitOfWork.SaveChangesAsync() çağrılınca tek transaction ile yapılır.
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);

    // Fiziksel silme yok: IsDeleted = true yapılır. Entity'yi parametre alıyoruz çünkü
    // servis zaten "var mı?" kontrolü için onu getirmiş oluyor; tekrar sorgu atmaya gerek yok.
    Task SoftDeleteAsync(T entity);
}
