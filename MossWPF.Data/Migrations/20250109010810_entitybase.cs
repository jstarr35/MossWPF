using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MossWPF.Data.Migrations
{
    public partial class entitybase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchingPassages");

            migrationBuilder.DropTable(
                name: "FilesPairs");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubmissionName = table.Column<string>(type: "TEXT", nullable: false),
                    SubmissionUrl = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileComparisons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    File1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    File2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Similarity = table.Column<double>(type: "REAL", nullable: false),
                    File1MatchPct = table.Column<double>(type: "REAL", nullable: false),
                    File2MatchPct = table.Column<double>(type: "REAL", nullable: false),
                    ComparisonUrl = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileComparisons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileComparisons_Files_File1Id",
                        column: x => x.File1Id,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileComparisons_Files_File2Id",
                        column: x => x.File2Id,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileComparisons_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubmissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    SelectedLanguage = table.Column<string>(type: "TEXT", nullable: false),
                    Sensitivity = table.Column<int>(type: "INTEGER", nullable: false),
                    ResultsToShow = table.Column<int>(type: "INTEGER", nullable: false),
                    UseDirectoryMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseExperimental = table.Column<bool>(type: "INTEGER", nullable: false),
                    Comments = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResultsLink = table.Column<string>(type: "TEXT", nullable: true),
                    SourceFiles = table.Column<string>(type: "TEXT", nullable: false),
                    BaseFiles = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionSettings_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComparisonId = table.Column<int>(type: "INTEGER", nullable: false),
                    File1Start = table.Column<int>(type: "INTEGER", nullable: false),
                    File1End = table.Column<int>(type: "INTEGER", nullable: false),
                    File2Start = table.Column<int>(type: "INTEGER", nullable: false),
                    File2End = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchRegions_FileComparisons_ComparisonId",
                        column: x => x.ComparisonId,
                        principalTable: "FileComparisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_SubmissionId",
                table: "Files",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FileComparisons_File1Id",
                table: "FileComparisons",
                column: "File1Id");

            migrationBuilder.CreateIndex(
                name: "IX_FileComparisons_File2Id",
                table: "FileComparisons",
                column: "File2Id");

            migrationBuilder.CreateIndex(
                name: "IX_FileComparisons_SubmissionId",
                table: "FileComparisons",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchRegions_ComparisonId",
                table: "MatchRegions",
                column: "ComparisonId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserId",
                table: "Submissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionSettings_SubmissionId",
                table: "SubmissionSettings",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Submissions_SubmissionId",
                table: "Files",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Submissions_SubmissionId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "MatchRegions");

            migrationBuilder.DropTable(
                name: "SubmissionSettings");

            migrationBuilder.DropTable(
                name: "FileComparisons");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Files_SubmissionId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Files");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubmissionId",
                table: "Files",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "FilesPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstFilePercentageScore = table.Column<int>(type: "INTEGER", nullable: false),
                    LinesMatched = table.Column<int>(type: "INTEGER", nullable: false),
                    Link = table.Column<string>(type: "TEXT", nullable: false),
                    SecondFilePercentageScore = table.Column<int>(type: "INTEGER", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    FirstFileLines = table.Column<string>(type: "TEXT", nullable: true),
                    MatchedLines = table.Column<string>(type: "TEXT", nullable: true),
                    SecondFileLines = table.Column<string>(type: "TEXT", nullable: true),
                    SubmissionId = table.Column<Guid>(type: "TEXT", nullable: false)
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
    }
}
