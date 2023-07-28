using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class StorageObjectTrackRespectiveTemporaryFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TemporaryFileKey",
                table: "storage_objects",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporaryFileKey",
                table: "storage_objects");
        }
    }
}
