using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TuProyecto.Models;

public class PreguntaSeguridad
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [Required]
    public int UsuarioID { get; set; }

    [ForeignKey("UsuarioID")]
    public Usuario Usuario { get; set; }

    [Required]
    [MaxLength(255)]
    public string Pregunta { get; set; }

    [Required]
    [MaxLength(255)]
    public string Respuesta { get; set; }

    public PreguntaSeguridad() { }

    public PreguntaSeguridad(int usuarioId, string pregunta, string respuesta)
    {
        UsuarioID = usuarioId;
        Pregunta = pregunta;
        Respuesta = respuesta;
    }
}
