using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations
{
    /// <inheritdoc />
    public partial class ExpandStorageObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientPath",
                table: "storage_objects",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "FileDirectory",
                table: "storage_objects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "storage_objects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FileHash",
                table: "storage_objects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "storage_objects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "GameProfileId",
                table: "storage_objects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "game_profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_profiles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_storage_objects_GameProfileId",
                table: "storage_objects",
                column: "GameProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_storage_objects_game_profiles_GameProfileId",
                table: "storage_objects",
                column: "GameProfileId",
                principalTable: "game_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_storage_objects_game_profiles_GameProfileId",
                table: "storage_objects");

            migrationBuilder.DropTable(
                name: "game_profiles");

            migrationBuilder.DropIndex(
                name: "IX_storage_objects_GameProfileId",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "FileDirectory",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "GameProfileId",
                table: "storage_objects");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "storage_objects",
                newName: "ClientPath");
        }
    }
}
