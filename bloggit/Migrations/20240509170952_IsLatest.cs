using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bloggit.Migrations
{
    /// <inheritdoc />
    public partial class IsLatest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Tags",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Tags",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isLatest",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Reactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Reactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "isLatest",
                table: "Reactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "isLatest",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "isLatest",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "isLatest",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "isLatest",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "isLatest",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "isLatest",
                table: "Blogs");
        }
    }
}
