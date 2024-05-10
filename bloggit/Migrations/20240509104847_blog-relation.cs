using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bloggit.Migrations
{
    /// <inheritdoc />
    public partial class blogrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Blogs",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_Author",
                table: "Blogs",
                column: "Author");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_Author",
                table: "Blogs",
                column: "Author",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_Author",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_Author",
                table: "Blogs");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Blogs",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");
        }
    }
}
