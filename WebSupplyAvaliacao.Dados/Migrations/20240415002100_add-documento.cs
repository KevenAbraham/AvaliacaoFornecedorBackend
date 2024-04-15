using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    /// <inheritdoc />
    public partial class adddocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fornecedor_Usuario_UsuarioId",
                table: "Fornecedor");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Fornecedor",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Fornecedor",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CNPJ",
                table: "Fornecedor",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "Documento",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FornecedorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documento", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Documento_Fornecedor_FornecedorID",
                        column: x => x.FornecedorID,
                        principalTable: "Fornecedor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documento_FornecedorID",
                table: "Documento",
                column: "FornecedorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Fornecedor_Usuario_UsuarioId",
                table: "Fornecedor",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fornecedor_Usuario_UsuarioId",
                table: "Fornecedor");

            migrationBuilder.DropTable(
                name: "Documento");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Fornecedor",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefone",
                table: "Fornecedor",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "CNPJ",
                table: "Fornecedor",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddForeignKey(
                name: "FK_Fornecedor_Usuario_UsuarioId",
                table: "Fornecedor",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
