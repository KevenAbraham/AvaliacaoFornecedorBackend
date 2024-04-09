using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    /// <inheritdoc />
    public partial class addalterfornecedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Fornecedor",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Especializacao",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especializacao", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EspecializacaoFornecedor",
                columns: table => new
                {
                    EspecializacaoID = table.Column<int>(type: "int", nullable: false),
                    FornecedorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecializacaoFornecedor", x => new { x.EspecializacaoID, x.FornecedorID });
                    table.ForeignKey(
                        name: "FK_EspecializacaoFornecedor_Especializacao_EspecializacaoID",
                        column: x => x.EspecializacaoID,
                        principalTable: "Especializacao",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EspecializacaoFornecedor_Fornecedor_FornecedorID",
                        column: x => x.FornecedorID,
                        principalTable: "Fornecedor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EspecializacaoFornecedor_FornecedorID",
                table: "EspecializacaoFornecedor",
                column: "FornecedorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EspecializacaoFornecedor");

            migrationBuilder.DropTable(
                name: "Especializacao");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Fornecedor");
        }
    }
}
