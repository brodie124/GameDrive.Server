using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations.SQLite.Migrations
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
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoragePath",
                table: "storage_objects",
                type: "TEXT",
                nullable: true);
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
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
