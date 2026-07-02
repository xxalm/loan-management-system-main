import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenKey = 'auth_token';
  private readonly usernameKey = 'auth_username';

  constructor(private readonly http: HttpClient) {}

  login(username: string, password: string): Observable<string> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/api/auth/login`, {
        username,
        password,
      })
      .pipe(
        tap((response) => {
          sessionStorage.setItem(this.tokenKey, response.token);
          sessionStorage.setItem(this.usernameKey, username);
        }),
        map((response) => response.token)
      );
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.usernameKey);
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  getUsername(): string {
    const username = sessionStorage.getItem(this.usernameKey);
    if (!username) {
      return 'Admin';
    }

    return username.charAt(0).toUpperCase() + username.slice(1);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
