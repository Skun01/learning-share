using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cards_DeckId",
                table: "Cards");

            migrationBuilder.AddColumn<int>(
                name: "AudioMediaId",
                table: "Cards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Hint",
                table: "Cards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Cards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Cards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AudioMediaId",
                table: "Cards",
                column: "AudioMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckId_Priority",
                table: "Cards",
                columns: new[] { "DeckId", "Priority" });

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_MediaFiles_AudioMediaId",
                table: "Cards",
                column: "AudioMediaId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_MediaFiles_AudioMediaId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_AudioMediaId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_DeckId_Priority",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "AudioMediaId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Hint",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Cards");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckId",
                table: "Cards",
                column: "DeckId");
        }
    }
}
