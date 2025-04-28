using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAIDE.BACKEND.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamiliarPaciente_usuarios_FamiliaresID",
                table: "FamiliarPaciente");

            migrationBuilder.DropForeignKey(
                name: "FK_FamiliarPaciente_usuarios_PacientesID",
                table: "FamiliarPaciente");

            migrationBuilder.DropForeignKey(
                name: "FK_PacientesFamiliares_usuarios_id_familiar",
                table: "PacientesFamiliares");

            migrationBuilder.DropForeignKey(
                name: "FK_PacientesFamiliares_usuarios_id_paciente",
                table: "PacientesFamiliares");

            migrationBuilder.DropIndex(
                name: "IX_PacientesFamiliares_id_paciente",
                table: "PacientesFamiliares");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "especialidad",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "id_paciente",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "id_pm",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "numero_colegiado",
                table: "usuarios");

            migrationBuilder.RenameColumn(
                name: "fecha_nacimiento",
                table: "usuarios",
                newName: "UltimaActividad");

            migrationBuilder.RenameColumn(
                name: "respuesta",
                table: "preguntasseguridad",
                newName: "Respuesta");

            migrationBuilder.RenameColumn(
                name: "pregunta",
                table: "preguntasseguridad",
                newName: "Pregunta");

            migrationBuilder.RenameColumn(
                name: "id_pregunta",
                table: "preguntasseguridad",
                newName: "ID");

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
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_PacientesFamiliares_id_paciente_id_familiar",
                table: "PacientesFamiliares",
                columns: new[] { "id_paciente", "id_familiar" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pms_numero_colegiado",
                table: "pms",
                column: "numero_colegiado",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FamiliarPaciente_familiares_FamiliaresID",
                table: "FamiliarPaciente",
                column: "FamiliaresID",
                principalTable: "familiares",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamiliarPaciente_pacientes_PacientesID",
                table: "FamiliarPaciente",
                column: "PacientesID",
                principalTable: "pacientes",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PacientesFamiliares_familiares_id_familiar",
                table: "PacientesFamiliares",
                column: "id_familiar",
                principalTable: "familiares",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PacientesFamiliares_pacientes_id_paciente",
                table: "PacientesFamiliares",
                column: "id_paciente",
                principalTable: "pacientes",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamiliarPaciente_familiares_FamiliaresID",
                table: "FamiliarPaciente");

            migrationBuilder.DropForeignKey(
                name: "FK_FamiliarPaciente_pacientes_PacientesID",
                table: "FamiliarPaciente");

            migrationBuilder.DropForeignKey(
                name: "FK_PacientesFamiliares_familiares_id_familiar",
                table: "PacientesFamiliares");

            migrationBuilder.DropForeignKey(
                name: "FK_PacientesFamiliares_pacientes_id_paciente",
                table: "PacientesFamiliares");

            migrationBuilder.DropTable(
                name: "familiares");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "pms");

            migrationBuilder.DropTable(
                name: "SUDOs");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_correo",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_nombre_usuario",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_PacientesFamiliares_id_paciente_id_familiar",
                table: "PacientesFamiliares");

            migrationBuilder.RenameColumn(
                name: "UltimaActividad",
                table: "usuarios",
                newName: "fecha_nacimiento");

            migrationBuilder.RenameColumn(
                name: "Respuesta",
                table: "preguntasseguridad",
                newName: "respuesta");

            migrationBuilder.RenameColumn(
                name: "Pregunta",
                table: "preguntasseguridad",
                newName: "pregunta");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "preguntasseguridad",
                newName: "id_pregunta");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "usuarios",
                type: "varchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "especialidad",
                table: "usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "id_paciente",
                table: "usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_pm",
                table: "usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numero_colegiado",
                table: "usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PacientesFamiliares_id_paciente",
                table: "PacientesFamiliares",
                column: "id_paciente");

            migrationBuilder.AddForeignKey(
                name: "FK_FamiliarPaciente_usuarios_FamiliaresID",
                table: "FamiliarPaciente",
                column: "FamiliaresID",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FamiliarPaciente_usuarios_PacientesID",
                table: "FamiliarPaciente",
                column: "PacientesID",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PacientesFamiliares_usuarios_id_familiar",
                table: "PacientesFamiliares",
                column: "id_familiar",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PacientesFamiliares_usuarios_id_paciente",
                table: "PacientesFamiliares",
                column: "id_paciente",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
