using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Playground.AspIdentityApp.Migrations
{
    public partial class seedadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "DF60C8FF-48F3-45F4-B883-DF458EE206AE", "DF60C8FF-48F3-45F4-B883-DF458EE206AE", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Department", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Position", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3CA25620-049F-4C7D-8B88-2CEF2478C99A", 0, "3CA25620-049F-4C7D-8B88-2CEF2478C99A", null, "admin@mail.com", true, false, null, "ADMIN@MAIL.COM", "ADMIN@MAIL.COM", "AQAAAAEAACcQAAAAEECuS3AdPSMESqQohVRjnCqjJvNV+cEBxvmOeHg5rOwVd1XyDMpYR9qmls1NIvjuwQ==", null, false, null, "1d51694e-0d75-4393-90a5-3e949ed917ca", false, "admin@mail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "DF60C8FF-48F3-45F4-B883-DF458EE206AE", "3CA25620-049F-4C7D-8B88-2CEF2478C99A" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "DF60C8FF-48F3-45F4-B883-DF458EE206AE", "3CA25620-049F-4C7D-8B88-2CEF2478C99A" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "DF60C8FF-48F3-45F4-B883-DF458EE206AE");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3CA25620-049F-4C7D-8B88-2CEF2478C99A");
        }
    }
}
