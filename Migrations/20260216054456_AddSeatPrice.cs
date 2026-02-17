using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEventManagement_TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "EventSeats",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "EventSeats");
        }
    }
}
