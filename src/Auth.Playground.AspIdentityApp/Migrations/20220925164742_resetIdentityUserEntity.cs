using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Playground.AspIdentityApp.Migrations
{
    public partial class resetIdentityUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3CA25620-049F-4C7D-8B88-2CEF2478C99A",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAECnb+LuGSKcD5xgKL8+G4J20T8L8d42Q5cS0de31g6BKP70X9FVpeeH4qyVHTdQeow==", "c08c7a02-c67f-4036-b0ec-ad1256fa5fe5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3CA25620-049F-4C7D-8B88-2CEF2478C99A",
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAEAACcQAAAAEECuS3AdPSMESqQohVRjnCqjJvNV+cEBxvmOeHg5rOwVd1XyDMpYR9qmls1NIvjuwQ==", "1d51694e-0d75-4393-90a5-3e949ed917ca" });
        }
    }
}
