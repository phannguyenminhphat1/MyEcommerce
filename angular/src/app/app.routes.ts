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
    path: 'product',
    component: AppLayout,
    loadChildren: () => import('./product/product.routes').then(m => m.productRoutes),
  },
];
