using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations
{
    /// <inheritdoc />
    public partial class StorageObjectMergeFileNameFileExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "storage_objects");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "storage_objects",
                newName: "FileNameWithExtension");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileNameWithExtension",
                table: "storage_objects",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "storage_objects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
