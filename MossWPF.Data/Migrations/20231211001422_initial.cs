using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MossWPF.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilesPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    LinesMatched = table.Column<int>(type: "INTEGER", nullable: false),
                    PercentageScore = table.Column<int>(type: "INTEGER", nullable: false),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesPairs_Files_FirstFileId",
                        column: x => x.FirstFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilesPairs_Files_SecondFileId",
                        column: x => x.SecondFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingPassages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilePairId = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchedLines = table.Column<string>(type: "TEXT", nullable: true),
                    FirstFileLines = table.Column<string>(type: "TEXT", nullable: true),
                    SecondFileLines = table.Column<string>(type: "TEXT", nullable: true),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingPassages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingPassages_FilesPairs_FilePairId",
                        column: x => x.FilePairId,
                        principalTable: "FilesPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilesPairs_FirstFileId",
                table: "FilesPairs",
                column: "FirstFileId");

            migrationBuilder.CreateIndex(
                name: "IX_FilesPairs_SecondFileId",
                table: "FilesPairs",
                column: "SecondFileId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingPassages_FilePairId",
                table: "MatchingPassages",
                column: "FilePairId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchingPassages");

            migrationBuilder.DropTable(
                name: "FilesPairs");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
