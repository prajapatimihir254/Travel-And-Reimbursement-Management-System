using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizTravel.Migrations
{
    /// <inheritdoc />
    public partial class AddOTPExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OTPGenratedTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPGenratedTime",
                table: "Users");
        }
    }
}
