import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActualizarClienteRequest,
  Cliente,
  CrearClienteRequest,
} from '../models/cliente';

@Injectable({ providedIn: 'root' })
export class ClienteService {
  private readonly baseUrl = '/api/Clientes';

  constructor(private readonly http: HttpClient) {}

  listar(activo?: boolean): Observable<Cliente[]> {
    let params = new HttpParams();
    if (activo !== undefined) {
      params = params.set('activo', activo);
    }
    return this.http.get<Cliente[]>(this.baseUrl, { params });
  }

  obtenerPorId(id: number): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.baseUrl}/${id}`);
  }

  crear(request: CrearClienteRequest): Observable<Cliente> {
    return this.http.post<Cliente>(this.baseUrl, request);
  }

  actualizar(id: number, request: ActualizarClienteRequest): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.baseUrl}/${id}`, request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
