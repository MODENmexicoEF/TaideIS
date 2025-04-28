using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuProyecto.Models;
using TuProyecto.Services;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using static TuProyecto.Models.Usuario;
using TuProyecto.Data;

namespace TuProyecto.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;


        public AuthController(UsuarioService usuarioService, ILogger<AuthController> logger, IConfiguration configuration, ApplicationDbContext context)
        {
            _usuarioService = usuarioService;
            _logger = logger;
            _configuration = configuration;
            _context = context;

        }
        public class LoginRequest
        {
            public string Correo { get; set; }
            public string Contrasena { get; set; }
        }

        [Authorize(Roles = "SUDO")]
        [HttpGet("activos")]


        // --- Método Login (Sin cambios) ---
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) // Se queda
        {
            Console.WriteLine($"[LOGIN] Intentando login con: {request.Correo}");

            if (string.IsNullOrEmpty(request.Correo) || string.IsNullOrEmpty(request.Contrasena))
                return BadRequest(new { message = "Correo y contraseña son requeridos." });
            if (string.IsNullOrEmpty(request.Correo) || string.IsNullOrEmpty(request.Contrasena))
                return BadRequest(new { message = "Correo y contraseña son requeridos." });

            try
            {
                var usuario = await _usuarioService.ObtenerUsuarioPorCorreo(request.Correo);
                if (usuario == null)
                    return Unauthorized(new { message = "Correo no registrado." });

                if (!BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena))
                    return Unauthorized(new { message = "Contraseña incorrecta." });

                await _usuarioService.ActualizarUltimaActividad(usuario.ID);

                var token = GenerateJwtToken(usuario);
                return Ok(new LoginResponse
                {
                    Message = "Inicio de sesión exitoso.",
                    UserId = usuario.ID,
                    Rol = (int)usuario.TipoUsuario,
                    Nombre = usuario.NombreUsuario,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error login {Correo}", request.Correo);
                return StatusCode(500, new { error = "Error interno.", message = "Error inesperado login." });
            }
        }


        // --- Método GenerateJwtToken (Sin cambios) ---
        private string GenerateJwtToken(Usuario usuario)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            _logger.LogInformation("Generando token para: {ID}, {Nombre}, {Rol}", usuario.ID, usuario.NombreUsuario, usuario.TipoUsuario);

            var claims = new[]
            {

        new Claim(ClaimTypes.NameIdentifier, usuario.ID.ToString()),
        new Claim(ClaimTypes.Name, usuario.NombreUsuario),
        new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        // --- Clase DTO RegistroRequestForm (CORREGIDA) ---
        public class RegistroRequestForm : IValidatableObject
        {
            [Required(ErrorMessage = "El nombre de usuario es requerido.")] public string NombreUsuario { get; set; }
            [Required(ErrorMessage = "El primer apellido (Ap1) es requerido.")] public string Ap1 { get; set; }
            public string? Ap2 { get; set; }
            [Required(ErrorMessage = "El correo electrónico es requerido.")][EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")] public string Correo { get; set; }
            [Required(ErrorMessage = "La contraseña es requerida.")] public string Contrasena { get; set; }

            // *** CAMBIO AQUÍ ***
            [Required(ErrorMessage = "El rol es requerido.")]
            public string RolInput { get; set; } // Renombrado de 'Rol' a 'RolInput'

            public List<string>? Preguntas { get; set; }
            public List<string>? Respuestas { get; set; }
            public string? NumeroColegiado { get; set; }
            public string? Especialidad { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                // *** CAMBIO AQUÍ *** (Usa RolInput para TryParse)
                if (!Enum.TryParse<Rol>(RolInput, true, out var parsedRol))
                {
                    // *** CAMBIO AQUÍ *** (El error de validación ahora se refiere a RolInput)
                    yield return new ValidationResult("El valor del Rol proporcionado no es válido.", new[] { nameof(RolInput) });
                }
                else
                {
                    // La comparación aquí usa el enum 'Rol' (tipo) y la variable 'parsedRol' (tipo Rol) - Esto está bien
                    if (parsedRol == Rol.PM)
                    {
                        if (string.IsNullOrWhiteSpace(NumeroColegiado)) { yield return new ValidationResult("El Número de Colegiado es requerido para el rol PM.", new[] { nameof(NumeroColegiado) }); }
                        if (string.IsNullOrWhiteSpace(Especialidad)) { yield return new ValidationResult("La Especialidad es requerida para el rol PM.", new[] { nameof(Especialidad) }); }
                    }
                }
                // El resto de validaciones no cambian
                bool pP = Preguntas?.Any(p => !string.IsNullOrWhiteSpace(p)) ?? false;
                bool rP = Respuestas?.Any(r => !string.IsNullOrWhiteSpace(r)) ?? false;
                if (pP != rP) { yield return new ValidationResult("Debe proporcionar tanto Preguntas como Respuestas de seguridad, o ninguna.", new[] { nameof(Preguntas), nameof(Respuestas) }); }
                else if (pP && rP && Preguntas.Count != Respuestas.Count) { yield return new ValidationResult("El número de Preguntas y Respuestas de seguridad debe coincidir.", new[] { nameof(Preguntas), nameof(Respuestas) }); }
            }
        }
        
        [Authorize]
        [HttpPost("actualizar-actividad")]
        public async Task<IActionResult> ActualizarUltimaActividad() // sudo
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                usuario.UltimaActividad = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Actividad actualizada." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando actividad del usuario.");
                return StatusCode(500, new { message = "Error actualizando actividad." });
            }
        }
        /*
        [Authorize(Roles = "SUDO")]
        [HttpGet("usuarios")]
        public async Task<IActionResult> ObtenerUsuarios() // sudo
        {
            try
            {
                var usuariosDB = await _context.Usuarios
                    .Select(u => new
                    {
                        ID = u.ID,
                        NombreUsuario = u.NombreUsuario,
                        TipoUsuario = (int)u.TipoUsuario,
                        UltimaActividad = u.UltimaActividad
                    })
                    .ToListAsync();

                var usuarios = usuariosDB.Select(u => new
                {
                    u.ID,
                    u.NombreUsuario,
                    u.TipoUsuario,
                    EnLinea = u.UltimaActividad.HasValue &&
                              (DateTime.UtcNow - u.UltimaActividad.Value).TotalMinutes <= 5
                }).ToList();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error interno al obtener usuarios." });
            }
        }

        */

        // --- Endpoint de Registro (CORREGIDO) ---
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarUsuario([FromForm] RegistroRequestForm requestForm) // se queda
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            string contrasenaHasheada = BCrypt.Net.BCrypt.HashPassword(requestForm.Contrasena);

            try
            {
                Usuario resultado = null;

                // *** CAMBIO AQUÍ *** (Usa requestForm.RolInput para TryParse)
                if (Enum.TryParse<Rol>(requestForm.RolInput, true, out var rolEnum))
                {
                    Usuario usuarioACrear = null;
                    switch (rolEnum)
                    {
                        case Rol.PM:
                            usuarioACrear = new PM(requestForm.NombreUsuario, contrasenaHasheada, requestForm.Correo, requestForm.NumeroColegiado, requestForm.Especialidad, requestForm.Ap1, requestForm.Ap2);
                            break;
                        case Rol.Familiar:
                            usuarioACrear = new Familiar(requestForm.NombreUsuario, contrasenaHasheada, requestForm.Correo, requestForm.Ap1, requestForm.Ap2);
                            break;
                        case Rol.Paciente:
                            usuarioACrear = new Paciente(requestForm.NombreUsuario, contrasenaHasheada, requestForm.Correo, requestForm.Ap1, requestForm.Ap2);
                            break;
                        case Rol.SUDO:
                            usuarioACrear = new SUDO(requestForm.NombreUsuario, contrasenaHasheada, requestForm.Correo, requestForm.Ap1, requestForm.Ap2);
                            break;
                    }

                    if (usuarioACrear != null)
                    {
                        switch (rolEnum)
                        {
                            case Rol.PM:
                                resultado = await _usuarioService.CrearPM((PM)usuarioACrear);
                                break;
                            case Rol.Familiar:
                                resultado = await _usuarioService.CrearFamiliar((Familiar)usuarioACrear);
                                break;
                            case Rol.Paciente:
                                resultado = await _usuarioService.CrearPaciente((Paciente)usuarioACrear);
                                break;
                            case Rol.SUDO:
                                resultado = await _usuarioService.CrearSUDO((SUDO)usuarioACrear);
                                break;
                        }

                        if (resultado != null)
                        {
                            // 👇 Crear preguntas de seguridad solo después de tener el ID real
                            if (requestForm.Preguntas != null && requestForm.Respuestas != null)
                            {
                                var preguntasSeguridad = requestForm.Preguntas
                                    .Zip(requestForm.Respuestas, (p, r) => new { p, r })
                                    .Where(pair => !string.IsNullOrWhiteSpace(pair.p) && !string.IsNullOrWhiteSpace(pair.r))
                                    .Select(pair => new PreguntaSeguridad(resultado.ID, pair.p, pair.r))
                                    .ToList();

                                await _usuarioService.GuardarPreguntasSeguridad(preguntasSeguridad);
                            }

                            return Ok(new
                            {
                                Message = "Usuario registrado exitosamente",
                                UsuarioId = resultado.ID,
                                Rol = resultado.TipoUsuario.ToString()
                            });
                        }
                        else
                        {
                            _logger.LogWarning("Crear{Rol} devolvió null para {Correo}", rolEnum, requestForm.Correo);
                            return BadRequest(new { Message = "No se pudo registrar (servicio)." });
                        }
                    }
                    else
                    {
                        _logger.LogError("Error lógico: Rol {Rol} parseado pero no se creó instancia.", requestForm.RolInput);
                        return StatusCode(500, new { Message = "Error interno tipo usuario." });
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Valor de Rol inválido." });
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error DB registro {Correo}", requestForm.Correo);
                if (dbEx.InnerException is MySqlConnector.MySqlException mySqlEx && mySqlEx.Number == 1062)
                {
                    return Conflict(new { Message = "El correo ya está registrado." });
                }
                return StatusCode(500, new { Message = "Error interno DB." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado registro {Correo}", requestForm.Correo);
                return StatusCode(500, new { Message = "Error interno servidor." });
            }

        }

        // --- Método ObtenerUsuarios (Sin cambios) ---

        [HttpPost("recuperar/obtener-preguntas")]
        public async Task<IActionResult> ObtenerPreguntas([FromBody] string correo) // se queda
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorCorreo(correo);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            if (usuario.PreguntasSeguridad == null || !usuario.PreguntasSeguridad.Any())
                return Ok(new { tienePreguntas = false });

            return Ok(new
            {
                tienePreguntas = true,
                preguntas = usuario.PreguntasSeguridad.Select(p => new { p.ID, p.Pregunta }).ToList()
            });
        }


        public class CambiarContrasenaRequest // se queda
        {
            public string Correo { get; set; }
            public Dictionary<int, string> Respuestas { get; set; } // ID de la pregunta, respuesta dada
            public string NuevaContrasena { get; set; }
        }

        [HttpPost("recuperar/cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaRequest request) // se queda
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorCorreo(request.Correo);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            var preguntas = usuario.PreguntasSeguridad;
            if (preguntas == null || !preguntas.Any())
                return BadRequest(new { message = "Este usuario no tiene preguntas de seguridad." });

            int correctas = preguntas.Count(p =>
                request.Respuestas.TryGetValue(p.ID, out var resp) &&
                p.Respuesta.Trim().Equals(resp.Trim(), StringComparison.OrdinalIgnoreCase)
            );

            if (correctas <= preguntas.Count / 2)
                return Unauthorized(new { message = "Respuestas incorrectas." });

            usuario.SetContrasena(BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena));
            await _usuarioService.ActualizarUsuario(usuario);


            return Ok(new { message = "Contraseña actualizada correctamente." });
        }


        // --- Clase LoginResponse (Sin cambios) ---
        public class LoginResponse { public string Message { get; set; } public int UserId { get; set; } public int Rol { get; set; } public string Nombre { get; set; } public string Token { get; set; } }
    }
}