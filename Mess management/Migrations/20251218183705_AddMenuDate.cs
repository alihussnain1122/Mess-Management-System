using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MenuDate",
                table: "WeeklyMenus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(3695), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7275), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7288), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7291), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7293), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7308), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7310), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7313), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7315), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7318), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7321), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7323), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7325), null });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CreatedAt", "MenuDate" },
                values: new object[] { new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7327), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuDate",
                table: "WeeklyMenus");

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
        }
    }
}
