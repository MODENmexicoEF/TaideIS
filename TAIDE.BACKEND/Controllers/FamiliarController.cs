using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TAIDE.BACKEND.Modells.Request;
using TAIDE.BACKEND.Models;
using TuProyecto.Data;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

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
            var familiarId = ObtenerIdDesdeToken();
            if (familiarId == null)
                return Unauthorized();

            // Validar si ya está vinculado
            bool yaExiste = await _context.PacientesFamiliares.AnyAsync(r =>
                r.FamiliarID == familiarId.Value &&
                r.PacienteID == request.PacienteId);

            if (yaExiste)
                return BadRequest(new { Message = "Ya estás vinculado con este paciente." });

            // Validar si ya hay una solicitud pendiente
            bool solicitudPendiente = await _context.Solicitudes.AnyAsync(s =>
                s.FamiliarId == familiarId.Value &&
                s.PacienteId == request.PacienteId &&
                s.Estado == EstadoSolicitud.Pendiente);

            if (solicitudPendiente)
                return BadRequest(new { Message = "Ya existe una solicitud pendiente para este paciente." });

            // Validar existencia en base de datos
            var existeFamiliar = await _context.Familiares.AnyAsync(f => f.ID == familiarId.Value);
            var existePaciente = await _context.Pacientes.AnyAsync(p => p.ID == request.PacienteId);

            if (!existeFamiliar || !existePaciente)
                return BadRequest(new { Message = "El paciente o el familiar no existen." });

            // Crear la solicitud
            var solicitud = new SolicitudFamiliarPaciente
            {
                FamiliarId = familiarId.Value,
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

        [Authorize(Roles = "Familiar")]
        [HttpGet("pacientes")]
        public async Task<IActionResult> ObtenerPacientesVinculados()
        {
            var familiarId = ObtenerIdDesdeToken();
            if (familiarId == null)
                return Unauthorized(new { Message = "Usuario no identificado en el token." });

            var pacientes = await _context.PacientesFamiliares
                .Where(pf => pf.FamiliarID == familiarId.Value)
                .Include(pf => pf.Paciente)
                .Select(pf => new
                {
                    id = pf.Paciente.ID,
                    nombre = pf.Paciente.NombreUsuario,
                    estado = pf.Paciente.Estado
                })
                .ToListAsync();

            return Ok(pacientes);
        }

        [Authorize(Roles = "Familiar")]
        [HttpGet("reportes/{pacienteId}")]
        public async Task<IActionResult> ObtenerReportesDelPaciente(int pacienteId)
        {
            var familiarId = ObtenerIdDesdeToken();
            if (familiarId == null)
                return Unauthorized();

            var estaVinculado = await _context.PacientesFamiliares
                .AnyAsync(fp => fp.FamiliarID == familiarId.Value && fp.PacienteID == pacienteId);

            if (!estaVinculado)
                return Forbid();

            var reportes = await _context.ReportesMedicos
                .Where(r => r.PacienteID == pacienteId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            return Ok(reportes);
        }


        private int? ObtenerIdDesdeToken()
        {
            var idClaim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier || c.Type == "id_usuario" || c.Type == "sub");

            if (idClaim == null) return null;

            return int.TryParse(idClaim.Value, out var id) ? id : null;
        }
    }
}
