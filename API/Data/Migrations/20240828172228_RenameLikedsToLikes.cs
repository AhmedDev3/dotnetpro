using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameLikedsToLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameTable(
        name: "Likeds",
        newName: "Likes");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameTable(
        name: "Likes",
        newName: "Likeds");
}
    }
}
