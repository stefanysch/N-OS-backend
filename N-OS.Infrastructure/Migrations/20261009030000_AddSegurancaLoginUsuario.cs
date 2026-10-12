using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using N_OS.Infrastructure.Data;

#nullable disable

namespace N_OS.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20261009030000_AddSegurancaLoginUsuario")]
    public partial class AddSegurancaLoginUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BloqueadoAte",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TentativasFalhasLogin",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VersaoToken",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // e-mails passam a ser gravados em minúsculas; os já cadastrados são normalizados
            migrationBuilder.Sql(@"UPDATE ""Usuarios"" SET ""Email"" = LOWER(TRIM(""Email""));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloqueadoAte",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TentativasFalhasLogin",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "VersaoToken",
                table: "Usuarios");
        }
    }
}
