using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetMeThere.API.Migrations
{
    /// <inheritdoc />
    public partial class Taxitables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Model = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SeatsCount = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HaveChildSeat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaxiDrivers",
                columns: table => new
                {
                    DriverId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VehicleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxiDrivers", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_TaxiDrivers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaxiDrivers_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaxiOrders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ClientName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PickupAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PickupLatitude = table.Column<double>(type: "double", nullable: false),
                    PickupLongitude = table.Column<double>(type: "double", nullable: false),
                    DestinationAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinationLatitude = table.Column<double>(type: "double", nullable: false),
                    DestinationLongitude = table.Column<double>(type: "double", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderPrice = table.Column<float>(type: "float", nullable: false),
                    IsCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NeedChildSeat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SeatsCount = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    TaxiDriverDriverId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxiOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_TaxiOrders_TaxiDrivers_TaxiDriverDriverId",
                        column: x => x.TaxiDriverDriverId,
                        principalTable: "TaxiDrivers",
                        principalColumn: "DriverId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiDrivers_UserId",
                table: "TaxiDrivers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiDrivers_VehicleId",
                table: "TaxiDrivers",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxiOrders_TaxiDriverDriverId",
                table: "TaxiOrders",
                column: "TaxiDriverDriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxiOrders");

            migrationBuilder.DropTable(
                name: "TaxiDrivers");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}
