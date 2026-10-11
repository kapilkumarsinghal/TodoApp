import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/accounts/pages/account-page/account-page').then((module) => module.AccountPageComponent),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/accounts/pages/account-page/account-page').then((module) => module.AccountPageComponent),
  },
  {
    path: 'habits',
    canActivate: [() => import('./features/accounts/data-access/auth.guard').then((module) => module.accountGuard)],
    loadComponent: () => import('./features/habits/pages/habit-tracker/habit-tracker').then((module) => module.HabitTrackerComponent),
  },
  {
    path: '',
    canActivate: [() => import('./features/accounts/data-access/auth.guard').then((module) => module.accountGuard)],
    loadComponent: () =>
      import('./features/todos/pages/todos-page/todos-page').then(
        (module) => module.TodosPageComponent,
      ),
  },
  { path: '**', redirectTo: '' },
];
