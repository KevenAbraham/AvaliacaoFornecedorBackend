using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    /// <inheritdoc />
    public partial class addalterfornecedorListToICollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EspecializacaoFornecedor_Especializacao_EspecializacaoID",
                table: "EspecializacaoFornecedor");

            migrationBuilder.DropForeignKey(
                name: "FK_EspecializacaoFornecedor_Fornecedor_FornecedorID",
                table: "EspecializacaoFornecedor");

            migrationBuilder.RenameColumn(
                name: "FornecedorID",
                table: "EspecializacaoFornecedor",
                newName: "FornecedoresID");

            migrationBuilder.RenameColumn(
                name: "EspecializacaoID",
                table: "EspecializacaoFornecedor",
                newName: "EspecializacoesID");

            migrationBuilder.RenameIndex(
                name: "IX_EspecializacaoFornecedor_FornecedorID",
                table: "EspecializacaoFornecedor",
                newName: "IX_EspecializacaoFornecedor_FornecedoresID");

            migrationBuilder.AddForeignKey(
                name: "FK_EspecializacaoFornecedor_Especializacao_EspecializacoesID",
                table: "EspecializacaoFornecedor",
                column: "EspecializacoesID",
                principalTable: "Especializacao",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EspecializacaoFornecedor_Fornecedor_FornecedoresID",
                table: "EspecializacaoFornecedor",
                column: "FornecedoresID",
                principalTable: "Fornecedor",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EspecializacaoFornecedor_Especializacao_EspecializacoesID",
                table: "EspecializacaoFornecedor");

            migrationBuilder.DropForeignKey(
                name: "FK_EspecializacaoFornecedor_Fornecedor_FornecedoresID",
                table: "EspecializacaoFornecedor");

            migrationBuilder.RenameColumn(
                name: "FornecedoresID",
                table: "EspecializacaoFornecedor",
                newName: "FornecedorID");

            migrationBuilder.RenameColumn(
                name: "EspecializacoesID",
                table: "EspecializacaoFornecedor",
                newName: "EspecializacaoID");

            migrationBuilder.RenameIndex(
                name: "IX_EspecializacaoFornecedor_FornecedoresID",
                table: "EspecializacaoFornecedor",
                newName: "IX_EspecializacaoFornecedor_FornecedorID");

            migrationBuilder.AddForeignKey(
                name: "FK_EspecializacaoFornecedor_Especializacao_EspecializacaoID",
                table: "EspecializacaoFornecedor",
                column: "EspecializacaoID",
                principalTable: "Especializacao",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EspecializacaoFornecedor_Fornecedor_FornecedorID",
                table: "EspecializacaoFornecedor",
                column: "FornecedorID",
                principalTable: "Fornecedor",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
