using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGrammarEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormationRules",
                table: "GrammarDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nuance",
                table: "GrammarDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Register",
                table: "GrammarDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsageNotes",
                table: "GrammarDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormationRules",
                table: "GrammarDetails");

            migrationBuilder.DropColumn(
                name: "Nuance",
                table: "GrammarDetails");

            migrationBuilder.DropColumn(
                name: "Register",
                table: "GrammarDetails");

            migrationBuilder.DropColumn(
                name: "UsageNotes",
                table: "GrammarDetails");
        }
    }
}
