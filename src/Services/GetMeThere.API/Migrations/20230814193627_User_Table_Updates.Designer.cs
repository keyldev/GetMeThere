﻿// <auto-generated />
using System;
using GetMeThere.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GetMeThere.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230814193627_User_Table_Updates")]
    partial class User_Table_Updates
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GetMeThere.API.Models.RefreshToken", b =>
                {
                    b.Property<int>("TokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiryTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TokenString")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("TokenId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("GetMeThere.API.Models.TaxiDriver", b =>
                {
                    b.Property<Guid>("DriverId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("char(36)");

                    b.HasKey("DriverId");

                    b.HasIndex("UserId");

                    b.HasIndex("VehicleId");

                    b.ToTable("TaxiDrivers");
                });

            modelBuilder.Entity("GetMeThere.API.Models.TaxiOrder", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ClientName")
                        .HasColumnType("longtext");

                    b.Property<string>("DestinationAddress")
                        .HasColumnType("longtext");

                    b.Property<double>("DestinationLatitude")
                        .HasColumnType("double");

                    b.Property<double>("DestinationLongitude")
                        .HasColumnType("double");

                    b.Property<Guid?>("DriverId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("NeedChildSeat")
                        .HasColumnType("tinyint(1)");

                    b.Property<float>("OrderPrice")
                        .HasColumnType("float");

                    b.Property<string>("PickupAddress")
                        .HasColumnType("longtext");

                    b.Property<double>("PickupLatitude")
                        .HasColumnType("double");

                    b.Property<double>("PickupLongitude")
                        .HasColumnType("double");

                    b.Property<DateTime>("PickupTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("SeatsCount")
                        .HasColumnType("int");

                    b.Property<Guid?>("TaxiDriverDriverId")
                        .HasColumnType("char(36)");

                    b.Property<int>("VehicleType")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("TaxiDriverDriverId");

                    b.ToTable("TaxiOrders");
                });

            modelBuilder.Entity("GetMeThere.API.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GetMeThere.API.Models.Vehicle", b =>
                {
                    b.Property<Guid>("VehicleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Color")
                        .HasColumnType("longtext");

                    b.Property<bool>("HaveChildSeat")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Model")
                        .HasColumnType("longtext");

                    b.Property<string>("Number")
                        .HasColumnType("longtext");

                    b.Property<int>("SeatsCount")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("VehicleId");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("GetMeThere.API.Models.TaxiDriver", b =>
                {
                    b.HasOne("GetMeThere.API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GetMeThere.API.Models.Vehicle", "Vehicle")
                        .WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("GetMeThere.API.Models.TaxiOrder", b =>
                {
                    b.HasOne("GetMeThere.API.Models.TaxiDriver", "TaxiDriver")
                        .WithMany()
                        .HasForeignKey("TaxiDriverDriverId");

                    b.Navigation("TaxiDriver");
                });
#pragma warning restore 612, 618
        }
    }
}
