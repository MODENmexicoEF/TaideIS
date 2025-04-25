using System.ComponentModel.DataAnnotations.Schema;

namespace TuProyecto.Models
{
    public class SUDO : Usuario
    {
        public SUDO() : base() { }

        public SUDO(string nombreUsuario, string contrasena, string correo, string ap1, string? ap2)
            : base(nombreUsuario, contrasena, correo, Rol.SUDO, ap1, ap2) // Llama a base con Rol.SUDO
        {
            // Discriminator = "SUDO"; // BORRADO - El constructor base ya debería asignarlo
        }

        // Puedes añadir propiedades específicas para SUDO aquí si las necesitas
    }
}