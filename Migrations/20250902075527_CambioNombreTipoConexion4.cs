using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class CambioNombreTipoConexion4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_integraciones_tipoconexion_TipoConexion",
                table: "integraciones");

            migrationBuilder.DropForeignKey(
                name: "FK_kittarjetas_integraciones_IntegracionId",
                table: "kittarjetas");

            migrationBuilder.DropForeignKey(
                name: "FK_tarjetas_kittarjetas_KitTarjetaId",
                table: "tarjetas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tipoconexion",
                table: "tipoconexion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_kittarjetas",
                table: "kittarjetas");

            migrationBuilder.RenameTable(
                name: "tipoconexion",
                newName: "Tipoconexion");

            migrationBuilder.RenameTable(
                name: "kittarjetas",
                newName: "KitTarjetas");

            migrationBuilder.RenameIndex(
                name: "IX_kittarjetas_IntegracionId",
                table: "KitTarjetas",
                newName: "IX_KitTarjetas_IntegracionId");

            migrationBuilder.RenameColumn(
                name: "TipoConexion",
                table: "integraciones",
                newName: "Tipoconexion");

            migrationBuilder.RenameIndex(
                name: "IX_integraciones_TipoConexion",
                table: "integraciones",
                newName: "IX_integraciones_Tipoconexion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tipoconexion",
                table: "Tipoconexion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KitTarjetas",
                table: "KitTarjetas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_integraciones_Tipoconexion_Tipoconexion",
                table: "integraciones",
                column: "Tipoconexion",
                principalTable: "Tipoconexion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tarjetas_KitTarjetas_KitTarjetaId",
                table: "tarjetas",
                column: "KitTarjetaId",
                principalTable: "KitTarjetas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_integraciones_Tipoconexion_Tipoconexion",
                table: "integraciones");

            migrationBuilder.DropForeignKey(
                name: "FK_KitTarjetas_integraciones_IntegracionId",
                table: "KitTarjetas");

            migrationBuilder.DropForeignKey(
                name: "FK_tarjetas_KitTarjetas_KitTarjetaId",
                table: "tarjetas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tipoconexion",
                table: "Tipoconexion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KitTarjetas",
                table: "KitTarjetas");

            migrationBuilder.RenameTable(
                name: "Tipoconexion",
                newName: "tipoconexion");

            migrationBuilder.RenameTable(
                name: "KitTarjetas",
                newName: "kittarjetas");

            migrationBuilder.RenameIndex(
                name: "IX_KitTarjetas_IntegracionId",
                table: "kittarjetas",
                newName: "IX_kittarjetas_IntegracionId");

            migrationBuilder.RenameColumn(
                name: "Tipoconexion",
                table: "integraciones",
                newName: "TipoConexion");

            migrationBuilder.RenameIndex(
                name: "IX_integraciones_Tipoconexion",
                table: "integraciones",
                newName: "IX_integraciones_TipoConexion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tipoconexion",
                table: "tipoconexion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kittarjetas",
                table: "kittarjetas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_integraciones_tipoconexion_TipoConexion",
                table: "integraciones",
                column: "TipoConexion",
                principalTable: "tipoconexion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_kittarjetas_integraciones_IntegracionId",
                table: "kittarjetas",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tarjetas_kittarjetas_KitTarjetaId",
                table: "tarjetas",
                column: "KitTarjetaId",
                principalTable: "kittarjetas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
