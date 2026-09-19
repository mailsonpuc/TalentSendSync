using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentSendSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirTipoIdHistoricoContato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HistoricoContatoId_Novo",
                table: "HistoricosContato",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricosContato",
                table: "HistoricosContato");

            migrationBuilder.DropColumn(
                name: "HistoricoContatoId",
                table: "HistoricosContato");

            migrationBuilder.RenameColumn(
                name: "HistoricoContatoId_Novo",
                table: "HistoricosContato",
                newName: "HistoricoContatoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricosContato",
                table: "HistoricosContato",
                column: "HistoricoContatoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HistoricoContatoId_Antigo",
                table: "HistoricosContato",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricosContato",
                table: "HistoricosContato");

            migrationBuilder.DropColumn(
                name: "HistoricoContatoId",
                table: "HistoricosContato");

            migrationBuilder.RenameColumn(
                name: "HistoricoContatoId_Antigo",
                table: "HistoricosContato",
                newName: "HistoricoContatoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricosContato",
                table: "HistoricosContato",
                column: "HistoricoContatoId");
        }
    }
}
