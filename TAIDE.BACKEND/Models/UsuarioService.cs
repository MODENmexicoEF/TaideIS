using TuProyecto.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Necesario para Task<>
using TuProyecto.Data; // Asegúrate de que este using sea correcto para ApplicationDbContext

namespace TuProyecto.Services // Espacio de nombres correcto para UsuarioService
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _dbContext;

        public UsuarioService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Usuario> ObtenerUsuarioPorCorreo(string correo)
        {

            return await _dbContext.Usuarios
                .Include(u => u.PreguntasSeguridad) // 🔁 esto es importante
                .FirstOrDefaultAsync(u => u.Correo.ToLower() == correo.ToLower());
            // Considera usar Include si necesitas cargar relaciones aquí, ej: PreguntasSeguridad
            // return await _dbContext.Usuarios.Include(u => u.PreguntasSeguridad).FirstOrDefaultAsync(u => u.Correo == correo);
        }

        // --- Métodos Crear... (Sin cambios necesarios aquí) ---
        public async Task<PM> CrearPM(PM pm)
        {
            _dbContext.Usuarios.Add(pm);
            await _dbContext.SaveChangesAsync();
            return pm;
        }
        public async Task ActualizarUltimaActividad(int usuarioId)
        {
            var usuario = await _dbContext.Usuarios.FindAsync(usuarioId);
            if (usuario != null)
            {
                usuario.UltimaActividad = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Familiar> CrearFamiliar(Familiar familiar)
        {
            _dbContext.Usuarios.Add(familiar);
            await _dbContext.SaveChangesAsync();
            return familiar;
        }

        public async Task<Paciente> CrearPaciente(Paciente paciente)
        {
            _dbContext.Usuarios.Add(paciente);
            await _dbContext.SaveChangesAsync();
            return paciente;
        }

        public async Task<SUDO> CrearSUDO(SUDO sudo)
        {
            _dbContext.Usuarios.Add(sudo);
            await _dbContext.SaveChangesAsync();
            return sudo;
        }

        // --- Método CambiarRolUsuario (CON CORRECCIÓN) ---
        public async Task<bool> CambiarRolUsuario(int usuarioId, Rol nuevoRol)
        {
            var usuario = await _dbContext.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return false; // Usuario no encontrado
            }

            var currentRol = usuario.TipoUsuario;

            // Si el rol ya es el deseado, no hacer nada (o devolver error/false si prefieres)
            if (currentRol == nuevoRol)
            {
                return true; // O false, dependiendo de tu lógica (¿Es un éxito no hacer nada?)
            }

            // --- Lógica especial para convertir Familiar a PM ---
            if (currentRol == Rol.Familiar && nuevoRol == Rol.PM)
            {
                // Es mejor obtener el 'familiar' directamente con sus propiedades
                var familiar = await _dbContext.Familiares // Busca directamente en el DbSet de Familiares si lo tienes
                                                           // O usa OfType si solo tienes Usuarios DbSet:
                                                           // .Usuarios.OfType<Familiar>()
                                       .FirstOrDefaultAsync(f => f.ID == usuarioId);

                if (familiar != null)
                {
                    // Crear un nuevo PM, pasando propiedades relevantes de Familiar, INCLUYENDO Ap1 y Ap2
                    // **ASUME que el constructor de PM ahora acepta 7 argumentos**
                    var nuevoPM = new PM(
                        familiar.NombreUsuario,
                        familiar.Contrasena, // Considera si la contraseña debe manejarse de forma especial en cambio de rol
                        familiar.Correo,
                        null, // NumeroColegiado - ¿Debería ser null o pedirlo?
                        null, // Especialidad - ¿Debería ser null o pedirlo?
                        familiar.Ap1, // <<< PASAR Ap1 del familiar
                        familiar.Ap2  // <<< PASAR Ap2 del familiar
                    );

                    // Considerar copiar otras propiedades base si es necesario (PreguntasSeguridad?)
                    // La relación de PreguntasSeguridad podría mantenerse por UsuarioID, pero verifica.
                    // Si las copias explícitamente, ten cuidado con el tracking de EF Core.
                    // nuevoPM.PreguntasSeguridad = familiar.PreguntasSeguridad;

                    _dbContext.Usuarios.Remove(familiar); // Eliminar el registro antiguo
                    await _dbContext.SaveChangesAsync(); // Guardar la eliminación antes de añadir para evitar conflictos de PK
                    _dbContext.Usuarios.Add(nuevoPM); // Añadir el nuevo registro PM
                }
                else
                {
                    return false; // No se encontró al familiar para convertir (error de datos inconsistentes?)
                }
            }
            // --- Lógica similar para otras conversiones de roles si es necesario ---
            // else if (currentRol == Rol.X && nuevoRol == Rol.Y) { ... }

            // --- Caso general: Actualizar solo el rol si no hay conversión especial ---
            else
            {
                usuario.TipoUsuario = nuevoRol;
                // Aquí podrías añadir lógica para limpiar propiedades del rol antiguo si fuera necesario
                // Ejemplo: si de PM pasa a Familiar, poner NumeroColegiado y Especialidad a null
                if (usuario is PM pm && nuevoRol != Rol.PM)
                {
                    pm.NumeroColegiado = null;
                    pm.Especialidad = null;
                }
                // Añadir lógica similar para otros roles...
            }

            // Guardar los cambios (ya sea la actualización simple o la adición del nuevo PM)
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task GuardarPreguntasSeguridad(List<PreguntaSeguridad> preguntas)
        {
            _dbContext.PreguntasSeguridad.AddRange(preguntas);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ActualizarUsuario(Usuario usuario)
        {
            _dbContext.Usuarios.Update(usuario);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ObtenerListaDeUsuarios()
        {
            // Considera usar AsNoTracking() si solo vas a leer los datos y no modificarlos
            // return await _dbContext.Usuarios.AsNoTracking().ToListAsync();
            return await _dbContext.Usuarios.ToListAsync();
        }
        public async Task<List<UsuarioDTO>> ObtenerUsuariosConEstadoEnLinea()
        {
            var umbral = DateTime.UtcNow.AddMinutes(-5);

            var usuarios = await _dbContext.Usuarios
                .Select(u => new UsuarioDTO
                {
                    ID = u.ID,
                    NombreUsuario = u.NombreUsuario,
                    TipoUsuario = (int)u.TipoUsuario,
                    Correo = u.Correo,
                    EstaEnLinea = u.UltimaActividad.HasValue && u.UltimaActividad.Value >= umbral
                })
                .ToListAsync();

            return usuarios;
        }

        // DTO auxiliar
        public class UsuarioDTO
        {
            public int ID { get; set; }
            public string NombreUsuario { get; set; }
            public int TipoUsuario { get; set; }
            public string Correo { get; set; }
            public bool EstaEnLinea { get; set; }
        }

        // --- Método Genérico Sugerido (Opcional) ---
        // Si decides usarlo en el AuthController, añádelo aquí:
        /*
        public async Task<Usuario> CrearUsuarioGenerico(Usuario usuario)
        {
            // Aquí podrías añadir validaciones adicionales si fueran necesarias antes de guardar
            // Ejemplo: Verificar si el correo ya existe (aunque la BD ya lo hace con UNIQUE)
            // var existe = await ObtenerUsuarioPorCorreo(usuario.Correo);
            // if (existe != null) {
            //     // Lanzar una excepción o devolver null para indicar el error
            //     throw new InvalidOperationException("El correo ya está registrado.");
            //     // O return null; (pero el controller debería manejarlo)
            // }

            _dbContext.Usuarios.Add(usuario);
            await _dbContext.SaveChangesAsync();
            return usuario; // Devuelve la entidad guardada (con el ID asignado)
        }
        */
        public async Task<List<Paciente>> ObtenerPacientes(int? pmId = null)
        {
            var query = _dbContext.Pacientes.AsQueryable();

            // Cuando tengas la relación PM–Paciente, descomenta:
            // if (pmId.HasValue)
            //     query = query.Where(p => p.PMId == pmId.Value);

            return await query.ToListAsync();
        }

        // 2) Actualizar el estado de un paciente
        public async Task<bool> ActualizarEstadoPaciente(int pacienteId, string nuevoEstado)
        {
            var paciente = await _dbContext.Pacientes.FindAsync(pacienteId);
            if (paciente == null) return false;

            paciente.Estado = nuevoEstado;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}