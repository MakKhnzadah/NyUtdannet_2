using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nyUtdannet2.Migrations
{
    public partial class AddFavoriteModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_JobListingId",
                table: "Favorites",
                columns: new[] { "UserId", "JobListingId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_JobListingId",
                table: "Favorites");
        }
    }
}