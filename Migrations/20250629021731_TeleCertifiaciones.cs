using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class TeleCertifiaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "telecertificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Enlace = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IntegracioneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telecertificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_telecertificaciones_integraciones_IntegracioneId",
                        column: x => x.IntegracioneId,
                        principalTable: "integraciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_telecertificaciones_IntegracioneId",
                table: "telecertificaciones",
                column: "IntegracioneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "telecertificaciones");
        }
    }
}
