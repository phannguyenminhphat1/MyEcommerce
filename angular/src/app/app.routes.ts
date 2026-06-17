import { Routes } from '@angular/router';
import { AppLayout } from './layout/component/app.layout';

export const appRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: AppLayout,
    loadChildren: () => import('./home/home.routes').then(m => m.homeRoutes),
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.routes').then(m => m.default),
  },
  {
    path: 'catalog',
    component: AppLayout,
    loadChildren: () => import('./catalog/catalog.routes').then(m => m.catalogRoutes),
  },
  {
    path: 'system',
    component: AppLayout,
    loadChildren: () => import('./system/system.routes').then(m => m.systemRoutes),
  },
];
