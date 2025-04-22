using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nyUtdannet2.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_AspNetUsers_UserId",
                table: "JobApps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_JobListings_JobListingId",
                table: "JobApps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_JobListings_JobListingId1",
                table: "JobApps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobApps",
                table: "JobApps");

            migrationBuilder.RenameTable(
                name: "JobApps",
                newName: "JobApp");

            migrationBuilder.RenameIndex(
                name: "IX_JobApps_UserId",
                table: "JobApp",
                newName: "IX_JobApp_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApps_JobListingId1",
                table: "JobApp",
                newName: "IX_JobApp_JobListingId1");

            migrationBuilder.RenameIndex(
                name: "IX_JobApps_JobListingId",
                table: "JobApp",
                newName: "IX_JobApp_JobListingId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "JobApp",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobApp",
                table: "JobApp",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_JobApp_ApplicationUserId",
                table: "JobApp",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApp_AspNetUsers_ApplicationUserId",
                table: "JobApp",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApp_AspNetUsers_UserId",
                table: "JobApp",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApp_JobListings_JobListingId",
                table: "JobApp",
                column: "JobListingId",
                principalTable: "JobListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApp_JobListings_JobListingId1",
                table: "JobApp",
                column: "JobListingId1",
                principalTable: "JobListings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApp_AspNetUsers_ApplicationUserId",
                table: "JobApp");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApp_AspNetUsers_UserId",
                table: "JobApp");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApp_JobListings_JobListingId",
                table: "JobApp");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApp_JobListings_JobListingId1",
                table: "JobApp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobApp",
                table: "JobApp");

            migrationBuilder.DropIndex(
                name: "IX_JobApp_ApplicationUserId",
                table: "JobApp");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "JobApp");

            migrationBuilder.RenameTable(
                name: "JobApp",
                newName: "JobApps");

            migrationBuilder.RenameIndex(
                name: "IX_JobApp_UserId",
                table: "JobApps",
                newName: "IX_JobApps_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApp_JobListingId1",
                table: "JobApps",
                newName: "IX_JobApps_JobListingId1");

            migrationBuilder.RenameIndex(
                name: "IX_JobApp_JobListingId",
                table: "JobApps",
                newName: "IX_JobApps_JobListingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobApps",
                table: "JobApps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_AspNetUsers_UserId",
                table: "JobApps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_JobListings_JobListingId",
                table: "JobApps",
                column: "JobListingId",
                principalTable: "JobListings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_JobListings_JobListingId1",
                table: "JobApps",
                column: "JobListingId1",
                principalTable: "JobListings",
                principalColumn: "Id");
        }
    }
}
