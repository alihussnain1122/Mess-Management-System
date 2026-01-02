using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suggestions",
                columns: table => new
                {
                    SuggestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminResponse = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RespondedByUserId = table.Column<int>(type: "int", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggestions", x => x.SuggestionId);
                    table.ForeignKey(
                        name: "FK_Suggestions_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suggestions_Users_RespondedByUserId",
                        column: x => x.RespondedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 926, DateTimeKind.Utc).AddTicks(8124));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2674));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2684));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2687));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2689));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2835));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2838));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2843));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2845));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2856));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2858));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2863));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2866));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 2, 11, 50, 17, 927, DateTimeKind.Utc).AddTicks(2868));

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_CreatedAt",
                table: "Suggestions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_MemberId",
                table: "Suggestions",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_RespondedByUserId",
                table: "Suggestions",
                column: "RespondedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_Status",
                table: "Suggestions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suggestions");

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(3695));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7275));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7288));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7291));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7293));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7308));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7310));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7313));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7315));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7318));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7321));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7323));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7325));

            migrationBuilder.UpdateData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 18, 18, 37, 0, 974, DateTimeKind.Utc).AddTicks(7327));
        }
    }
}
