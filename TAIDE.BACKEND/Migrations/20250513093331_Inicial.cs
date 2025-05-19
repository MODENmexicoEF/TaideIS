using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAIDE.BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
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
                    UltimaActividad = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id_usuario);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "familiares",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_familiares", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_familiares_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    estado = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_pacientes_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pms",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    numero_colegiado = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    especialidad = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pms", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_pms_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "preguntasseguridad",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    Pregunta = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Respuesta = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preguntasseguridad", x => x.ID);
                    table.ForeignKey(
                        name: "FK_preguntasseguridad_usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReportesMedicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PM_ID = table.Column<int>(type: "int", nullable: false),
                    PacienteID = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contenido = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PMID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesMedicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesMedicos_usuarios_PMID",
                        column: x => x.PMID,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportesMedicos_usuarios_PacienteID",
                        column: x => x.PacienteID,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FamiliarId = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitudes_usuarios_FamiliarId",
                        column: x => x.FamiliarId,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Solicitudes_usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SUDOs",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUDOs", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_SUDOs_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FamiliaresPacientes",
                columns: table => new
                {
                    FamiliarID = table.Column<int>(type: "int", nullable: false),
                    PacienteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamiliaresPacientes", x => new { x.FamiliarID, x.PacienteID });
                    table.ForeignKey(
                        name: "FK_FamiliaresPacientes_familiares_FamiliarID",
                        column: x => x.FamiliarID,
                        principalTable: "familiares",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamiliaresPacientes_pacientes_PacienteID",
                        column: x => x.PacienteID,
                        principalTable: "pacientes",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_FamiliarPaciente_familiares_FamiliaresID",
                        column: x => x.FamiliaresID,
                        principalTable: "familiares",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamiliarPaciente_pacientes_PacientesID",
                        column: x => x.PacientesID,
                        principalTable: "pacientes",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FamiliaresPacientes_PacienteID_FamiliarID",
                table: "FamiliaresPacientes",
                columns: new[] { "PacienteID", "FamiliarID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamiliarPaciente_PacientesID",
                table: "FamiliarPaciente",
                column: "PacientesID");

            migrationBuilder.CreateIndex(
                name: "IX_pms_numero_colegiado",
                table: "pms",
                column: "numero_colegiado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preguntasseguridad_UsuarioID",
                table: "preguntasseguridad",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesMedicos_PacienteID",
                table: "ReportesMedicos",
                column: "PacienteID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesMedicos_PMID",
                table: "ReportesMedicos",
                column: "PMID");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_FamiliarId",
                table: "Solicitudes",
                column: "FamiliarId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_PacienteId",
                table: "Solicitudes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_correo",
                table: "usuarios",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FamiliaresPacientes");

            migrationBuilder.DropTable(
                name: "FamiliarPaciente");

            migrationBuilder.DropTable(
                name: "pms");

            migrationBuilder.DropTable(
                name: "preguntasseguridad");

            migrationBuilder.DropTable(
                name: "ReportesMedicos");

            migrationBuilder.DropTable(
                name: "Solicitudes");

            migrationBuilder.DropTable(
                name: "SUDOs");

            migrationBuilder.DropTable(
                name: "familiares");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
