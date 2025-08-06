using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class KitTarjetasIndexUnicoFuera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Quitar la FK
            migrationBuilder.DropForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas");

            // 2. Quitar el índice único (que está "atado" a la FK)
            migrationBuilder.DropIndex(
                name: "IX_KitTarjetas_IntegracionId",
                table: "KitTarjetas");

            // 3. Crear índice NO único
            migrationBuilder.CreateIndex(
                name: "IX_KitTarjetas_IntegracionId",
                table: "KitTarjetas",
                column: "IntegracionId",
                unique: false);

            // 4. Volver a crear la FK (ahora con índice no único)
            migrationBuilder.AddForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Hacer la operación inversa

            migrationBuilder.DropForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas");

            migrationBuilder.DropIndex(
                name: "IX_KitTarjetas_IntegracionId",
                table: "KitTarjetas");

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

    }
}
