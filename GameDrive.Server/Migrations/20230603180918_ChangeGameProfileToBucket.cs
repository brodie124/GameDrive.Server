using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDrive.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGameProfileToBucket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "GameProfileId",
                table: "storage_objects");

            migrationBuilder.AddColumn<string>(
                name: "BucketId",
                table: "storage_objects",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "buckets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buckets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_storage_objects_BucketId",
                table: "storage_objects",
                column: "BucketId");

            migrationBuilder.AddForeignKey(
                name: "FK_storage_objects_buckets_BucketId",
                table: "storage_objects",
                column: "BucketId",
                principalTable: "buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_storage_objects_buckets_BucketId",
                table: "storage_objects");

            migrationBuilder.DropTable(
                name: "buckets");

            migrationBuilder.DropIndex(
                name: "IX_storage_objects_BucketId",
                table: "storage_objects");

            migrationBuilder.DropColumn(
                name: "BucketId",
                table: "storage_objects");

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
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ExcludePatterns_Delimiter = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExcludePatterns_Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IncludePatterns_Delimiter = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IncludePatterns_Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SearchableDirectories_Delimiter = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SearchableDirectories_Value = table.Column<string>(type: "longtext", nullable: false)
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
    }
}
