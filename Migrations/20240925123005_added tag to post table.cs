using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blogsite.Migrations
{
    /// <inheritdoc />
    public partial class addedtagtoposttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Posts");
        }
    }
}
