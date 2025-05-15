using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AverbacaoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AverbacaoService");

            migrationBuilder.CreateTable(
                name: "Averbacoes",
                schema: "AverbacaoService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proposta_Codigo = table.Column<string>(type: "VARCHAR(15)", nullable: false),
                    Convenio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proposta_Proponente_Cpf = table.Column<string>(type: "VARCHAR(15)", nullable: false),
                    Proposta_Proponente_Nome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Proposta_Proponente_Sobrenome = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Proposta_Proponente_DataNascimento = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Proposta_Valor = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Proposta_Prazo_Meses = table.Column<int>(type: "INT", nullable: false),
                    FormalizacaoCodigoIntegracao = table.Column<int>(type: "INT", nullable: true),
                    FormalizacaoData = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    FormalizacaoDetalhes = table.Column<string>(type: "VARCHAR(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Averbacoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Averbacoes",
                schema: "AverbacaoService");
        }
    }
}
