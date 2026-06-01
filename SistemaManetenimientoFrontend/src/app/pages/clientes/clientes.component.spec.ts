import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ClientesComponent } from './clientes.component';
import { ClienteService } from '../../services/cliente.service';
import {
  Cliente,
  MENSAJE_DOCUMENTO_DUPLICADO,
  MENSAJE_TELEFONO_INVALIDO,
} from '../../models/cliente';

describe('ClientesComponent', () => {
  let component: ClientesComponent;
  let fixture: ComponentFixture<ClientesComponent>;
  let clienteService: jasmine.SpyObj<ClienteService>;

  const clienteMock: Cliente = {
    id: 1,
    nombre: 'Juan Pérez',
    documentoIdentidad: '12345678',
    direccion: 'Calle 1',
    telefono: '3001234567',
    activo: true,
  };

  const otroCliente: Cliente = {
    id: 2,
    nombre: 'María López',
    documentoIdentidad: '87654321',
    direccion: 'Calle 2',
    telefono: '3009876543',
    activo: false,
  };

  beforeEach(async () => {
    clienteService = jasmine.createSpyObj('ClienteService', [
      'listar',
      'crear',
      'actualizar',
      'eliminar',
    ]);
    clienteService.listar.and.returnValue(of([clienteMock, otroCliente]));

    await TestBed.configureTestingModule({
      imports: [ClientesComponent],
      providers: [
        provideRouter([]),
        { provide: ClienteService, useValue: clienteService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ClientesComponent);
    component = fixture.componentInstance;
  });

  function initComponent(): void {
    fixture.detectChanges();
  }

  it('debería crearse y cargar clientes al iniciar', () => {
    initComponent();
    expect(component.clientes()).toEqual([clienteMock, otroCliente]);
    expect(component.todosClientes()).toEqual([clienteMock, otroCliente]);
    expect(component.loading()).toBeFalse();
  });

  it('cargarClientes debería manejar errores', () => {
    clienteService.listar.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'Error API' } }))
    );
    initComponent();
    expect(component.errorMessage()).toBe('Error API');
    expect(component.loading()).toBeFalse();
  });

  it('cargarClientes debería usar filtro activos', () => {
    initComponent();
    component.filtroActivo.set('activos');
    component.aplicarFiltroActivo();
    expect(clienteService.listar).toHaveBeenCalledWith(true);
  });

  it('cargarClientes debería usar filtro inactivos', () => {
    initComponent();
    component.filtroActivo.set('inactivos');
    component.aplicarFiltroActivo();
    expect(clienteService.listar).toHaveBeenCalledWith(false);
  });

  it('debería abrir y cerrar modales', () => {
    initComponent();
    expect(component.tituloModal()).toBe('');

    component.abrirCrear();
    expect(component.modalMode()).toBe('crear');
    expect(component.tituloModal()).toBe('Nuevo cliente');

    component.cerrarModal();
    expect(component.modalMode()).toBeNull();
    expect(component.clienteSeleccionado()).toBeNull();

    component.abrirEditar(clienteMock);
    expect(component.modalMode()).toBe('editar');
    expect(component.tituloModal()).toBe('Editar cliente');
    expect(component.formulario().nombre).toBe('Juan Pérez');

    component.abrirConsultar(clienteMock);
    expect(component.modalMode()).toBe('consultar');
    expect(component.tituloModal()).toBe('Detalle del cliente');

    component.abrirEliminar(clienteMock);
    expect(component.modalMode()).toBe('eliminar');
    expect(component.tituloModal()).toBe('Eliminar cliente');
  });

  it('actualizarFormulario debería modificar el formulario', () => {
    initComponent();
    component.actualizarFormulario('nombre', 'Nuevo nombre');
    expect(component.formulario().nombre).toBe('Nuevo nombre');
  });

  it('guardar debería validar campos obligatorios', () => {
    initComponent();
    component.abrirCrear();

    component.guardar();
    expect(component.errorMessage()).toBe('El nombre completo es obligatorio.');

    component.actualizarFormulario('nombre', 'Test');
    component.guardar();
    expect(component.errorMessage()).toBe('El documento de identidad es obligatorio.');

    component.actualizarFormulario('documentoIdentidad', '111');
    component.guardar();
    expect(component.errorMessage()).toBe('La dirección es obligatoria.');

    component.actualizarFormulario('direccion', 'Dir');
    component.guardar();
    expect(component.errorMessage()).toBe('El teléfono es obligatorio.');

    component.actualizarFormulario('telefono', '123');
    component.guardar();
    expect(component.errorMessage()).toBe(MENSAJE_TELEFONO_INVALIDO);
  });

  it('guardar debería detectar documento duplicado', () => {
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('nombre', 'Nuevo');
    component.actualizarFormulario('documentoIdentidad', '12345678');
    component.actualizarFormulario('direccion', 'Dir');
    component.actualizarFormulario('telefono', '3001234567');

    component.guardar();
    expect(component.errorMessage()).toBe(MENSAJE_DOCUMENTO_DUPLICADO);
  });

  it('guardar debería crear un cliente', () => {
    clienteService.crear.and.returnValue(of(clienteMock));
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('nombre', 'Nuevo Cliente');
    component.actualizarFormulario('documentoIdentidad', '99999999');
    component.actualizarFormulario('direccion', 'Nueva Dir');
    component.actualizarFormulario('telefono', '3001112233');

    component.guardar();

    expect(clienteService.crear).toHaveBeenCalledWith({
      nombre: 'Nuevo Cliente',
      documentoIdentidad: '99999999',
      direccion: 'Nueva Dir',
      telefono: '3001112233',
    });
    expect(component.saving()).toBeFalse();
    expect(component.modalMode()).toBeNull();
  });

  it('guardar debería actualizar un cliente', () => {
    clienteService.actualizar.and.returnValue(of(clienteMock));
    initComponent();
    component.abrirEditar(clienteMock);
    component.actualizarFormulario('nombre', 'Juan Actualizado');

    component.guardar();

    expect(clienteService.actualizar).toHaveBeenCalledWith(1, {
      nombre: 'Juan Actualizado',
      documentoIdentidad: '12345678',
      direccion: 'Calle 1',
      telefono: '3001234567',
      activo: true,
    });
    expect(component.modalMode()).toBeNull();
  });

  it('guardar debería manejar errores del servicio', () => {
    clienteService.crear.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: null }))
    );
    initComponent();
    component.abrirCrear();
    component.actualizarFormulario('nombre', 'Nuevo');
    component.actualizarFormulario('documentoIdentidad', '99999999');
    component.actualizarFormulario('direccion', 'Dir');
    component.actualizarFormulario('telefono', '3001112233');

    component.guardar();
    expect(component.errorMessage()).toBe('Ocurrió un error al procesar la solicitud.');
    expect(component.saving()).toBeFalse();
  });

  it('confirmarEliminar no debería hacer nada sin cliente seleccionado', () => {
    initComponent();
    component.confirmarEliminar();
    expect(clienteService.eliminar).not.toHaveBeenCalled();
  });

  it('confirmarEliminar debería eliminar el cliente', () => {
    clienteService.eliminar.and.returnValue(of(void 0));
    initComponent();
    component.abrirEliminar(clienteMock);

    component.confirmarEliminar();

    expect(clienteService.eliminar).toHaveBeenCalledWith(1);
    expect(component.modalMode()).toBeNull();
    expect(component.saving()).toBeFalse();
  });

  it('confirmarEliminar debería manejar errores', () => {
    clienteService.eliminar.and.returnValue(
      throwError(() => new HttpErrorResponse({ error: { mensaje: 'No se puede eliminar' } }))
    );
    initComponent();
    component.abrirEliminar(clienteMock);

    component.confirmarEliminar();
    expect(component.errorMessage()).toBe('No se puede eliminar');
    expect(component.saving()).toBeFalse();
  });
});
