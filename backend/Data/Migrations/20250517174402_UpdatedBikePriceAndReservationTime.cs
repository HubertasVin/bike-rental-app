using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeRentalApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBikePriceAndReservationTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "RentPrice",
                table: "Bikes",
                newName: "PricePerMinute");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "PricePerMinute",
                table: "Bikes",
                newName: "RentPrice");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTime",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
