using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations.MySQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStorageObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameDrivePath",
                table: "storage_objects");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplicationDate",
                table: "storage_objects",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoragePath",
                table: "storage_objects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplicationDate",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "StoragePath",
                table: "storage_objects");

            migrationBuilder.AddColumn<string>(
                name: "GameDrivePath",
                table: "storage_objects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
