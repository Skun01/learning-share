using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceVocabularyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdjectiveType",
                table: "VocabularyDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Antonyms",
                table: "VocabularyDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommonCollocations",
                table: "VocabularyDetails",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "VocabularyDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JLPTLevel",
                table: "VocabularyDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KanjiComponents",
                table: "VocabularyDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Transitivity",
                table: "VocabularyDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerbGroup",
                table: "VocabularyDetails",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WaniKaniLevel",
                table: "VocabularyDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjectiveType",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "Antonyms",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "CommonCollocations",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "JLPTLevel",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "KanjiComponents",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "Transitivity",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "VerbGroup",
                table: "VocabularyDetails");

            migrationBuilder.DropColumn(
                name: "WaniKaniLevel",
                table: "VocabularyDetails");
        }
    }
}
