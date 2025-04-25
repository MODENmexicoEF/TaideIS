using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAIDE.BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguraTPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_usuario = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contrasena = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    correo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_usuario = table.Column<int>(type: "int", nullable: false),
                    Ap1 = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ap2 = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Discriminator = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_pm = table.Column<int>(type: "int", nullable: true),
                    numero_colegiado = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    especialidad = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_paciente = table.Column<int>(type: "int", nullable: true),
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FamiliarPaciente",
                columns: table => new
                {
                    FamiliaresID = table.Column<int>(type: "int", nullable: false),
                    PacientesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamiliarPaciente", x => new { x.FamiliaresID, x.PacientesID });
                    table.ForeignKey(
                        name: "FK_FamiliarPaciente_usuarios_FamiliaresID",
                        column: x => x.FamiliaresID,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamiliarPaciente_usuarios_PacientesID",
                        column: x => x.PacientesID,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PacientesFamiliares",
                columns: table => new
                {
                    id_pacientefamiliar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_familiar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacientesFamiliares", x => x.id_pacientefamiliar);
                    table.ForeignKey(
                        name: "FK_PacientesFamiliares_usuarios_id_familiar",
                        column: x => x.id_familiar,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PacientesFamiliares_usuarios_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "preguntasseguridad",
                columns: table => new
                {
                    id_pregunta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    pregunta = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    respuesta = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preguntasseguridad", x => x.id_pregunta);
                    table.ForeignKey(
                        name: "FK_preguntasseguridad_usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FamiliarPaciente_PacientesID",
                table: "FamiliarPaciente",
                column: "PacientesID");

            migrationBuilder.CreateIndex(
                name: "IX_PacientesFamiliares_id_familiar",
                table: "PacientesFamiliares",
                column: "id_familiar");

            migrationBuilder.CreateIndex(
                name: "IX_PacientesFamiliares_id_paciente",
                table: "PacientesFamiliares",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_preguntasseguridad_UsuarioID",
                table: "preguntasseguridad",
                column: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FamiliarPaciente");

            migrationBuilder.DropTable(
                name: "PacientesFamiliares");

            migrationBuilder.DropTable(
                name: "preguntasseguridad");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
