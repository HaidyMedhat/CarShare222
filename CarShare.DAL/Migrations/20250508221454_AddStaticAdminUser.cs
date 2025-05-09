using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShare.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AccountStatus", "CreatedAt", "Email", "FirstName", "IsActive", "IsVerified", "LastLogin", "LastName", "PasswordHash", "PasswordSalt", "PhoneNumber", "ProfilePictureUrl", "Role", "Username" },
                values: new object[] { new Guid("443ef10f-92bc-467b-9cd6-21c378d1607a"), 1, new DateTime(2025, 5, 8, 22, 14, 53, 971, DateTimeKind.Utc).AddTicks(402), "admin@carshare.com", "Admin", true, true, null, "System", new byte[] { 56, 15, 219, 133, 55, 35, 213, 207, 30, 61, 127, 164, 173, 115, 65, 198, 189, 100, 38, 60, 26, 69, 212, 21, 10, 77, 205, 107, 119, 197, 206, 59, 43, 153, 88, 169, 88, 160, 214, 31, 100, 95, 35, 118, 139, 255, 145, 89, 223, 137, 6, 39, 162, 149, 100, 135, 13, 140, 228, 240, 3, 145, 11, 99 }, new byte[] { 212, 95, 23, 194, 120, 64, 62, 67, 250, 28, 138, 117, 179, 47, 229, 58, 240, 210, 126, 69, 157, 188, 196, 71, 209, 12, 70, 100, 58, 216, 50, 39, 55, 239, 253, 2, 6, 228, 173, 126, 75, 88, 87, 214, 109, 34, 159, 231, 125, 219, 155, 102, 62, 249, 28, 27, 78, 107, 121, 185, 14, 130, 2, 126, 1, 91, 10, 182, 246, 237, 86, 64, 82, 235, 137, 182, 153, 52, 183, 132, 82, 35, 94, 126, 112, 119, 239, 128, 27, 13, 197, 249, 210, 196, 80, 60, 134, 110, 64, 49, 82, 84, 197, 163, 45, 38, 221, 45, 142, 183, 36, 90, 136, 46, 67, 68, 53, 200, 40, 223, 192, 158, 121, 86, 92, 14, 81, 141 }, "010000000000", null, "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("443ef10f-92bc-467b-9cd6-21c378d1607a"));
        }
    }
}
