import {
  MENSAJE_TELEFONO_INVALIDO,
  telefonoEsValido,
} from './tecnico';

export interface Cliente {
  id: number;
  nombre: string;
  documentoIdentidad: string;
  direccion: string;
  telefono: string;
  activo: boolean;
}

export interface CrearClienteRequest {
  nombre: string;
  documentoIdentidad: string;
  direccion: string;
  telefono: string;
}

export interface ActualizarClienteRequest {
  nombre: string;
  documentoIdentidad: string;
  direccion: string;
  telefono: string;
  activo: boolean;
}

export interface ClienteFormulario {
  nombre: string;
  documentoIdentidad: string;
  direccion: string;
  telefono: string;
  activo: boolean;
}

export const MENSAJE_DOCUMENTO_DUPLICADO =
  'Ya existe un cliente con el mismo documento de identidad.';

export { MENSAJE_TELEFONO_INVALIDO, telefonoEsValido };
