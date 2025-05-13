using TuProyecto.Models;

namespace TAIDE.BACKEND.Models
{
    public class ReporteMedico
    {
        public int Id { get; set; }
        public int PM_ID { get; set; }
        public int PacienteID { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; }

        // Relaciones (opcional, para EF Core)
        public Usuario PM { get; set; }
        public Usuario Paciente { get; set; }
    }
}
