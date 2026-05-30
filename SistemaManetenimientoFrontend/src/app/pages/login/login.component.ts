import { Component, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class LoginComponent {
  nombreUsuario = '';
  contrasena = '';
  errorMessage = '';
  loading = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  onSubmit(): void {
    this.errorMessage = '';
    this.loading = true;

    this.authService
      .login({
        nombreUsuario: this.nombreUsuario,
        contrasena: this.contrasena,
      })
      .subscribe({
        next: () => {
          this.loading = false;
          void this.router.navigate(['/home']);
        },
        error: () => {
          this.loading = false;
          this.errorMessage = 'Autenticación fallida';
        },
      });
  }
}
