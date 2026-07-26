using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace vsa_w_controller_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdToUserProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublidId",
                table: "UserProfile",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublidId",
                table: "UserProfile");
        }
    }
}
