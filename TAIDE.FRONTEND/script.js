"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
function login(email, password) {
    return __awaiter(this, void 0, void 0, function* () {
        var _a, _b;
        const errorMessageElement = document.getElementById('error-message');
        if (errorMessageElement)
            errorMessageElement.textContent = '';
        try {
            const response = yield fetch('https://localhost:7274/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    correo: email,
                    contrasena: password
                })
            });
            const data = yield response.json();
            if (!response.ok) {
                throw new Error(data.message || `Error ${response.status}`);
            }
            alert(data.Message || "Iniciaste sesi�n correctamente.");
            if (data.token) {
                localStorage.setItem("token", data.token);
            }
            if (data.NombreUsuario) {
                localStorage.setItem("nombre_usuario", data.NombreUsuario);
            }
            const tipo = (_b = (_a = data.TipoUsuario) !== null && _a !== void 0 ? _a : data.tipo_usuario) !== null && _b !== void 0 ? _b : data.Rol; // para cubrir ambas opciones
            // Mostrar el dashboard correspondiente seg�n el tipo de usuario
            switch (tipo) {
                case 0: // Paciente
                    showPacienteDashboard();
                    break;
                case 1: // Profesional M�dico (PM)
                    showPmDashboard();
                    break;
                case 2: // Familiar
                    showFamiliarDashboard();
                    break;
                case 3: // SUDO
                    showSudoDashboard();
                    break;
                default:
                    alert("Tipo de usuario no reconocido");
                    showLoginForm();
            }
        }
        catch (error) {
            console.error("Error al iniciar sesi�n:", error);
            const mensaje = error instanceof Error ? error.message : "Error desconocido";
            if (errorMessageElement) {
                errorMessageElement.textContent = mensaje;
            }
            else {
                alert(mensaje);
            }
        }
    });
}
// ----- FUNCI�N REGISTRO (Usando FormData) -----
// Acepta el FormData directamente
function registerUser(formData) {
    return __awaiter(this, void 0, void 0, function* () {
        const registerErrorMessageElement = document.getElementById('register-error-message');
        const registerSuccessMessageElement = document.getElementById('register-success-message');
        if (registerErrorMessageElement)
            registerErrorMessageElement.textContent = '';
        if (registerSuccessMessageElement)
            registerSuccessMessageElement.style.display = 'none';
        try {
            const response = yield fetch('https://localhost:7274/api/auth/registrar', {
                method: 'POST',
                // NO establecer Content-Type manualmente con FormData
                body: formData, // Enviar el FormData construido
            });
            const data = yield response.json(); // Leer respuesta JSON
            if (!response.ok) {
                // Intenta usar el mensaje de error del backend si existe
                throw new Error(data.Error || data.Message || `Error ${response.status}`);
            }
            console.log('Registro exitoso:', data);
            if (registerSuccessMessageElement) {
                registerSuccessMessageElement.textContent = data.Message;
                registerSuccessMessageElement.style.display = 'block';
            }
            else {
                alert(data.Message);
            }
            // Mostrar login despu�s de 2 segundos
            setTimeout(() => {
                showLoginForm(); // Llama a la funci�n que muestra el login y oculta otros
            }, 2000);
        }
        catch (error) {
            console.error('Error al registrar usuario:', error);
            const message = (error instanceof Error) ? error.message : 'Error desconocido';
            if (registerErrorMessageElement) {
                registerErrorMessageElement.textContent = message;
            }
            else {
                alert(message);
            }
        }
    });
}
// ----- MANEJADOR DE ENV�O DE LOGIN -----
function handleLoginSubmit(event) {
    event.preventDefault(); // Prevenir recarga de p�gina
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');
    if (emailInput && passwordInput) {
        login(emailInput.value, passwordInput.value);
    }
    else {
        console.error('No se encontraron los elementos de correo o contrase�a.');
        alert('Error interno: No se encontraron los campos de inicio de sesi�n.');
    }
}
function showPmDashboard() {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pacienteDashboard = document.getElementById('paciente-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    if (loginContainer)
        loginContainer.style.display = 'none';
    if (registerContainer)
        registerContainer.style.display = 'none';
    if (pacienteDashboard)
        pacienteDashboard.style.display = 'none';
    if (familiarDashboard)
        familiarDashboard.style.display = 'none';
    if (sudoDashboard)
        sudoDashboard.style.display = 'none';
    if (pmDashboard)
        pmDashboard.style.display = 'flex';
    const nombre = localStorage.getItem("nombre_usuario") || "Profesional";
    const nameElements = document.querySelectorAll("#pm-welcome-name, #pm-welcome-name-main");
    nameElements.forEach(e => e.textContent = nombre);
}
function showFamiliarDashboard() {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pacienteDashboard = document.getElementById('paciente-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    if (loginContainer)
        loginContainer.style.display = 'none';
    if (registerContainer)
        registerContainer.style.display = 'none';
    if (pacienteDashboard)
        pacienteDashboard.style.display = 'none';
    if (pmDashboard)
        pmDashboard.style.display = 'none';
    if (sudoDashboard)
        sudoDashboard.style.display = 'none';
    if (familiarDashboard)
        familiarDashboard.style.display = 'flex';
    const nombre = localStorage.getItem("nombre_usuario") || "Familiar";
    const nameElements = document.querySelectorAll("#familiar-welcome-name, #familiar-welcome-name-main");
    nameElements.forEach(e => e.textContent = nombre);
}
// ----- MANEJADOR DE ENV�O DE REGISTRO (Construye FormData desde el form) -----
function handleRegisterSubmit(event) {
    event.preventDefault();
    const nombreInput = document.getElementById('nombre-reg');
    const ap1Input = document.getElementById('ap1-reg');
    const ap2Input = document.getElementById('ap2-reg');
    const correoInput = document.getElementById('correo-reg');
    const contrasenaInput = document.getElementById('contrasena-reg');
    const rolRadioSelected = document.querySelector('#register-form input[name="RolInput"]:checked');
    const numeroColegiadoInput = document.getElementById('numero-colegiado-reg');
    const especialidadInput = document.getElementById('especialidad-reg');
    const preguntasInputs = document.querySelectorAll('input[name="Preguntas[]"]');
    const respuestasInputs = document.querySelectorAll('input[name="Respuestas[]"]');
    // Validaci�n b�sica
    if (!nombreInput.value.trim() ||
        !ap1Input.value.trim() ||
        !correoInput.value.trim() ||
        !contrasenaInput.value.trim() ||
        !rolRadioSelected) {
        alert('Por favor, complete todos los campos requeridos.');
        return;
    }
    // Validaci�n para PM
    const selectedTipoUsuario = rolRadioSelected.value;
    if (selectedTipoUsuario === "1") {
        if (!numeroColegiadoInput.value.trim() ||
            !especialidadInput.value.trim()) {
            alert('Por favor, complete n�mero de colegiado y especialidad.');
            return;
        }
    }
    // Validaci�n de preguntas de seguridad
    if (preguntasInputs.length === 0 || respuestasInputs.length === 0) {
        alert("Debes agregar al menos una pregunta y una respuesta.");
        return;
    }
    const formData = new FormData();
    formData.append("NombreUsuario", nombreInput.value);
    formData.append("Ap1", ap1Input.value);
    formData.append("Ap2", (ap2Input === null || ap2Input === void 0 ? void 0 : ap2Input.value) || "");
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
// ----- OBTENER LISTA DE USUARIOS -----
function fetchUserList() {
    return __awaiter(this, void 0, void 0, function* () {
        const userListContainer = document.getElementById('user-list-container');
        if (!userListContainer)
            return;
        try {
            const token = localStorage.getItem('token');
            if (!token || token === '1911') { // No intentar si no hay token real
                throw new Error('No autenticado. Inicie sesi�n.');
            }
            const response = yield fetch('https://localhost:7274/api/auth/usuarios', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json', // La respuesta s� ser� JSON
                    'Authorization': `Bearer ${token}`
                }
            });
            if (!response.ok) {
                let errorMsg = `Error ${response.status}`;
                try {
                    const errorData = yield response.json();
                    errorMsg = errorData.message || errorData.title || errorMsg;
                }
                catch (e) { /* Ignora si el cuerpo de error no es JSON */ }
                throw new Error(errorMsg);
            }
            const users = yield response.json(); // Espera array de User (con PascalCase)
            // Generar HTML de la tabla
            let userTableHTML = `<table class="user-table">
                                <thead>
                                  <tr>
                                    <th>ID</th>
                                    <th>Nombre Usuario</th>
                                    <th>Rol</th>
                                    <th>En L�nea</th>
                                  </tr>
                                </thead>
                                <tbody>`;
            users.forEach(user => {
                let rolTexto;
                switch (user.TipoUsuario) {
                    case 0:
                        rolTexto = 'Paciente';
                        break;
                    case 1:
                        rolTexto = 'PM';
                        break;
                    case 2:
                        rolTexto = 'Familiar';
                        break;
                    case 3:
                        rolTexto = 'SUDO';
                        break;
                    default:
                        rolTexto = 'Desconocido';
                        break;
                }
                userTableHTML += `<tr>
                                <td>${user.ID}</td>
                                <td>${user.NombreUsuario}</td>
                                <td>${rolTexto}</td>
                                <td><span class="${user.EnLinea ? 'online' : 'offline'}">${user.EnLinea ? 'S�' : 'No'}</span></td>
                              </tr>`;
            });
            userTableHTML += '</tbody></table>';
            userListContainer.innerHTML = userTableHTML;
        }
        catch (error) {
            console.error('Error al cargar la lista de usuarios:', error);
            const message = (error instanceof Error) ? error.message : 'Error desconocido';
            // Mostrar error solo si el contenedor existe
            if (userListContainer) {
                userListContainer.innerHTML = `<p class="error-message">Error al cargar la lista de usuarios: ${message}</p>`;
            }
        }
    });
}
// ----- CAMBIAR ROL -----
// Verifica si este endpoint en tu backend espera [FromBody] (JSON) o [FromForm]
// Este c�digo asume [FromBody] (JSON) como estaba antes.
function changeUserRole(usuarioId, nuevoRolNombre) {
    return __awaiter(this, void 0, void 0, function* () {
        // El backend espera el nombre del rol ("PM", "SUDO", etc.) seg�n CambiarRolRequest.cs
        let rolParaEnviar = nuevoRolNombre.toUpperCase();
        try {
            const token = localStorage.getItem('token');
            if (!token || token === '1911')
                throw new Error('No autenticado.');
            const response = yield fetch('https://localhost:7274/api/auth/cambiar-rol', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json', // Asumiendo JSON para este endpoint
                    'Authorization': `Bearer ${token}`
                },
                // El objeto debe coincidir con CambiarRolRequest.cs
                body: JSON.stringify({ UsuarioId: usuarioId, NuevoRol: rolParaEnviar })
            });
            // Intenta leer la respuesta como JSON, incluso si no es 2xx OK
            const data = yield response.json();
            if (!response.ok) {
                // Usa el mensaje del JSON si est� disponible
                throw new Error(data.message || data.title || `Error ${response.status}`);
            }
            alert(data.message || 'Rol cambiado con �xito');
            fetchUserList(); // Recargar lista para ver el cambio
        }
        catch (error) {
            console.error('Error al cambiar el rol:', error);
            const message = (error instanceof Error) ? error.message : 'Error desconocido';
            alert(message);
        }
    });
}
// ----- MANEJADOR CAMBIO DE ROL -----
// Este listener se a�ade m�s abajo en DOMContentLoaded
function handleChangeRoleSubmit(event) {
    event.preventDefault();
    const usuarioIdInput = document.getElementById('sudo-usuarioId');
    const nuevoRolInput = document.getElementById('sudo-nuevoRol');
    if (usuarioIdInput && nuevoRolInput && usuarioIdInput.value && nuevoRolInput.value) {
        const userId = parseInt(usuarioIdInput.value, 10);
        const nuevoRol = nuevoRolInput.value;
        if (!isNaN(userId)) {
            changeUserRole(userId, nuevoRol); // Llamar a la funci�n
            usuarioIdInput.value = ''; // Limpiar campos
            nuevoRolInput.value = '';
        }
        else {
            alert('El ID del usuario debe ser un n�mero.');
        }
    }
    else {
        alert('Por favor, complete todos los campos para cambiar el rol.');
    }
}
function showPacienteDashboard() {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pacienteDashboard = document.getElementById('paciente-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    if (loginContainer)
        loginContainer.style.display = 'none';
    if (registerContainer)
        registerContainer.style.display = 'none';
    if (familiarDashboard)
        familiarDashboard.style.display = 'none';
    if (pmDashboard)
        pmDashboard.style.display = 'none';
    if (sudoDashboard)
        sudoDashboard.style.display = 'none';
    if (pacienteDashboard)
        pacienteDashboard.style.display = 'flex'; // o block
}
// ----- FUNCIONES DE VISIBILIDAD -----
function showSudoDashboard() {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const pmDashboard = document.getElementById('pm-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    if (loginContainer)
        loginContainer.style.display = 'none';
    if (registerContainer)
        registerContainer.style.display = 'none';
    if (pmDashboard)
        pmDashboard.style.display = 'none';
    if (familiarDashboard)
        familiarDashboard.style.display = 'none';
    if (sudoDashboard)
        sudoDashboard.style.display = 'flex'; // O 'block' seg�n tu CSS
    fetchUserList(); // Cargar lista al mostrar dashboard
}
function showRegisterForm() {
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    if (loginContainer)
        loginContainer.style.display = 'none';
    if (registerContainer)
        registerContainer.style.display = 'flex'; // O 'block'
    if (sudoDashboard)
        sudoDashboard.style.display = 'none';
    if (pmDashboard)
        pmDashboard.style.display = 'none';
    if (familiarDashboard)
        familiarDashboard.style.display = 'none';
}
function showLoginForm() {
    hideAllSections();
    const loginContainer = document.getElementById('login-container');
    const registerContainer = document.getElementById('register-container');
    const sudoDashboard = document.getElementById('sudo-dashboard');
    const pmDashboard = document.getElementById('pm-dashboard');
    const familiarDashboard = document.getElementById('familiar-dashboard');
    if (loginContainer)
        loginContainer.style.display = 'flex'; // O 'block'
    if (registerContainer)
        registerContainer.style.display = 'none';
    if (sudoDashboard)
        sudoDashboard.style.display = 'none';
    if (pmDashboard)
        pmDashboard.style.display = 'none';
    if (familiarDashboard)
        familiarDashboard.style.display = 'none';
    const loginForm = document.getElementById("login-form");
    if (loginForm)
        loginForm.reset();
}
// ----- FUNCI�N LOGOUT -----
function logout() {
    localStorage.removeItem('token');
    alert('Sesi�n cerrada.');
    // Ocultar todos los dashboards manualmente
    const dashboards = [
        'paciente-dashboard',
        'pm-dashboard',
        'familiar-dashboard',
        'sudo-dashboard'
    ];
    dashboards.forEach(id => {
        const element = document.getElementById(id);
        if (element)
            element.style.display = 'none';
    });
    showLoginForm(); // Muestra el login despu�s de cerrar sesi�n
}
// ----- EVENT LISTENERS (Organizados y correctos) -----
document.addEventListener('DOMContentLoaded', () => {
    // Listener para el formulario de Login
    const loginForm = document.getElementById('login-form');
    if (loginForm) {
        loginForm.addEventListener('submit', handleLoginSubmit);
    }
    else {
        console.error('No se encontr� el formulario de inicio de sesi�n.');
    }
    // Listener para el formulario de Registro
    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        registerForm.addEventListener('submit', handleRegisterSubmit); // Escucha submit del form
        // L�gica para mostrar/ocultar campos de PM basada en el rol seleccionado
        const rolPaciente = document.getElementById('rol-paciente');
        const rolPM = document.getElementById('rol-pm');
        const rolFamiliar = document.getElementById('rol-familiar');
        const pmFields = document.getElementById('pm-fields');
        function togglePMFields() {
            if (pmFields && rolPM) {
                pmFields.style.display = rolPM.checked ? 'block' : 'none';
            }
        }
        if (rolPaciente && rolPM && rolFamiliar && pmFields) {
            rolPaciente.addEventListener('change', togglePMFields);
            rolPM.addEventListener('change', togglePMFields);
            rolFamiliar.addEventListener('change', togglePMFields);
            togglePMFields(); // Estado inicial al cargar
        }
    }
    else {
        console.error('No se encontr� el formulario de registro.');
    }
    // Listener para el formulario de cambio de rol
    const changeRoleForm = document.getElementById('change-role-form');
    if (changeRoleForm) {
        changeRoleForm.addEventListener('submit', handleChangeRoleSubmit);
    }
    else {
        console.warn('No se encontr� el formulario de cambio de rol (puede ser normal si no se es SUDO).');
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
    // Verificar sesi�n al cargar o mostrar login por defecto
    // checkUserSession(); // Descomenta si quieres intentar verificar sesi�n
    showLoginForm(); // Muestra el login al inicio por defecto
});
// === Funciones para Recuperaci�n de Contrase�a ===
// === Funciones para Recuperaci�n de Contrase�a ===
function showRecoverForm() {
    hideAllSections();
    const recoverContainer = document.getElementById("recover-container");
    if (recoverContainer)
        recoverContainer.style.display = "block";
}
function handleRecoverSubmit(event) {
    event.preventDefault();
    const correoInput = document.getElementById("recover-email");
    const errorBox = document.getElementById("recover-error-message");
    if (!correoInput || !correoInput.value.trim()) {
        if (errorBox)
            errorBox.textContent = "Por favor introduce tu correo.";
        return;
    }
    errorBox.textContent = "Cargando preguntas...";
    fetch("https://localhost:7274/api/auth/recuperar/obtener-preguntas", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(correoInput.value.trim())
    })
        .then(res => res.json())
        .then(data => {
        const questionsContainer = document.getElementById("questions-container");
        const questionsForm = document.getElementById("questions-form");
        questionsForm.innerHTML = "";
        if (!data.tienePreguntas) {
            errorBox.textContent = "Consulta con un t�cnico para la recuperaci�n de tu contrase�a.";
            return;
        }
        errorBox.textContent = "";
        data.preguntas.forEach((p) => {
            const div = document.createElement("div");
            div.className = "form-group";
            div.innerHTML = `<label>${p.Pregunta}</label><input type="text" data-id="${p.ID}" required>`;
            questionsForm.appendChild(div);
        });
        questionsContainer.style.display = "block";
    })
        .catch(err => {
        errorBox.textContent = "Error al obtener preguntas.";
        console.error(err);
    });
}
function handleQuestionsSubmit(event) {
    event.preventDefault();
    const inputs = document.querySelectorAll("#questions-form input[data-id]");
    const respuestas = {};
    inputs.forEach(input => {
        const id = parseInt(input.dataset.id || "0");
        const val = input.value.trim();
        if (id && val)
            respuestas[id] = val;
    });
    if (Object.keys(respuestas).length === 0)
        return alert("Responde al menos una pregunta.");
    document.getElementById("new-password-container").style.display = "block";
    const newPasswordForm = document.getElementById("new-password-form");
    newPasswordForm.onsubmit = function (e) {
        e.preventDefault();
        const nuevaPassInput = document.getElementById("new-password");
        const correo = document.getElementById("recover-email").value;
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
            if (!ok)
                throw new Error(data.message || "Error inesperado");
            alert("Contrase�a cambiada correctamente. Inicia sesi�n con la nueva contrase�a.");
            showLoginForm();
        })
            .catch(err => {
            alert(err.message || "Error al cambiar la contrase�a.");
        });
    };
}
function hideAllSections() {
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
        if (el)
            el.style.display = "none";
    });
}
// === Eventos DOM ===
document.addEventListener("DOMContentLoaded", () => {
    const recoverBtn = document.getElementById("switch-to-recover");
    if (recoverBtn)
        recoverBtn.addEventListener("click", showRecoverForm);
    const recoverForm = document.getElementById("recover-form");
    if (recoverForm)
        recoverForm.addEventListener("submit", handleRecoverSubmit);
    const questionsForm = document.getElementById("questions-form");
    const verifyButton = document.querySelector("#questions-container button[type='submit']");
    if (verifyButton)
        verifyButton.addEventListener("click", handleQuestionsSubmit);
    const returnToLoginBtn = document.getElementById("return-to-login");
    if (returnToLoginBtn)
        returnToLoginBtn.addEventListener("click", showLoginForm);
    // === Bot�n para agregar m�s preguntas din�micas ===
    const agregarPreguntaBtn = document.getElementById("agregar-pregunta");
    if (agregarPreguntaBtn && !agregarPreguntaBtn.hasAttribute("data-listener-added")) {
        agregarPreguntaBtn.addEventListener("click", () => {
            const container = document.getElementById("preguntas-dinamicas");
            if (!container)
                return;
            const preguntaGroup = document.createElement("div");
            preguntaGroup.className = "form-group";
            preguntaGroup.innerHTML = `
      <label>Pregunta de Recuperaci�n:</label>
      <input type="text" name="Preguntas[]" required>
    `;
            const respuestaGroup = document.createElement("div");
            respuestaGroup.className = "form-group";
            respuestaGroup.innerHTML = `
      <label>Respuesta de Recuperaci�n:</label>
      <input type="text" name="Respuestas[]" required>
    `;
            container.appendChild(preguntaGroup);
            container.appendChild(respuestaGroup);
        });
        agregarPreguntaBtn.setAttribute("data-listener-added", "true");
    }
});
// ----- FUNCI�N CHECK SESSION (Placeholder - requiere implementaci�n real) -----
function checkUserSession() {
    const token = localStorage.getItem('token');
    if (token && token !== '1911') { // Solo intentar si hay un token "real"
        // TODO: Implementar l�gica real aqu�.
        // 1. Llamar a un endpoint API /api/auth/verify o similar con el token.
        // 2. El backend verifica el token y devuelve la info del usuario (ID, Rol).
        // 3. Basado en la respuesta, mostrar el dashboard correcto o el login.
        // Ejemplo:
        // fetch('/api/auth/verify', { headers: {'Authorization': `Bearer ${token}`}})
        //   .then(res => res.ok ? res.json() : Promise.reject('Token inv�lido'))
        //   .then(userData => {
        //        if (userData.TipoUsuario === 3) showSudoDashboard();
        //        else if (userData.TipoUsuario === 1) window.location.href = 'pm-dashboard.html';
        //        // ... etc ...
        //   })
        //   .catch(err => { console.error(err); localStorage.removeItem('token'); showLoginForm(); });
        console.warn('Simulando verificaci�n de sesi�n. En producci�n, verifica el token con tu API.');
        // Simulaci�n simple (INSEGURA): Asume SUDO si hay token
        showSudoDashboard();
    }
    else {
        // Si no hay token o es el falso, muestra el formulario de login
        showLoginForm();
    }
}
