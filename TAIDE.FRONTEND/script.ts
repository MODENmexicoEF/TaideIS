// ----- INTERFACES (Alineadas con PascalCase del Backend para RESPUESTAS JSON) -----
// Estas interfaces se usan para leer las RESPUESTAS JSON del servidor
interface AuthResponse {
    Message: string;
    UserId?: number;
    Rol?: number;               // <-- Agrega esto
    TipoUsuario?: number;       // <-- ya existente
    Nombre?: string;            // <-- si no se usa NombreUsuario
    NombreUsuario?: string;
    Token?: string;
    Error?: string;
}

interface RegisterResponse {
    Message: string;
    UsuarioId?: number;
    TipoUsuario?: number;
    Error?: string;
}

// Asegúrate que el casing aquí coincida con el JSON devuelto por /api/auth/usuarios
interface User {
    ID: number;            // Asumiendo PascalCase desde C#
    NombreUsuario: string; // Asumiendo PascalCase
    TipoUsuario: number;   // Asumiendo PascalCase
    EnLinea: boolean;      // Asumiendo PascalCase
}
async function login(email: string, password: string): Promise<void> {
    const errorMessageElement = document.getElementById('error-message');
    if (errorMessageElement) errorMessageElement.textContent = '';

    try {
        const response = await fetch('https://localhost:7274/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                correo: email,
                contrasena: password
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            const errorData = errorText ? JSON.parse(errorText) : null;
            const mensaje = errorData?.message || `Error ${response.status}`;
            if (errorMessageElement) errorMessageElement.textContent = mensaje;
            return;
        }

        const data = await response.json();

        alert(data.Message || "Iniciaste sesión correctamente.");

        if (data.token) {
            localStorage.setItem("token", data.token);
        }
        if (data.NombreUsuario) {
            localStorage.setItem("nombre_usuario", data.NombreUsuario);
        }

        const tipo = data.TipoUsuario ?? data.tipo_usuario ?? data.Rol;

        switch (tipo) {
            case 0:
                showPacienteDashboard();
                break;
            case 1:
                showPmDashboard();
                break;
            case 2:
                showFamiliarDashboard();
                break;
            case 3:
                showSudoDashboard();
                break;
            default:
                alert("Tipo de usuario no reconocido");
                showLoginForm();
        }

    } catch (error) {
        console.error("Error en el login:", error);
        if (errorMessageElement) {
            errorMessageElement.textContent = 'No se pudo conectar al servidor.';
        }
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

// ----- Función para mostrar usuarios activos -----
async function cargarUsuariosActivos() {
    const lista = document.getElementById('user-list')!;
    lista.innerHTML = '<li>Cargando usuarios activos...</li>';

    try {
        const response = await fetch('https://localhost:7274/api/sudo/usuarios/activos', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) throw new Error('Error al obtener usuarios activos.');

        const usuarios = await response.json();
        mostrarUsuarios(usuarios, true);
    } catch (error) {
        console.error('Error:', error);
        mostrarErrorUsuarios('Error al cargar usuarios activos.');
    }
}


// ----- Función para mostrar todos los usuarios -----
async function cargarTodosLosUsuarios() {
    const lista = document.getElementById('user-list')!;
    lista.innerHTML = '<li>Cargando todos los usuarios...</li>';

    try {
        const response = await fetch('https://localhost:7274/api/sudo/usuarios', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) throw new Error('Error al obtener usuarios.');

        const usuarios = await response.json();
        mostrarUsuarios(usuarios, false);
    } catch (error) {
        console.error('Error:', error);
        mostrarErrorUsuarios('Error al cargar todos los usuarios.');
    }
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

// ----- Asignar eventos a los botones -----
document.getElementById('ver-usuarios-activos')?.addEventListener('click', cargarUsuariosActivos);
document.getElementById('ver-todos-usuarios')?.addEventListener('click', cargarTodosLosUsuarios);



// ----- OBTENER LISTA DE USUARIOS -----
async function fetchUserList(): Promise<void> {
    const userListContainer = document.getElementById('user-list-container');
    if (!userListContainer) return;

    try {
        const token = localStorage.getItem('token');
        if (!token || token === '1911') { // No intentar si no hay token real
            throw new Error('No autenticado. Inicie sesión.');
        }

        const response = await fetch('https://localhost:7274/api/auth/usuarios', { // TU URL BACKEND
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
            let rolTexto: string;
            switch (user.TipoUsuario) {
                case 0: rolTexto = 'Paciente'; break;
                case 1: rolTexto = 'PM'; break;
                case 2: rolTexto = 'Familiar'; break;
                case 3: rolTexto = 'SUDO'; break;
                default: rolTexto = 'Desconocido'; break;
            }
            userTableHTML += `<tr>
                                <td>${user.ID}</td>
                                <td>${user.NombreUsuario}</td>
                                <td>${rolTexto}</td>
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

        const response = await fetch('https://localhost:7274/api/auth/cambiar-rol', { // TU URL BACKEND
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Asumiendo JSON para este endpoint
                'Authorization': `Bearer ${token}`
            },
            // El objeto debe coincidir con CambiarRolRequest.cs
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
// Este listener se añade más abajo en DOMContentLoaded
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

function showPacienteDashboard(): void {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pacienteDashboard = document.getElementById('paciente-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');

    if (loginContainer) loginContainer.style.display = 'none';
    if (registerContainer) registerContainer.style.display = 'none';
    if (familiarDashboard) familiarDashboard.style.display = 'none';
    if (pmDashboard) pmDashboard.style.display = 'none';
    if (sudoDashboard) sudoDashboard.style.display = 'none';
    if (pacienteDashboard) pacienteDashboard.style.display = 'flex'; // o block
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

// ----- EVENT LISTENERS (Organizados y correctos) -----
document.addEventListener('DOMContentLoaded', () => {
    // Listener para el formulario de Login
    const loginForm = document.getElementById('login-form') as HTMLFormElement;
    if (loginForm) {
        loginForm.addEventListener('submit', handleLoginSubmit);
    } else {
        console.error('No se encontró el formulario de inicio de sesión.');
    }

    // Listener para el formulario de Registro
    const registerForm = document.getElementById('register-form') as HTMLFormElement;
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegisterSubmit); // Escucha submit del form

         // Lógica para mostrar/ocultar campos de PM basada en el rol seleccionado
         const rolPaciente = document.getElementById('rol-paciente') as HTMLInputElement;
         const rolPM = document.getElementById('rol-pm') as HTMLInputElement;
         const rolFamiliar = document.getElementById('rol-familiar') as HTMLInputElement;
         const pmFields = document.getElementById('pm-fields');

         function togglePMFields() {
             if(pmFields && rolPM) {
                pmFields.style.display = rolPM.checked ? 'block' : 'none';
             }
         }

         if (rolPaciente && rolPM && rolFamiliar && pmFields) {
             rolPaciente.addEventListener('change', togglePMFields);
             rolPM.addEventListener('change', togglePMFields);
             rolFamiliar.addEventListener('change', togglePMFields);
             togglePMFields(); // Estado inicial al cargar
         }

    } else {
        console.error('No se encontró el formulario de registro.');
    }

     // Listener para el formulario de cambio de rol
     const changeRoleForm = document.getElementById('change-role-form') as HTMLFormElement;
     if (changeRoleForm) {
         changeRoleForm.addEventListener('submit', handleChangeRoleSubmit);
     } else {
         console.warn('No se encontró el formulario de cambio de rol (puede ser normal si no se es SUDO).');
     }


    // Listeners para botones de cambio de vista
    const switchToRegisterButton = document.getElementById('switch-to-register');
    if (switchToRegisterButton) {
        switchToRegisterButton.addEventListener('click', showRegisterForm);
    }

    const switchToLoginButton = document.getElementById('switch-to-login');
    if (switchToLoginButton) {
        switchToLoginButton.addEventListener('click', showLoginForm);
    }

    // Listeners para botones de Logout (agrupados)
    const logoutButtons = document.querySelectorAll('#logout-button, #logout-button-pm, #logout-button-familiar, #logout-button-paciente');
    logoutButtons.forEach(button => {
        button.addEventListener('click', logout);
    });


    // Verificar sesión al cargar o mostrar login por defecto
    // checkUserSession(); // Descomenta si quieres intentar verificar sesión
    showLoginForm(); // Muestra el login al inicio por defecto

});

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
    body: JSON.stringify(correoInput.value.trim())
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
  newPasswordForm.onsubmit = function (e) {
    e.preventDefault();
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

// === Eventos DOM ===
document.addEventListener("DOMContentLoaded", () => {
  const recoverBtn = document.getElementById("switch-to-recover");
  if (recoverBtn) recoverBtn.addEventListener("click", showRecoverForm);

  const recoverForm = document.getElementById("recover-form");
  if (recoverForm) recoverForm.addEventListener("submit", handleRecoverSubmit);

  const questionsForm = document.getElementById("questions-form");
  const verifyButton = document.querySelector("#questions-container button[type='submit']")!;
  if (verifyButton) verifyButton.addEventListener("click", handleQuestionsSubmit);

  const returnToLoginBtn = document.getElementById("return-to-login");
  if (returnToLoginBtn) returnToLoginBtn.addEventListener("click", showLoginForm);

  // === Botón para agregar más preguntas dinámicas ===
const agregarPreguntaBtn = document.getElementById("agregar-pregunta");

if (agregarPreguntaBtn && !agregarPreguntaBtn.hasAttribute("data-listener-added")) {
  agregarPreguntaBtn.addEventListener("click", () => {
    const container = document.getElementById("preguntas-dinamicas");
    if (!container) return;

    const preguntaGroup = document.createElement("div");
    preguntaGroup.className = "form-group";
    preguntaGroup.innerHTML = `
      <label>Pregunta de Recuperación:</label>
      <input type="text" name="Preguntas[]" required>
    `;

    const respuestaGroup = document.createElement("div");
    respuestaGroup.className = "form-group";
    respuestaGroup.innerHTML = `
      <label>Respuesta de Recuperación:</label>
      <input type="text" name="Respuestas[]" required>
    `;

    container.appendChild(preguntaGroup);
    container.appendChild(respuestaGroup);
  });

  agregarPreguntaBtn.setAttribute("data-listener-added", "true");
}
});
// ----- FUNCIÓN CHECK SESSION (Placeholder - requiere implementación real) -----
function checkUserSession(): void {
    const token = localStorage.getItem('token');
    if (token && token !== '1911') { // Solo intentar si hay un token "real"
        // TODO: Implementar lógica real aquí.
        // 1. Llamar a un endpoint API /api/auth/verify o similar con el token.
        // 2. El backend verifica el token y devuelve la info del usuario (ID, Rol).
        // 3. Basado en la respuesta, mostrar el dashboard correcto o el login.
        // Ejemplo:
        // fetch('/api/auth/verify', { headers: {'Authorization': `Bearer ${token}`}})
        //   .then(res => res.ok ? res.json() : Promise.reject('Token inválido'))
        //   .then(userData => {
        //        if (userData.TipoUsuario === 3) showSudoDashboard();
        //        else if (userData.TipoUsuario === 1) window.location.href = 'pm-dashboard.html';
        //        // ... etc ...
        //   })
        //   .catch(err => { console.error(err); localStorage.removeItem('token'); showLoginForm(); });

        console.warn('Simulando verificación de sesión. En producción, verifica el token con tu API.');
        // Simulación simple (INSEGURA): Asume SUDO si hay token
         showSudoDashboard();
    } else {
        // Si no hay token o es el falso, muestra el formulario de login
        showLoginForm();
    }
}