import { Component } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NgIf } from '@angular/common';
import { filter } from 'rxjs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    NgIf,
    RouterOutlet,
    MatToolbarModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  showHeader = false;

  constructor(
    public readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => this.updateHeaderVisibility());

    this.updateHeaderVisibility();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  private updateHeaderVisibility(): void {
    this.showHeader =
      this.authService.isLoggedIn() && !this.router.url.startsWith('/login');
  }
}
