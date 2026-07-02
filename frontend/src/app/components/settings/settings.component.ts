import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [MatCardModule, MatIconModule],
  template: `
    <section class="settings-page">
      <mat-card class="settings-card">
        <mat-card-content class="settings-content">
          <mat-icon class="settings-icon">construction</mat-icon>
          <p>Settings module coming soon.</p>
        </mat-card-content>
      </mat-card>
    </section>
  `,
  styles: [
    `
      .settings-page {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: calc(100vh - 4rem);
      }

      .settings-card {
        max-width: 420px;
        width: 100%;
        text-align: center;
      }

      .settings-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 0.75rem;
        padding: 2.5rem 1.5rem;
      }

      .settings-icon {
        font-size: 2.5rem;
        width: 2.5rem;
        height: 2.5rem;
        opacity: 0.7;
      }

      p {
        margin: 0;
        font-size: 1.05rem;
        color: inherit;
      }
    `,
  ],
})
export class SettingsComponent {}
