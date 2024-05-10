using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bloggit.Migrations
{
    /// <inheritdoc />
    public partial class fixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "isDeleted",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "isDeleted",
                table: "Reactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Blogs",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "isDeleted",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "isDeleted",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "AspNetUsers");
        }
    }
}
