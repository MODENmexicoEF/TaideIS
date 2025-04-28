using System.ComponentModel.DataAnnotations.Schema;
using TuProyecto.Models; // Asegúrate que el using de Usuario y Rol esté presente

namespace TuProyecto.Models
{
    public class PM : Usuario
    {
        // Propiedades específicas de PM


        [Column("numero_colegiado")]
        public string? NumeroColegiado { get; set; }

        [Column("especialidad")]
        public string? Especialidad { get; set; }

        // Constructor por defecto (requerido por EF Core)
        public PM() : base() { }

        // --- Constructor Principal CORREGIDO ---
        // Ahora acepta 7 argumentos, incluyendo los específicos de PM y los de Usuario (ap1, ap2)
        public PM(
            string nombreUsuario,
            string contrasena,
            string correo,
            string? numeroColegiado,
            string? especialidad,
            string ap1,
            string? ap2         // Parámetro para la base
        )
            : base(nombreUsuario, contrasena, correo, Rol.PM, ap1, ap2) // Llama a base() con Rol.PM y pasa ap1, ap2
        {
            // Asigna las propiedades específicas de PM usando los parámetros recibidos
            NumeroColegiado = numeroColegiado;
            Especialidad = especialidad;

            // El Discriminator se establece en base() si el constructor base lo hace,
            // o puedes asegurarlo aquí si prefieres (aunque es redundante si base() lo hace)
            // Discriminator = "PM";
        }

        // Métodos Get/Set (puedes mantenerlos o usar directamente las propiedades públicas)
        public string? GetNumeroColegiado() => NumeroColegiado;
        public string? GetEspecialidad() => Especialidad;
        public void SetNumeroColegiado(string? numeroColegiado) => NumeroColegiado = numeroColegiado;
        public void SetEspecialidad(string? especialidad) => Especialidad = especialidad;
    }
}