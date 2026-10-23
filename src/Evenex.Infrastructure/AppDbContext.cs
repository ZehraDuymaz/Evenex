using Microsoft.EntityFrameworkCore;
using Evenex.Domain.Entities;

namespace Evenex.Infrastructure;

/// <summary>
/// EF Core'un veritabanı ile konuştuğu ana sınıf. Her DbSet bir tabloya karşılık gelir.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
    {

    }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventSection> EventSections { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Soft delete kullandığımız için fiziksel cascade delete'e hiç ihtiyacımız yok.
        // Tüm foreign key'leri Restrict yapıyoruz — bir kayıt, ona bağlı başka
        // kayıtlar varken asla otomatik silinmeyecek; silme işlemi her zaman
        // IsDeleted = true ile Application katmanında kontrollü şekilde yapılacak.
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Global query filter: Venues tablosuna yapılan her sorguya otomatik olarak
        // "WHERE IsDeleted = 0" eklenir. Böylece silinmiş mekânları her seferinde elle filtrelemek gerekmez.
        modelBuilder.Entity<Venue>().HasQueryFilter(v => !v.IsDeleted);

        // Para alanları: toplam 18 basamak, virgülden sonra 2 basamak (örn. 1234567890123456.78).
        // Belirtilmezse EF uyarı veriyor ve büyük değerler sessizce kırpılabiliyor.
        modelBuilder.Entity<EventSection>().Property(es => es.BasePrice).HasPrecision(18, 2);
        modelBuilder.Entity<Discount>().Property(d => d.Value).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}
