import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HeaderComponent } from './header.component';
import { LayoutService } from '../services/layout.service';

describe('HeaderComponent', () => {
  let fixture: ComponentFixture<HeaderComponent>;
  let layoutService: LayoutService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HeaderComponent],
    }).compileComponents();

    layoutService = TestBed.inject(LayoutService);
    fixture = TestBed.createComponent(HeaderComponent);
    fixture.detectChanges();
  });

  it('debería crearse', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('debería alternar el sidebar al hacer clic en el botón', () => {
    expect(layoutService.sidebarCollapsed()).toBeFalse();

    const button = fixture.nativeElement.querySelector(
      'button[aria-label="Alternar menú lateral"]'
    ) as HTMLButtonElement;
    button.click();
    fixture.detectChanges();

    expect(layoutService.sidebarCollapsed()).toBeTrue();
  });

  it('debería mostrar el título del sistema', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Mantenimiento');
    expect(compiled.textContent).toContain('Administrador');
  });
});
