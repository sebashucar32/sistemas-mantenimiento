export enum EstadoOrdenServicio {
  Pendiente = 'Pendiente',
  EnProgreso = 'EnProgreso',
  Finalizada = 'Finalizada',
}

export const ESTADOS_ORDEN_SERVICIO: {
  value: EstadoOrdenServicio;
  label: string;
}[] = [
  { value: EstadoOrdenServicio.Pendiente, label: 'Pendiente' },
  { value: EstadoOrdenServicio.EnProgreso, label: 'En progreso' },
  { value: EstadoOrdenServicio.Finalizada, label: 'Finalizada' },
];
