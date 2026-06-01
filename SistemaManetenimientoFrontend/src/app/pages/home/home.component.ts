import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { BodyComponent } from '../../layout/body/body.component';
import { FooterComponent } from '../../layout/footer/footer.component';
import { HeaderComponent } from '../../layout/header/header.component';
import { SidebarComponent } from '../../layout/sidebar/sidebar.component';

interface SummaryCard {
  title: string;
  value: string;
  description: string;
}

@Component({
  selector: 'app-home',
  imports: [HeaderComponent, SidebarComponent, BodyComponent, FooterComponent],
  templateUrl: './home.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent {
  readonly summaryCards = signal<SummaryCard[]>([
    {
      title: 'Equipos activos',
      value: '24',
      description: 'Registrados en el sistema',
    },
    {
      title: 'Mantenimientos pendientes',
      value: '7',
      description: 'Programados esta semana',
    },
    {
      title: 'Órdenes completadas',
      value: '156',
      description: 'En el último mes',
    },
  ]);
}
