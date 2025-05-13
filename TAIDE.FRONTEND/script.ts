// ----- INTERFACES (Alineadas con PascalCase del Backend para RESPUESTAS JSON) -----
// Estas interfaces se usan para leer las RESPUESTAS JSON del servidor
/*
interface AuthResponse {
    Message: string;
    UserId?: number;
    Rol?: number;
    TipoUsuario?: number;
    Nombre?: string;
    NombreUsuario?: string;
    Token?: string;
    token?: string; //  Añade esto
    Error?: string;
}
*/
interface AuthResponse {
  message: string;
  userId?: number;
  rol?: number;
  nombre?: string;
  token?: string;
  error?: string;
}

interface RegisterResponse {
    Message: string;
    UsuarioId?: number;
    TipoUsuario?: number;
    Error?: string;
}
interface SolicitudPendiente {
  id: number;
  paciente: string;
  familiar: string;
  fechaSolicitud: string;
}

// Asegúrate que el casing aquí coincida con el JSON devuelto por /api/auth/usuarios
interface User {
    ID: number;            // Asumiendo PascalCase desde C#
    NombreUsuario: string; // Asumiendo PascalCase
    TipoUsuario: number;   // Asumiendo PascalCase
    EnLinea: boolean;      // Asumiendo PascalCase
}
interface FamiliarVinculado {
  id: number;
  nombre: string;
  correo: string;
}

async function cargarFamiliaresVinculados(): Promise<void> {
  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch("https://localhost:7274/api/pacientes/familiares", {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    if (res.status === 403) return;
    if (!res.ok) throw new Error("Error al obtener familiares vinculados");

    const familiares: FamiliarVinculado[] = await res.json();
    const tbody = document.querySelector("#tabla-familiares-vinculados tbody");
    if (!tbody) return;

    if (familiares.length === 0) {
      tbody.innerHTML = `<tr><td colspan="3">No tienes familiares vinculados.</td></tr>`;
      return;
    }

    tbody.innerHTML = familiares.map(f => `
      <tr>
        <td>${f.id}</td>
        <td>${f.nombre}</td>
        <td>${f.correo}</td>
      </tr>
    `).join("");

  } catch (error) {
    console.error("Error al cargar familiares vinculados:", error);
    const tbody = document.querySelector("#tabla-familiares-vinculados tbody");
    if (tbody) {
      tbody.innerHTML = `<tr><td colspan="3">Error al cargar datos.</td></tr>`;
    }
  }
}
async function cargarVinculosPacienteFamiliar() {
  const token = localStorage.getItem("token");
  if (!token) return alert("Token no encontrado.");

  try {
    const res = await fetch("https://localhost:7274/api/pm/vinculos", {
      headers: { Authorization: `Bearer ${token}` },
    });

    const vinculos = await res.json();
    const tbody = document.querySelector("#tabla-vinculos-familiares tbody");
    if (!tbody) return;

    if (vinculos.length === 0) {
      tbody.innerHTML = "<tr><td colspan='3'>Sin vínculos actualmente.</td></tr>";
      return;
    }

    tbody.innerHTML = vinculos.map((v: any) => `
      <tr>
        <td>${v.Paciente}</td>
        <td>${v.Familiar}</td>
        <td>
          <button onclick="quitarVinculo(${v.FamiliarId}, ${v.PacienteId})">❌ Quitar</button>
        </td>
      </tr>
    `).join("");
  } catch (e: any) {
    alert("Error al cargar vínculos.");
    console.error(e);
  }
}

async function quitarVinculo(familiarId: number, pacienteId: number) {
  const token = localStorage.getItem("token");
  if (!token) return alert("Token no encontrado.");

  if (!confirm("¿Deseas quitar el acceso del familiar?")) return;

  try {
    const res = await fetch(`https://localhost:7274/api/pm/familiares/${familiarId}/paciente/${pacienteId}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });

    const data = await res.json();
    if (!res.ok) throw new Error(data.message || "Error al quitar acceso.");

    alert(data.message || "Acceso retirado correctamente.");
    cargarVinculosPacienteFamiliar();
  } catch (e: any) {
    alert(e.message || "Error al eliminar vínculo.");
  }
}
async function login(email: string, password: string): Promise<void> {
    const errorMessageElement = document.getElementById('error-message');
    if (errorMessageElement) errorMessageElement.textContent = '';

    try {
        const response = await fetch('https://localhost:7274/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ correo: email, contrasena: password })
        });

        if (!response.ok) {
            const errorText = await response.text();
            const errorData = errorText ? JSON.parse(errorText) : null;
            const mensaje = errorData?.message || `Error ${response.status}`;
            if (errorMessageElement) errorMessageElement.textContent = mensaje;
            return;
        }

        const data = await response.json(); // ya devuelve { token, rol, nombre }

        alert(data.message || "Iniciaste sesión correctamente.");

        // Guardar token
        if (data.token) {
            localStorage.setItem("token", data.token);
        } else {
            alert("Error: token no recibido.");
            return;
        }

        // Guardar nombre
        if (data.nombre && data.ap1) {
            const nombreCompleto = `${data.nombre} ${data.ap1}`;
            localStorage.setItem("nombre_usuario", nombreCompleto);
        }


        // Guardar rol
        if (typeof data.rol !== 'undefined') {
            localStorage.setItem("rol", data.rol.toString());
        }

        // Selección del panel según rol
        switch (data.rol) {
            case 0: showPacienteDashboard(); break;
            case 1: showPmDashboard(); break;
            case 2: showFamiliarDashboard(); break;
            case 3: showSudoDashboard(); break;
            default:
                alert("Tipo de usuario no reconocido.");
                showLoginForm();
        }

    } catch (error) {
        console.error("Error en el login:", error);
        if (errorMessageElement) {
            errorMessageElement.textContent = 'No se pudo conectar al servidor.';
        }
    }
}



function renderSolicitudesPendientes(solicitudes: SolicitudPendiente[]): void {
  const cont = document.querySelector("#solicitudes-container tbody");
  if (!cont) return;

  if (solicitudes.length === 0) {
    cont.innerHTML = "<tr><td colspan='5'>No hay solicitudes pendientes.</td></tr>";
    return;
  }

  cont.innerHTML = solicitudes.map(s => `
    <tr>
      <td>${s.id}</td>
      <td>${s.paciente}</td>
      <td>${s.familiar}</td>
      <td>${new Date(s.fechaSolicitud).toLocaleDateString()}</td>
      <td><button data-id="${s.id}" class="btn-aprobar-solicitud">Aprobar</button></td>
    </tr>
  `).join("");

  document.querySelectorAll(".btn-aprobar-solicitud").forEach(btn => {
    btn.addEventListener("click", async () => {
      const id = (btn as HTMLButtonElement).dataset.id!;
      await aprobarSolicitud(parseInt(id));
    });
  });
}
async function cargarSolicitudesPendientes(): Promise<void> {
  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch("https://localhost:7274/api/pm/solicitudes", {
      headers: { Authorization: `Bearer ${token}` }
    });

    if (res.status === 403) {
      // Usuario sin rol PM: ignorar silenciosamente
      return;
    }

    if (!res.ok) {
      throw new Error("Error al cargar solicitudes");
    }

    const data: SolicitudPendiente[] = await res.json();
    renderSolicitudesPendientes(data);

  } catch (error) {
    // Solo loguea si no es un 403 silencioso
    console.error("Error cargando solicitudes pendientes:", error);
    const tbody = document.querySelector("#solicitudes-container tbody");
    if (tbody) {
      tbody.innerHTML = `<tr><td colspan="5">Error al cargar solicitudes.</td></tr>`;
    }
  }
}

async function aprobarSolicitud(id: number) {
  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch(`https://localhost:7274/api/pm/solicitudes/${id}/aprobar`, {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`
      }
    });
    if (!res.ok) throw new Error("No se pudo aprobar solicitud");

    alert("Solicitud aprobada correctamente");
    cargarSolicitudesPendientes();
    cargarPacientesPM();
  } catch (error) {
    alert("Error al aprobar la solicitud");
    console.error(error);
  }
}



// ----- FUNCIÓN REGISTRO (Usando FormData) -----
// Acepta el FormData directamente
async function registerUser(formData: FormData): Promise<void> {
    const registerErrorMessageElement = document.getElementById('register-error-message');
    const registerSuccessMessageElement = document.getElementById('register-success-message');

    if (registerErrorMessageElement) registerErrorMessageElement.textContent = '';
    if (registerSuccessMessageElement) registerSuccessMessageElement.style.display = 'none';

    try {
        const response = await fetch('https://localhost:7274/api/auth/registrar', { // TU URL BACKEND
            method: 'POST',
            // NO establecer Content-Type manualmente con FormData
            body: formData, // Enviar el FormData construido
        });

        const data: RegisterResponse = await response.json(); // Leer respuesta JSON

        if (!response.ok) {
             // Intenta usar el mensaje de error del backend si existe
            throw new Error(data.Error || data.Message || `Error ${response.status}`);
        }

        console.log('Registro exitoso:', data);
        if (registerSuccessMessageElement) {
            registerSuccessMessageElement.textContent = data.Message;
            registerSuccessMessageElement.style.display = 'block';
        } else {
            alert(data.Message);
        }

        // Mostrar login después de 2 segundos
        setTimeout(() => {
            showLoginForm(); // Llama a la función que muestra el login y oculta otros
        }, 2000);

    } catch (error) {
        console.error('Error al registrar usuario:', error);
        const message = (error instanceof Error) ? error.message : 'Error desconocido';
        if (registerErrorMessageElement) {
            registerErrorMessageElement.textContent = message;
        } else {
            alert(message);
        }
    }
}

// ----- MANEJADOR DE ENVÍO DE LOGIN -----
function handleLoginSubmit(event: Event): void {
    event.preventDefault(); // Prevenir recarga de página
    const emailInput = document.getElementById('email') as HTMLInputElement;
    const passwordInput = document.getElementById('password') as HTMLInputElement;

    if (emailInput && passwordInput) {
        login(emailInput.value, passwordInput.value);
    } else {
        console.error('No se encontraron los elementos de correo o contraseña.');
        alert('Error interno: No se encontraron los campos de inicio de sesión.');
    }
}
function mostrarErrorUsuarios(mensaje: string) {
    const lista = document.getElementById('user-list')!;
    lista.innerHTML = ''; // Limpiamos lista anterior
    const errorItem = document.createElement('li');
    errorItem.style.color = 'red';
    errorItem.textContent = mensaje;
    lista.appendChild(errorItem);
}
function showPmDashboard(): void {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pacienteDashboard = document.getElementById('paciente-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');

    if (loginContainer) loginContainer.style.display = 'none';
    if (registerContainer) registerContainer.style.display = 'none';
    if (pacienteDashboard) pacienteDashboard.style.display = 'none';
    if (familiarDashboard) familiarDashboard.style.display = 'none';
    if (sudoDashboard) sudoDashboard.style.display = 'none';
    if (pmDashboard) pmDashboard.style.display = 'flex';

    const nombre = localStorage.getItem("nombre_usuario") || "Profesional";
    const nameElements = document.querySelectorAll("#pm-welcome-name, #pm-welcome-name-main");
    nameElements.forEach(e => e.textContent = nombre);
    cargarPacientesPM();
    cargarSolicitudesPendientes();
}
function getUserListContainer(): HTMLElement {
  const el = document.getElementById("user-list-container");
  if (!el) throw new Error("Falta #user-list-container en el DOM");
  return el;
}

function normalizaUsuario(u: any) {
  return {
    ID:            u.ID            ?? u.id            ?? 0,
    nombre:        u.NombreUsuario ?? u.Nombre        ?? u.nombre ?? "",
    correo:        u.Correo        ?? u.correo        ?? "",
    estaEnLinea:   u.EstaEnLinea   ?? u.estaEnLinea   ?? false,
    tipoUsuario:   u.TipoUsuario   ?? u.tipoUsuario   ?? null
  };
}

function showFamiliarDashboard(): void {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pacienteDashboard = document.getElementById('paciente-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');

    if (loginContainer) loginContainer.style.display = 'none';
    if (registerContainer) registerContainer.style.display = 'none';
    if (pacienteDashboard) pacienteDashboard.style.display = 'none';
    if (pmDashboard) pmDashboard.style.display = 'none';
    if (sudoDashboard) sudoDashboard.style.display = 'none';
    if (familiarDashboard) familiarDashboard.style.display = 'flex';

    const nombre = localStorage.getItem("nombre_usuario") || "Familiar";
    const nameElements = document.querySelectorAll("#familiar-welcome-name, #familiar-welcome-name-main");
    nameElements.forEach(e => e.textContent = nombre);
    cargarPacientesVinculados();
}
async function enviarSolicitudFamiliar(event: Event): Promise<void> {
    event.preventDefault();
    const pacienteIdInput = document.getElementById('paciente-id-input') as HTMLInputElement;
    const mensajeDiv = document.getElementById('familiar-solicitud-mensaje');

    if (!pacienteIdInput || !mensajeDiv) return;

    const pacienteId = parseInt(pacienteIdInput.value.trim(), 10);
    if (isNaN(pacienteId) || pacienteId <= 0) {
        mensajeDiv.textContent = "Ingrese un ID válido de paciente.";
        mensajeDiv.style.color = "red";
        return;
    }

    const token = localStorage.getItem('token');
    if (!token) {
        mensajeDiv.textContent = "Sesión expirada. Inicie sesión nuevamente.";
        mensajeDiv.style.color = "red";
        return;
    }

    try {
        const res = await fetch('https://localhost:7274/api/familiar/solicitudes', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                FamiliarId: 0,   // El backend lo sobreescribe con el ID del token, si no, tendrás que ajustar aquí.
                PacienteId: pacienteId
            })
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || "Error al enviar solicitud");

        mensajeDiv.textContent = data.Message || "Solicitud enviada.";
        mensajeDiv.style.color = "green";
        pacienteIdInput.value = ""; // Limpiar campo
    } catch (error: any) {
        mensajeDiv.textContent = error.message || "Error al enviar la solicitud.";
        mensajeDiv.style.color = "red";
    }
}

// ----- MANEJADOR DE ENVÍO DE REGISTRO (Construye FormData desde el form) -----
function handleRegisterSubmit(event: Event): void {
    event.preventDefault();

    const nombreInput = document.getElementById('nombre-reg') as HTMLInputElement;
    const ap1Input = document.getElementById('ap1-reg') as HTMLInputElement;
    const ap2Input = document.getElementById('ap2-reg') as HTMLInputElement;
    const correoInput = document.getElementById('correo-reg') as HTMLInputElement;
    const contrasenaInput = document.getElementById('contrasena-reg') as HTMLInputElement;
    const rolRadioSelected = document.querySelector('#register-form input[name="RolInput"]:checked') as HTMLInputElement;

    const numeroColegiadoInput = document.getElementById('numero-colegiado-reg') as HTMLInputElement;
    const especialidadInput = document.getElementById('especialidad-reg') as HTMLInputElement;

    const preguntasInputs = document.querySelectorAll<HTMLInputElement>('input[name="Preguntas[]"]');
    const respuestasInputs = document.querySelectorAll<HTMLInputElement>('input[name="Respuestas[]"]');

    // Validación básica
    if (
        !nombreInput.value.trim() ||
        !ap1Input.value.trim() ||
        !correoInput.value.trim() ||
        !contrasenaInput.value.trim() ||
        !rolRadioSelected
    ) {
        alert('Por favor, complete todos los campos requeridos.');
        return;
    }

    // Validación para PM
    const selectedTipoUsuario = rolRadioSelected.value;
    if (selectedTipoUsuario === "1") {
        if (
            !numeroColegiadoInput.value.trim() ||
            !especialidadInput.value.trim()
        ) {
            alert('Por favor, complete número de colegiado y especialidad.');
            return;
        }
    }

    // Validación de preguntas de seguridad
    if (preguntasInputs.length === 0 || respuestasInputs.length === 0) {
        alert("Debes agregar al menos una pregunta y una respuesta.");
        return;
    }

    const formData = new FormData();
    formData.append("NombreUsuario", nombreInput.value);
    formData.append("Ap1", ap1Input.value);
    formData.append("Ap2", ap2Input?.value || "");
    formData.append("Correo", correoInput.value);
    formData.append("Contrasena", contrasenaInput.value);
    formData.append("RolInput", selectedTipoUsuario);

    if (selectedTipoUsuario === "1") {
        formData.append("NumeroColegiado", numeroColegiadoInput.value);
        formData.append("Especialidad", especialidadInput.value);
    }

    preguntasInputs.forEach(input => {
        if (input.value.trim()) {
            formData.append("Preguntas[]", input.value.trim());
        }
    });

    respuestasInputs.forEach(input => {
        if (input.value.trim()) {
            formData.append("Respuestas[]", input.value.trim());
        
        }
    });

    registerUser(formData);
}

// ----- Función para cargar usuarios activos -----
async function cargarUsuariosActivos() {
  const cont = getUserListContainer();
  cont.innerHTML = "<p>Cargando usuarios activos…</p>";

  try {
    const token = localStorage.getItem("token");
    const res   = await fetch("https://localhost:7274/api/sudo/usuarios/activos", {
      headers: { "Authorization": `Bearer ${token}` }
    });
    if (!res.ok) throw new Error("Error al obtener usuarios activos");
    const data = await res.json();
    renderUsuarios(data.map(normalizaUsuario), true);
  } catch (e) {
    cont.innerHTML = "<p class='error-message'>No se pudo cargar la lista.</p>";
    console.error(e);
  }
}

// ----- Función para cargar todos los usuarios -----
async function cargarTodosLosUsuarios() {
  const cont = getUserListContainer();
  cont.innerHTML = "<p>Cargando todos los usuarios…</p>";

  try {
    const token = localStorage.getItem("token");
    const res   = await fetch("https://localhost:7274/api/sudo/usuarios", {
      headers: { "Authorization": `Bearer ${token}` }
    });
    if (!res.ok) throw new Error("Error al obtener usuarios");
    const data = await res.json();
    renderUsuarios(data.map(normalizaUsuario), false);
  } catch (e) {
    cont.innerHTML = "<p class='error-message'>No se pudo cargar la lista.</p>";
    console.error(e);
  }
}

function fmtUptime(totalSegundos: number): string {
  const d = Math.floor(totalSegundos / 86400);
  const h = Math.floor((totalSegundos % 86400) / 3600);
  const m = Math.floor((totalSegundos % 3600) / 60);
  return `${d}d ${h}h ${m}m`;
}



// ----- Función para renderizar los usuarios -----
function mostrarUsuarios(usuarios: any[], mostrarEstado: boolean) {
    const lista = document.getElementById('user-list')!;
    lista.innerHTML = '';

    usuarios.forEach(usuario => {
        const li = document.createElement('li');
        li.textContent = `${usuario.nombre} - ${usuario.correo}`;

        if (mostrarEstado) {
            const estado = document.createElement('span');
            estado.textContent = usuario.estaEnLinea ? ' En línea' : ' Desconectado';
            li.appendChild(estado);
        }

        lista.appendChild(li);
    });
}
/* ----------- LISTADO DE USUARIOS ----------- */

// (por ejemplo justo antes de renderUsuarios)
function rolTexto(tipo: number | null): string {
  switch (tipo) {
    case 0: return "Paciente";
    case 1: return "PM";
    case 2: return "Familiar";
    case 3: return "SUDO";
    default: return "¿?";
  }
}

/* ----------- LISTADO DE USUARIOS ----------- */
function renderUsuarios(users: any[], mostrarEstado: boolean): void {
  const cont = getUserListContainer();

  const filas = users
    .map(u => {
const estado = mostrarEstado
  ? `<span class="${u.estaEnLinea ? "online" : "offline"}">
       ${u.estaEnLinea ? "S\u00ED" : "No"}
     </span>`
  : "";

return `
  <tr>
    <td>${u.ID}</td>
    <td>${u.nombre}</td>
    <td>${rolTexto(u.tipoUsuario)}</td>
    <td>${estado}</td>
    <td><button class="del-btn" data-id="${u.ID}">\u{1F5D1}</button></td>
  </tr>`;
    })
    .join("");

  cont.innerHTML = `
    <table class="user-table">
      <thead>
  <tr>
        <th>ID</th>
        <th>Nombre Usuario</th>
        <th>Rol</th>
        <th>En L\u00EDnea</th>
        <th>Acciones</th>
      </tr>
      </thead>
      <tbody>
        ${filas}
      </tbody>
    </table>`;
}

/* ---------- MONITOR DE RECURSOS ---------- */
async function deleteUser(id: number): Promise<void> {
  if (!confirm("¿Seguro que quieres eliminar al usuario " + id + "?")) return;

  const token = localStorage.getItem("token");
  if (!token) return alert("No autenticado.");

  try {
    const res = await fetch(`https://localhost:7274/api/sudo/usuarios/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` }
    });

    const data = await res.json();
    if (!res.ok) throw new Error(data.message || `Error ${res.status}`);

    alert(data.message || "Usuario eliminado.");
    fetchUserList(); // recargar lista
  } catch (e: any) {
    alert(e.message || "Error al eliminar usuario.");
    console.error(e);
  }
}

// delegación de eventos: un solo listener para toda la tabla

let monitorInterval: number | undefined;

interface RecursosResponse {
  CpuPercent: number;
  MemoriaUsadaMB: number;
  MemoriaTotalMB: number;
  UptimeSeconds: number;
}
interface PacienteInfo {
  id: number;
  nombreUsuario: string;
  estado: string | null;
}
interface PacienteApi {
  ID: number;
  NombreUsuario: string;
  Estado: string | null;
}
async function actualizarEstadoPaciente(id: number, estado: string) {
  if (!estado.trim()) return alert('Debes escribir un estado');
  const token = localStorage.getItem('token');
  if (!token) return alert('Sesión expirada');

  try {
    const res = await fetch(
      `https://localhost:7274/api/pm/pacientes/${id}/estado`,
      {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ estado }),
      }
    );

    if (!res.ok) throw new Error(`Error ${res.status}`);
    alert('Estado actualizado');
    cargarPacientesPM(); // refresca la tabla
  } catch (e: any) {
    alert(e.message || 'Error al actualizar estado');
  }
}


/* ---- NUEVO / Mueve aquí si ya la tenías más abajo ---- */
function renderPacientesParaPM(pacientes: PacienteInfo[]): void {
  const cont = document.getElementById("pacientes-container");
  if (!cont) return;
  if (pacientes.length === 0) {
  cont.innerHTML = "<tr><td colspan='4'>No hay pacientes registrados.</td></tr>";
  return;
}

  cont.innerHTML = pacientes
    .map(
      p => `
        <tr>
          <td>${p.id}</td>
          <td>${p.nombreUsuario}</td>
          <td>${p.estado ?? "-"}</td>
          <td>
            <input type="text" id="estado-${p.id}" placeholder="Nuevo estado">
            <button class="btn-guardar-estado" data-id="${p.id}">Guardar</button>
          </td>
        </tr>`
    )
    .join("");

  /* ⬇️  Un solo listener delegado para todos los botones */
  cont.querySelectorAll<HTMLButtonElement>(".btn-guardar-estado")
      .forEach(btn =>
        btn.addEventListener("click", () => {
          const id = Number(btn.dataset.id);
          const input = document.getElementById(`estado-${id}`) as HTMLInputElement;
          actualizarEstadoPaciente(id, input.value);
        }));
}

async function cargarPacientesPM() {
  const token = localStorage.getItem("token");
  if (!token) return alert("Sesión expirada.");

  const res = await fetch("https://localhost:7274/api/pm/pacientes", {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!res.ok) return alert("Error al cargar pacientes");

  const apiData: PacienteApi[] = await res.json();

  const pacientes: PacienteInfo[] = apiData.map(p => ({
    id: p.ID,
    nombreUsuario: p.NombreUsuario,
    estado: p.Estado
  }));

  renderPacientesParaPM(pacientes);
}
interface PacienteVinculado {
  id: number;
  nombre: string;
  estado: string | null;
}

async function cargarPacientesVinculados(): Promise<void> {
  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch("https://localhost:7274/api/familiar/pacientes", {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    if (!res.ok) throw new Error("Error al obtener pacientes vinculados");

    const pacientes: PacienteVinculado[] = await res.json();

    const tbody = document.querySelector("#tabla-pacientes-vinculados tbody");
    if (!tbody) return;

    if (pacientes.length === 0) {
      tbody.innerHTML = "<tr><td colspan='3'>No tienes pacientes vinculados.</td></tr>";
      return;
    }

    tbody.innerHTML = pacientes.map(p => `
      <tr>
        <td>${p.id}</td>
        <td>${p.nombre}</td>
        <td>${p.estado ?? "Sin estado"}</td>
      </tr>
    `).join("");

  } catch (error) {
    console.error("Error cargando pacientes vinculados:", error);
    const tbody = document.querySelector("#tabla-pacientes-vinculados tbody");
    if (tbody) {
      tbody.innerHTML = `<tr><td colspan='3'>Error al cargar pacientes.</td></tr>`;
    }
  }
}

async function actualizarRecursos(): Promise<void> {
  // 1. Recuperar el token UNA sola vez, fuera del try
  const token = localStorage.getItem("token");
  if (!token) return;               // 2. Si no hay token, salimos

  try {
    // 3. Hacer la petición protegida
    const res = await fetch("https://localhost:7274/api/sudo/recursos", {
      headers: { Authorization: `Bearer ${token}` }
    });
    if (!res.ok) throw new Error(`Error ${res.status}`);

    const data = (await res.json()) as RecursosResponse;

    document.getElementById("cpu-porc")!.textContent =
      data.CpuPercent.toFixed(1) + " %";

    document.getElementById("mem-uso")!.textContent =
      data.MemoriaUsadaMB.toFixed(0);

    document.getElementById("mem-tot")!.textContent =
      data.MemoriaTotalMB.toFixed(0);

    document.getElementById("uptime")!.textContent =
      formateaSegundos(data.UptimeSeconds);
  } catch (e) {
    console.error("Error al actualizar recursos:", e);
  }
}
function formateaSegundos(seg: number): string {
  const h = Math.floor(seg / 3600);
  const m = Math.floor((seg % 3600) / 60);
  const s = Math.floor(seg % 60);
  return `${h} h ${m} m ${s} s`;
}



// ----- OBTENER LISTA DE USUARIOS -----
async function fetchUserList(): Promise<void> {
    const userListContainer = document.getElementById('user-list-container');
    const token = localStorage.getItem('token');
    if (!userListContainer) return;

    try {
        
        if (!token || token === '1911') { // No intentar si no hay token real
            throw new Error('No autenticado. Inicie sesión.');
        }

        const response = await fetch('https://localhost:7274/api/sudo/usuarios', { // TU URL BACKEND
            method: 'GET',
            headers: {
                'Content-Type': 'application/json', // La respuesta sí será JSON
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            let errorMsg = `Error ${response.status}`;
            try {
                const errorData = await response.json();
                errorMsg = errorData.message || errorData.title || errorMsg;
            } catch (e) { /* Ignora si el cuerpo de error no es JSON */ }
            throw new Error(errorMsg);
        }

        const users: User[] = await response.json(); // Espera array de User (con PascalCase)

        // Generar HTML de la tabla
        let userTableHTML = `<table class="user-table">
                                <thead>
                                  <tr>
                                    <th>ID</th>
                                    <th>Nombre Usuario</th>
                                    <th>Rol</th>
                                    <th>En Línea</th>
                                  </tr>
                                </thead>
                                <tbody>`;

        users.forEach(user => {
            let rolNombre: string;
            switch (user.TipoUsuario) {
                case 0: rolNombre = 'Paciente'; break;
                case 1: rolNombre = 'PM'; break;
                case 2: rolNombre = 'Familiar'; break;
                case 3: rolNombre = 'SUDO'; break;
                default: rolNombre = 'Desconocido'; break;
            }
            userTableHTML += `<tr>
                                <td>${user.ID}</td>
                                <td>${user.NombreUsuario}</td>
                                <td>${rolNombre}</td>
                                <td><span class="${user.EnLinea ? 'online' : 'offline'}">${user.EnLinea ? 'Sí' : 'No'}</span></td>
                              </tr>`;
        });
        userTableHTML += '</tbody></table>';
        userListContainer.innerHTML = userTableHTML;

    } catch (error) {
        console.error('Error al cargar la lista de usuarios:', error);
        const message = (error instanceof Error) ? error.message : 'Error desconocido';
        // Mostrar error solo si el contenedor existe
        if (userListContainer) {
            userListContainer.innerHTML = `<p class="error-message">Error al cargar la lista de usuarios: ${message}</p>`;
        }
    }
}

// ----- CAMBIAR ROL -----
// Verifica si este endpoint en tu backend espera [FromBody] (JSON) o [FromForm]
// Este código asume [FromBody] (JSON) como estaba antes.
async function changeUserRole(usuarioId: number, nuevoRolNombre: string): Promise<void> {
    // El backend espera el nombre del rol ("PM", "SUDO", etc.) según CambiarRolRequest.cs
    let rolParaEnviar = nuevoRolNombre.toUpperCase();

    try {
        const token = localStorage.getItem('token');
        if (!token || token === '1911') throw new Error('No autenticado.');

    const response = await fetch('https://localhost:7274/api/sudo/cambiar-rol', {  // ← AQUÍ
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ UsuarioId: usuarioId, NuevoRol: rolParaEnviar })
    });

        // Intenta leer la respuesta como JSON, incluso si no es 2xx OK
        const data: any = await response.json();

        if (!response.ok) {
            // Usa el mensaje del JSON si está disponible
            throw new Error(data.message || data.title || `Error ${response.status}`);
        }

        alert(data.message || 'Rol cambiado con éxito');
        fetchUserList(); // Recargar lista para ver el cambio

    } catch (error) {
        console.error('Error al cambiar el rol:', error);
        const message = (error instanceof Error) ? error.message : 'Error desconocido';
        alert(message);
    }
}


// ----- MANEJADOR CAMBIO DE ROL -----
// Este listener se añade más abajo en DOMContentLoaded no pegar
function handleChangeRoleSubmit(event: Event) {
     event.preventDefault();
     const usuarioIdInput = document.getElementById('sudo-usuarioId') as HTMLInputElement;
     const nuevoRolInput = document.getElementById('sudo-nuevoRol') as HTMLInputElement;
     if (usuarioIdInput && nuevoRolInput && usuarioIdInput.value && nuevoRolInput.value) {
         const userId = parseInt(usuarioIdInput.value, 10);
         const nuevoRol = nuevoRolInput.value;
         if (!isNaN(userId)) {
              changeUserRole(userId, nuevoRol); // Llamar a la función
              usuarioIdInput.value = ''; // Limpiar campos
              nuevoRolInput.value = '';
         } else {
              alert('El ID del usuario debe ser un número.');
         }
     } else {
         alert('Por favor, complete todos los campos para cambiar el rol.');
     }
}

// ----- FUNCIONES DE VISIBILIDAD -----
function showSudoDashboard(): void {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pmDashboard = document.getElementById('pm-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');

    if (loginContainer) loginContainer.style.display = 'none';
    if (registerContainer) registerContainer.style.display = 'none';
    if (pmDashboard) pmDashboard.style.display = 'none';
    if (familiarDashboard) familiarDashboard.style.display = 'none';
    if (sudoDashboard) sudoDashboard.style.display = 'flex'; // O 'block' según tu CSS

    fetchUserList(); // Cargar lista al mostrar dashboard
}

function showRegisterForm(): void {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');


    if (loginContainer) loginContainer.style.display = 'none';
    if (registerContainer) registerContainer.style.display = 'flex'; // O 'block'
    if (sudoDashboard) sudoDashboard.style.display = 'none';
    if (pmDashboard) pmDashboard.style.display = 'none';
    if (familiarDashboard) familiarDashboard.style.display = 'none';
}

function showLoginForm(): void {
hideAllSections();
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');


    if (loginContainer) loginContainer.style.display = 'flex'; // O 'block'
    if (registerContainer) registerContainer.style.display = 'none';
    if (sudoDashboard) sudoDashboard.style.display = 'none';
    if (pmDashboard) pmDashboard.style.display = 'none';
    if (familiarDashboard) familiarDashboard.style.display = 'none';
  const loginForm = document.getElementById("login-form") as HTMLFormElement;
  if (loginForm) loginForm.reset();
}

// ----- FUNCIÓN LOGOUT -----
function logout(): void {
    localStorage.removeItem('token');
    alert('Sesión cerrada.');

    // Ocultar todos los dashboards manualmente
    const dashboards = [
        'paciente-dashboard',
        'pm-dashboard',
        'familiar-dashboard',
        'sudo-dashboard'
    ];

    dashboards.forEach(id => {
        const element = document.getElementById(id);
        if (element) element.style.display = 'none';
    });

    showLoginForm(); // Muestra el login después de cerrar sesión
}

// ----- EVENT LISTENERS (Organizados y correctos) 1-----


// === Funciones para Recuperación de Contraseña ===
// === Funciones para Recuperación de Contraseña ===

function showRecoverForm() {
  hideAllSections();
  const recoverContainer = document.getElementById("recover-container");
  if (recoverContainer) recoverContainer.style.display = "block";
}

function handleRecoverSubmit(event: Event): void {
  event.preventDefault();
  const correoInput = document.getElementById("recover-email") as HTMLInputElement;
  const errorBox = document.getElementById("recover-error-message");
  if (!correoInput || !correoInput.value.trim()) {
    if (errorBox) errorBox.textContent = "Por favor introduce tu correo.";
    return;
  }
  errorBox!.textContent = "Cargando preguntas...";

  fetch("https://localhost:7274/api/auth/recuperar/obtener-preguntas", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ correo: correoInput.value.trim() })
  })
    .then(res => res.json())
    .then(data => {
      const questionsContainer = document.getElementById("questions-container")!;
      const questionsForm = document.getElementById("questions-form")!;
      questionsForm.innerHTML = "";

      if (!data.tienePreguntas) {
        errorBox!.textContent = "Consulta con un técnico para la recuperación de tu contraseña.";
        return;
      }

      errorBox!.textContent = "";
data.preguntas.forEach((p: any) => {
  const div = document.createElement("div");
  div.className = "form-group";
  div.innerHTML = `<label>${p.Pregunta}</label><input type="text" data-id="${p.ID}" required>`;
  questionsForm.appendChild(div);
});

      questionsContainer.style.display = "block";
    })
    .catch(err => {
      errorBox!.textContent = "Error al obtener preguntas.";
      console.error(err);
    });
}

function handleQuestionsSubmit(event: Event): void {
  event.preventDefault();
  const inputs = document.querySelectorAll("#questions-form input[data-id]");
  const respuestas: Record<number, string> = {};
  inputs.forEach(input => {
    const id = parseInt((input as HTMLInputElement).dataset.id || "0");
    const val = (input as HTMLInputElement).value.trim();
    if (id && val) respuestas[id] = val;
  });

  if (Object.keys(respuestas).length === 0) return alert("Responde al menos una pregunta.");

  (document.getElementById("new-password-container") as HTMLElement).style.display = "block";

  const newPasswordForm = document.getElementById("new-password-form")!;
  if (!newPasswordForm) {
  alert("Error interno: Formulario no encontrado.");
  return;
}
  newPasswordForm.onsubmit = function (e) {
    e.preventDefault();
}
    const nuevaPassInput = document.getElementById("new-password") as HTMLInputElement;
    const correo = (document.getElementById("recover-email") as HTMLInputElement).value;

    fetch("https://localhost:7274/api/auth/recuperar/cambiar-contrasena", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        correo,
        respuestas,
        nuevaContrasena: nuevaPassInput.value
      })
    })
      .then(res => res.json().then(data => ({ ok: res.ok, data })))
      .then(({ ok, data }) => {
        if (!ok) throw new Error(data.message || "Error inesperado");
        alert("Contraseña cambiada correctamente. Inicia sesión con la nueva contraseña.");
        showLoginForm();
      })
      .catch(err => {
        alert(err.message || "Error al cambiar la contraseña.");
      });
  };
async function buscarPacientesPorNombre(nombre: string): Promise<void> {
    const sugerenciasDiv = document.getElementById('pacientes-sugeridos');
    if (!sugerenciasDiv) return;

    sugerenciasDiv.innerHTML = "Buscando...";

    const token = localStorage.getItem('token');
    if (!token) {
        sugerenciasDiv.innerHTML = "<p>Sesión expirada.</p>";
        return;
    }

    try {
        const res = await fetch(`https://localhost:7274/api/familiar/buscar-pacientes?query=${encodeURIComponent(nombre)}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!res.ok) throw new Error('Error buscando pacientes.');

        const pacientes = await res.json();

        if (pacientes.length === 0) {
            sugerenciasDiv.innerHTML = "<p>No se encontraron pacientes.</p>";
            return;
        }

        sugerenciasDiv.innerHTML = pacientes.map((p: any) => `
            <button class="paciente-sugerido" data-id="${p.ID}">
                ${p.NombreUsuario} (${p.Ap1})
            </button>
        `).join('');
        
        document.querySelectorAll('.paciente-sugerido').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const target = e.target as HTMLElement;
                const pacienteId = target.getAttribute('data-id');
                if (pacienteId) {
                    enviarSolicitudSeleccionada(parseInt(pacienteId));
                }
            });
        });

    } catch (error) {
        sugerenciasDiv.innerHTML = "<p>Error al buscar.</p>";
        console.error(error);
    }
}
async function enviarSolicitudSeleccionada(pacienteId: number): Promise<void> {
    const mensajeDiv = document.getElementById('familiar-solicitud-mensaje');
    if (!mensajeDiv) return;

    const token = localStorage.getItem('token');
    if (!token) {
        mensajeDiv.textContent = "Sesión expirada.";
        mensajeDiv.style.color = "red";
        return;
    }

    try {
        const res = await fetch('https://localhost:7274/api/familiar/solicitudes', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                FamiliarId: 0,
                PacienteId: pacienteId
            })
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.message || "Error al enviar solicitud");

        mensajeDiv.textContent = data.Message || "Solicitud enviada.";
        mensajeDiv.style.color = "green";
        
        const sugerenciasDiv = document.getElementById('pacientes-sugeridos');
        if (sugerenciasDiv) sugerenciasDiv.innerHTML = ""; // Limpiar sugerencias

    } catch (error: any) {
        mensajeDiv.textContent = error.message || "Error al enviar solicitud.";
        mensajeDiv.style.color = "red";
    }
}
async function cargarReportesDePaciente(): Promise<void> {
  const idInput = document.getElementById("reporte-paciente-id") as HTMLInputElement;
  const lista = document.getElementById("lista-reportes") as HTMLUListElement;
  if (!idInput || !lista) return;

  const pacienteId = parseInt(idInput.value);
  if (isNaN(pacienteId)) {
    alert("ID de paciente inválido.");
    return;
  }

  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch(`https://localhost:7274/api/pm/reportes/${pacienteId}`, {
      headers: { Authorization: `Bearer ${token}` }
    });

    const reportes = await res.json();
    lista.innerHTML = reportes.length
      ? reportes.map((r: any) => `<li><strong>${r.titulo}</strong>: ${r.contenido}</li>`).join("")
      : "<li>No hay reportes disponibles.</li>";
  } catch (e) {
    lista.innerHTML = "<li>Error al obtener reportes.</li>";
  }
}

async function cargarReportesParaFamiliar(): Promise<void> {
  const input = document.getElementById("reporte-familiar-paciente-id") as HTMLInputElement;
  const lista = document.getElementById("lista-reportes-familiar") as HTMLUListElement;
  if (!input || !lista) return;

  const id = parseInt(input.value);
  if (isNaN(id)) {
    alert("ID inválido.");
    return;
  }

  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch(`https://localhost:7274/api/familiar/reportes/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    });

    const reportes = await res.json();
    lista.innerHTML = reportes.length
      ? reportes.map((r: any) => `<li><strong>${r.titulo}</strong>: ${r.contenido}</li>`).join("")
      : "<li>No hay reportes disponibles.</li>";
  } catch (e) {
    lista.innerHTML = "<li>Error al cargar reportes.</li>";
  }
}


async function enviarNuevoReporte(): Promise<void> {
  const pacienteIdInput = document.getElementById("nuevo-id-paciente") as HTMLInputElement;
  const tituloInput = document.getElementById("titulo-reporte") as HTMLInputElement;
  const contenidoInput = document.getElementById("contenido-reporte") as HTMLTextAreaElement;

  const pacienteId = parseInt(pacienteIdInput.value);
  if (isNaN(pacienteId) || !tituloInput.value.trim() || !contenidoInput.value.trim()) {
    alert("Completa todos los campos correctamente.");
    return;
  }

  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const res = await fetch(`https://localhost:7274/api/pm/reportes`, {
      method: "POST",
      headers: {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        PacienteId: pacienteId,
        Titulo: tituloInput.value.trim(),
        Contenido: contenidoInput.value.trim()
      })
    });

    const data = await res.json();
    if (!res.ok) throw new Error(data.message || "Error al enviar reporte.");

    alert(data.message || "Reporte enviado correctamente.");
    tituloInput.value = "";
    contenidoInput.value = "";

  } catch (e: any) {
    alert(e.message || "Error al enviar reporte.");
  }
}

async function cargarMiEstadoPaciente() {
    const estadoElemento = document.getElementById("paciente-estado");
    if (!estadoElemento) {
        console.error("Elemento #paciente-estado no encontrado.");
        return;
    }

    const token = localStorage.getItem("token");
    if (!token) {
        estadoElemento.textContent = "No autenticado.";
        return;
    }

    try {
        const response = await fetch("https://localhost:7274/api/pacientes/mi-estado", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`,   // <<<<<< 🔥 AQUÍ se manda el token
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error(`Error al obtener estado: ${response.status}`);
        }

        const data = await response.json();
        estadoElemento.textContent = data.Estado || "Sin estado disponible.";
    } catch (error) {
        console.error(error);
        estadoElemento.textContent = "Error al obtener estado.";
    }
}



function showPacienteDashboard(): void {
  const loginContainer = document.getElementById('login-container');
  const registerContainer = document.getElementById('register-container');
  const pacienteDashboard = document.getElementById('paciente-dashboard');
  const familiarDashboard = document.getElementById('familiar-dashboard');
  const pmDashboard = document.getElementById('pm-dashboard');
  const sudoDashboard = document.getElementById('sudo-dashboard');

  if (loginContainer) loginContainer.style.display = 'none';
  if (registerContainer && loginContainer) loginContainer.style.display = 'none';
  if (familiarDashboard) familiarDashboard.style.display = 'none';
  if (pmDashboard) pmDashboard.style.display = 'none';
  if (sudoDashboard) sudoDashboard.style.display = 'none';
  if (pacienteDashboard) pacienteDashboard.style.display = 'flex';

  const nombre = localStorage.getItem("nombre_usuario") || "Paciente";
    const nameElements = document.querySelectorAll("#paciente-welcome-name, #paciente-welcome-name-main");
    nameElements.forEach(e => e.textContent = nombre);

  cargarMiEstadoPaciente(); // <-- AGREGAR ESTA LÍNEA
  cargarFamiliaresVinculados();
}

function hideAllSections(): void {
  [
    "login-container",
    "register-container",
    "recover-container",
    "paciente-dashboard",
    "pm-dashboard",
    "familiar-dashboard",
    "sudo-dashboard"
  ].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.style.display = "none";
  });
}
let globalPollingInterval: number | null = null;

function iniciarPollingGlobal() {
  if (globalPollingInterval !== null) return;

  globalPollingInterval = setInterval(() => {
    const token = localStorage.getItem("token");
    if (!token) return; //  Evita hacer llamadas si aún no hay sesión

    const pmVisible = document.getElementById("pm-dashboard")?.style.display === "flex";
    const familiarVisible = document.getElementById("familiar-dashboard")?.style.display === "flex";
    const pacienteVisible = document.getElementById("paciente-dashboard")?.style.display === "flex";
    const sudoVisible = document.getElementById("sudo-dashboard")?.style.display === "flex";

    if (pmVisible) {
      cargarPacientesPM();
      cargarSolicitudesPendientes();
    }
    if (familiarVisible) {
      cargarPacientesVinculados();
    }
    if (pacienteVisible) {
      cargarMiEstadoPaciente();
      cargarFamiliaresVinculados();
    }
    if (sudoVisible) {
      fetchUserList();
      actualizarRecursos();
    }
  }, 5000);
}


document.addEventListener("DOMContentLoaded", () => {
  // --- Tus configuraciones previas ---
  const pacienteBusquedaInput = document.getElementById('paciente-busqueda-input') as HTMLInputElement;
if (pacienteBusquedaInput) {
    pacienteBusquedaInput.addEventListener('input', () => {
        if (pacienteBusquedaInput.value.length >= 2) { // Para no buscar si escribe solo 1 letra
            buscarPacientesPorNombre(pacienteBusquedaInput.value.trim());
        }
    });
}
// Mostrar/ocultar campos de PM al seleccionar el rol
const pmFields = document.getElementById("pm-fields") as HTMLElement;
const rolRadios = document.querySelectorAll<HTMLInputElement>("input[name='RolInput']");

rolRadios.forEach(radio => {
  radio.addEventListener("change", () => {
    if (radio.value === "1" && radio.checked) {
      pmFields.style.display = "block";
    } else {
      pmFields.style.display = "none";
    }
  });
});

  const switchToRegisterButton = document.getElementById('switch-to-register');
  if (switchToRegisterButton) {
    switchToRegisterButton.addEventListener('click', showRegisterForm);
  }
    const familiarSolicitudForm = document.getElementById('familiar-solicitud-form') as HTMLFormElement;
    if (familiarSolicitudForm) {
        familiarSolicitudForm.addEventListener('submit', enviarSolicitudFamiliar);
    }

  const switchToLoginButton = document.getElementById('switch-to-login');
  if (switchToLoginButton) {
    switchToLoginButton.addEventListener('click', showLoginForm);
  }

  const userListContainer = document.getElementById("user-list-container");
  if (userListContainer) {
    userListContainer.addEventListener("click", (event) => {
      const target = event.target as HTMLElement;
      if (target.classList.contains("del-btn")) {
        const id = Number(target.getAttribute("data-id"));
        if (!isNaN(id)) {
          deleteUser(id);
        } else {
          console.error("ID de usuario inválido para eliminación.");
        }
      }
    });
  }
    const btnVerReportesFamiliar = document.getElementById("btn-ver-reportes-familiar");
    if (btnVerReportesFamiliar) {
      btnVerReportesFamiliar.addEventListener("click", cargarReportesParaFamiliar);
    }

  const recoverBtn = document.getElementById("switch-to-recover");
  if (recoverBtn) recoverBtn.addEventListener("click", showRecoverForm);

  const returnToLoginBtn = document.getElementById("return-to-login");
  if (returnToLoginBtn) returnToLoginBtn.addEventListener("click", showLoginForm);

  // === Formularios principales ===
  const loginForm = document.getElementById('login-form') as HTMLFormElement;
  if (loginForm) loginForm.addEventListener('submit', handleLoginSubmit);

  const registerForm = document.getElementById('register-form') as HTMLFormElement;
  if (registerForm) registerForm.addEventListener('submit', handleRegisterSubmit);

  const recoverForm = document.getElementById("recover-form");
  if (recoverForm) recoverForm.addEventListener("submit", handleRecoverSubmit);

  const changeRoleForm = document.getElementById('change-role-form') as HTMLFormElement;
  if (changeRoleForm) changeRoleForm.addEventListener('submit', handleChangeRoleSubmit);

  const questionsForm = document.getElementById("questions-form");
  const verifyButton = document.querySelector("#questions-container button[type='submit']");
  if (verifyButton) verifyButton.addEventListener("click", handleQuestionsSubmit);

  const agregarPreguntaBtn = document.getElementById("agregar-pregunta");
  if (agregarPreguntaBtn && !agregarPreguntaBtn.hasAttribute("data-listener-added")) {
    agregarPreguntaBtn.addEventListener("click", () => {
      const container = document.getElementById("preguntas-dinamicas");
      if (!container) return;

      const preguntaGroup = document.createElement("div");
      preguntaGroup.className = "form-group";
      preguntaGroup.innerHTML = `<label>Pregunta de Recuperación:</label><input type="text" name="Preguntas[]" required>`;

      const respuestaGroup = document.createElement("div");
      respuestaGroup.className = "form-group";
      respuestaGroup.innerHTML = `<label>Respuesta de Recuperación:</label><input type="text" name="Respuestas[]" required>`;

      container.appendChild(preguntaGroup);
      container.appendChild(respuestaGroup);
    });
    agregarPreguntaBtn.setAttribute("data-listener-added", "true");
  }
const btnCargarReportes = document.getElementById("btn-cargar-reportes");
if (btnCargarReportes) {
  btnCargarReportes.addEventListener("click", cargarReportesDePaciente);
}
const btnEnviarReporte = document.getElementById("btn-enviar-reporte");
if (btnEnviarReporte) {
  btnEnviarReporte.addEventListener("click", enviarNuevoReporte);
}

  // === Botones del Dashboard SUDO ===
  const btnActivos = document.getElementById("ver-usuarios-activos");
  if (btnActivos) btnActivos.addEventListener("click", cargarUsuariosActivos);

  const btnTodos = document.getElementById("ver-todos-usuarios");
  if (btnTodos) btnTodos.addEventListener("click", cargarTodosLosUsuarios);


  const btnRecursos = document.getElementById("ver-recursos");
  if (btnRecursos) {
    btnRecursos.addEventListener("click", () => {
      const panel = document.getElementById("recursos-panel")!;
      const visible = panel.style.display === "block";

      if (visible) {
        panel.style.display = "none";
        clearInterval(monitorInterval);
      } else {
        panel.style.display = "block";
        actualizarRecursos();
        monitorInterval = window.setInterval(actualizarRecursos, 5000);
      }
    });
  }

  const logoutButtons = document.querySelectorAll('#logout-button, #logout-button-pm, #logout-button-familiar, #logout-button-paciente');
  logoutButtons.forEach(button => {
    button.addEventListener('click', logout);
  });

  const reloadBtn = document.getElementById("pm-reload-pacientes");
  if (reloadBtn) reloadBtn.addEventListener("click", cargarPacientesPM);
  
  const btnCargarVinculos = document.getElementById("btn-cargar-vinculos");
    if (btnCargarVinculos) {
      btnCargarVinculos.addEventListener("click", cargarVinculosPacienteFamiliar);
    }


  showLoginForm(); // Mostrar login por defecto
  iniciarPollingGlobal();
});

// ----- FUNCIÓN CHECK SESSION (Placeholder - requiere implementación real) -----
async function checkUserSession(): Promise<void> {
  const token = localStorage.getItem('token');

  if (!token || token === '1911') {
    showLoginForm();
    return;
  }

  try {
    const response = await fetch('https://localhost:7274/api/auth/verify', {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token}`
      }
    });

    if (!response.ok) {
      // El token es inválido o expiró
      localStorage.removeItem('token');
      showLoginForm();
      return;
    }

    const userData = await response.json(); // Espera que devuelva ID, TipoUsuario, etc.

    const tipoUsuario = userData.TipoUsuario ?? userData.tipoUsuario ?? null;

    if (tipoUsuario === null) {
      throw new Error('Tipo de usuario no especificado.');
    }

    switch (tipoUsuario) {
      case 0: showPacienteDashboard(); break;
      case 1: showPmDashboard(); break;
      case 2: showFamiliarDashboard(); break;
      case 3: showSudoDashboard(); break;
      default:
        console.warn('Tipo de usuario desconocido:', tipoUsuario);
        showLoginForm();
    }

  } catch (error) {
    console.error('Error verificando sesión:', error);
    localStorage.removeItem('token');
    showLoginForm();
  }
}
