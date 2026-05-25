import { Routes } from '@angular/router';

export const productRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./product.component').then(m => m.ProductComponent),
  },
];
