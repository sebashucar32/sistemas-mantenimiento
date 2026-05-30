import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoginRequest } from '../models/login-request';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly loginUrl = '/api/auth/login';

  constructor(private readonly http: HttpClient) {}

  login(credentials: LoginRequest): Observable<unknown> {
    return this.http.post(this.loginUrl, credentials);
  }
}
