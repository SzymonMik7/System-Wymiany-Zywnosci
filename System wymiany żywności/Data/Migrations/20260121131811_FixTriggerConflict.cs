using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace System_wymiany_żywności.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTriggerConflict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "58c44ca9-5c19-4826-8474-41f029a70409", "AQAAAAIAAYagAAAAEEuRxwtniiI8vRCiLoUyooti/kbXu6uqgpC/3+UnQVSFuq5Cw/3+EPGLIpSJ+WL5tw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "88d02670-8e82-4ddb-801c-7292522b801a", "AQAAAAIAAYagAAAAEEFPDW7WfS2s9ntZ7rhwF5Q530u39eirPj7OzIHCjoMUwWr4/gdmZqrN9Sf4+aYkEg==" });
        }
    }
}
