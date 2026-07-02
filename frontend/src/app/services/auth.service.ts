import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, of, tap } from 'rxjs';
import { environment } from '../../environments/environment';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenKey = 'auth_token';

  constructor(private readonly http: HttpClient) {}

  login(username = 'admin', password = 'admin'): Observable<string> {
    const existingToken = this.getToken();
    if (existingToken) {
      return of(existingToken);
    }

    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/api/auth/login`, {
        username,
        password,
      })
      .pipe(
        tap((response) => sessionStorage.setItem(this.tokenKey, response.token)),
        map((response) => response.token)
      );
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }
}
