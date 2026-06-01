import { EstadoOrdenServicio } from './enums/estado-orden-servicio';

export interface OrdenServicio {
  id: number;
  fechaCreacion: string;
  estado: EstadoOrdenServicio;
  estadoDescripcion: string;
  descripcion: string;
  tecnicoId: number;
  tecnicoNombre: string;
  tecnicoEspecialidad: string;
  clienteId: number;
  clienteNombre: string;
  clienteDocumentoIdentidad: string;
}

export interface CrearOrdenServicioRequest {
  descripcion: string;
  tecnicoId: number;
  clienteId: number;
  estado?: EstadoOrdenServicio;
}

export interface ActualizarOrdenServicioRequest {
  descripcion: string;
  tecnicoId: number;
  clienteId: number;
  estado: EstadoOrdenServicio;
}

export interface CambiarEstadoOrdenServicioRequest {
  estado: EstadoOrdenServicio;
}

export interface ListarOrdenesServicioFiltros {
  estado?: EstadoOrdenServicio;
  clienteId?: number;
  tecnicoId?: number;
  tecnicoBusqueda?: string;
  clienteBusqueda?: string;
  fechaCreacionDesde?: string;
  fechaCreacionHasta?: string;
}

export interface OrdenServicioFormulario {
  descripcion: string;
  tecnicoId: number | null;
  clienteId: number | null;
  estado: EstadoOrdenServicio;
}
