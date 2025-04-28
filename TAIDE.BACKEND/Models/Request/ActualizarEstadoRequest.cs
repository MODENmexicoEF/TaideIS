using System.ComponentModel.DataAnnotations;

namespace TAIDE.BACKEND.Modells.Request
{
    public class ActualizarEstadoRequest
    {
        [Required]
        [MaxLength(255)]
        public string Estado { get; set; } = default!;
    }
}
