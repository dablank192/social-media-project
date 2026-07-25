using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBlogImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "BlogImages",
                newName: "PublicId");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "BlogImages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "BlogImages");

            migrationBuilder.RenameColumn(
                name: "PublicId",
                table: "BlogImages",
                newName: "StorageKey");
        }
    }
}
