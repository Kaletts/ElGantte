using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class CuadernosPrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cuadernosprueba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CuadernoPrueba = table.Column<byte[]>(type: "longblob", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    NombreArchivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoMime = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IntegracioneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuadernosprueba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cuadernosprueba_integraciones_IntegracioneId",
                        column: x => x.IntegracioneId,
                        principalTable: "integraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_cuadernosprueba_IntegracioneId",
                table: "cuadernosprueba",
                column: "IntegracioneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cuadernosprueba");
        }
    }
}
