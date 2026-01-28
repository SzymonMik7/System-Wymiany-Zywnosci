using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace System_wymiany_żywności.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixReservationRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "88d02670-8e82-4ddb-801c-7292522b801a", "AQAAAAIAAYagAAAAEEFPDW7WfS2s9ntZ7rhwF5Q530u39eirPj7OzIHCjoMUwWr4/gdmZqrN9Sf4+aYkEg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "59e48b81-0ab3-41f9-a763-cade12325153", "AQAAAAIAAYagAAAAEGQvoqh/RerkayIXwM47+U9eECdtO8M5KMwAdj2cZcj7Qd0KDT12UAZ4LBsEY2f1yA==" });
        }
    }
}
