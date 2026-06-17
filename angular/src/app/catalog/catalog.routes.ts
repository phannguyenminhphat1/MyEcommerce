import { Routes } from '@angular/router';

export const catalogRoutes: Routes = [
  {
    path: 'product',
    pathMatch: 'full',
    loadComponent: () => import('./product/product.component').then(m => m.ProductComponent),
  },
  {
    path: 'attribute',
    pathMatch: 'full',
    loadComponent: () => import('./attribute/attribute.component').then(m => m.AttributeComponent),
  },
];
