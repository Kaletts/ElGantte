using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class AgregoUltimoDiaIntegraciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoDiaIntegrando",
                table: "integraciones",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoDiaStandBy",
                table: "integraciones",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UltimoDiaIntegrando",
                table: "integraciones");

            migrationBuilder.DropColumn(
                name: "UltimoDiaStandBy",
                table: "integraciones");
        }
    }
}
