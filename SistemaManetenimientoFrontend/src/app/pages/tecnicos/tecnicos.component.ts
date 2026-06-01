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
  ActualizarTecnicoRequest,
  CrearTecnicoRequest,
  MENSAJE_TELEFONO_INVALIDO,
  Tecnico,
  TecnicoFormulario,
  telefonoEsValido,
} from '../../models/tecnico';
import { TecnicoService } from '../../services/tecnico.service';

type ModalMode = 'crear' | 'editar' | 'consultar' | 'eliminar' | null;

@Component({
  selector: 'app-tecnicos',
  imports: [
    FormsModule,
    HeaderComponent,
    SidebarComponent,
    BodyComponent,
    FooterComponent,
  ],
  templateUrl: './tecnicos.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TecnicosComponent implements OnInit {
  private readonly tecnicoService = inject(TecnicoService);

  readonly tecnicos = signal<Tecnico[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly errorMessage = signal('');
  readonly modalMode = signal<ModalMode>(null);
  readonly tecnicoSeleccionado = signal<Tecnico | null>(null);
  readonly filtroActivo = signal<'todos' | 'activos' | 'inactivos'>('todos');

  readonly formulario = signal<TecnicoFormulario>(this.crearFormularioVacio());

  ngOnInit(): void {
    this.cargarTecnicos();
  }

  cargarTecnicos(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    const filtro = this.filtroActivo();
    const activo =
      filtro === 'activos' ? true : filtro === 'inactivos' ? false : undefined;

    this.tecnicoService.listar(activo).subscribe({
      next: (tecnicos) => {
        this.tecnicos.set(tecnicos);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.loading.set(false);
      },
    });
  }

  aplicarFiltroActivo(): void {
    this.cargarTecnicos();
  }

  abrirCrear(): void {
    this.formulario.set(this.crearFormularioVacio());
    this.errorMessage.set('');
    this.modalMode.set('crear');
  }

  abrirEditar(tecnico: Tecnico): void {
    this.tecnicoSeleccionado.set(tecnico);
    this.formulario.set({
      nombre: tecnico.nombre,
      telefono: tecnico.telefono,
      especialidad: tecnico.especialidad,
      activo: tecnico.activo,
    });
    this.errorMessage.set('');
    this.modalMode.set('editar');
  }

  abrirConsultar(tecnico: Tecnico): void {
    this.tecnicoSeleccionado.set(tecnico);
    this.errorMessage.set('');
    this.modalMode.set('consultar');
  }

  abrirEliminar(tecnico: Tecnico): void {
    this.tecnicoSeleccionado.set(tecnico);
    this.errorMessage.set('');
    this.modalMode.set('eliminar');
  }

  cerrarModal(): void {
    this.modalMode.set(null);
    this.tecnicoSeleccionado.set(null);
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

    const operacion =
      this.modalMode() === 'editar' && this.tecnicoSeleccionado()
        ? this.tecnicoService.actualizar(
            this.tecnicoSeleccionado()!.id,
            this.construirRequestActualizar(form)
          )
        : this.tecnicoService.crear(this.construirRequestCrear(form));

    operacion.subscribe({
      next: () => {
        this.saving.set(false);
        this.cerrarModal();
        this.cargarTecnicos();
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.saving.set(false);
      },
    });
  }

  confirmarEliminar(): void {
    const tecnico = this.tecnicoSeleccionado();
    if (!tecnico) {
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    this.tecnicoService.eliminar(tecnico.id).subscribe({
      next: () => {
        this.saving.set(false);
        this.cerrarModal();
        this.cargarTecnicos();
      },
      error: (error) => {
        this.errorMessage.set(this.obtenerMensajeError(error));
        this.saving.set(false);
      },
    });
  }

  actualizarFormulario(campo: keyof TecnicoFormulario, valor: unknown): void {
    this.formulario.update((form) => ({
      ...form,
      [campo]: valor,
    }));
  }

  tituloModal(): string {
    switch (this.modalMode()) {
      case 'crear':
        return 'Nuevo técnico';
      case 'editar':
        return 'Editar técnico';
      case 'consultar':
        return 'Detalle del técnico';
      case 'eliminar':
        return 'Eliminar técnico';
      default:
        return '';
    }
  }

  private construirRequestCrear(form: TecnicoFormulario): CrearTecnicoRequest {
    return {
      nombre: form.nombre.trim(),
      telefono: form.telefono.trim(),
      especialidad: form.especialidad.trim(),
    };
  }

  private construirRequestActualizar(
    form: TecnicoFormulario
  ): ActualizarTecnicoRequest {
    return {
      ...this.construirRequestCrear(form),
      activo: form.activo,
    };
  }

  private crearFormularioVacio(): TecnicoFormulario {
    return {
      nombre: '',
      telefono: '',
      especialidad: '',
      activo: true,
    };
  }

  private validarFormulario(form: TecnicoFormulario): string | null {
    if (!form.nombre.trim()) {
      return 'El nombre completo es obligatorio.';
    }

    if (!form.telefono.trim()) {
      return 'El teléfono es obligatorio.';
    }

    if (!telefonoEsValido(form.telefono)) {
      return MENSAJE_TELEFONO_INVALIDO;
    }

    if (!form.especialidad.trim()) {
      return 'La especialidad es obligatoria.';
    }

    return null;
  }

  private obtenerMensajeError(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 401) {
        return 'Sesión no válida o expirada. Inicie sesión nuevamente.';
      }

      const apiError = error.error as ApiError | null;
      if (apiError?.mensaje) {
        return apiError.mensaje;
      }

      const validationErrors = error.error?.errors as
        | Record<string, string[]>
        | undefined;
      if (validationErrors) {
        const mensajes = Object.values(validationErrors).flat();
        if (mensajes.length > 0) {
          return mensajes.join(' ');
        }
      }
    }

    return 'Ocurrió un error al procesar la solicitud.';
  }
}
