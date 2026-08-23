using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N_OS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoverEnderecoParaTabelaPropria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClienteEnderecos",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Cep = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    Logradouro = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteEnderecos", x => x.ClienteId);
                    table.ForeignKey(
                        name: "FK_ClienteEnderecos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // preserva qualquer endereço já preenchido antes de derrubar as
            // colunas antigas — só migra linhas que tenham ao menos o CEP.
            migrationBuilder.Sql(
                """
                INSERT INTO "ClienteEnderecos"
                    ("ClienteId", "Cep", "Logradouro", "Numero", "Complemento", "Bairro", "Cidade", "Estado")
                SELECT "Id", "Cep", "Logradouro", "Numero", "Complemento", "Bairro", "Cidade", "Estado"
                FROM "Clientes"
                WHERE "Cep" IS NOT NULL AND "Cep" <> '';
                """);

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Clientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteEnderecos");

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Clientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Clientes",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Clientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "Clientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Clientes",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "Clientes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Clientes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
