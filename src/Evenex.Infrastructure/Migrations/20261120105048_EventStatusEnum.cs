using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evenex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Event.Status artık string değil EventStatus enum'ı; ama veritabanına yine İSİM olarak yazılıyor
            // ("Draft", "Published"...). O yüzden kolonun tipi aynı, sadece nvarchar(max) -> nvarchar(20) kısalıyor.
            // EF "veri kaybı olabilir" uyarısı veriyor çünkü kolon kısalıyor; uygulamadan önce kontrol edildi:
            // tablodaki tüm değerler "Draft" (5 karakter), kayıp yok.
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
