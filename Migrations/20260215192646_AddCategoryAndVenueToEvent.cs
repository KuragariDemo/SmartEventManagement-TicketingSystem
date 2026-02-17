using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEventManagement_TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndVenueToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VenueId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CategoryId",
                table: "Events",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_VenueId",
                table: "Events",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Venues_VenueId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CategoryId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_VenueId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
