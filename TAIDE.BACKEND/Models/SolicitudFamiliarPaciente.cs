using TuProyecto.Models;

namespace TAIDE.BACKEND.Models
{
    public class SolicitudFamiliarPaciente
    {
        public int Id { get; set; }
        public int FamiliarId { get; set; }
        public int PacienteId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

        public Familiar Familiar { get; set; }
        public Paciente Paciente { get; set; }
    }

    public enum EstadoSolicitud
    {
        Pendiente = 0,
        Aceptada = 1,
        Rechazada = 2
    }
}
