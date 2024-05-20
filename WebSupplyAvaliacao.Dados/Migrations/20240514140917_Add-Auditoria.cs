using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Acao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Chave = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    AcaoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Auditoria_Acao_AcaoID",
                        column: x => x.AcaoID,
                        principalTable: "Acao",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Auditoria_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_AcaoID",
                table: "Auditoria",
                column: "AcaoID");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_UsuarioId",
                table: "Auditoria",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditoria");

            migrationBuilder.DropTable(
                name: "Acao");
        }
    }
}
