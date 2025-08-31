using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class NuevaVariableTipoConexion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "TipoConexion",
                table: "integraciones",
                type: "tinyint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TipoConexion",
                columns: table => new
                {
                    Id = table.Column<sbyte>(type: "tinyint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoConexion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_integraciones_TipoConexion",
                table: "integraciones",
                column: "TipoConexion");

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE7",
                table: "TipoConexion",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_integraciones_TipoConexion_TipoConexion",
                table: "integraciones",
                column: "TipoConexion",
                principalTable: "TipoConexion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_integraciones_TipoConexion_TipoConexion",
                table: "integraciones");

            migrationBuilder.DropTable(
                name: "TipoConexion");

            migrationBuilder.DropIndex(
                name: "IX_integraciones_TipoConexion",
                table: "integraciones");

            migrationBuilder.DropColumn(
                name: "TipoConexion",
                table: "integraciones");
        }
    }
}
