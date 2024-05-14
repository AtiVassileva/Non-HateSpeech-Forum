using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NonHateSpeechForum.Data.Migrations
{
    public partial class AddIsFlaggedToPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "Posts");
        }
    }
}
