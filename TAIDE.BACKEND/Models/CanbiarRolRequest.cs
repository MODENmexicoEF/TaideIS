using System.ComponentModel.DataAnnotations;

public class CambiarRolRequest
{
    [Required] public int UsuarioId { get; set; }
    [Required] public string NuevoRol { get; set; }
}