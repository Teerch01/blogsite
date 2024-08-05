using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blogsite.Migrations
{
    /// <inheritdoc />
    public partial class updatedpostmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LikedByCurrentUser",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikedByCurrentUser",
                table: "Posts");
        }
    }
}
