﻿// <auto-generated />
using System;
using GameDrive.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameDrive.Server.Migrations
{
    [DbContext(typeof(GameDriveDbContext))]
    [Migration("20230604221924_RenameFilePathToClientRelativePath")]
    partial class RenameFilePathToClientRelativePath
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GameDrive.Server.Domain.Models.Bucket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("buckets");
                });

            modelBuilder.Entity("GameDrive.Server.Domain.Models.StorageObject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("BucketId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ClientRelativePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("GameDrivePath")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BucketId");

                    b.HasIndex("OwnerId");

                    b.ToTable("storage_objects");
                });

            modelBuilder.Entity("GameDrive.Server.Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("GameDrive.Server.Domain.Models.StorageObject", b =>
                {
                    b.HasOne("GameDrive.Server.Domain.Models.Bucket", "Bucket")
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameDrive.Server.Domain.Models.User", "Owner")
                        .WithMany("StorageObjects")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bucket");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("GameDrive.Server.Domain.Models.User", b =>
                {
                    b.Navigation("StorageObjects");
                });
#pragma warning restore 612, 618
        }
    }
}
