import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { OrdenesServicioComponent } from './ordenes-servicio.component';
import { OrdenServicioService } from '../../services/orden-servicio.service';
import { TecnicoService } from '../../services/tecnico.service';
import { ClienteService } from '../../services/cliente.service';
import { EstadoOrdenServicio } from '../../models/enums/estado-orden-servicio';
import { OrdenServicio } from '../../models/orden-servicio';
import { Tecnico } from '../../models/tecnico';
import { Cliente } from '../../models/cliente';

describe('OrdenesServicioComponent', () => {
  let component: OrdenesServicioComponent;
  let fixture: ComponentFixture<OrdenesServicioComponent>;
  let ordenServicioService: jasmine.SpyObj<OrdenServicioService>;
  let tecnicoService: jasmine.SpyObj<TecnicoService>;
  let clienteService: jasmine.SpyObj<ClienteService>;

  const tecnicoMock: Tecnico = {
    id: 2,
    nombre: 'Carlos Gómez',
    telefono: '3009876543',
    especialidad: 'Electricidad',
    activo: true,
  };

  const clienteMock: Cliente = {
    id: 3,
    nombre: 'Juan Pérez',
    documentoIdentidad: '12345678',
    direccion: 'Calle 1',
    telefono: '3001234567',
    activo: true,
  };

  const ordenMock: OrdenServicio = {
    id: 1,
    fechaCreacion: '2026-05-31T10:00:00Z',
    estado: EstadoOrdenServicio.Pendiente,
    estadoDescripcion: 'Pendiente',
    descripcion: 'Reparación',
    tecnicoId: 2,
    tecnicoNombre: 'Carlos Gómez',
    tecnicoEspecialidad: 'Electricidad',
    clienteId: 3,
    clienteNombre: 'Juan Pérez',
    clienteDocumentoIdentidad: '12345678',
  };

  beforeEach(async () => {
    ordenServicioService = jasmine.createSpyObj('OrdenServicioService', [
      'listar',
      'crear',
      'actualizar',
      'cambiarEstado',
      'eliminar',
    ]);
    tecnicoService = jasmine.createSpyObj('TecnicoService', ['listar']);
    clienteService = jasmine.createSpyObj('ClienteService', ['listar']);

    ordenServicioService.listar.and.returnValue(of([ordenMock]));
    tecnicoService.listar.and.returnValue(of([tecnicoMock]));
    clienteService.listar.and.returnValue(of([clienteMock]));

    await TestBed.configureTestingModule({
      imports: [OrdenesServicioComponent],
      providers: [
        provideRouter([]),
        { provide: OrdenServicioService, useValue: ordenServicioService },
        { provide: TecnicoService, useValue: tecnicoService },
        { provide: ClienteService, useValue: clienteService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(OrdenesServicioComponent);
    component = fixture.componentInstance;
  });

  function initComponent(): void {
    fixture.detectChanges();
  }

  it('debería crearse y cargar datos al iniciar', () => {
    initComponent();
    expect(component.ordenes()).toEqual([ordenMock]);
    expect(component.tecnicos()).toEqual([tecnicoMock]);
    expect(component.clientes()).toEqual([clienteMock]);
    expect(tecnicoService.listar).toHaveBeenCalledWith(true);
    expect(clienteService.listar).toHaveBeenCalledWith(true);
  });

  it('cargarOrdenes debería filtrar por estado', () => {
    initComponent();
    component.filtroEstado.set(EstadoOrdenServicio.Pendiente);
    component.aplicarFiltroEstado();
    expect(ordenServicioService.listar).toHaveBeenCalledWith({
      estado: EstadoOrdenServicio.Pendiente,
    });
  });

  it('cargarOrdenes debería manejar errores', () => {
    ordenServicioService.listar.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'Error API' } }))
    );
    initComponent();
    expect(component.errorMessage()).toBe('Error API');
  });

  it('debería abrir modales con títulos correctos', () => {
    initComponent();
    expect(component.tituloModal()).toBe('');

    component.abrirCrear();
    expect(component.tituloModal()).toBe('Nueva orden de servicio');

    component.abrirEditar(ordenMock);
    expect(component.tituloModal()).toBe('Editar orden de servicio');

    component.abrirConsultar(ordenMock);
    expect(component.tituloModal()).toBe('Detalle de la orden');

    component.abrirCambiarEstado(ordenMock);
    expect(component.tituloModal()).toBe('Cambiar estado');
    expect(component.nuevoEstado()).toBe(EstadoOrdenServicio.Pendiente);

    component.abrirEliminar(ordenMock);
    expect(component.tituloModal()).toBe('Eliminar orden');

    component.cerrarModal();
    expect(component.modalMode()).toBeNull();
  });

  it('claseEstado debería retornar clases según el estado', () => {
    initComponent();
    expect(component.claseEstado(EstadoOrdenServicio.Pendiente)).toContain('amber');
    expect(component.claseEstado(EstadoOrdenServicio.EnProgreso)).toContain('blue');
    expect(component.claseEstado(EstadoOrdenServicio.Finalizada)).toContain('green');
    expect(
      component.claseEstado('Desconocido' as EstadoOrdenServicio)
    ).toContain('gray');
  });

  it('actualizarFormulario debería modificar el formulario', () => {
    initComponent();
    component.actualizarFormulario('descripcion', 'Nueva descripción');
    expect(component.formulario().descripcion).toBe('Nueva descripción');
  });

  it('guardar debería validar campos obligatorios', () => {
    initComponent();
    component.abrirCrear();

    component.guardar();
    expect(component.errorMessage()).toBe('La descripción es obligatoria.');

    component.actualizarFormulario('descripcion', 'Trabajo');
    component.guardar();
    expect(component.errorMessage()).toBe('Debe seleccionar un técnico.');

    component.actualizarFormulario('tecnicoId', 2);
    component.guardar();
    expect(component.errorMessage()).toBe('Debe seleccionar un cliente.');

    component.actualizarFormulario('clienteId', 3);
    component.actualizarFormulario('tecnicoId', 99);
    component.guardar();
    expect(component.errorMessage()).toBe(
      'El técnico seleccionado no existe o no está activo.'
    );

    component.actualizarFormulario('tecnicoId', 2);
    component.actualizarFormulario('clienteId', 99);
    component.guardar();
    expect(component.errorMessage()).toBe(
      'El cliente seleccionado no existe o no está activo.'
    );
  });

  it('guardar debería crear una orden', () => {
    ordenServicioService.crear.and.returnValue(of(ordenMock));
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('descripcion', 'Nueva orden');
    component.actualizarFormulario('tecnicoId', 2);
    component.actualizarFormulario('clienteId', 3);

    component.guardar();

    expect(ordenServicioService.crear).toHaveBeenCalledWith({
      descripcion: 'Nueva orden',
      tecnicoId: 2,
      clienteId: 3,
      estado: EstadoOrdenServicio.Pendiente,
    });
    expect(component.modalMode()).toBeNull();
  });

  it('guardar debería actualizar una orden', () => {
    ordenServicioService.actualizar.and.returnValue(of(ordenMock));
    initComponent();
    component.abrirEditar(ordenMock);
    component.actualizarFormulario('descripcion', 'Orden actualizada');

    component.guardar();

    expect(ordenServicioService.actualizar).toHaveBeenCalledWith(1, {
      descripcion: 'Orden actualizada',
      tecnicoId: 2,
      clienteId: 3,
      estado: EstadoOrdenServicio.Pendiente,
    });
  });

  it('guardar debería manejar errores', () => {
    ordenServicioService.crear.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: null }))
    );
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('descripcion', 'Nueva orden');
    component.actualizarFormulario('tecnicoId', 2);
    component.actualizarFormulario('clienteId', 3);

    component.guardar();
    expect(component.errorMessage()).toBe('Ocurrió un error al procesar la solicitud.');
  });

  it('confirmarCambioEstado no debería hacer nada sin orden seleccionada', () => {
    initComponent();
    component.confirmarCambioEstado();
    expect(ordenServicioService.cambiarEstado).not.toHaveBeenCalled();
  });

  it('confirmarCambioEstado debería cambiar el estado', () => {
    ordenServicioService.cambiarEstado.and.returnValue(of(ordenMock));
    initComponent();
    component.abrirCambiarEstado(ordenMock);
    component.nuevoEstado.set(EstadoOrdenServicio.Finalizada);

    component.confirmarCambioEstado();

    expect(ordenServicioService.cambiarEstado).toHaveBeenCalledWith(1, {
      estado: EstadoOrdenServicio.Finalizada,
    });
    expect(component.modalMode()).toBeNull();
  });

  it('confirmarCambioEstado debería manejar errores', () => {
    ordenServicioService.cambiarEstado.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'Error estado' } }))
    );
    initComponent();
    component.abrirCambiarEstado(ordenMock);

    component.confirmarCambioEstado();
    expect(component.errorMessage()).toBe('Error estado');
  });

  it('confirmarEliminar no debería hacer nada sin orden seleccionada', () => {
    initComponent();
    component.confirmarEliminar();
    expect(ordenServicioService.eliminar).not.toHaveBeenCalled();
  });

  it('confirmarEliminar debería eliminar la orden', () => {
    ordenServicioService.eliminar.and.returnValue(of(void 0));
    initComponent();
    component.abrirEliminar(ordenMock);

    component.confirmarEliminar();

    expect(ordenServicioService.eliminar).toHaveBeenCalledWith(1);
    expect(component.modalMode()).toBeNull();
  });

  it('confirmarEliminar debería manejar errores', () => {
    ordenServicioService.eliminar.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'Error eliminar' } }))
    );
    initComponent();
    component.abrirEliminar(ordenMock);

    component.confirmarEliminar();
    expect(component.errorMessage()).toBe('Error eliminar');
  });
});
