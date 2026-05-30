-- Migración: campos requeridos para técnicos y clientes
-- Base de datos: sistema_mantenimiento (PostgreSQL)

ALTER TABLE tecnicos
    ADD COLUMN IF NOT EXISTS telefono VARCHAR(50);

ALTER TABLE tecnicos
    DROP COLUMN IF EXISTS correo_electronico;

UPDATE tecnicos
SET telefono = '3000000000'
WHERE telefono IS NULL OR telefono = '';

UPDATE tecnicos
SET especialidad = 'General'
WHERE especialidad IS NULL OR especialidad = '';

ALTER TABLE tecnicos
    ALTER COLUMN telefono SET NOT NULL,
    ALTER COLUMN especialidad SET NOT NULL;

ALTER TABLE clientes
    ADD COLUMN IF NOT EXISTS documento_identidad VARCHAR(50),
    ADD COLUMN IF NOT EXISTS direccion VARCHAR(300);

ALTER TABLE clientes
    DROP COLUMN IF EXISTS correo_electronico;

UPDATE clientes
SET documento_identidad = 'DOC-' || id::TEXT
WHERE documento_identidad IS NULL OR documento_identidad = '';

UPDATE clientes
SET direccion = 'Sin dirección registrada'
WHERE direccion IS NULL OR direccion = '';

UPDATE clientes
SET telefono = '3000000000'
WHERE telefono IS NULL OR telefono = '';

ALTER TABLE clientes
    ALTER COLUMN documento_identidad SET NOT NULL,
    ALTER COLUMN direccion SET NOT NULL,
    ALTER COLUMN telefono SET NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS idx_clientes_documento_identidad
    ON clientes (documento_identidad);
