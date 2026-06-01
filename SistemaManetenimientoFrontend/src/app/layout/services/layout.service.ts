import { Injectable, computed, signal } from '@angular/core';
import { NavItem } from '../models/nav-item.model';

@Injectable({ providedIn: 'root' })
export class LayoutService {
  readonly sidebarCollapsed = signal(false);
  readonly activeNavRoute = signal('/home');

  readonly navItems = signal<NavItem[]>([
    { label: 'Inicio', route: '/home', icon: 'home' },
    { label: 'Ordenes y servicios', route: '/ordenes-servicio', icon: 'equipment' },
    { label: 'Tecnicos', route: '/tecnicos', icon: 'maintenance' },
    { label: 'Clientes', route: '/clientes', icon: 'reports' },
  ]);

  readonly sidebarWidth = computed(() =>
    this.sidebarCollapsed() ? '4.5rem' : '16rem'
  );

  toggleSidebar(): void {
    this.sidebarCollapsed.update((collapsed) => !collapsed);
  }

  setActiveNavRoute(route: string): void {
    this.activeNavRoute.set(route);
  }
}
