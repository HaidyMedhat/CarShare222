﻿// <auto-generated />
using System;
using CarShare.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarShare.DAL.Migrations
{
    [DbContext(typeof(CarShareDbContext))]
    [Migration("20250512145942_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarShare.DAL.Models.Car", b =>
                {
                    b.Property<Guid>("CarId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("AverageRating")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CarType")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FuelType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Latitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal?>("Longitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("PricePerDay")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RentalStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Seats")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Transmission")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("CarId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("Brand", "Model", "Year", "RentalStatus", "IsApproved");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("CarShare.DAL.Models.CarFeature", b =>
                {
                    b.Property<Guid>("FeatureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("IconUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("FeatureId");

                    b.ToTable("CarFeatures");
                });

            modelBuilder.Entity("CarShare.DAL.Models.CarFeatureMapping", b =>
                {
                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FeatureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MappingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CarId", "FeatureId");

                    b.HasIndex("FeatureId");

                    b.ToTable("CarFeatureMappings");
                });

            modelBuilder.Entity("CarShare.DAL.Models.CarImage", b =>
                {
                    b.Property<Guid>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsMain")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ImageId");

                    b.HasIndex("CarId");

                    b.ToTable("CarImages");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Notification", b =>
                {
                    b.Property<Guid>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("NotificationType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("RelatedEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RelatedEntityType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("NotificationId");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Rental", b =>
                {
                    b.Property<Guid>("RentalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ActualEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ActualStartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("OwnerNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProposalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RenterNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("RentalId");

                    b.HasIndex("ProposalId")
                        .IsUnique();

                    b.ToTable("Rentals");
                });

            modelBuilder.Entity("CarShare.DAL.Models.RentalProposal", b =>
                {
                    b.Property<Guid>("ProposalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalDocumentsUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LicenseVerificationUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Message")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("ProposalDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RenterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ProposalId");

                    b.HasIndex("CarId");

                    b.HasIndex("RenterId");

                    b.HasIndex("StartDate", "EndDate");

                    b.ToTable("RentalProposals");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Review", b =>
                {
                    b.Property<Guid>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("OwnerReply")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<Guid>("RentalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RenterId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ReplyDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ReviewId");

                    b.HasIndex("CarId");

                    b.HasIndex("Rating");

                    b.HasIndex("RentalId")
                        .IsUnique();

                    b.HasIndex("RenterId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("CarShare.DAL.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccountStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(MAX)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(128)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ProfilePictureUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Role", "IsVerified");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("72a5c862-4291-413f-8174-a5da85a54472"),
                            AccountStatus = 1,
                            CreatedAt = new DateTime(2025, 5, 12, 14, 59, 40, 982, DateTimeKind.Utc).AddTicks(3444),
                            Email = "admin@carshare.com",
                            FirstName = "Admin",
                            IsActive = true,
                            IsVerified = true,
                            LastName = "System",
                            PasswordHash = new byte[] { 247, 195, 116, 93, 89, 67, 230, 116, 207, 90, 75, 191, 174, 18, 226, 28, 98, 206, 230, 16, 24, 4, 179, 243, 164, 249, 83, 103, 154, 199, 110, 156, 107, 216, 36, 240, 2, 166, 32, 177, 167, 97, 77, 84, 23, 113, 130, 40, 79, 28, 196, 141, 129, 248, 254, 208, 234, 65, 196, 253, 77, 134, 206, 144 },
                            PasswordSalt = new byte[] { 82, 144, 79, 6, 111, 233, 52, 161, 32, 178, 93, 196, 22, 107, 80, 206, 58, 44, 146, 4, 152, 183, 67, 38, 28, 6, 115, 10, 65, 37, 131, 42, 28, 31, 136, 10, 204, 192, 76, 144, 190, 105, 181, 207, 20, 237, 123, 73, 251, 93, 47, 121, 201, 250, 169, 151, 53, 138, 152, 36, 50, 107, 240, 242, 90, 214, 223, 33, 95, 208, 10, 29, 92, 199, 125, 161, 254, 196, 255, 240, 38, 199, 44, 163, 209, 248, 188, 229, 80, 151, 83, 109, 202, 70, 125, 208, 156, 28, 222, 135, 64, 204, 212, 237, 223, 201, 69, 201, 17, 221, 76, 0, 181, 5, 104, 48, 102, 142, 64, 214, 250, 36, 102, 41, 4, 200, 198, 112 },
                            PhoneNumber = "010000000000",
                            Role = "Admin",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("CarShare.DAL.Models.Car", b =>
                {
                    b.HasOne("CarShare.DAL.Models.User", "Owner")
                        .WithMany("OwnedCars")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("CarShare.DAL.Models.CarFeatureMapping", b =>
                {
                    b.HasOne("CarShare.DAL.Models.Car", "Car")
                        .WithMany("CarFeatureMappings")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarShare.DAL.Models.CarFeature", "Feature")
                        .WithMany("CarFeatureMappings")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("Feature");
                });

            modelBuilder.Entity("CarShare.DAL.Models.CarImage", b =>
                {
                    b.HasOne("CarShare.DAL.Models.Car", "Car")
                        .WithMany("CarImages")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Notification", b =>
                {
                    b.HasOne("CarShare.DAL.Models.User", "User")
                        .WithMany("Notifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Rental", b =>
                {
                    b.HasOne("CarShare.DAL.Models.RentalProposal", "Proposal")
                        .WithOne("Rental")
                        .HasForeignKey("CarShare.DAL.Models.Rental", "ProposalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Proposal");
                });

            modelBuilder.Entity("CarShare.DAL.Models.RentalProposal", b =>
                {
                    b.HasOne("CarShare.DAL.Models.Car", "Car")
                        .WithMany("RentalProposals")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CarShare.DAL.Models.User", "Renter")
                        .WithMany("RentalProposals")
                        .HasForeignKey("RenterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("Renter");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Review", b =>
                {
                    b.HasOne("CarShare.DAL.Models.Car", "Car")
                        .WithMany("Reviews")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarShare.DAL.Models.Rental", "Rental")
                        .WithOne("Review")
                        .HasForeignKey("CarShare.DAL.Models.Review", "RentalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarShare.DAL.Models.User", "Renter")
                        .WithMany("Reviews")
                        .HasForeignKey("RenterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("Rental");

                    b.Navigation("Renter");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Car", b =>
                {
                    b.Navigation("CarFeatureMappings");

                    b.Navigation("CarImages");

                    b.Navigation("RentalProposals");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("CarShare.DAL.Models.CarFeature", b =>
                {
                    b.Navigation("CarFeatureMappings");
                });

            modelBuilder.Entity("CarShare.DAL.Models.Rental", b =>
                {
                    b.Navigation("Review")
                        .IsRequired();
                });

            modelBuilder.Entity("CarShare.DAL.Models.RentalProposal", b =>
                {
                    b.Navigation("Rental")
                        .IsRequired();
                });

            modelBuilder.Entity("CarShare.DAL.Models.User", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("OwnedCars");

                    b.Navigation("RentalProposals");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
