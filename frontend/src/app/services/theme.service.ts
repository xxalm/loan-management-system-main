import { Injectable } from '@angular/core';

export type AppTheme = 'light' | 'dark';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private readonly storageKey = 'fundo_app_theme';
  private currentTheme: AppTheme = 'light';

  init(): void {
    const storedTheme = localStorage.getItem(this.storageKey) as AppTheme | null;
    this.applyTheme(storedTheme === 'dark' ? 'dark' : 'light');
  }

  toggleTheme(): void {
    this.applyTheme(this.currentTheme === 'light' ? 'dark' : 'light');
  }

  isDarkTheme(): boolean {
    return this.currentTheme === 'dark';
  }

  private applyTheme(theme: AppTheme): void {
    this.currentTheme = theme;
    document.body.classList.remove('light-theme', 'dark-theme');
    document.body.classList.add(`${theme}-theme`);
    localStorage.setItem(this.storageKey, theme);
  }
}
