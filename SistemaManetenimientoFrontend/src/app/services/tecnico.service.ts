import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActualizarTecnicoRequest,
  CrearTecnicoRequest,
  Tecnico,
} from '../models/tecnico';

@Injectable({ providedIn: 'root' })
export class TecnicoService {
  private readonly baseUrl = '/api/Tecnicos';

  constructor(private readonly http: HttpClient) {}

  listar(activo?: boolean): Observable<Tecnico[]> {
    let params = new HttpParams();
    if (activo !== undefined) {
      params = params.set('activo', activo);
    }
    return this.http.get<Tecnico[]>(this.baseUrl, { params });
  }

  obtenerPorId(id: number): Observable<Tecnico> {
    return this.http.get<Tecnico>(`${this.baseUrl}/${id}`);
  }

  crear(request: CrearTecnicoRequest): Observable<Tecnico> {
    return this.http.post<Tecnico>(this.baseUrl, request);
  }

  actualizar(id: number, request: ActualizarTecnicoRequest): Observable<Tecnico> {
    return this.http.put<Tecnico>(`${this.baseUrl}/${id}`, request);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
