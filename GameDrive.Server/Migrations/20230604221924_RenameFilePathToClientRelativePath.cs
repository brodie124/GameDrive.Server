using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameFilePathToClientRelativePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileNameWithExtension",
                table: "storage_objects",
                newName: "ClientRelativePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientRelativePath",
                table: "storage_objects",
                newName: "FileNameWithExtension");
        }
    }
}
