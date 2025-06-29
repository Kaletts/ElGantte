using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class CorregirRelacionCarta2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartaCesion",
                table: "integraciones");

            migrationBuilder.DropIndex(
                name: "FK_CartaCesion_idx",
                table: "integraciones");

            migrationBuilder.DropColumn(
                name: "CartaCesion",
                table: "integraciones");

            migrationBuilder.AddColumn<int>(
                name: "IntegracioneId",
                table: "cartascesion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_cartascesion_IntegracioneId",
                table: "cartascesion",
                column: "IntegracioneId");

            migrationBuilder.AddForeignKey(
                name: "FK_cartascesion_integraciones_IntegracioneId",
                table: "cartascesion",
                column: "IntegracioneId",
                principalTable: "integraciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cartascesion_integraciones_IntegracioneId",
                table: "cartascesion");

            migrationBuilder.DropIndex(
                name: "IX_cartascesion_IntegracioneId",
                table: "cartascesion");

            migrationBuilder.DropColumn(
                name: "IntegracioneId",
                table: "cartascesion");

            migrationBuilder.AddColumn<int>(
                name: "CartaCesion",
                table: "integraciones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_CartaCesion_idx",
                table: "integraciones",
                column: "CartaCesion");

            migrationBuilder.AddForeignKey(
                name: "FK_CartaCesion",
                table: "integraciones",
                column: "CartaCesion",
                principalTable: "cartascesion",
                principalColumn: "Id");
        }
    }
}
