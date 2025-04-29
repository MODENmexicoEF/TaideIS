using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAIDE.BACKEND.Modells.Request;
using TAIDE.BACKEND.Models;
using TuProyecto.Data;
using System;
using System.Threading.Tasks;

namespace TAIDE.BACKEND.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamiliarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FamiliarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Familiar")]
        [HttpPost("solicitudes")]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudRequest request)
        {
            int familiarId = int.Parse(User.FindFirst("id")!.Value);

            bool yaExiste = await _context.PacientesFamiliares.AnyAsync(r =>
                r.FamiliarID == request.FamiliarId &&
                r.PacienteID == request.PacienteId);

            if (yaExiste)
                return BadRequest(new { Message = "Ya estás vinculado con este paciente." });

            bool solicitudPendiente = await _context.Solicitudes.AnyAsync(s =>
                s.FamiliarId == request.FamiliarId &&
                s.PacienteId == request.PacienteId &&
                s.Estado == EstadoSolicitud.Pendiente);

            if (solicitudPendiente)
                return BadRequest(new { Message = "Ya existe una solicitud pendiente para este paciente." });

            // AQUI VALIDAMOS ANTES DE CREAR LA SOLICITUD
            var existeFamiliar = await _context.Familiares.AnyAsync(f => f.ID == request.FamiliarId);
            var existePaciente = await _context.Pacientes.AnyAsync(p => p.ID == request.PacienteId);

            if (!existeFamiliar || !existePaciente)
                return BadRequest(new { Message = "El paciente o el familiar no existen." });

            var solicitud = new SolicitudFamiliarPaciente
            {
                FamiliarId = request.FamiliarId,
                PacienteId = request.PacienteId,
                FechaSolicitud = DateTime.UtcNow
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Solicitud enviada correctamente." });
        }

        [Authorize(Roles = "Familiar")]
        [HttpGet("buscar-pacientes")]
        public async Task<IActionResult> BuscarPacientes([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { Message = "Consulta vacía" });

            var pacientes = await _context.Pacientes
                .Where(p => p.NombreUsuario.Contains(query) || p.Ap1.Contains(query))
                .Select(p => new
                {
                    p.ID,
                    p.NombreUsuario,
                    p.Ap1
                })
                .ToListAsync();

            return Ok(pacientes);
        }

    }
}
