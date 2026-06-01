import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BodyComponent } from './body.component';

describe('BodyComponent', () => {
  let fixture: ComponentFixture<BodyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BodyComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BodyComponent);
  });

  it('debería crearse con valores por defecto', () => {
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Panel principal');
    expect(compiled.textContent).toContain('Bienvenido al sistema de mantenimiento');
  });

  it('debería mostrar título y subtítulo personalizados', () => {
    fixture.componentRef.setInput('title', 'Clientes');
    fixture.componentRef.setInput('subtitle', 'Gestión de clientes');
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Clientes');
    expect(compiled.textContent).toContain('Gestión de clientes');
  });
});
