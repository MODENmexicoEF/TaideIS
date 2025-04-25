using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuProyecto.Data;
using TuProyecto.Models;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace TuProyecto.Models
{
    public class PacientesFamiliares // Cambiado el nombre de la clase
    {
        [Key]
        [Column("id_pacientefamiliar")] // Clave primaria de la tabla de relación
        public int IdPacienteFamiliar { get; set; }

        [Required]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Required]
        [Column("id_familiar")]
        public int IdFamiliar { get; set; }

        // Relaciones
        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; }

        [ForeignKey("IdFamiliar")]
        public Familiar Familiar { get; set; }
    }
}