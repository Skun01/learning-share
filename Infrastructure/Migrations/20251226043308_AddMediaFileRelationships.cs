using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFileRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "CardExamples");

            migrationBuilder.AddColumn<int>(
                name: "AvatarMediaId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageMediaId",
                table: "Cards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AudioMediaId",
                table: "CardExamples",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarMediaId",
                table: "Users",
                column: "AvatarMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ImageMediaId",
                table: "Cards",
                column: "ImageMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_CardExamples_AudioMediaId",
                table: "CardExamples",
                column: "AudioMediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardExamples_MediaFiles_AudioMediaId",
                table: "CardExamples",
                column: "AudioMediaId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_MediaFiles_ImageMediaId",
                table: "Cards",
                column: "ImageMediaId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_MediaFiles_AvatarMediaId",
                table: "Users",
                column: "AvatarMediaId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardExamples_MediaFiles_AudioMediaId",
                table: "CardExamples");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_MediaFiles_ImageMediaId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_MediaFiles_AvatarMediaId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AvatarMediaId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Cards_ImageMediaId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_CardExamples_AudioMediaId",
                table: "CardExamples");

            migrationBuilder.DropColumn(
                name: "AvatarMediaId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageMediaId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "AudioMediaId",
                table: "CardExamples");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Cards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "CardExamples",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
