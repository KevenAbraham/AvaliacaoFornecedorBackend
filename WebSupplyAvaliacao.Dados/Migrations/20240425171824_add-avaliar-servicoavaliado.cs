using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    /// <inheritdoc />
    public partial class addavaliarservicoavaliado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServicoAvaliado",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicoAvaliado", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Avaliar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Detalhes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    FornecedorId = table.Column<int>(type: "int", nullable: true),
                    ServicoAvaliadoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Avaliar_Fornecedor_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedor",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Avaliar_ServicoAvaliado_ServicoAvaliadoId",
                        column: x => x.ServicoAvaliadoId,
                        principalTable: "ServicoAvaliado",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Avaliar_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliar_FornecedorId",
                table: "Avaliar",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliar_ServicoAvaliadoId",
                table: "Avaliar",
                column: "ServicoAvaliadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliar_UsuarioId",
                table: "Avaliar",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliar");

            migrationBuilder.DropTable(
                name: "ServicoAvaliado");

            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Fornecedor",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
