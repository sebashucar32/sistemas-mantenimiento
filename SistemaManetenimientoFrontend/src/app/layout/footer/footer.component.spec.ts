import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FooterComponent } from './footer.component';

describe('FooterComponent', () => {
  let fixture: ComponentFixture<FooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FooterComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(FooterComponent);
    fixture.detectChanges();
  });

  it('debería crearse', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('debería mostrar el copyright con el año actual', () => {
    const year = new Date().getFullYear();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain(`© ${year} Sistema de Mantenimiento`);
    expect(compiled.textContent).toContain('v1.0.0');
  });
});
