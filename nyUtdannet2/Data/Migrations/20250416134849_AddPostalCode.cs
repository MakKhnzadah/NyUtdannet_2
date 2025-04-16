using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nyUtdannet2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostalCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_AspNetUsers_UserId",
                table: "JobApps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_AspNetUsers_EmployerUserId",
                table: "JobListings");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Favorites");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "JobApps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobListingId1",
                table: "JobApps",
                type: "INTEGER",
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

            migrationBuilder.CreateIndex(
                name: "IX_JobApps_JobListingId1",
                table: "JobApps",
                column: "JobListingId1");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_JobListingId",
                table: "Favorites",
                columns: new[] { "UserId", "JobListingId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_AspNetUsers_ApplicationUserId",
                table: "JobApps",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_AspNetUsers_UserId",
                table: "JobApps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_JobListings_JobListingId1",
                table: "JobApps",
                column: "JobListingId1",
                principalTable: "JobListings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_AspNetUsers_EmployerUserId",
                table: "JobListings",
                column: "EmployerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_AspNetUsers_ApplicationUserId",
                table: "JobApps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_AspNetUsers_UserId",
                table: "JobApps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApps_JobListings_JobListingId1",
                table: "JobApps");

            migrationBuilder.DropForeignKey(
                name: "FK_JobListings_AspNetUsers_EmployerUserId",
                table: "JobListings");

            migrationBuilder.DropIndex(
                name: "IX_JobApps_ApplicationUserId",
                table: "JobApps");

            migrationBuilder.DropIndex(
                name: "IX_JobApps_JobListingId1",
                table: "JobApps");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId_JobListingId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "JobApps");

            migrationBuilder.DropColumn(
                name: "JobListingId1",
                table: "JobApps");

            migrationBuilder.DropColumn(
                name: "PostalNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Favorites",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApps_AspNetUsers_UserId",
                table: "JobApps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobListings_AspNetUsers_EmployerUserId",
                table: "JobListings",
                column: "EmployerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
