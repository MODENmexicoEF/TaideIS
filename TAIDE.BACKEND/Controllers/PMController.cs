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
}
