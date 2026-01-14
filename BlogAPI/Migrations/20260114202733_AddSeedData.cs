using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "Timestamp" },
                values: new object[] { "I built this blog using .NET 8, PostgreSQL, and GraphQL. Stay tuned for more updates!", new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$mDY2MZj4p98WNErw1ee56uNmQga6E4.m6lJc7WrCOxODF8boph9La");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 2, "john@example.com", "$2a$11$VCampvE7Kbfzf3BJDU/XQOZQDe9/EOvb9E/skfaa3LdSlHVB4Ys76", "john_doe" },
                    { 3, "jane@example.com", "$2a$11$wpsNOk93W3ObjhATX/QEmuxF8HQUekE0pmHLtYb9i0yL/rCX4Y0Oe", "jane_smith" }
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "Content", "Perex", "Timestamp", "Title", "UserId" },
                values: new object[,]
                {
                    { 2, "GraphQL is a powerful query language for APIs. In this article, we'll explore how to set up a GraphQL server using HotChocolate in .NET 8.", "Learn the basics of GraphQL and how to integrate it with .NET.", new DateTime(2026, 1, 5, 14, 30, 0, 0, DateTimeKind.Utc), "Getting Started with GraphQL", 2 },
                    { 3, "PostgreSQL is a robust relational database. Here are some best practices for indexing, query optimization, and maintaining data integrity.", "Tips and tricks for optimizing your PostgreSQL database.", new DateTime(2026, 1, 10, 9, 15, 0, 0, DateTimeKind.Utc), "PostgreSQL Best Practices", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "Timestamp" },
                values: new object[] { "I built this blog using .NET 7, PostgreSQL, and GraphQL. Stay tuned for more updates!", new DateTime(2026, 1, 14, 17, 45, 19, 90, DateTimeKind.Utc).AddTicks(9600) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$zJpGrT0Gz16xC9z0KMVXXujjC.Veluh3IDBtFL1R9TdkHWpBHpx4e");
        }
    }
}
