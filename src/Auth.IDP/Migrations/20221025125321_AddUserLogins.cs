using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.IDP.Migrations
{
    public partial class AddUserLogins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("0ab89291-7093-40e7-a8c4-03435a819400"), "b55753fe-9679-4e7a-bde9-088885ff3bd4", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Flagg" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("2021ac90-7983-43be-b0ec-0b70ff8e544b"), "5db9f376-cebe-416b-b3ed-66031d03ea99", "country", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "be" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("26b51e50-5b5d-4eb8-982a-cb36c0df3be2"), "8bdf2811-524a-4ef4-9441-4f0486f19e6e", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Emma" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("a1526154-7775-4377-964a-f78fa6f3e0fe"), "fc0b779d-0403-4c76-9dc3-c986f06ffac3", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "David" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("baeba30a-8fb8-476d-a9d9-71b3588359e9"), "a00204b8-85de-4e7f-852f-6b9946a41b2c", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Flagg" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("be4fb02f-9aa9-42c2-ac8e-3e3e80b9d205"), "228fcd12-a273-4719-97f5-80db879870f2", "role", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "PayingUser" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("c71e6516-aa0a-4459-b868-5541b9fc5bd3"), "67fae643-fdc9-494e-9d2d-060ebd4939b7", "country", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "nl" });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[] { new Guid("d72e5151-e61e-4565-833d-9b5aa3d76958"), "00cffbc0-006f-4834-944e-b387768bf784", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "FreeUser" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                columns: new[] { "ConcurrencyStamp", "Password" },
                values: new object[] { "514a8a5e-6f36-4f3e-9135-8c5214e35ca9", "AQAAAAEAACcQAAAAEKFzpqbJZztQiEoUlWok01l543K6ur7CFyD3hzbrg4BL1fsBuWzHW4WkoygQz6y3gQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                columns: new[] { "ConcurrencyStamp", "Password" },
                values: new object[] { "a216f92e-f709-4a38-9380-8ac15e37c34a", "AQAAAAEAACcQAAAAEKFzpqbJZztQiEoUlWok01l543K6ur7CFyD3hzbrg4BL1fsBuWzHW4WkoygQz6y3gQ==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("0ab89291-7093-40e7-a8c4-03435a819400"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("2021ac90-7983-43be-b0ec-0b70ff8e544b"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("26b51e50-5b5d-4eb8-982a-cb36c0df3be2"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("a1526154-7775-4377-964a-f78fa6f3e0fe"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("baeba30a-8fb8-476d-a9d9-71b3588359e9"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("be4fb02f-9aa9-42c2-ac8e-3e3e80b9d205"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("c71e6516-aa0a-4459-b868-5541b9fc5bd3"));

            migrationBuilder.DeleteData(
                table: "UserClaims",
                keyColumn: "Id",
                keyValue: new Guid("d72e5151-e61e-4565-833d-9b5aa3d76958"));

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
                columns: new[] { "ConcurrencyStamp", "Password" },
                values: new object[] { "3d1730ee-dfb7-45af-b2b3-cbe5910775d9", "password" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                columns: new[] { "ConcurrencyStamp", "Password" },
                values: new object[] { "1c9e08b5-f66c-47ed-9b82-ccd543309785", "password" });
        }
    }
}
