-- Crear esquema
CREATE SCHEMA IF NOT EXISTS VITACARE DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE VITACARE;

-- Tabla base para todos los usuarios
DROP TABLE IF EXISTS PreguntasSeguridad, FamiliaresPacientes, SUDOs, Familiares, PMs, Pacientes, Usuarios;

DROP TABLE IF EXISTS PreguntasSeguridad, FamiliaresPacientes, SUDOs, Familiares, PMs, Pacientes, Usuarios;

CREATE TABLE Usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombre_usuario VARCHAR(255) NOT NULL,
    Ap1 VARCHAR(255) NOT NULL,
    Ap2 VARCHAR(255),
    Correo VARCHAR(255) UNIQUE NOT NULL,
    Contrasena VARCHAR(255) NOT NULL,
    Recuperar_contrasenia BOOLEAN DEFAULT FALSE,
    TokenRecuperacion VARCHAR(255),
    TokenRecuperacionExpira DATETIME,
    tipo_usuario INT NOT NULL,
    -- 👇 ESTA LÍNEA SE ELIMINA 👇
    -- Discriminator VARCHAR(50) NOT NULL,
    INDEX (Correo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- Tabla para PACIENTES (TPT)
CREATE TABLE Pacientes (
    id_usuario INT PRIMARY KEY,
    fecha_nacimiento DATE,
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla para PROFESIONALES MÉDICOS (TPT)
CREATE TABLE PMs (
    id_usuario INT PRIMARY KEY,
    numero_colegiado VARCHAR(255),
    especialidad VARCHAR(255),
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla para FAMILIARES (TPT)
CREATE TABLE Familiares (
    id_usuario INT PRIMARY KEY,
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla para SUDOs (TPT)
CREATE TABLE SUDOs (
    id_usuario INT PRIMARY KEY,
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Relación N:M entre Familiares y Pacientes
CREATE TABLE FamiliaresPacientes (
    FamiliarID INT NOT NULL,
    PacienteID INT NOT NULL,
    PRIMARY KEY (FamiliarID, PacienteID),
    FOREIGN KEY (FamiliarID) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE,
    FOREIGN KEY (PacienteID) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla para preguntas de seguridad
CREATE TABLE PreguntasSeguridad (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioID INT NOT NULL,
    Pregunta VARCHAR(255) NOT NULL,
    Respuesta VARCHAR(255) NOT NULL,
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

select * from Usuarios;

select * from PreguntasSeguridad;
SELECT * FROM preguntasseguridad WHERE UsuarioID = (SELECT id_usuario FROM usuarios WHERE correo = 'j@j.com');
