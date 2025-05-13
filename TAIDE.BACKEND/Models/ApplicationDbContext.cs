using Microsoft.EntityFrameworkCore;
using TAIDE.BACKEND.Models;
using TuProyecto.Models;
// using TuProyecto.Services; // Probablemente no necesites este using aquí

namespace TuProyecto.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets para todas las entidades en la jerarquía y relacionadas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PM> PMs { get; set; }
        public DbSet<Familiar> Familiares { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<SUDO> SUDOs { get; set; } // Inclúyelo aunque no tenga tabla propia en TPT
        public DbSet<PreguntaSeguridad> PreguntasSeguridad { get; set; }
        public DbSet<PacientesFamiliares> PacientesFamiliares { get; set; }
        public DbSet<SolicitudFamiliarPaciente> Solicitudes { get; set; }
        public DbSet<ReporteMedico> ReportesMedicos { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN TPT ---
            // Mapear la clase base a la tabla 'usuarios'
            modelBuilder.Entity<Usuario>().ToTable("usuarios");

            // Mapear las clases derivadas a sus tablas específicas
            // EF Core entiende que la PK (ID de Usuario) se usa en estas tablas también
            modelBuilder.Entity<PM>().ToTable("pms");
            modelBuilder.Entity<Paciente>().ToTable("pacientes");
            modelBuilder.Entity<Familiar>().ToTable("familiares");
            // SUDO no necesita .ToTable() si no tiene propiedades/columnas adicionales.
            // Existirá solo como una fila en 'usuarios' con el TipoUsuario correcto.

            // --- CONFIGURACIÓN DE RELACIONES ---

            // Relación muchos a muchos entre Paciente y Familiar (Usando entidad de unión explícita)
            modelBuilder.Entity<PacientesFamiliares>()
                .ToTable("FamiliaresPacientes") // Nombre de la tabla de unión
                .HasKey(pf => new { pf.FamiliarID, pf.PacienteID }); // Clave primaria de la tabla de unión

            modelBuilder.Entity<PacientesFamiliares>()
                .HasOne(pf => pf.Paciente) // Un lado de la relación
                .WithMany(p => p.PacientesFamiliares) // Colección de navegación en Paciente
                .HasForeignKey(pf => pf.PacienteID) // Clave foránea en la tabla de unión
                .OnDelete(DeleteBehavior.Cascade); // Comportamiento al borrar

            modelBuilder.Entity<PacientesFamiliares>()
                .HasOne(pf => pf.Familiar) // Otro lado de la relación
                .WithMany(f => f.PacientesFamiliares) // Colección de navegación en Familiar
                .HasForeignKey(pf => pf.FamiliarID) // Clave foránea en la tabla de unión
                .OnDelete(DeleteBehavior.Cascade); // Comportamiento al borrar

            // Relación uno a muchos entre Usuario y PreguntaSeguridad
            modelBuilder.Entity<PreguntaSeguridad>()
                    .ToTable("preguntasseguridad"); // Nombre de la tabla

            modelBuilder.Entity<PreguntaSeguridad>()
                .HasOne(ps => ps.Usuario) // Una PreguntaSeguridad pertenece a un Usuario
                .WithMany(u => u.PreguntasSeguridad) // Un Usuario tiene muchas PreguntasSeguridad
                .HasForeignKey(ps => ps.UsuarioID) // Clave foránea en PreguntaSeguridad
                .OnDelete(DeleteBehavior.Cascade); // Comportamiento al borrar

            // --- ÍNDICES Y RESTRICCIONES UNIQUE ---
            // (Aseguran unicidad a nivel de base de datos)

            modelBuilder.Entity<Usuario>()
               .HasIndex(u => u.Correo)
               .IsUnique(); // Correo debe ser único en 'usuarios'

            modelBuilder.Entity<Usuario>()
               .HasIndex(u => u.NombreUsuario)
               .IsUnique(); // NombreUsuario debe ser único en 'usuarios'

            modelBuilder.Entity<PM>()
               .HasIndex(p => p.NumeroColegiado)
               .IsUnique(); // NumeroColegiado debe ser único en 'pms' (si aplica)

            // Índice para la relación M-N (evita duplicados de pares paciente-familiar)
            modelBuilder.Entity<PacientesFamiliares>()
               .HasIndex(pf => new { pf.PacienteID, pf.FamiliarID })
               .IsUnique();
            modelBuilder.Entity<SolicitudFamiliarPaciente>()
                .HasOne(s => s.Familiar)
                .WithMany()
                .HasForeignKey(s => s.FamiliarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitudFamiliarPaciente>()
                .HasOne(s => s.Paciente)
                .WithMany()
                .HasForeignKey(s => s.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}