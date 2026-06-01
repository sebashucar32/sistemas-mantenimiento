import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly loginUrl = '/api/auth/login';
  private readonly tokenKey = 'auth_token';

  constructor(private readonly http: HttpClient) {}

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.loginUrl, credentials).pipe(
      tap((response) => this.setToken(response.token))
    );
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
