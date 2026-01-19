using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace System_wymiany_żywności.Data.Migrations
{
    public partial class AddSqlFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TWORZENIE FUNKCJI SQL: Przelicza punkty na PLN taka dodatkowa opcja do rozwinięca w przyszłości
            migrationBuilder.Sql(@"
                CREATE FUNCTION fn_CalculateWalletValue (@Points INT)
                RETURNS DECIMAL(10,2)
                AS
                BEGIN
                    -- Logika: 1 punkt wymiany jest wart 0.50 PLN
                    RETURN CAST(@Points * 0.50 AS DECIMAL(10,2))
                END
            ");

            // Aktualizacja danych administratora (odświeżenie haseł i znaczników bezpieczeństwa)
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "59e48b81-0ab3-41f9-a763-cade12325153", "AQAAAAIAAYagAAAAEGQvoqh/RerkayIXwM47+U9eECdtO8M5KMwAdj2cZcj7Qd0KDT12UAZ4LBsEY2f1yA==" });
        }

        // Metoda Down: Usuwa funkcję podczas wycofywania zmian
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Usunięcie funkcji z bazy danych
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS fn_CalculateWalletValue");

            // Przywrócenie poprzednich wartości haseł administratora
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cb1a3e11-0ffa-42fa-ac2f-05fa85b2d4cb", "AQAAAAIAAYagAAAAEHjBJXlHPYv+OKhZV46Y7o8qm2hN9ZQypxrAyH/ujxzYiIKzKlRFjNnV1i9MexE4Vg==" });
        }
    }
}