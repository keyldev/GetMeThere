using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetMeThere.API.Migrations
{
    /// <inheritdoc />
    public partial class Mvp_AuthAndOrderState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "TaxiOrders");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                table: "Users",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FcmDeviceToken",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "TaxiOrders",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DriverArrivedAt",
                table: "TaxiOrders",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrival",
                table: "TaxiOrders",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoutePolyline",
                table: "TaxiOrders",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TaxiOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLatitude",
                table: "TaxiDrivers",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLongitude",
                table: "TaxiDrivers",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLocationUpdate",
                table: "TaxiDrivers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TaxiDrivers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FcmDeviceToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "DriverArrivedAt",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "EstimatedArrival",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "RoutePolyline",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "TaxiDrivers");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "TaxiDrivers");

            migrationBuilder.DropColumn(
                name: "LastLocationUpdate",
                table: "TaxiDrivers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TaxiDrivers");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "ConnectionId",
                keyValue: null,
                column: "ConnectionId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionId",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TaxiOrders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "TaxiOrders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
