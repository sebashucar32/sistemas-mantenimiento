import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';
import { LoginResponse } from '../../models/login-response';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;

  const loginResponse: LoginResponse = {
    token: 'token-123',
    expiraEn: '2026-05-31T12:00:00Z',
    usuario: {
      id: 1,
      nombreUsuario: 'admin',
      nombreCompleto: 'Administrador',
    },
  };

  beforeEach(async () => {
    authService = jasmine.createSpyObj('AuthService', ['login']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.returnValue(Promise.resolve(true));
  });

  it('debería crearse', () => {
    expect(component).toBeTruthy();
  });

  it('onSubmit debería autenticar y navegar a home', () => {
    authService.login.and.returnValue(of(loginResponse));
    component.nombreUsuario = 'admin';
    component.contrasena = 'secret';

    component.onSubmit();

    expect(authService.login).toHaveBeenCalledWith({
      nombreUsuario: 'admin',
      contrasena: 'secret',
    });
    expect(component.loading).toBeFalse();
    expect(component.errorMessage).toBe('');
    expect(router.navigate).toHaveBeenCalledWith(['/home']);
  });

  it('onSubmit debería mostrar error cuando falla la autenticación', () => {
    authService.login.and.returnValue(
      throwError(() => new Error('Unauthorized'))
    );
    component.nombreUsuario = 'admin';
    component.contrasena = 'wrong';

    component.onSubmit();

    expect(component.loading).toBeFalse();
    expect(component.errorMessage).toBe('Autenticación fallida');
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
