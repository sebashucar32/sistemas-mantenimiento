import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LayoutService } from '../services/layout.service';

@Component({
  selector: 'app-header',
  imports: [],
  templateUrl: './header.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  protected readonly layoutService = inject(LayoutService);
}
