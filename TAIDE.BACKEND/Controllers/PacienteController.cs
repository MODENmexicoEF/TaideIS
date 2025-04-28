using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TAIDE.BACKEND.Modells.Request;
using TuProyecto.Data; // tu namespace correcto
using TuProyecto.Models;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PacientesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPut("{pacienteId:int}/estado")]
    public async Task<IActionResult> ActualizarEstado(int pacienteId, [FromBody] ActualizarEstadoRequest request)
    {
        var paciente = await _context.Pacientes.FindAsync(pacienteId);
        if (paciente == null)
            return NotFound(new { Message = "Paciente no encontrado" });

        paciente.Estado = request.Estado;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Estado actualizado correctamente",
            paciente.ID,
            paciente.Estado
        });
    }
    [HttpGet("estado")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> ObtenerEstadoPaciente()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id_usuario");
        if (userIdClaim == null) return Unauthorized(new { Message = "Token inválido." });

        if (!int.TryParse(userIdClaim.Value, out int pacienteId))
            return BadRequest(new { Message = "ID inválido en el token." });

        var paciente = await _context.Pacientes.FindAsync(pacienteId);
        if (paciente == null)
            return NotFound(new { Message = "Paciente no encontrado." });

        return Ok(new
        {
            paciente.ID,
            paciente.NombreUsuario,
            paciente.Estado
        });
    }
    [Authorize(Roles = "Paciente")]
    [HttpGet("mi-estado")]
    public async Task<IActionResult> ObtenerMiEstado()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { Message = "ID de usuario no encontrado en el token." });
        }

        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { Message = "ID de usuario inválido." });
        }

        var paciente = await _context.Pacientes.FindAsync(userId);
        if (paciente == null)
        {
            return NotFound(new { Message = "Paciente no encontrado." });
        }

        return Ok(new
        {
            Estado = paciente.Estado
        });
    }

}
