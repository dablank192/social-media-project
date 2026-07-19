using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogImages_Blog_Id",
                table: "BlogImages");

            migrationBuilder.CreateIndex(
                name: "IX_BlogImages_BlogId",
                table: "BlogImages",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogImages_Blog_BlogId",
                table: "BlogImages",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogImages_Blog_BlogId",
                table: "BlogImages");

            migrationBuilder.DropIndex(
                name: "IX_BlogImages_BlogId",
                table: "BlogImages");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogImages_Blog_Id",
                table: "BlogImages",
                column: "Id",
                principalTable: "Blog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
