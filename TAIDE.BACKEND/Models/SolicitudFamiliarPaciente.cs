using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TuProyecto.Models;

namespace TAIDE.BACKEND.Models
{
    public class SolicitudFamiliarPaciente
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Familiar")]
        public int FamiliarId { get; set; }

        [ForeignKey("Paciente")]
        public int PacienteId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

        public Usuario Familiar { get; set; }
        public Usuario Paciente { get; set; }
    }

    public enum EstadoSolicitud
    {
        Pendiente = 0,
        Aceptada = 1,
        Rechazada = 2,
    }
}
