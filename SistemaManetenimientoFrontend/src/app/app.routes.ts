import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { OrdenesServicioComponent } from './pages/ordenes-servicio/ordenes-servicio.component';
import { ClientesComponent } from './pages/clientes/clientes.component';
import { TecnicosComponent } from './pages/tecnicos/tecnicos.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
  { path: 'ordenes-servicio', component: OrdenesServicioComponent },
  { path: 'tecnicos', component: TecnicosComponent },
  { path: 'clientes', component: ClientesComponent },
  { path: '**', redirectTo: 'login' },
];
