namespace TAIDE.BACKEND.Models.Request
{
    public class CrearReporteRequest
    {
        public int PacienteID { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
    }
}
