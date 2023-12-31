﻿// <auto-generated />
using System;
using GameDrive.Server.Domain.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameDrive.Server.Migrations.SQLite.Migrations
{
    [DbContext(typeof(GameDriveDbContext))]
    partial class GameDriveDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("GameDrive.Server.Domain.Models.Bucket", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("buckets");
                });

            modelBuilder.Entity("GameDrive.Server.Domain.Models.StorageObject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("BucketId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientRelativePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ReplicationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("StoragePath")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TemporaryFileKey")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UploadedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BucketId");

                    b.HasIndex("OwnerId");

                    b.ToTable("storage_objects");
                });

            modelBuilder.Entity("GameDrive.Server.Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

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
