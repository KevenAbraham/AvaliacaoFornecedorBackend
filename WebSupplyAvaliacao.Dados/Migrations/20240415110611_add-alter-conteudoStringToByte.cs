using Microsoft.EntityFrameworkCore.Migrations;

namespace WebSupplyAvaliacao.Dados.Migrations;

public partial class addalterconteudoStringToByte : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "NovoConteudo",
            table: "Documento",
            type: "varbinary(max)",
            nullable: true);

        migrationBuilder.Sql("UPDATE Documento SET NovoConteudo = CONVERT(varbinary(max), Conteudo)");

        migrationBuilder.DropColumn(
            name: "Conteudo",
            table: "Documento");

        migrationBuilder.RenameColumn(
            name: "NovoConteudo",
            table: "Documento",
            newName: "Conteudo");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Conteudo",
            table: "Documento");

        migrationBuilder.AddColumn<string>(
            name: "Conteudo",
            table: "Documento",
            type: "nvarchar(max)",
            nullable: false);
    }
}



