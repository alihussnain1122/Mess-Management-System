using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MessManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Members_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BreakfastPresent = table.Column<bool>(type: "bit", nullable: false),
                    LunchPresent = table.Column<bool>(type: "bit", nullable: false),
                    DinnerPresent = table.Column<bool>(type: "bit", nullable: false),
                    MarkedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendances_Users_MarkedBy",
                        column: x => x.MarkedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripePaymentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterTeaRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WaterCount = table.Column<int>(type: "int", nullable: false),
                    TeaCount = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterTeaRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterTeaRecords_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "WeeklyMenus",
                columns: new[] { "Id", "CreatedAt", "DayOfWeek", "DishName", "MealType", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(5829), "Monday", "Rice with Dal", "Lunch", 50m, null },
                    { 2, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7965), "Monday", "Roti with Sabzi", "Dinner", 45m, null },
                    { 3, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7970), "Tuesday", "Biryani", "Lunch", 80m, null },
                    { 4, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7972), "Tuesday", "Chapati with Curry", "Dinner", 50m, null },
                    { 5, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7974), "Wednesday", "Fried Rice", "Lunch", 60m, null },
                    { 6, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7981), "Wednesday", "Paratha with Curd", "Dinner", 55m, null },
                    { 7, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7983), "Thursday", "Paneer Curry with Rice", "Lunch", 75m, null },
                    { 8, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7984), "Thursday", "Khichdi", "Dinner", 40m, null },
                    { 9, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7986), "Friday", "Chicken Curry", "Lunch", 100m, null },
                    { 10, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7989), "Friday", "Egg Curry with Rice", "Dinner", 70m, null },
                    { 11, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7990), "Saturday", "Special Thali", "Lunch", 120m, null },
                    { 12, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7992), "Saturday", "Pulao", "Dinner", 65m, null },
                    { 13, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7994), "Sunday", "Fish Curry", "Lunch", 110m, null },
                    { 14, new DateTime(2025, 12, 14, 12, 1, 55, 607, DateTimeKind.Utc).AddTicks(7995), "Sunday", "Mixed Veg Rice", "Dinner", 55m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MarkedBy",
                table: "Attendances",
                column: "MarkedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MemberId_Date",
                table: "Attendances",
                columns: new[] { "MemberId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_RoomNumber",
                table: "Members",
                column: "RoomNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                table: "Members",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Date",
                table: "Payments",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MemberId",
                table: "Payments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaterTeaRecords_MemberId_Date",
                table: "WaterTeaRecords",
                columns: new[] { "MemberId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyMenus_DayOfWeek_MealType",
                table: "WeeklyMenus",
                columns: new[] { "DayOfWeek", "MealType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "WaterTeaRecords");

            migrationBuilder.DropTable(
                name: "WeeklyMenus");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
