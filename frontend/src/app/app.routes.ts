import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./components/login/login.component').then((m) => m.LoginComponent),
    canActivate: [guestGuard],
  },
  {
    path: 'loans',
    loadComponent: () =>
      import('./components/loan-list/loan-list.component').then(
        (m) => m.LoanListComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'settings',
    loadComponent: () =>
      import('./components/settings/settings.component').then(
        (m) => m.SettingsComponent
      ),
    canActivate: [authGuard],
  },
  {
    path: 'loans/:id',
    loadComponent: () =>
      import('./components/loan-detail/loan-detail.component').then(
        (m) => m.LoanDetailComponent
      ),
    canActivate: [authGuard],
  },
];
