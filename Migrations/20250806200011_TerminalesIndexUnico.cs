using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class TerminalesIndexUnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_terminales_integraciones_IntegracionId",
                table: "terminales");

            migrationBuilder.AlterColumn<int>(
                name: "IntegracionId",
                table: "terminales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_terminales_integraciones_IntegracionId",
                table: "terminales",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_terminales_integraciones_IntegracionId",
                table: "terminales");

            migrationBuilder.AlterColumn<int>(
                name: "IntegracionId",
                table: "terminales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_terminales_integraciones_IntegracionId",
                table: "terminales",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
