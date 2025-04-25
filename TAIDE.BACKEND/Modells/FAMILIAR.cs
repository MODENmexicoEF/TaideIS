using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuProyecto.Data;
using TuProyecto.Models;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace TuProyecto.Models
{
    public class Familiar : Usuario
    {
        public List<Paciente> Pacientes { get; set; } = new List<Paciente>(); // Considera si esta relación es necesaria aquí o solo a través de PacientesFamiliares
        public ICollection<PacientesFamiliares> PacientesFamiliares { get; set; } = new List<PacientesFamiliares>();

        public Familiar() { }

        public Familiar(string nombreUsuario, string contrasena, string correo, string ap1, string? ap2)
            : base(nombreUsuario, contrasena, correo, Rol.Familiar, ap1, ap2) // Llama a base con Rol.Familiar
        {
            // Discriminator = "Familiar"; // BORRADO - El constructor base ya debería asignarlo
        }

        public void SetPacientes(List<Paciente> pacientes) => Pacientes = pacientes;
    }
}