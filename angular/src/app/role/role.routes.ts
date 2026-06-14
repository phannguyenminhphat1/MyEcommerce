import { Routes } from '@angular/router';

export const roleRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./role.component').then(m => m.RoleComponent),
  },
];
