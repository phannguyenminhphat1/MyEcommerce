import { permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const systemRoutes: Routes = [
  {
    path: 'role',
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'AbpIdentity.Roles',
    },
    loadComponent: () => import('./role/role.component').then(m => m.RoleComponent),
  },
  {
    path: 'user',
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'AbpIdentity.Users',
    },
    loadComponent: () => import('./user/user.component').then(m => m.UserComponent),
  },
];
