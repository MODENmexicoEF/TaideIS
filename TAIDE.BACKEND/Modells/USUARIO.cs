using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuProyecto.Models;

namespace TuProyecto.Models
{
    public enum Rol
    {
        Paciente = 0,
        PM = 1,
        Familiar = 2,
        SUDO = 3
    }

    public class Usuario
    {
        [Key]
        [Column("id_usuario")] // Mapeo explícito al nombre de la columna
        public int ID { get; set; }

        [Required]
        [Column("nombre_usuario")] // Mapeo explícito al nombre de la columna
        [MaxLength(50)]
        public string NombreUsuario { get; set; } // Cambiado a NombreUsuario

        [Required]
        [Column("contrasena")] // Mapeo explícito al nombre de la columna
        [MaxLength(100)]
        public string Contrasena { get; protected set; }

        [Required]
        [Column("correo")] // Mapeo explícito al nombre de la columna
        [MaxLength(100)]
        public string Correo { get; set; }

        [Column("tipo_usuario")] // Mapeo explícito al nombre de la columna
        public Rol TipoUsuario { get; set; } // Cambiado a TipoUsuario

        [Required] // Si Ap1 es requerido en BD
        [Column("Ap1")] // Mapeo si el nombre difiere del de BD (aunque parece que coinciden)
        public string Ap1 { get; set; }

        [Column("Ap2")] // Mapeo si el nombre difiere del de BD
        public string? Ap2 { get; set; } // Nullable si puede ser nulo
        public virtual ICollection<PreguntaSeguridad> PreguntasSeguridad { get; set; } = new List<PreguntaSeguridad>();

        // Relaciones (ajustadas según necesidad)
        //public List<FamiliaresPacientes> PacienteFamiliares { get; set; }

        public Usuario(string nombreUsuario, string contrasena, string correo, Rol rol, string ap1, string? ap2) // Añadir ap1, ap2
        {
            NombreUsuario = nombreUsuario;
            Contrasena = contrasena;
            Correo = correo;
            TipoUsuario = rol;
            Ap1 = ap1; // Asignar Ap1
            Ap2 = ap2; // Asignar Ap2
        }

        protected Usuario() { } // Constructor sin parámetros requerido por EF Core

        public void SetContrasena(string contrasena)
        {
            Contrasena = contrasena;
        }

        public int GetID() => ID;
        public Rol GetRol() => TipoUsuario; // Cambiado a TipoUsuario
        public string GetNombre() => NombreUsuario; // Cambiado a NombreUsuario
        public string GetContrasena() => Contrasena;
    }
}