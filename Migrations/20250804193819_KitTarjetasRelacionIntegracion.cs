using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class KitTarjetasRelacionIntegracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntegracionId",
                table: "KitTarjetas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_KitTarjetas_IntegracionId",
                table: "KitTarjetas",
                column: "IntegracionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas");

            migrationBuilder.DropIndex(
                name: "IX_KitTarjetas_IntegracionId",
                table: "KitTarjetas");

            migrationBuilder.DropColumn(
                name: "IntegracionId",
                table: "KitTarjetas");
        }
    }
}
