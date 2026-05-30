-- Script de base de datos para OrdenServicio
-- Base de datos: sistema_mantenimiento (PostgreSQL)

-- Crear la base de datos si no existe
CREATE DATABASE IF NOT EXISTS sistema_mantenimiento;
USE sistema_mantenimiento;

CREATE TABLE IF NOT EXISTS public.usuarios (
    id SERIAL PRIMARY KEY,
    nombre_usuario VARCHAR(50) NOT NULL UNIQUE,
    hash_contrasena VARCHAR(255) NOT NULL,
    nombre_completo VARCHAR(150) NOT NULL,
    correo_electronico VARCHAR(150),
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

INSERT INTO usuarios (
    nombre_usuario,
    hash_contrasena,
    nombre_completo,
    correo_electronico,
    activo
)
VALUES (
    'admin',
    '$2a$11$v8rkcZNGmWoWF.z2rALR5u1N2dX0YTAondp2LWsJRU9v4Z./KXPY2',
    'Administrador',
    'admin@example.com',
    TRUE
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

CREATE INDEX IF NOT EXISTS idx_ordenes_servicio_cliente ON ordenes_servicio(cliente_id);
CREATE INDEX IF NOT EXISTS idx_ordenes_servicio_tecnico ON ordenes_servicio(tecnico_id);
CREATE INDEX IF NOT EXISTS idx_ordenes_servicio_estado ON ordenes_servicio(estado);

-- Datos de ejemplo (ejecutar solo si las tablas están vacías)
INSERT INTO clientes (nombre, documento_identidad, direccion, telefono)
SELECT 'Empresa ABC S.A.', '900123456-1', 'Calle 100 # 20-30, Bogotá', '3001234567'
WHERE NOT EXISTS (SELECT 1 FROM clientes);

INSERT INTO clientes (nombre, documento_identidad, direccion, telefono)
SELECT 'Comercial XYZ Ltda.', '800987654-3', 'Av. El Dorado # 68-40, Bogotá', '3109876543'
WHERE NOT EXISTS (SELECT 1 FROM clientes WHERE nombre = 'Comercial XYZ Ltda.');

INSERT INTO tecnicos (nombre, telefono, especialidad)
SELECT 'Carlos Méndez', '3201112233', 'Mecánica industrial'
WHERE NOT EXISTS (SELECT 1 FROM tecnicos);

INSERT INTO tecnicos (nombre, telefono, especialidad)
SELECT 'Ana Torres', '3154445566', 'Electricidad'
WHERE NOT EXISTS (SELECT 1 FROM tecnicos WHERE nombre = 'Ana Torres');
