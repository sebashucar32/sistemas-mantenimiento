import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActualizarOrdenServicioRequest,
  CambiarEstadoOrdenServicioRequest,
  CrearOrdenServicioRequest,
  ListarOrdenesServicioFiltros,
  OrdenServicio,
} from '../models/orden-servicio';

@Injectable({ providedIn: 'root' })
export class OrdenServicioService {
  private readonly baseUrl = '/api/OrdenesServicio';

  constructor(private readonly http: HttpClient) {}

  listar(filtros: ListarOrdenesServicioFiltros = {}): Observable<OrdenServicio[]> {
    let params = new HttpParams();

    if (filtros.estado !== undefined) {
      params = params.set('estado', filtros.estado);
    }
    if (filtros.clienteId !== undefined) {
      params = params.set('clienteId', filtros.clienteId);
    }
    if (filtros.tecnicoId !== undefined) {
      params = params.set('tecnicoId', filtros.tecnicoId);
    }
    if (filtros.tecnicoBusqueda) {
      params = params.set('tecnicoBusqueda', filtros.tecnicoBusqueda);
    }
    if (filtros.clienteBusqueda) {
      params = params.set('clienteBusqueda', filtros.clienteBusqueda);
    }
    if (filtros.fechaCreacionDesde) {
      params = params.set('fechaCreacionDesde', filtros.fechaCreacionDesde);
    }
    if (filtros.fechaCreacionHasta) {
      params = params.set('fechaCreacionHasta', filtros.fechaCreacionHasta);
    }

    return this.http.get<OrdenServicio[]>(this.baseUrl, { params });
  }

  obtenerPorId(id: number): Observable<OrdenServicio> {
    return this.http.get<OrdenServicio>(`${this.baseUrl}/${id}`);
  }

  crear(request: CrearOrdenServicioRequest): Observable<OrdenServicio> {
    return this.http.post<OrdenServicio>(this.baseUrl, request);
  }

  actualizar(
    id: number,
    request: ActualizarOrdenServicioRequest
  ): Observable<OrdenServicio> {
    return this.http.put<OrdenServicio>(`${this.baseUrl}/${id}`, request);
  }

  cambiarEstado(
    id: number,
    request: CambiarEstadoOrdenServicioRequest
  ): Observable<OrdenServicio> {
    return this.http.patch<OrdenServicio>(
      `${this.baseUrl}/${id}/estado`,
      request
    );
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
