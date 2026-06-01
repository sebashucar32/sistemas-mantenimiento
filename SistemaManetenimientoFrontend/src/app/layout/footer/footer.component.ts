import { ChangeDetectionStrategy, Component, computed, signal } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FooterComponent {
  private readonly currentYear = signal(new Date().getFullYear());

  protected readonly copyright = computed(
    () => `© ${this.currentYear()} Sistema de Mantenimiento`
  );
}
