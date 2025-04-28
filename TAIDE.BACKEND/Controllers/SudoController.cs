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

namespace TAIDE.BACKEND.Controllers
{
    [ApiController]
    [Route("api/sudo")]
    [Authorize(Roles = "SUDO")]
    public class SudoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<SudoController> _logger;
        private readonly TimeSpan _archivableAfter = TimeSpan.FromDays(90);

        public SudoController(ApplicationDbContext context, UsuarioService usuarioService, ILogger<SudoController> logger)
        {
            _context = context;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [Authorize(Roles = "SUDO")]
        [HttpGet("usuarios/activos")]
        public async Task<IActionResult> ObtenerUsuariosActivos()
        {
            var umbral = DateTime.UtcNow.AddMinutes(-5);

            var usuarios = await _context.Usuarios
                .Select(u => new
                {
                    Nombre = u.NombreUsuario,
                    u.Correo,
                    EstaEnLinea = u.UltimaActividad.HasValue &&
                                   u.UltimaActividad.Value >= umbral
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        [Authorize(Roles = "SUDO")]
        [HttpGet("usuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.ObtenerUsuariosConEstadoEnLinea();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return StatusCode(500, new { Message = "Error interno del servidor." });
            }
        }
        // DTO que llega desde el front
        public class CambiarRolRequest
        {
            public int UsuarioId { get; set; }
            public string NuevoRol { get; set; } = null!;
        }
        [Authorize(Roles = "SUDO")]
        [HttpDelete("usuarios/{id:int}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            // 1) No permitir que un SUDO se elimine a sí mismo (opcional)
            var idPropio = int.Parse(User.FindFirst("id_usuario")?.Value ?? "0");
            if (id == idPropio)
                return BadRequest(new { message = "No puedes eliminar tu propio usuario." });

            // 2) Buscar al usuario (incluye dependencias si necesitas borrado manual)
            var usuario = await _context.Usuarios
                                        .Include(u => u.PreguntasSeguridad)
                                        .FirstOrDefaultAsync(u => u.ID == id);

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // 3) Si tienes claves foráneas sin ON DELETE CASCADE, bórralas primero
            //    _context.PreguntasSeguridad.RemoveRange(usuario.PreguntasSeguridad);

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario eliminado con éxito." });
        }
        [Authorize(Roles = "SUDO")]
        [HttpPost("cambiar-rol")]
        public async Task<IActionResult> CambiarRol([FromBody] CambiarRolRequest req)
        {
            if (!Enum.TryParse<Rol>(req.NuevoRol, true, out var rolDestino))
                return BadRequest(new { message = "Rol solicitado no es válido." });

            var usuario = await _context.Usuarios
                                        .Include(u => u.PreguntasSeguridad) // si hace falta
                                        .FirstOrDefaultAsync(u => u.ID == req.UsuarioId);

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            // 1) Si ya tiene el rol deseado
            if (usuario.TipoUsuario == rolDestino)
                return Ok(new { message = "El usuario ya posee ese rol." });

            // 2) Para cambios simples basta con actualizar el discriminador
            usuario.TipoUsuario = rolDestino;

            // 3) Si usas TPT y necesitas mover datos entre tablas hijas
            //    delega la lógica a tu servicio:
            //    await _usuarioService.PromocionarUsuario(usuario, rolDestino);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Rol cambiado con éxito." });
        }
        


        // DTO
        public record RecursosResponse(
            double CpuPercent,
            double MemoriaUsadaMB,
            double MemoriaTotalMB,
            double UptimeSeconds   //  ←  tiempo en segundos
        );

        // Acción protegida para SUDO
        [Authorize(Roles = "SUDO")]
        [HttpGet("recursos")]
        public async Task<IActionResult> ObtenerRecursos()
        {
            /* ---------- CPU (%) ---------- */
            var p = Process.GetCurrentProcess();
            var cpuInicio = p.TotalProcessorTime;
            var tInicio = DateTime.UtcNow;

            await Task.Delay(250);

            p.Refresh();
            var cpuFin = p.TotalProcessorTime;
            var tFin = DateTime.UtcNow;

            var cpuPercent = (cpuFin - cpuInicio).TotalMilliseconds /
                             tFin.Subtract(tInicio).TotalMilliseconds /
                             Environment.ProcessorCount * 100.0;

            /* ---------- Memoria ---------- */
            var memUsadaMB = p.WorkingSet64 / 1024.0 / 1024.0;
            var memTotalMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024.0 / 1024.0;

            /* ---------- Uptime (segundos) ---------- */
            var uptimeSeg = (DateTime.UtcNow - p.StartTime.ToUniversalTime()).TotalSeconds;

            var dto = new RecursosResponse(
                Math.Round(cpuPercent, 2),
                Math.Round(memUsadaMB, 1),
                Math.Round(memTotalMB, 1),
                Math.Round(uptimeSeg, 0)        //  ⬅ double, no TimeSpan
            );

            return Ok(dto);
        }


    }
}
