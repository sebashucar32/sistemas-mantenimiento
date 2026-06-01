import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ClienteService } from './cliente.service';
import {
  ActualizarClienteRequest,
  Cliente,
  CrearClienteRequest,
} from '../models/cliente';

describe('ClienteService', () => {
  let service: ClienteService;
  let httpMock: HttpTestingController;

  const baseUrl = '/api/Clientes';

  const clienteMock: Cliente = {
    id: 1,
    nombre: 'Juan Pérez',
    documentoIdentidad: '12345678',
    direccion: 'Calle 1 #2-3',
    telefono: '3001234567',
    activo: true,
  };

  const crearRequest: CrearClienteRequest = {
    nombre: 'Juan Pérez',
    documentoIdentidad: '12345678',
    direccion: 'Calle 1 #2-3',
    telefono: '3001234567',
  };

  const actualizarRequest: ActualizarClienteRequest = {
    ...crearRequest,
    activo: false,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(ClienteService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería crearse', () => {
    expect(service).toBeTruthy();
  });

  it('listar debería obtener clientes sin filtro', () => {
    service.listar().subscribe((clientes) => {
      expect(clientes).toEqual([clienteMock]);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.has('activo')).toBeFalse();
    req.flush([clienteMock]);
  });

  it('listar debería enviar el parámetro activo cuando se proporciona', () => {
    service.listar(true).subscribe();

    const req = httpMock.expectOne((request) => request.url === baseUrl);
    expect(req.request.params.get('activo')).toBe('true');
    req.flush([]);
  });

  it('obtenerPorId debería obtener un cliente por id', () => {
    service.obtenerPorId(1).subscribe((cliente) => {
      expect(cliente).toEqual(clienteMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(clienteMock);
  });

  it('crear debería enviar POST con el request', () => {
    service.crear(crearRequest).subscribe((cliente) => {
      expect(cliente).toEqual(clienteMock);
    });

    const req = httpMock.expectOne(baseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(crearRequest);
    req.flush(clienteMock);
  });

  it('actualizar debería enviar PUT con el request', () => {
    service.actualizar(1, actualizarRequest).subscribe((cliente) => {
      expect(cliente).toEqual(clienteMock);
    });

    const req = httpMock.expectOne(`${baseUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(actualizarRequest);
    req.flush(clienteMock);
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
