using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceStudyLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudyLogs_UserId",
                table: "StudyLogs");

            migrationBuilder.AddColumn<int>(
                name: "ExampleId",
                table: "StudyLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewLevel",
                table: "StudyLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OldLevel",
                table: "StudyLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponseTimeMs",
                table: "StudyLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewType",
                table: "StudyLogs",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "StudyLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAnswer",
                table: "StudyLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyLogs_ExampleId",
                table: "StudyLogs",
                column: "ExampleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyLogs_SessionId",
                table: "StudyLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyLogs_UserId_ReviewDate",
                table: "StudyLogs",
                columns: new[] { "UserId", "ReviewDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudyLogs_CardExamples_ExampleId",
                table: "StudyLogs",
                column: "ExampleId",
                principalTable: "CardExamples",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyLogs_CardExamples_ExampleId",
                table: "StudyLogs");

            migrationBuilder.DropIndex(
                name: "IX_StudyLogs_ExampleId",
                table: "StudyLogs");

            migrationBuilder.DropIndex(
                name: "IX_StudyLogs_SessionId",
                table: "StudyLogs");

            migrationBuilder.DropIndex(
                name: "IX_StudyLogs_UserId_ReviewDate",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "ExampleId",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "NewLevel",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "OldLevel",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "ResponseTimeMs",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "ReviewType",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "StudyLogs");

            migrationBuilder.DropColumn(
                name: "UserAnswer",
                table: "StudyLogs");

            migrationBuilder.CreateIndex(
                name: "IX_StudyLogs_UserId",
                table: "StudyLogs",
                column: "UserId");
        }
    }
}
