﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TuProyecto.Data;

#nullable disable

namespace TAIDE.BACKEND.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250513093331_Inicial")]
    partial class Inicial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("FamiliarPaciente", b =>
                {
                    b.Property<int>("FamiliaresID")
                        .HasColumnType("int");

                    b.Property<int>("PacientesID")
                        .HasColumnType("int");

                    b.HasKey("FamiliaresID", "PacientesID");

                    b.HasIndex("PacientesID");

                    b.ToTable("FamiliarPaciente");
                });

            modelBuilder.Entity("PreguntaSeguridad", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Pregunta")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Respuesta")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<int>("UsuarioID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UsuarioID");

                    b.ToTable("preguntasseguridad", (string)null);
                });

            modelBuilder.Entity("TAIDE.BACKEND.Models.ReporteMedico", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenido")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("PMID")
                        .HasColumnType("int");

                    b.Property<int>("PM_ID")
                        .HasColumnType("int")
                        .HasColumnName("PM_ID");

                    b.Property<int>("PacienteID")
                        .HasColumnType("int")
                        .HasColumnName("PacienteID");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("PMID");

                    b.HasIndex("PacienteID");

                    b.ToTable("ReportesMedicos", (string)null);
                });

            modelBuilder.Entity("TAIDE.BACKEND.Models.SolicitudFamiliarPaciente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Estado")
                        .HasColumnType("int");

                    b.Property<int>("FamiliarId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaSolicitud")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("PacienteId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("FamiliarId");

                    b.HasIndex("PacienteId");

                    b.ToTable("Solicitudes");
                });

            modelBuilder.Entity("TuProyecto.Models.PacientesFamiliares", b =>
                {
                    b.Property<int>("FamiliarID")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("PacienteID")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("FamiliarID", "PacienteID");

                    b.HasIndex("PacienteID", "FamiliarID")
                        .IsUnique();

                    b.ToTable("FamiliaresPacientes", (string)null);
                });

            modelBuilder.Entity("TuProyecto.Models.Usuario", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id_usuario");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Ap1")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("Ap1");

                    b.Property<string>("Ap2")
                        .HasColumnType("longtext")
                        .HasColumnName("Ap2");

                    b.Property<string>("Contrasena")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("contrasena");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("correo");

                    b.Property<string>("NombreUsuario")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("nombre_usuario");

                    b.Property<int>("TipoUsuario")
                        .HasColumnType("int")
                        .HasColumnName("tipo_usuario");

                    b.Property<DateTime?>("UltimaActividad")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ID");

                    b.HasIndex("Correo")
                        .IsUnique();

                    b.HasIndex("NombreUsuario")
                        .IsUnique();

                    b.ToTable("usuarios", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("TuProyecto.Models.Familiar", b =>
                {
                    b.HasBaseType("TuProyecto.Models.Usuario");

                    b.ToTable("familiares", (string)null);
                });

            modelBuilder.Entity("TuProyecto.Models.PM", b =>
                {
                    b.HasBaseType("TuProyecto.Models.Usuario");

                    b.Property<string>("Especialidad")
                        .HasColumnType("longtext")
                        .HasColumnName("especialidad");

                    b.Property<string>("NumeroColegiado")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("numero_colegiado");

                    b.HasIndex("NumeroColegiado")
                        .IsUnique();

                    b.ToTable("pms", (string)null);
                });

            modelBuilder.Entity("TuProyecto.Models.Paciente", b =>
                {
                    b.HasBaseType("TuProyecto.Models.Usuario");

                    b.Property<string>("Estado")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("estado");

                    b.Property<DateTime?>("FechaNacimiento")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("fecha_nacimiento");

                    b.ToTable("pacientes", (string)null);
                });

            modelBuilder.Entity("TuProyecto.Models.SUDO", b =>
                {
                    b.HasBaseType("TuProyecto.Models.Usuario");

                    b.ToTable("SUDOs");
                });

            modelBuilder.Entity("FamiliarPaciente", b =>
                {
                    b.HasOne("TuProyecto.Models.Familiar", null)
                        .WithMany()
                        .HasForeignKey("FamiliaresID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TuProyecto.Models.Paciente", null)
                        .WithMany()
                        .HasForeignKey("PacientesID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PreguntaSeguridad", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", "Usuario")
                        .WithMany("PreguntasSeguridad")
                        .HasForeignKey("UsuarioID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("TAIDE.BACKEND.Models.ReporteMedico", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", "PM")
                        .WithMany()
                        .HasForeignKey("PMID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TuProyecto.Models.Usuario", "Paciente")
                        .WithMany()
                        .HasForeignKey("PacienteID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PM");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("TAIDE.BACKEND.Models.SolicitudFamiliarPaciente", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", "Familiar")
                        .WithMany()
                        .HasForeignKey("FamiliarId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TuProyecto.Models.Usuario", "Paciente")
                        .WithMany()
                        .HasForeignKey("PacienteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Familiar");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("TuProyecto.Models.PacientesFamiliares", b =>
                {
                    b.HasOne("TuProyecto.Models.Familiar", "Familiar")
                        .WithMany("PacientesFamiliares")
                        .HasForeignKey("FamiliarID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TuProyecto.Models.Paciente", "Paciente")
                        .WithMany("PacientesFamiliares")
                        .HasForeignKey("PacienteID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Familiar");

                    b.Navigation("Paciente");
                });

            modelBuilder.Entity("TuProyecto.Models.Familiar", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", null)
                        .WithOne()
                        .HasForeignKey("TuProyecto.Models.Familiar", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TuProyecto.Models.PM", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", null)
                        .WithOne()
                        .HasForeignKey("TuProyecto.Models.PM", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TuProyecto.Models.Paciente", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", null)
                        .WithOne()
                        .HasForeignKey("TuProyecto.Models.Paciente", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TuProyecto.Models.SUDO", b =>
                {
                    b.HasOne("TuProyecto.Models.Usuario", null)
                        .WithOne()
                        .HasForeignKey("TuProyecto.Models.SUDO", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TuProyecto.Models.Usuario", b =>
                {
                    b.Navigation("PreguntasSeguridad");
                });

            modelBuilder.Entity("TuProyecto.Models.Familiar", b =>
                {
                    b.Navigation("PacientesFamiliares");
                });

            modelBuilder.Entity("TuProyecto.Models.Paciente", b =>
                {
                    b.Navigation("PacientesFamiliares");
                });
#pragma warning restore 612, 618
        }
    }
}
