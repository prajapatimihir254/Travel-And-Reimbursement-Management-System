using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizTravel.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDestinationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "TravelRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "TravelRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
