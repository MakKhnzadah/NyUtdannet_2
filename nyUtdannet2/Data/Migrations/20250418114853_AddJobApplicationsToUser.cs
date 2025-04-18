using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nyUtdannet2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJobApplicationsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_AspNetUsers_ApplicationUserId",
                table: "JobApps");

            migrationBuilder.DropIndex(
                name: "IX_JobApps_ApplicationUserId",
                table: "JobApps");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "JobApps");

            migrationBuilder.DropColumn(
                name: "PostalNumber",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "JobApps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_JobApps_ApplicationUserId",
                table: "JobApps",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_AspNetUsers_ApplicationUserId",
                table: "JobApps",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
