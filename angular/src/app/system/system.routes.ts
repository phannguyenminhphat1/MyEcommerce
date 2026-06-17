import { Routes } from '@angular/router';

export const systemRoutes: Routes = [
  {
    path: 'role',
    pathMatch: 'full',
    loadComponent: () => import('./role/role.component').then(m => m.RoleComponent),
  },
  {
    path: 'user',
    pathMatch: 'full',
    loadComponent: () => import('./user/user.component').then(m => m.UserComponent),
  },
];
