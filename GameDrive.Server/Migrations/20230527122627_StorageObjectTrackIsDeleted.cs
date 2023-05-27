using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations
{
    /// <inheritdoc />
    public partial class StorageObjectTrackIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "storage_objects",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "storage_objects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "storage_objects");
        }
    }
}
