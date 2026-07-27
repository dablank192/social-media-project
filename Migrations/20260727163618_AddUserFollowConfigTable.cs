using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFollowConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolloweeCount",
                table: "UserProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FollowerCount",
                table: "UserProfile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserFollow",
                columns: table => new
                {
                    FolloweeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollow", x => new { x.FolloweeId, x.FollowerId });
                    table.ForeignKey(
                        name: "FK_UserFollow_UserProfile_FolloweeId",
                        column: x => x.FolloweeId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollow_UserProfile_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_FollowerId",
                table: "UserFollow",
                column: "FollowerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollow");

            migrationBuilder.DropColumn(
                name: "FolloweeCount",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "FollowerCount",
                table: "UserProfile");
        }
    }
}
