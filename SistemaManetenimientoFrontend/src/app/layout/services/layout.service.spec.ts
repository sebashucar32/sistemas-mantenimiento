import { TestBed } from '@angular/core/testing';
import { LayoutService } from './layout.service';

describe('LayoutService', () => {
  let service: LayoutService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LayoutService);
  });

  it('debería crearse', () => {
    expect(service).toBeTruthy();
  });

  it('debería iniciar con sidebar expandido', () => {
    expect(service.sidebarCollapsed()).toBeFalse();
    expect(service.sidebarWidth()).toBe('16rem');
  });

  it('toggleSidebar debería alternar el estado colapsado', () => {
    service.toggleSidebar();
    expect(service.sidebarCollapsed()).toBeTrue();
    expect(service.sidebarWidth()).toBe('4.5rem');

    service.toggleSidebar();
    expect(service.sidebarCollapsed()).toBeFalse();
    expect(service.sidebarWidth()).toBe('16rem');
  });

  it('debería iniciar con la ruta activa en /home', () => {
    expect(service.activeNavRoute()).toBe('/home');
  });

  it('setActiveNavRoute debería actualizar la ruta activa', () => {
    service.setActiveNavRoute('/clientes');
    expect(service.activeNavRoute()).toBe('/clientes');
  });

  it('navItems debería contener las rutas principales', () => {
    const routes = service.navItems().map((item) => item.route);
    expect(routes).toEqual([
      '/home',
      '/ordenes-servicio',
      '/tecnicos',
      '/clientes',
    ]);
  });
});
