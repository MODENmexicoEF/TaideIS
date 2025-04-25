using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using TuProyecto.Data;
using TuProyecto.Models;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace TuProyecto.Models
{
    public class Paciente : Usuario
    {

        [Column("fecha_nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        public ICollection<Familiar> Familiares { get; set; } = new List<Familiar>();
        public ICollection<PacientesFamiliares> PacientesFamiliares { get; set; } = new List<PacientesFamiliares>();

        public Paciente() { }

        public Paciente(string nombreUsuario, string contrasena, string correo, string ap1, string? ap2)
            : base(nombreUsuario, contrasena, correo, Rol.Paciente, ap1, ap2) // Llama a base con Rol.Paciente
        {
            // Discriminator = "Paciente"; // BORRADO - El constructor base ya debería asignarlo
        }
    }
}