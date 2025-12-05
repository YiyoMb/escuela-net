using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaEscolar.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escuelas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escuelas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Padres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EsPadre = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Padres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Grado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PadreId = table.Column<int>(type: "int", nullable: false),
                    MadreId = table.Column<int>(type: "int", nullable: false),
                    EscuelaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alumnos_Escuelas_EscuelaId",
                        column: x => x.EscuelaId,
                        principalTable: "Escuelas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alumnos_Padres_MadreId",
                        column: x => x.MadreId,
                        principalTable: "Padres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alumnos_Padres_PadreId",
                        column: x => x.PadreId,
                        principalTable: "Padres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Escuelas",
                columns: new[] { "Id", "Direccion", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "Av. Principal 123", "Escuela Primaria Benito Juárez", "4491234567" },
                    { 2, "Calle Reforma 456", "Instituto Miguel Hidalgo", "4499876543" }
                });

            migrationBuilder.InsertData(
                table: "Padres",
                columns: new[] { "Id", "Apellido", "Email", "EsPadre", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "García", "juan.garcia@email.com", true, "Juan", "4491111111" },
                    { 2, "Rodríguez", "maria.rodriguez@email.com", false, "María", "4492222222" },
                    { 3, "López", "pedro.lopez@email.com", true, "Pedro", "4493333333" },
                    { 4, "Martínez", "ana.martinez@email.com", false, "Ana", "4494444444" }
                });

            migrationBuilder.InsertData(
                table: "Alumnos",
                columns: new[] { "Id", "Apellido", "EscuelaId", "FechaNacimiento", "Grado", "MadreId", "Nombre", "PadreId" },
                values: new object[,]
                {
                    { 1, "García Rodríguez", 1, new DateTime(2015, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "3° Primaria", 2, "Carlos", 1 },
                    { 2, "López Martínez", 1, new DateTime(2016, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "2° Primaria", 4, "Laura", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_EscuelaId",
                table: "Alumnos",
                column: "EscuelaId");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_MadreId",
                table: "Alumnos",
                column: "MadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_PadreId",
                table: "Alumnos",
                column: "PadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Padres_EsPadre",
                table: "Padres",
                column: "EsPadre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "Escuelas");

            migrationBuilder.DropTable(
                name: "Padres");
        }
    }
}
