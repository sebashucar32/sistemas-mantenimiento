import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { OrdenServicioService } from './orden-servicio.service';
import { EstadoOrdenServicio } from '../models/enums/estado-orden-servicio';
import {
  ActualizarOrdenServicioRequest,
  CambiarEstadoOrdenServicioRequest,
  CrearOrdenServicioRequest,
  OrdenServicio,
} from '../models/orden-servicio';

describe('OrdenServicioService', () => {
  let service: OrdenServicioService;
  let httpMock: HttpTestingController;

  const baseUrl = '/api/OrdenesServicio';

  const ordenMock: OrdenServicio = {
    id: 1,
    fechaCreacion: '2026-05-31T10:00:00Z',
    estado: EstadoOrdenServicio.Pendiente,
    estadoDescripcion: 'Pendiente',
    descripcion: 'Reparación de equipo',
    tecnicoId: 2,
    tecnicoNombre: 'Carlos Gómez',
    tecnicoEspecialidad: 'Electricidad',
    clienteId: 3,
    clienteNombre: 'Juan Pérez',
    clienteDocumentoIdentidad: '12345678',
  };

  const crearRequest: CrearOrdenServicioRequest = {
    descripcion: 'Reparación de equipo',
    tecnicoId: 2,
    clienteId: 3,
  };

  const actualizarRequest: ActualizarOrdenServicioRequest = {
    descripcion: 'Reparación actualizada',
    tecnicoId: 2,
    clienteId: 3,
    estado: EstadoOrdenServicio.EnProgreso,
  };

  const cambiarEstadoRequest: CambiarEstadoOrdenServicioRequest = {
    estado: EstadoOrdenServicio.Finalizada,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(OrdenServicioService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería crearse', () => {
    expect(service).toBeTruthy();
  });

  it('listar debería obtener órdenes sin filtros', () => {
    service.listar().subscribe((ordenes) => {
      expect(ordenes).toEqual([ordenMock]);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.keys().length).toBe(0);
    req.flush([ordenMock]);
  });

  it('listar debería enviar todos los filtros proporcionados', () => {
    service
      .listar({
        estado: EstadoOrdenServicio.Pendiente,
        clienteId: 3,
        tecnicoId: 2,
        tecnicoBusqueda: 'Carlos',
        clienteBusqueda: 'Juan',
        fechaCreacionDesde: '2026-01-01',
        fechaCreacionHasta: '2026-12-31',
      })
      .subscribe();

    const req = httpMock.expectOne((request) => request.url === baseUrl);
    const params = req.request.params;
    expect(params.get('estado')).toBe(EstadoOrdenServicio.Pendiente);
    expect(params.get('clienteId')).toBe('3');
    expect(params.get('tecnicoId')).toBe('2');
    expect(params.get('tecnicoBusqueda')).toBe('Carlos');
    expect(params.get('clienteBusqueda')).toBe('Juan');
    expect(params.get('fechaCreacionDesde')).toBe('2026-01-01');
    expect(params.get('fechaCreacionHasta')).toBe('2026-12-31');
    req.flush([]);
  });

  it('obtenerPorId debería obtener una orden por id', () => {
    service.obtenerPorId(1).subscribe((orden) => {
      expect(orden).toEqual(ordenMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(ordenMock);
  });

  it('crear debería enviar POST con el request', () => {
    service.crear(crearRequest).subscribe((orden) => {
      expect(orden).toEqual(ordenMock);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(crearRequest);
    req.flush(ordenMock);
  });

  it('actualizar debería enviar PUT con el request', () => {
    service.actualizar(1, actualizarRequest).subscribe((orden) => {
      expect(orden).toEqual(ordenMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(actualizarRequest);
    req.flush(ordenMock);
  });

  it('cambiarEstado debería enviar PATCH al endpoint de estado', () => {
    service.cambiarEstado(1, cambiarEstadoRequest).subscribe((orden) => {
      expect(orden).toEqual(ordenMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1/estado`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(cambiarEstadoRequest);
    req.flush(ordenMock);
  });

  it('eliminar debería enviar DELETE', () => {
    service.eliminar(1).subscribe((response) => {
      expect(response).toBeNull();
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
