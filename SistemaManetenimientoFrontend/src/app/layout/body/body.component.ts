import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-body',
  imports: [],
  templateUrl: './body.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BodyComponent {
  readonly title = input('Panel principal');
  readonly subtitle = input('Bienvenido al sistema de mantenimiento');
}
