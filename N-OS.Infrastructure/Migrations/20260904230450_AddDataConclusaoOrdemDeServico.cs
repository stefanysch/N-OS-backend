using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N_OS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataConclusaoOrdemDeServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataConclusao",
                table: "OrdensDeServico",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataConclusao",
                table: "OrdensDeServico");
        }
    }
}
