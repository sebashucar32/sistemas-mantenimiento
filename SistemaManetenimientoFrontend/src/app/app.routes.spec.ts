import { routes } from './app.routes';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { OrdenesServicioComponent } from './pages/ordenes-servicio/ordenes-servicio.component';
import { TecnicosComponent } from './pages/tecnicos/tecnicos.component';
import { ClientesComponent } from './pages/clientes/clientes.component';

describe('app.routes', () => {
  it('debería definir las rutas principales', () => {
    expect(routes.length).toBe(7);
    expect(routes[0]).toEqual({ path: '', redirectTo: 'login', pathMatch: 'full' });
    expect(routes[1].path).toBe('login');
    expect(routes[1].component).toBe(LoginComponent);
    expect(routes[2].path).toBe('home');
    expect(routes[2].component).toBe(HomeComponent);
    expect(routes[3].path).toBe('ordenes-servicio');
    expect(routes[3].component).toBe(OrdenesServicioComponent);
    expect(routes[4].path).toBe('tecnicos');
    expect(routes[4].component).toBe(TecnicosComponent);
    expect(routes[5].path).toBe('clientes');
    expect(routes[5].component).toBe(ClientesComponent);
    expect(routes[6]).toEqual({ path: '**', redirectTo: 'login' });
  });
});
