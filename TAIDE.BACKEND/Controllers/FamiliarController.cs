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
            // Obtener el ID del familiar autenticado desde el JWT
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out int familiarId))
            {
                return Unauthorized(new { Message = "Usuario no identificado en el token." });
            }

            // Validar si ya está vinculado
            bool yaExiste = await _context.PacientesFamiliares.AnyAsync(r =>
                r.FamiliarID == familiarId &&
                r.PacienteID == request.PacienteId);

            if (yaExiste)
                return BadRequest(new { Message = "Ya estás vinculado con este paciente." });

            // Validar si ya hay una solicitud pendiente
            bool solicitudPendiente = await _context.Solicitudes.AnyAsync(s =>
                s.FamiliarId == familiarId &&
                s.PacienteId == request.PacienteId &&
                s.Estado == EstadoSolicitud.Pendiente);

            if (solicitudPendiente)
                return BadRequest(new { Message = "Ya existe una solicitud pendiente para este paciente." });

            // Validar existencia en base de datos
            var existeFamiliar = await _context.Familiares.AnyAsync(f => f.ID == familiarId);
            var existePaciente = await _context.Pacientes.AnyAsync(p => p.ID == request.PacienteId);

            if (!existeFamiliar || !existePaciente)
                return BadRequest(new { Message = "El paciente o el familiar no existen." });

            // Crear la solicitud
            var solicitud = new SolicitudFamiliarPaciente
            {
                FamiliarId = familiarId,
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
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out int familiarId))
                return Unauthorized();

            var pacientes = await _context.PacientesFamiliares
                .Where(pf => pf.FamiliarID == familiarId)
                .Include(pf => pf.Paciente)
                .Select(pf => new
                {
                    id = pf.Paciente.ID, // ← asegúrate que es .ID
                    nombre = pf.Paciente.NombreUsuario, // ← asegúrate que estás heredando de Usuario correctamente
                    estado = pf.Paciente.Estado
                })
                .ToListAsync();

            return Ok(pacientes);
        }


        [Authorize(Roles = "FAMILIAR")]
        [HttpGet("api/familiar/reportes/{pacienteId}")]
        public async Task<IActionResult> ObtenerReportesDelPaciente(int pacienteId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out int familiarId))
                return Unauthorized();


            var estaVinculado = await _context.PacientesFamiliares
                .AnyAsync(fp => fp.FamiliarID == familiarId && fp.PacienteID == pacienteId);

            if (!estaVinculado)
                return Forbid();

            var reportes = await _context.ReportesMedicos
                .Where(r => r.PacienteID == pacienteId)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            return Ok(reportes);
        }



    }
}
