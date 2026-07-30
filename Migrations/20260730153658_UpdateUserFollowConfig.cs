using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFollowConfig : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_UserProfile_FolloweeId",
                table: "UserFollow");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_UserProfile_FollowerId",
                table: "UserFollow");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UserProfile_UserId",
                table: "UserProfile");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_UserProfile_FolloweeId",
                table: "UserFollow",
                column: "FolloweeId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_UserProfile_FollowerId",
                table: "UserFollow",
                column: "FollowerId",
                principalTable: "UserProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
