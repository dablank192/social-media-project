using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_UserProfile_FolloweeId",
                table: "UserFollow");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_UserProfile_FollowerId",
                table: "UserFollow");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_UserProfile_FolloweeId",
                table: "UserFollow",
                column: "FolloweeId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_UserProfile_FollowerId",
                table: "UserFollow",
                column: "FollowerId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_UserProfile_FolloweeId",
                table: "UserFollow");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_UserProfile_FollowerId",
                table: "UserFollow");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_UserProfile_FolloweeId",
                table: "UserFollow",
                column: "FolloweeId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_UserProfile_FollowerId",
                table: "UserFollow",
                column: "FollowerId",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
