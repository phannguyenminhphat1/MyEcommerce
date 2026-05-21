import { Routes } from '@angular/router';
import { AppLayout } from './layout/component/app.layout';

export const appRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadChildren: () => import('./home/home.routes').then(m => m.homeRoutes),
    component: AppLayout,
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.routes').then(m => m.default),
  },
  // {
  //   path: 'account',
  //   loadChildren: () => import('@abp/ng.account').then(m => m.createRoutes()),
  // },
  // {
  //   path: 'identity',
  //   loadChildren: () => import('@abp/ng.identity').then(m => m.createRoutes()),
  // },
  // {
  //   path: 'tenant-management',
  //   loadChildren: () =>
  //     import('@abp/ng.tenant-management').then(m => m.createRoutes()),
  // },
  // {
  //   path: 'setting-management',
  //   loadChildren: () =>
  //     import('@abp/ng.setting-management').then(m => m.createRoutes()),
  // },
];
