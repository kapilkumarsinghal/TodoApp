import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'habits',
    loadComponent: () => import('./features/habits/pages/habit-tracker/habit-tracker').then((module) => module.HabitTrackerComponent),
  },
  {
    path: '',
    loadComponent: () =>
      import('./features/todos/pages/todos-page/todos-page').then(
        (module) => module.TodosPageComponent,
      ),
  },
  { path: '**', redirectTo: '' },
];
