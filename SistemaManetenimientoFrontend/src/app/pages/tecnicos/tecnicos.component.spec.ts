import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { TecnicosComponent } from './tecnicos.component';
import { TecnicoService } from '../../services/tecnico.service';
import {
  MENSAJE_TELEFONO_INVALIDO,
  Tecnico,
} from '../../models/tecnico';

describe('TecnicosComponent', () => {
  let component: TecnicosComponent;
  let fixture: ComponentFixture<TecnicosComponent>;
  let tecnicoService: jasmine.SpyObj<TecnicoService>;

  const tecnicoMock: Tecnico = {
    id: 1,
    nombre: 'Carlos Gómez',
    telefono: '3009876543',
    especialidad: 'Electricidad',
    activo: true,
  };

  beforeEach(async () => {
    tecnicoService = jasmine.createSpyObj('TecnicoService', [
      'listar',
      'crear',
      'actualizar',
      'eliminar',
    ]);
    tecnicoService.listar.and.returnValue(of([tecnicoMock]));

    await TestBed.configureTestingModule({
      imports: [TecnicosComponent],
      providers: [
        provideRouter([]),
        { provide: TecnicoService, useValue: tecnicoService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TecnicosComponent);
    component = fixture.componentInstance;
  });

  function initComponent(): void {
    fixture.detectChanges();
  }

  it('debería crearse y cargar técnicos al iniciar', () => {
    initComponent();
    expect(component.tecnicos()).toEqual([tecnicoMock]);
    expect(component.loading()).toBeFalse();
  });

  it('cargarTecnicos debería manejar errores genéricos', () => {
    tecnicoService.listar.and.returnValue(throwError(() => new Error('fail')));
    initComponent();
    expect(component.errorMessage()).toBe('Ocurrió un error al procesar la solicitud.');
  });

  it('cargarTecnicos debería manejar error 401', () => {
    tecnicoService.listar.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 401 }))
    );
    initComponent();
    expect(component.errorMessage()).toBe(
      'Sesión no válida o expirada. Inicie sesión nuevamente.'
    );
  });

  it('cargarTecnicos debería manejar errores de validación', () => {
    tecnicoService.listar.and.returnValue(
      throwError(
        () =>
          new HttpErrorResponse({
            error: { errors: { nombre: ['Campo requerido'] } },
          })
      )
    );
    initComponent();
    expect(component.errorMessage()).toBe('Campo requerido');
  });

  it('cargarTecnicos debería manejar mensaje de API', () => {
    tecnicoService.listar.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'Error API' } }))
    );
    initComponent();
    expect(component.errorMessage()).toBe('Error API');
  });

  it('aplicarFiltroActivo debería filtrar activos e inactivos', () => {
    initComponent();
    component.filtroActivo.set('activos');
    component.aplicarFiltroActivo();
    expect(tecnicoService.listar).toHaveBeenCalledWith(true);

    component.filtroActivo.set('inactivos');
    component.aplicarFiltroActivo();
    expect(tecnicoService.listar).toHaveBeenCalledWith(false);
  });

  it('debería abrir modales con títulos correctos', () => {
    initComponent();
    expect(component.tituloModal()).toBe('');

    component.abrirCrear();
    expect(component.tituloModal()).toBe('Nuevo técnico');

    component.abrirEditar(tecnicoMock);
    expect(component.tituloModal()).toBe('Editar técnico');

    component.abrirConsultar(tecnicoMock);
    expect(component.tituloModal()).toBe('Detalle del técnico');

    component.abrirEliminar(tecnicoMock);
    expect(component.tituloModal()).toBe('Eliminar técnico');

    component.cerrarModal();
    expect(component.modalMode()).toBeNull();
  });

  it('actualizarFormulario debería modificar el formulario', () => {
    initComponent();
    component.actualizarFormulario('especialidad', 'Plomería');
    expect(component.formulario().especialidad).toBe('Plomería');
  });

  it('guardar debería validar campos obligatorios', () => {
    initComponent();
    component.abrirCrear();

    component.guardar();
    expect(component.errorMessage()).toBe('El nombre completo es obligatorio.');

    component.actualizarFormulario('nombre', 'Test');
    component.guardar();
    expect(component.errorMessage()).toBe('El teléfono es obligatorio.');

    component.actualizarFormulario('telefono', '123');
    component.guardar();
    expect(component.errorMessage()).toBe(MENSAJE_TELEFONO_INVALIDO);

    component.actualizarFormulario('telefono', '3001234567');
    component.guardar();
    expect(component.errorMessage()).toBe('La especialidad es obligatoria.');
  });

  it('guardar debería crear un técnico', () => {
    tecnicoService.crear.and.returnValue(of(tecnicoMock));
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('nombre', 'Nuevo Técnico');
    component.actualizarFormulario('telefono', '3001112233');
    component.actualizarFormulario('especialidad', 'Gas');

    component.guardar();

    expect(tecnicoService.crear).toHaveBeenCalledWith({
      nombre: 'Nuevo Técnico',
      telefono: '3001112233',
      especialidad: 'Gas',
    });
    expect(component.modalMode()).toBeNull();
  });

  it('guardar debería actualizar un técnico', () => {
    tecnicoService.actualizar.and.returnValue(of(tecnicoMock));
    initComponent();
    component.abrirEditar(tecnicoMock);
    component.actualizarFormulario('nombre', 'Carlos Actualizado');

    component.guardar();

    expect(tecnicoService.actualizar).toHaveBeenCalledWith(1, {
      nombre: 'Carlos Actualizado',
      telefono: '3009876543',
      especialidad: 'Electricidad',
      activo: true,
    });
  });

  it('guardar debería manejar errores', () => {
    tecnicoService.crear.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'Error al crear' } }))
    );
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('nombre', 'Nuevo');
    component.actualizarFormulario('telefono', '3001112233');
    component.actualizarFormulario('especialidad', 'Gas');

    component.guardar();
    expect(component.errorMessage()).toBe('Error al crear');
  });

  it('confirmarEliminar no debería hacer nada sin técnico seleccionado', () => {
    initComponent();
    component.confirmarEliminar();
    expect(tecnicoService.eliminar).not.toHaveBeenCalled();
  });

  it('confirmarEliminar debería eliminar el técnico', () => {
    tecnicoService.eliminar.and.returnValue(of(void 0));
    initComponent();
    component.abrirEliminar(tecnicoMock);

    component.confirmarEliminar();

    expect(tecnicoService.eliminar).toHaveBeenCalledWith(1);
    expect(component.modalMode()).toBeNull();
  });

  it('confirmarEliminar debería manejar errores', () => {
    tecnicoService.eliminar.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: null }))
    );
    initComponent();
    component.abrirEliminar(tecnicoMock);

    component.confirmarEliminar();
    expect(component.errorMessage()).toBe('Ocurrió un error al procesar la solicitud.');
  });
});
