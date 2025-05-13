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
    using TAIDE.BACKEND.Models.Request;

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
        [HttpGet("solicitudes")]
        public async Task<IActionResult> ObtenerSolicitudesPendientes()
        {
            var solicitudes = await _context.Solicitudes
                .Where(s => s.Estado == EstadoSolicitud.Pendiente)
                .Include(s => s.Familiar)
                .Include(s => s.Paciente)
                .Select(s => new
                {
                    id = s.Id,
                    paciente = $"{s.Paciente.NombreUsuario}",
                    familiar = $"{s.Familiar.Ap1} {s.Familiar.Ap2 ?? ""} ({s.Familiar.NombreUsuario})", // nombre completo y nombre_usuario
                    fechaSolicitud = s.FechaSolicitud.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return Ok(solicitudes);
        }
        [Authorize(Roles = "PM")]
        [HttpGet("vinculos")]
        public async Task<IActionResult> ObtenerVinculos()
        {
            var lista = await _context.PacientesFamiliares
                .Include(fp => fp.Familiar).Include(fp => fp.Paciente)
                .Select(fp => new {
                    FamiliarId = fp.FamiliarID,
                    Familiar = fp.Familiar.NombreUsuario,
                    PacienteId = fp.PacienteID,
                    Paciente = fp.Paciente.NombreUsuario
                }).ToListAsync();

            return Ok(lista);
        }

        [Authorize(Roles = "PM")]
        [HttpDelete("familiares/{familiarId}/paciente/{pacienteId}")]
        public async Task<IActionResult> EliminarVinculo(int familiarId, int pacienteId)
        {
            var vinculo = await _context.PacientesFamiliares
                .FirstOrDefaultAsync(v => v.FamiliarID == familiarId && v.PacienteID == pacienteId);

            if (vinculo == null)
                return NotFound(new { message = "Vínculo no encontrado." });

            _context.PacientesFamiliares.Remove(vinculo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Acceso del familiar retirado correctamente." });
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
        [Authorize(Roles = "PM")]
        [HttpPost("solicitudes/{id}/aprobar")]
        public async Task<IActionResult> AprobarSolicitud(int id)
        {
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.Id == id && s.Estado == EstadoSolicitud.Pendiente);


            if (solicitud == null)
                return NotFound(new { Message = "Solicitud no encontrada o ya procesada." });

            // Cambiar estado
            solicitud.Estado = EstadoSolicitud.Aceptada;

            // Crear vinculación en tabla PacientesFamiliares
            var yaExiste = await _context.PacientesFamiliares.AnyAsync(pf =>
                pf.PacienteID == solicitud.PacienteId && pf.FamiliarID == solicitud.FamiliarId);

            if (!yaExiste)
            {
                var vinculo = new PacientesFamiliares
                {
                    PacienteID = solicitud.PacienteId,
                    FamiliarID = solicitud.FamiliarId
                };
                _context.PacientesFamiliares.Add(vinculo);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Solicitud aprobada y vínculo creado." });
        }

    [Authorize(Roles = "PM")]
    [HttpPost("api/pm/reportes")]
    public async Task<IActionResult> CrearReporte([FromBody] CrearReporteRequest request)
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out int pmId))
            return Unauthorized();
        // método que recupera ID del token JWT
        var existe = await _context.Usuarios.AnyAsync(u => u.ID == request.PacienteID && u.TipoUsuario == Rol.Paciente);


        if (!existe)
            return NotFound(new { message = "Paciente no encontrado." });

        var reporte = new ReporteMedico
        {
            PM_ID = pmId,
            PacienteID = request.PacienteID,
            Titulo = request.Titulo,
            Contenido = request.Contenido,
            Fecha = DateTime.Now
        };

        _context.ReportesMedicos.Add(reporte);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Reporte creado exitosamente." });
    }

    [Authorize(Roles = "PM")]
    [HttpGet("api/pm/reportes/{pacienteId}")]
    public async Task<IActionResult> ObtenerReportesPaciente(int pacienteId)
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out int pmId))
            return Unauthorized();


        var vinculado = await _context.PacientesFamiliares
            .AnyAsync(v => v.PacienteID == pacienteId); // opcionalmente, puedes validar que el PM tenga al paciente asignado

        if (!vinculado)
            return Forbid();

        var reportes = await _context.ReportesMedicos
            .Where(r => r.PacienteID == pacienteId)
            .OrderByDescending(r => r.Fecha)
            .ToListAsync();

        return Ok(reportes);
    }

}
