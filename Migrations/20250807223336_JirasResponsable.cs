using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElGantte.Migrations
{
    /// <inheritdoc />
    public partial class JirasResponsable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "jiras",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "jiras");
        }
    }
}
