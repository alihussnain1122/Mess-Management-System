using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(6592));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9342));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9347));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9349));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9351));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9362));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9364));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9365));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9367));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9370));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9371));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9373));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9374));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 22, 27, 57, 358, DateTimeKind.Utc).AddTicks(9376));

            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_UserId",
                table: "PasswordHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordHistories");

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(5829));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7965));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7970));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7972));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7974));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7981));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7983));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7984));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7986));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7989));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7990));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7992));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7994));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7995));
        }
    }
}
