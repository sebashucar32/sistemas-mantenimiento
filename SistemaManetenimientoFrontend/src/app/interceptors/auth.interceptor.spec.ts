import { TestBed } from '@angular/core/testing';
import {
  HttpClient,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { authInterceptor } from './auth.interceptor';
import { AuthService } from '../services/auth.service';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authService = jasmine.createSpyObj('AuthService', ['getToken']);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: authService },
      ],
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debería agregar Authorization cuando existe token', () => {
    authService.getToken.and.returnValue('jwt-token');

    http.get('/api/Clientes').subscribe();
    const req = httpMock.expectOne('/api/Clientes');

    expect(req.request.headers.get('Authorization')).toBe('Bearer jwt-token');
    req.flush([]);
  });

  it('no debería agregar Authorization en login', () => {
    authService.getToken.and.returnValue('jwt-token');

    http.post('/api/auth/login', {}).subscribe();
    const req = httpMock.expectOne('/api/auth/login');

    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });

  it('no debería agregar Authorization sin token', () => {
    authService.getToken.and.returnValue(null);

    http.get('/api/Tecnicos').subscribe();
    const req = httpMock.expectOne('/api/Tecnicos');

    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush([]);
  });
});
