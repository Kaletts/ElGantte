using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class CuadernoPruebasModeloCambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuadernoPrueba",
                table: "cuadernosprueba");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "cuadernosprueba",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "RutaArchivo",
                table: "cuadernosprueba",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RutaArchivo",
                table: "cuadernosprueba");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Fecha",
                table: "cuadernosprueba",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<byte[]>(
                name: "CuadernoPrueba",
                table: "cuadernosprueba",
                type: "longblob",
                nullable: false);
        }
    }
}
