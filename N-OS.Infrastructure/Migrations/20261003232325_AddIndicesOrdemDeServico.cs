using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace N_OS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndicesOrdemDeServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServico_DataAbertura",
                table: "OrdensDeServico",
                column: "DataAbertura");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensDeServico_Status",
                table: "OrdensDeServico",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServico_DataAbertura",
                table: "OrdensDeServico");

            migrationBuilder.DropIndex(
                name: "IX_OrdensDeServico_Status",
                table: "OrdensDeServico");
        }
    }
}
