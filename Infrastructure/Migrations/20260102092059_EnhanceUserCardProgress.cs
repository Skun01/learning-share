using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceUserCardProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BurnedDate",
                table: "UserCardProgress",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CorrectCount",
                table: "UserCardProgress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "EaseFactor",
                table: "UserCardProgress",
                type: "real",
                nullable: false,
                defaultValue: 2.5f);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstLearnedDate",
                table: "UserCardProgress",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncorrectCount",
                table: "UserCardProgress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "UserCardProgress",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LapseCount",
                table: "UserCardProgress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedUntil",
                table: "UserCardProgress",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalReviews",
                table: "UserCardProgress",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BurnedDate",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "CorrectCount",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "EaseFactor",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "FirstLearnedDate",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "IncorrectCount",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "LapseCount",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "SuspendedUntil",
                table: "UserCardProgress");

            migrationBuilder.DropColumn(
                name: "TotalReviews",
                table: "UserCardProgress");
        }
    }
}
