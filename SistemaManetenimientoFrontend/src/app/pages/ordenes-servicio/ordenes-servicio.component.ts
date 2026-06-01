import { DatePipe } from '@angular/common';
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
import { Cliente } from '../../models/cliente';
import {
  ESTADOS_ORDEN_SERVICIO,
  EstadoOrdenServicio,
} from '../../models/enums/estado-orden-servicio';
import { OrdenServicio, OrdenServicioFormulario } from '../../models/orden-servicio';
import { Tecnico } from '../../models/tecnico';
import { ClienteService } from '../../services/cliente.service';
import { OrdenServicioService } from '../../services/orden-servicio.service';
import { TecnicoService } from '../../services/tecnico.service';

type ModalMode = 'crear' | 'editar' | 'consultar' | 'estado' | 'eliminar' | null;

@Component({
  selector: 'app-ordenes-servicio',
  imports: [
    DatePipe,
    FormsModule,
    HeaderComponent,
    SidebarComponent,
    BodyComponent,
    FooterComponent,
  ],
  templateUrl: './ordenes-servicio.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrdenesServicioComponent implements OnInit {
  private readonly ordenServicioService = inject(OrdenServicioService);
  private readonly tecnicoService = inject(TecnicoService);
  private readonly clienteService = inject(ClienteService);

  readonly estados = ESTADOS_ORDEN_SERVICIO;
  readonly ordenes = signal<OrdenServicio[]>([]);
  readonly tecnicos = signal<Tecnico[]>([]);
  readonly clientes = signal<Cliente[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal('');
  readonly modalMode = signal<ModalMode>(null);
  readonly ordenSeleccionada = signal<OrdenServicio | null>(null);
  readonly filtroEstado = signal<EstadoOrdenServicio | ''>('');

  readonly formulario = signal<OrdenServicioFormulario>(this.crearFormularioVacio());
  readonly nuevoEstado = signal<EstadoOrdenServicio>(EstadoOrdenServicio.Pendiente);

  ngOnInit(): void {
    this.cargarCatalogos();
    this.cargarOrdenes();
  }

  cargarOrdenes(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    const estado = this.filtroEstado();
    this.ordenServicioService
      .listar(estado ? { estado } : {})
      .subscribe({
        next: (ordenes) => {
          this.ordenes.set(ordenes);
          this.loading.set(false);
        },
        error: (error) => {
          this.errorMessage.set(this.obtenerMensajeError(error));
          this.loading.set(false);
        },
      });
  }

  aplicarFiltroEstado(): void {
    this.cargarOrdenes();
  }

  abrirCrear(): void {
    this.formulario.set(this.crearFormularioVacio());
    this.errorMessage.set('');
    this.modalMode.set('crear');
  }

  abrirEditar(orden: OrdenServicio): void {
    this.ordenSeleccionada.set(orden);
    this.formulario.set({
      descripcion: orden.descripcion,
      tecnicoId: orden.tecnicoId,
      clienteId: orden.clienteId,
      estado: orden.estado,
    });
    this.errorMessage.set('');
    this.modalMode.set('editar');
  }

  abrirConsultar(orden: OrdenServicio): void {
    this.ordenSeleccionada.set(orden);
    this.errorMessage.set('');
    this.modalMode.set('consultar');
  }

  abrirCambiarEstado(orden: OrdenServicio): void {
    this.ordenSeleccionada.set(orden);
    this.nuevoEstado.set(orden.estado);
    this.errorMessage.set('');
    this.modalMode.set('estado');
  }

  abrirEliminar(orden: OrdenServicio): void {
    this.ordenSeleccionada.set(orden);
    this.errorMessage.set('');
    this.modalMode.set('eliminar');
  }

  cerrarModal(): void {
    this.modalMode.set(null);
    this.ordenSeleccionada.set(null);
    this.errorMessage.set('');
  }

  guardar(): void {
    const form = this.formulario();
    const errorValidacion = this.validarFormulario(form);

    if (errorValidacion) {
      this.errorMessage.set(errorValidacion);
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    const request = {
      descripcion: form.descripcion.trim(),
      tecnicoId: form.tecnicoId!,
      clienteId: form.clienteId!,
      estado: form.estado,
    };

    const operacion =
      this.modalMode() === 'editar' && this.ordenSeleccionada()
        ? this.ordenServicioService.actualizar(
            this.ordenSeleccionada()!.id,
            request
          )
        : this.ordenServicioService.crear(request);

    operacion.subscribe({
      next: () => {
        this.saving.set(false);
        this.cerrarModal();
        this.cargarOrdenes();
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.saving.set(false);
      },
    });
  }

  confirmarCambioEstado(): void {
    const orden = this.ordenSeleccionada();
    if (!orden) {
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    this.ordenServicioService
      .cambiarEstado(orden.id, { estado: this.nuevoEstado() })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.cerrarModal();
          this.cargarOrdenes();
        },
        error: (error) => {
          this.errorMessage.set(this.obtenerMensajeError(error));
          this.saving.set(false);
        },
      });
  }

  confirmarEliminar(): void {
    const orden = this.ordenSeleccionada();
    if (!orden) {
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    this.ordenServicioService.eliminar(orden.id).subscribe({
      next: () => {
        this.saving.set(false);
        this.cerrarModal();
        this.cargarOrdenes();
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.saving.set(false);
      },
    });
  }

  actualizarFormulario(campo: keyof OrdenServicioFormulario, valor: unknown): void {
    this.formulario.update((form) => ({
      ...form,
      [campo]: valor,
    }));
  }

  claseEstado(estado: EstadoOrdenServicio): string {
    switch (estado) {
      case EstadoOrdenServicio.Pendiente:
        return 'bg-amber-100 text-amber-800';
      case EstadoOrdenServicio.EnProgreso:
        return 'bg-blue-100 text-blue-800';
      case EstadoOrdenServicio.Finalizada:
        return 'bg-green-100 text-green-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  tituloModal(): string {
    switch (this.modalMode()) {
      case 'crear':
        return 'Nueva orden de servicio';
      case 'editar':
        return 'Editar orden de servicio';
      case 'consultar':
        return 'Detalle de la orden';
      case 'estado':
        return 'Cambiar estado';
      case 'eliminar':
        return 'Eliminar orden';
      default:
        return '';
    }
  }

  private cargarCatalogos(): void {
    this.tecnicoService.listar(true).subscribe({
      next: (tecnicos) => this.tecnicos.set(tecnicos),
    });

    this.clienteService.listar(true).subscribe({
      next: (clientes) => this.clientes.set(clientes),
    });
  }

  private crearFormularioVacio(): OrdenServicioFormulario {
    return {
      descripcion: '',
      tecnicoId: null,
      clienteId: null,
      estado: EstadoOrdenServicio.Pendiente,
    };
  }

  private validarFormulario(form: OrdenServicioFormulario): string | null {
    if (!form.descripcion.trim()) {
      return 'La descripción es obligatoria.';
    }

    if (!form.tecnicoId) {
      return 'Debe seleccionar un técnico.';
    }

    if (!form.clienteId) {
      return 'Debe seleccionar un cliente.';
    }

    const tecnicoValido = this.tecnicos().some((t) => t.id === form.tecnicoId);
    if (!tecnicoValido) {
      return 'El técnico seleccionado no existe o no está activo.';
    }

    const clienteValido = this.clientes().some((c) => c.id === form.clienteId);
    if (!clienteValido) {
      return 'El cliente seleccionado no existe o no está activo.';
    }

    return null;
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
