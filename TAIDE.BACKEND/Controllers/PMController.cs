using Microsoft.AspNetCore.Mvc;
using TAIDE.BACKEND.Modells.Request;
using TuProyecto.Data;
using TAIDE.BACKEND.Modells.Request; // ESTE using ES EL BUENO
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TuProyecto.Data; // Cambia si tu namespace de Data es diferente
using TuProyecto.Services; // Cambia si tu namespace de Services es diferente
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TuProyecto.Data;
using TuProyecto.Services;
using TuProyecto.Models;
using System.Diagnostics;
using TAIDE.BACKEND.Models;

[ApiController]
[Route("api/[controller]")]
public class PMController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PMController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("pacientes")]
    public async Task<IActionResult> ObtenerPacientes()
    {
        // Devuelve todos los pacientes (esto depende si quieres filtrarlos o no)
        var pacientes = await _context.Pacientes
            .Select(p => new
            {
                p.ID,
                p.NombreUsuario,
                p.Estado
            })
            .ToListAsync();

        return Ok(pacientes);
    }
    [HttpPut("pacientes/{pacienteId:int}/estado")]
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
    [Authorize(Roles = "PM")]
    [HttpGet("solicitudes/pendientes")]
    public async Task<IActionResult> ObtenerSolicitudesPendientes()
    {
        var solicitudes = await _context.Solicitudes
            .Where(s => s.Estado == EstadoSolicitud.Pendiente)
            .Include(s => s.Familiar)
            .Include(s => s.Paciente)
            .ToListAsync();

        return Ok(solicitudes);
    }

    [Authorize(Roles = "PM")]
    [HttpPut("solicitudes/{id}/respuesta")]
    public async Task<IActionResult> ResponderSolicitud(int id, [FromBody] ResponderSolicitudRequest request)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);
        if (solicitud == null)
            return NotFound(new { Message = "Solicitud no encontrada" });

        if (request.Respuesta.ToLower() == "aceptar")
        {
            solicitud.Estado = EstadoSolicitud.Aceptada;

            var relacion = new PacientesFamiliares
            {
                PacienteID = solicitud.PacienteId,
                FamiliarID = solicitud.FamiliarId
            };

            _context.PacientesFamiliares.Add(relacion);
        }
        else if (request.Respuesta.ToLower() == "rechazar")
        {
            solicitud.Estado = EstadoSolicitud.Rechazada;
        }
        else
        {
            return BadRequest(new { Message = "Respuesta inválida" });
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Solicitud procesada correctamente" });
    }

}
