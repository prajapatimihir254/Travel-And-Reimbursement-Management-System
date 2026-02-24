using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizTravel.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalAmountToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalAmount",
                table: "TravelRequest",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalAmount",
                table: "TravelRequest");
        }
    }
}
