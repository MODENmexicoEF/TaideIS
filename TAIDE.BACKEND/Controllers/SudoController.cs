using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TAIDE.BACKEND.Data; // Cambia si tu namespace de Data es diferente
using TAIDE.BACKEND.Services; // Cambia si tu namespace de Services es diferente

namespace TAIDE.BACKEND.Controllers
{
    [ApiController]
    [Route("api/sudo")]
    public class SudoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<SudoController> _logger;

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
                    EstaEnLinea = u.UltimaActividad >= umbral
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
                var usuarios = await _usuarioService.ObtenerListaDeUsuarios();
                var dto = usuarios.Select(u => new
                {
                    id = u.ID,
                    nombre = u.NombreUsuario,
                    rol = u.TipoUsuario.ToString(),
                    correo = u.Correo
                }).ToList();

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return StatusCode(500, new { Message = "Error interno del servidor." });
            }
        }
    }
}
