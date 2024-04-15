using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    public partial class ConvertConteudoToByteArray : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adiciona uma nova coluna para armazenar o conteúdo como byte array
            migrationBuilder.AddColumn<byte[]>(
                name: "ConteudoTemp",
                table: "Documento",
                type: "varbinary(max)",
                nullable: true);

            // Atualiza a nova coluna com os dados convertidos da coluna Conteudo original
            migrationBuilder.Sql("UPDATE Documento SET ConteudoTemp = CONVERT(varbinary(max), Conteudo)");

            // Remove a coluna Conteudo original
            migrationBuilder.DropColumn(
                name: "Conteudo",
                table: "Documento");

            // Renomeia a nova coluna para o nome original
            migrationBuilder.RenameColumn(
                name: "ConteudoTemp",
                table: "Documento",
                newName: "Conteudo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Adiciona uma nova coluna para armazenar o conteúdo como string
            migrationBuilder.AddColumn<string>(
                name: "ConteudoTemp",
                table: "Documento",
                type: "nvarchar(max)",
                nullable: true);

            // Atualiza a nova coluna com os dados convertidos da coluna Conteudo original
            migrationBuilder.Sql("UPDATE Documento SET ConteudoTemp = CONVERT(nvarchar(max), Conteudo)");

            // Remove a coluna Conteudo original
            migrationBuilder.DropColumn(
                name: "Conteudo",
                table: "Documento");

            // Renomeia a nova coluna para o nome original
            migrationBuilder.RenameColumn(
                name: "ConteudoTemp",
                table: "Documento",
                newName: "Conteudo");
        }
    }
}
