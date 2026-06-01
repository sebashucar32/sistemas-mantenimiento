import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TecnicoService } from './tecnico.service';
import {
  ActualizarTecnicoRequest,
  CrearTecnicoRequest,
  Tecnico,
} from '../models/tecnico';

describe('TecnicoService', () => {
  let service: TecnicoService;
  let httpMock: HttpTestingController;

  const baseUrl = '/api/Tecnicos';

  const tecnicoMock: Tecnico = {
    id: 1,
    nombre: 'Carlos Gómez',
    telefono: '3009876543',
    especialidad: 'Electricidad',
    activo: true,
  };

  const crearRequest: CrearTecnicoRequest = {
    nombre: 'Carlos Gómez',
    telefono: '3009876543',
    especialidad: 'Electricidad',
  };

  const actualizarRequest: ActualizarTecnicoRequest = {
    ...crearRequest,
    activo: false,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(TecnicoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería crearse', () => {
    expect(service).toBeTruthy();
  });

  it('listar debería obtener técnicos sin filtro', () => {
    service.listar().subscribe((tecnicos) => {
      expect(tecnicos).toEqual([tecnicoMock]);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.has('activo')).toBeFalse();
    req.flush([tecnicoMock]);
  });

  it('listar debería enviar el parámetro activo cuando se proporciona', () => {
    service.listar(false).subscribe();

    const req = httpMock.expectOne((request) => request.url === baseUrl);
    expect(req.request.params.get('activo')).toBe('false');
    req.flush([]);
  });

  it('obtenerPorId debería obtener un técnico por id', () => {
    service.obtenerPorId(1).subscribe((tecnico) => {
      expect(tecnico).toEqual(tecnicoMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(tecnicoMock);
  });

  it('crear debería enviar POST con el request', () => {
    service.crear(crearRequest).subscribe((tecnico) => {
      expect(tecnico).toEqual(tecnicoMock);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(crearRequest);
    req.flush(tecnicoMock);
  });

  it('actualizar debería enviar PUT con el request', () => {
    service.actualizar(1, actualizarRequest).subscribe((tecnico) => {
      expect(tecnico).toEqual(tecnicoMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(actualizarRequest);
    req.flush(tecnicoMock);
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
