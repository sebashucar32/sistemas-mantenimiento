import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
  let fixture: ComponentFixture<HomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    fixture.detectChanges();
  });

  it('debería crearse', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('debería mostrar las tarjetas de resumen', () => {
    const cards = fixture.componentInstance.summaryCards();
    expect(cards.length).toBe(3);
    expect(cards[0].title).toBe('Equipos activos');
    expect(cards[1].title).toBe('Mantenimientos pendientes');
    expect(cards[2].title).toBe('Órdenes completadas');
  });

  it('debería renderizar el layout principal', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-header')).not.toBeNull();
    expect(compiled.querySelector('app-sidebar')).not.toBeNull();
    expect(compiled.querySelector('app-body')).not.toBeNull();
    expect(compiled.querySelector('app-footer')).not.toBeNull();
  });
});
