using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEventManagement_TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEventPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventPreferences",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPreferences",
                table: "AspNetUsers");
        }
    }
}
