import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  const credentials: LoginRequest = {
    nombreUsuario: 'admin',
    contrasena: 'secret',
  };

  const loginResponse: LoginResponse = {
    token: 'jwt-token-123',
    expiraEn: '2026-05-31T12:00:00Z',
    usuario: {
      id: 1,
      nombreUsuario: 'admin',
      nombreCompleto: 'Administrador',
    },
  };

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('debería crearse', () => {
    expect(service).toBeTruthy();
  });

  it('login debería enviar POST y guardar el token', () => {
    service.login(credentials).subscribe((response) => {
      expect(response).toEqual(loginResponse);
      expect(service.getToken()).toBe('jwt-token-123');
      expect(service.isAuthenticated()).toBeTrue();
    });

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(credentials);
    req.flush(loginResponse);
  });

  it('getToken debería retornar null cuando no hay token', () => {
    expect(service.getToken()).toBeNull();
  });

  it('setToken debería persistir el token en localStorage', () => {
    service.setToken('mi-token');
    expect(localStorage.getItem('auth_token')).toBe('mi-token');
    expect(service.getToken()).toBe('mi-token');
  });

  it('logout debería eliminar el token', () => {
    service.setToken('mi-token');
    service.logout();
    expect(service.getToken()).toBeNull();
    expect(service.isAuthenticated()).toBeFalse();
  });

  it('isAuthenticated debería retornar false sin token', () => {
    expect(service.isAuthenticated()).toBeFalse();
  });
});
