export interface Tecnico {
  id: number;
  nombre: string;
  telefono: string;
  especialidad: string;
  activo: boolean;
}

export interface CrearTecnicoRequest {
  nombre: string;
  telefono: string;
  especialidad: string;
}

export interface ActualizarTecnicoRequest {
  nombre: string;
  telefono: string;
  especialidad: string;
  activo: boolean;
}

export interface TecnicoFormulario {
  nombre: string;
  telefono: string;
  especialidad: string;
  activo: boolean;
}

export const TELEFONO_REGEX = /^\+?[0-9]{7,15}$/;

export const MENSAJE_TELEFONO_INVALIDO =
  'El teléfono debe contener entre 7 y 15 dígitos numéricos, con un "+" opcional al inicio.';

export function telefonoEsValido(telefono: string): boolean {
  return TELEFONO_REGEX.test(telefono.trim());
}
