using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class FixTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogImages_Blog_BlogId",
                table: "BlogImages");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "BlogImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlogId",
                table: "BlogImages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogImages_Blog_BlogId",
                table: "BlogImages",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogImages_Blog_BlogId",
                table: "BlogImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlogId",
                table: "BlogImages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "BlogImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogImages_Blog_BlogId",
                table: "BlogImages",
                column: "BlogId",
                principalTable: "Blog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
