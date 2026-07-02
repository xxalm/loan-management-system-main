import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [MatCardModule, MatIconModule],
  template: `
    <mat-card class="settings-card">
      <mat-card-header>
        <mat-icon mat-card-avatar>settings</mat-icon>
        <mat-card-title>Settings</mat-card-title>
        <mat-card-subtitle>Application preferences</mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        <p>Theme: Azure Blue</p>
        <p>Language: English</p>
        <p>Notifications: Enabled</p>
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .settings-card {
        max-width: 560px;
        margin: 0 auto;
      }
    `,
  ],
})
export class SettingsComponent {}
