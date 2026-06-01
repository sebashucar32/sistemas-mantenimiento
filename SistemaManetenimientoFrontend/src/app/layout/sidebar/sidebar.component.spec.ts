import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { SidebarComponent } from './sidebar.component';
import { LayoutService } from '../services/layout.service';

describe('SidebarComponent', () => {
  let fixture: ComponentFixture<SidebarComponent>;
  let layoutService: LayoutService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SidebarComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    layoutService = TestBed.inject(LayoutService);
    fixture = TestBed.createComponent(SidebarComponent);
    fixture.detectChanges();
  });

  it('debería crearse', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('debería renderizar los ítems de navegación', () => {
    const links = fixture.nativeElement.querySelectorAll('a');
    expect(links.length).toBe(4);
    expect(fixture.nativeElement.textContent).toContain('Inicio');
    expect(fixture.nativeElement.textContent).toContain('Clientes');
  });

  it('debería actualizar la ruta activa al hacer clic en un enlace', () => {
    const link = fixture.nativeElement.querySelector(
      'a[href="/clientes"]'
    ) as HTMLAnchorElement;
    link.click();
    fixture.detectChanges();

    expect(layoutService.activeNavRoute()).toBe('/clientes');
  });

  it('debería ocultar etiquetas cuando el sidebar está colapsado', () => {
    layoutService.toggleSidebar();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('Inicio');
  });
});
