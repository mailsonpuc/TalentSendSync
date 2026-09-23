using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentSendSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Curriculos",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "legacy");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Candidaturas",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "legacy");

            migrationBuilder.CreateIndex(
                name: "IX_Curriculos_UserId",
                table: "Curriculos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_UserId",
                table: "Candidaturas",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Curriculos_UserId",
                table: "Curriculos");

            migrationBuilder.DropIndex(
                name: "IX_Candidaturas_UserId",
                table: "Candidaturas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Candidaturas");
        }
    }
}
