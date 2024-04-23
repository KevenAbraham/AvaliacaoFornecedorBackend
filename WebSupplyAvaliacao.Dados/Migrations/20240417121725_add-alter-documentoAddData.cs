using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSupplyAvaliacao.Dados.Migrations
{
    /// <inheritdoc />
    public partial class addalterdocumentoAddData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Data",
                table: "Documento",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Documento");
        }
    }
}
