using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class PruebaBaseTarjetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integracion_M",
                table: "terminales");

            migrationBuilder.DropTable(
                name: "kitintegracion");

            migrationBuilder.DropTable(
                name: "kittarjetas");

            migrationBuilder.DropIndex(
                name: "FK_Integracion_idx1",
                table: "terminales");

            migrationBuilder.DropColumn(
                name: "Integracion",
                table: "terminales");

            migrationBuilder.RenameIndex(
                name: "id_UNIQUE6",
                table: "statuses",
                newName: "id_UNIQUE5");

            migrationBuilder.RenameIndex(
                name: "id_UNIQUE5",
                table: "partners",
                newName: "id_UNIQUE4");

            migrationBuilder.AlterColumn<string>(
                name: "Serie",
                table: "terminales",
                type: "varchar(90)",
                maxLength: 90,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimoCambio",
                table: "terminales",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IntegracionId",
                table: "terminales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "FK_Integracion_idx",
                table: "terminales",
                column: "IntegracionId");

            migrationBuilder.AddForeignKey(
                name: "FK_terminales_integraciones_IntegracionId",
                table: "terminales",
                column: "IntegracionId",
                principalTable: "integraciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_terminales_integraciones_IntegracionId",
                table: "terminales");

            migrationBuilder.DropIndex(
                name: "FK_Integracion_idx",
                table: "terminales");

            migrationBuilder.DropColumn(
                name: "FechaUltimoCambio",
                table: "terminales");

            migrationBuilder.DropColumn(
                name: "IntegracionId",
                table: "terminales");

            migrationBuilder.RenameIndex(
                name: "id_UNIQUE5",
                table: "statuses",
                newName: "id_UNIQUE6");

            migrationBuilder.RenameIndex(
                name: "id_UNIQUE4",
                table: "partners",
                newName: "id_UNIQUE5");

            migrationBuilder.AlterColumn<string>(
                name: "Serie",
                table: "terminales",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(string),
                oldType: "varchar(90)",
                oldMaxLength: 90)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<int>(
                name: "Integracion",
                table: "terminales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "kittarjetas",
                columns: table => new
                {
                    Id = table.Column<sbyte>(type: "tinyint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Numero = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PIN = table.Column<short>(type: "smallint", nullable: true),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "kitintegracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Integracion = table.Column<int>(type: "int", nullable: false),
                    Tarjetas = table.Column<sbyte>(type: "tinyint", nullable: false),
                    Terminal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Integracion",
                        column: x => x.Integracion,
                        principalTable: "integraciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tarjetas",
                        column: x => x.Tarjetas,
                        principalTable: "kittarjetas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Terminal",
                        column: x => x.Terminal,
                        principalTable: "terminales",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "FK_Integracion_idx1",
                table: "terminales",
                column: "Integracion");

            migrationBuilder.CreateIndex(
                name: "FK_Integracion_idx",
                table: "kitintegracion",
                column: "Integracion");

            migrationBuilder.CreateIndex(
                name: "FK_Tarjetas_idx",
                table: "kitintegracion",
                column: "Tarjetas");

            migrationBuilder.CreateIndex(
                name: "FK_Terminal_idx",
                table: "kitintegracion",
                column: "Terminal");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE4",
                table: "kittarjetas",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Integracion_M",
                table: "terminales",
                column: "Integracion",
                principalTable: "integraciones",
                principalColumn: "Id");
        }
    }
}
