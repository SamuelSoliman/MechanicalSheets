import { Routes } from '@angular/router';
import { authGuard, mechanicGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'sheets' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'sheets',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/sheets/list/sheets-list').then((m) => m.SheetsList),
  },
  {
    path: 'sheets/new',
    canActivate: [mechanicGuard],
    loadComponent: () =>
      import('./features/sheets/create/sheet-create').then((m) => m.SheetCreate),
  },
  {
    path: 'sheets/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/sheets/detail/sheet-detail').then((m) => m.SheetDetail),
  },
  { path: '**', redirectTo: 'sheets' },
];
