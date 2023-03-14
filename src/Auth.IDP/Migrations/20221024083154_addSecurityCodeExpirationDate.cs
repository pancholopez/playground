using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.IDP.Migrations
{
    public partial class addSecurityCodeExpirationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("09ea3415-bc2e-4953-af37-d1b91b2c0078"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("134747ef-c5e7-496a-b5f6-a96a432e06ad"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("32feabc1-5d1b-4865-a4b5-f76d98fe7dba"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("6523a726-913f-481a-898e-2741ad613e42"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("bcd70036-512f-4ff6-bb3d-f04bb79dfd60"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("c2e98207-5e76-4e17-be5f-d95496fe9c43"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("c71ee8f9-097b-4e3e-888a-563242508b97"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("f7445bf2-a883-4c48-b0d3-c1dbbc863229"));

            migrationBuilder.AddColumn<DateTime>(
                name: "SecureCodeExpirationDate",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("0c85c839-907a-4326-a437-1a46c4c2a681"), "1e7d3b5c-47f0-4a16-aa07-bd3791248d27", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "David" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("0d1d8564-6391-4cec-b95f-790257fc7a16"), "f29a81ff-efa6-4524-ac66-2cacf955d5dd", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "FreeUser" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("0ddbcf1c-0cfc-4193-9ed0-f01d86f29c34"), "0e9fb127-de44-4043-84dd-35d2a5613238", "role", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "PayingUser" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("64ff3322-e1c4-4aed-ba9b-a928d37a40ee"), "6b161bb1-3684-410b-b706-e67b4b144251", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Flagg" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("99140109-6efa-4ac2-b7a3-0dafd8f5db2d"), "288524a5-cbab-45a4-b3ac-541ab0d195e6", "country", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "be" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("c7587622-1dff-42fd-8d57-94ead01b858c"), "49cecffd-c9c0-4693-9b95-427596e4687f", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Flagg" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("cac4559c-f37b-46c6-9905-43f8ea552672"), "d9c6132e-cc24-4bf3-9136-8e5337f9bdce", "country", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "nl" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("ff7eee30-3740-444a-b51a-848cf14b9efd"), "fa554ba0-7130-4d37-aee0-da94d5f3730b", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Emma" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                column: "ConcurrencyStamp",
                value: "3d1730ee-dfb7-45af-b2b3-cbe5910775d9");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                column: "ConcurrencyStamp",
                value: "1c9e08b5-f66c-47ed-9b82-ccd543309785");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("0c85c839-907a-4326-a437-1a46c4c2a681"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("0d1d8564-6391-4cec-b95f-790257fc7a16"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("0ddbcf1c-0cfc-4193-9ed0-f01d86f29c34"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("64ff3322-e1c4-4aed-ba9b-a928d37a40ee"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("99140109-6efa-4ac2-b7a3-0dafd8f5db2d"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("c7587622-1dff-42fd-8d57-94ead01b858c"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("cac4559c-f37b-46c6-9905-43f8ea552672"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("ff7eee30-3740-444a-b51a-848cf14b9efd"));

            migrationBuilder.DropColumn(
                name: "SecureCodeExpirationDate",
                table: "Users");

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("09ea3415-bc2e-4953-af37-d1b91b2c0078"), "73cfba11-52d1-42e4-bcc5-dd0213649515", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "David" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("134747ef-c5e7-496a-b5f6-a96a432e06ad"), "2f55c0dc-863c-4719-9026-6e2c5b0a6e97", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "FreeUser" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("32feabc1-5d1b-4865-a4b5-f76d98fe7dba"), "8e4addeb-35cc-4c52-9951-c7718dc7fda7", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Flagg" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("6523a726-913f-481a-898e-2741ad613e42"), "c4730041-cc5c-4236-8b06-5f6f693c2ab9", "country", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "be" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("bcd70036-512f-4ff6-bb3d-f04bb79dfd60"), "88a4d394-55a5-4825-a5e8-a2852636cff2", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Flagg" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("c2e98207-5e76-4e17-be5f-d95496fe9c43"), "41816a49-e2ae-420e-8650-259a6b666778", "role", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "PayingUser" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("c71ee8f9-097b-4e3e-888a-563242508b97"), "36a2d12b-1f76-4313-96e4-60a7c50bb0e4", "country", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "nl" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("f7445bf2-a883-4c48-b0d3-c1dbbc863229"), "3e24bed8-817e-47e3-a6c1-8123d66d0e1c", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Emma" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                column: "ConcurrencyStamp",
                value: "44025686-0992-4afa-b5ab-c217be09e8a3");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                column: "ConcurrencyStamp",
                value: "bfa20f43-a8e2-4a4e-bcf6-a3d0f4626642");
        }
    }
}
