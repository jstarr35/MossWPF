using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MossWPF.Data.Migrations
{
    public partial class AddFilePercentageScores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PercentageScore",
                table: "FilesPairs",
                newName: "SecondFilePercentageScore");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubmissionId",
                table: "MatchingPassages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubmissionId",
                table: "FilesPairs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "FirstFilePercentageScore",
                table: "FilesPairs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "FilesPairs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubmissionId",
                table: "Files",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstFilePercentageScore",
                table: "FilesPairs");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "FilesPairs");

            migrationBuilder.RenameColumn(
                name: "SecondFilePercentageScore",
                table: "FilesPairs",
                newName: "PercentageScore");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "MatchingPassages",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "FilesPairs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }
    }
}
