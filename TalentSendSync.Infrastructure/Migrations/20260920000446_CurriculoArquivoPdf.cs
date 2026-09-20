using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentSendSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CurriculoArquivoPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlArquivo",
                table: "Curriculos",
                newName: "StorageKey");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Curriculos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TamanhoBytes",
                table: "Curriculos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Curriculos");

            migrationBuilder.DropColumn(
                name: "TamanhoBytes",
                table: "Curriculos");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "Curriculos",
                newName: "UrlArquivo");
        }
    }
}
