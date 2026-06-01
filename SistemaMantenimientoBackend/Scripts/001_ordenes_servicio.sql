-- =============================================================================
-- Sistema de Mantenimiento - Script inicial (PostgreSQL)
-- Base de datos: sistema_mantenimiento
--
-- Ejecutar desde la raíz del proyecto:
--   psql -U postgres -f Scripts/001_ordenes_servicio.sql
--
-- Credenciales de prueba tras ejecutar el script:
--   Usuario: admin
--   Contraseña: Admin123!
-- =============================================================================

-- 1. Crear la base de datos si no existe (comandos psql)
SELECT 'CREATE DATABASE sistema_mantenimiento'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'sistema_mantenimiento')\gexec

\c sistema_mantenimiento

-- 2. Tablas
CREATE TABLE IF NOT EXISTS usuarios (
    id SERIAL PRIMARY KEY,
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
    hash_contrasena VARCHAR(255) NOT NULL,
    nombre_completo VARCHAR(150) NOT NULL,
    correo_electronico VARCHAR(150),
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS clientes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    documento_identidad VARCHAR(50) NOT NULL,
    direccion VARCHAR(300) NOT NULL,
    telefono VARCHAR(50) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_clientes_documento_identidad
    ON clientes (documento_identidad);

CREATE TABLE IF NOT EXISTS tecnicos (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    telefono VARCHAR(50) NOT NULL,
    especialidad VARCHAR(200) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS ordenes_servicio (
    id SERIAL PRIMARY KEY,
    fecha_creacion TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    estado VARCHAR(20) NOT NULL DEFAULT 'pendiente'
        CHECK (estado IN ('pendiente', 'en_progreso', 'finalizada')),
    descripcion TEXT NOT NULL,
    tecnico_id INT NOT NULL REFERENCES tecnicos(id),
    cliente_id INT NOT NULL REFERENCES clientes(id)
);

CREATE INDEX IF NOT EXISTS idx_ordenes_servicio_cliente ON ordenes_servicio (cliente_id);
CREATE INDEX IF NOT EXISTS idx_ordenes_servicio_tecnico ON ordenes_servicio (tecnico_id);
CREATE INDEX IF NOT EXISTS idx_ordenes_servicio_estado ON ordenes_servicio (estado);

-- 3. Datos iniciales (un registro por entidad; idempotente)
INSERT INTO usuarios (nombre_usuario, hash_contrasena, nombre_completo, correo_electronico, activo)
SELECT
    'admin',
    '$2a$11$v8rkcZNGmWoWF.z2rALR5u1N2dX0YTAondp2LWsJRU9v4Z./KXPY2',
    'Administrador',
    'admin@example.com',
    TRUE
WHERE NOT EXISTS (SELECT 1 FROM usuarios WHERE nombre_usuario = 'admin');

INSERT INTO clientes (nombre, documento_identidad, direccion, telefono)
SELECT 'Empresa ABC S.A.', '900123456-1', 'Calle 100 # 20-30, Bogotá', '3001234567'
WHERE NOT EXISTS (SELECT 1 FROM clientes);

INSERT INTO tecnicos (nombre, telefono, especialidad)
SELECT 'Carlos Méndez', '3201112233', 'Mecánica industrial'
WHERE NOT EXISTS (SELECT 1 FROM tecnicos);

INSERT INTO ordenes_servicio (descripcion, tecnico_id, cliente_id)
SELECT
    'Mantenimiento preventivo de equipos industriales.',
    (SELECT id FROM tecnicos LIMIT 1),
    (SELECT id FROM clientes LIMIT 1)
WHERE NOT EXISTS (SELECT 1 FROM ordenes_servicio);
