import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BodyComponent } from '../../layout/body/body.component';
import { FooterComponent } from '../../layout/footer/footer.component';
import { HeaderComponent } from '../../layout/header/header.component';
import { SidebarComponent } from '../../layout/sidebar/sidebar.component';
import { ApiError } from '../../models/api-error';
import {
  Cliente,
  ClienteFormulario,
  MENSAJE_DOCUMENTO_DUPLICADO,
  MENSAJE_TELEFONO_INVALIDO,
  telefonoEsValido,
} from '../../models/cliente';
import { ClienteService } from '../../services/cliente.service';

type ModalMode = 'crear' | 'editar' | 'consultar' | 'eliminar' | null;

@Component({
  selector: 'app-clientes',
  imports: [
    FormsModule,
    HeaderComponent,
    SidebarComponent,
    BodyComponent,
    FooterComponent,
  ],
  templateUrl: './clientes.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientesComponent implements OnInit {
  private readonly clienteService = inject(ClienteService);

  readonly clientes = signal<Cliente[]>([]);
  readonly todosClientes = signal<Cliente[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal('');
  readonly modalMode = signal<ModalMode>(null);
  readonly clienteSeleccionado = signal<Cliente | null>(null);
  readonly filtroActivo = signal<'todos' | 'activos' | 'inactivos'>('todos');

  readonly formulario = signal<ClienteFormulario>(this.crearFormularioVacio());

  ngOnInit(): void {
    this.cargarClientes();
    this.cargarTodosClientes();
  }

  cargarClientes(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    const filtro = this.filtroActivo();
    const activo =
      filtro === 'activos' ? true : filtro === 'inactivos' ? false : undefined;

    this.clienteService.listar(activo).subscribe({
      next: (clientes) => {
        this.clientes.set(clientes);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.loading.set(false);
      },
    });
  }

  aplicarFiltroActivo(): void {
    this.cargarClientes();
  }

  abrirCrear(): void {
    this.formulario.set(this.crearFormularioVacio());
    this.errorMessage.set('');
    this.modalMode.set('crear');
  }

  abrirEditar(cliente: Cliente): void {
    this.clienteSeleccionado.set(cliente);
    this.formulario.set({
      nombre: cliente.nombre,
      documentoIdentidad: cliente.documentoIdentidad,
      direccion: cliente.direccion,
      telefono: cliente.telefono,
      activo: cliente.activo,
    });
    this.errorMessage.set('');
    this.modalMode.set('editar');
  }

  abrirConsultar(cliente: Cliente): void {
    this.clienteSeleccionado.set(cliente);
    this.errorMessage.set('');
    this.modalMode.set('consultar');
  }

  abrirEliminar(cliente: Cliente): void {
    this.clienteSeleccionado.set(cliente);
    this.errorMessage.set('');
    this.modalMode.set('eliminar');
  }

  cerrarModal(): void {
    this.modalMode.set(null);
    this.clienteSeleccionado.set(null);
    this.errorMessage.set('');
  }

  guardar(): void {
    const form = this.formulario();
    const excludeId =
      this.modalMode() === 'editar' ? this.clienteSeleccionado()?.id : undefined;
    const errorValidacion = this.validarFormulario(form, excludeId);

    if (errorValidacion) {
      this.errorMessage.set(errorValidacion);
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    const request = {
      nombre: form.nombre.trim(),
      documentoIdentidad: form.documentoIdentidad.trim(),
      direccion: form.direccion.trim(),
      telefono: form.telefono.trim(),
    };

    const operacion =
      this.modalMode() === 'editar' && this.clienteSeleccionado()
        ? this.clienteService.actualizar(this.clienteSeleccionado()!.id, {
            ...request,
            activo: form.activo,
          })
        : this.clienteService.crear(request);

    operacion.subscribe({
      next: () => {
        this.saving.set(false);
        this.cerrarModal();
        this.cargarClientes();
        this.cargarTodosClientes();
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.saving.set(false);
      },
    });
  }

  confirmarEliminar(): void {
    const cliente = this.clienteSeleccionado();
    if (!cliente) {
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    this.clienteService.eliminar(cliente.id).subscribe({
      next: () => {
        this.saving.set(false);
        this.cerrarModal();
        this.cargarClientes();
        this.cargarTodosClientes();
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.saving.set(false);
      },
    });
  }

  actualizarFormulario(campo: keyof ClienteFormulario, valor: unknown): void {
    this.formulario.update((form) => ({
      ...form,
      [campo]: valor,
    }));
  }

  tituloModal(): string {
    switch (this.modalMode()) {
      case 'crear':
        return 'Nuevo cliente';
      case 'editar':
        return 'Editar cliente';
      case 'consultar':
        return 'Detalle del cliente';
      case 'eliminar':
        return 'Eliminar cliente';
      default:
        return '';
    }
  }

  private cargarTodosClientes(): void {
    this.clienteService.listar().subscribe({
      next: (clientes) => this.todosClientes.set(clientes),
    });
  }

  private crearFormularioVacio(): ClienteFormulario {
    return {
      nombre: '',
      documentoIdentidad: '',
      direccion: '',
      telefono: '',
      activo: true,
    };
  }

  private validarFormulario(
    form: ClienteFormulario,
    excludeId?: number
  ): string | null {
    if (!form.nombre.trim()) {
      return 'El nombre completo es obligatorio.';
    }

    if (!form.documentoIdentidad.trim()) {
      return 'El documento de identidad es obligatorio.';
    }

    if (this.documentoDuplicado(form.documentoIdentidad, excludeId)) {
      return MENSAJE_DOCUMENTO_DUPLICADO;
    }

    if (!form.direccion.trim()) {
      return 'La dirección es obligatoria.';
    }

    if (!form.telefono.trim()) {
      return 'El teléfono es obligatorio.';
    }

    if (!telefonoEsValido(form.telefono)) {
      return MENSAJE_TELEFONO_INVALIDO;
    }

    return null;
  }

  private documentoDuplicado(documento: string, excludeId?: number): boolean {
    const docNormalizado = documento.trim().toLowerCase();
    return this.todosClientes().some(
      (cliente) =>
        cliente.documentoIdentidad.trim().toLowerCase() === docNormalizado &&
        cliente.id !== excludeId
    );
  }

  private obtenerMensajeError(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      const apiError = error.error as ApiError | null;
      if (apiError?.mensaje) {
        return apiError.mensaje;
      }
    }

    return 'Ocurrió un error al procesar la solicitud.';
  }
}
