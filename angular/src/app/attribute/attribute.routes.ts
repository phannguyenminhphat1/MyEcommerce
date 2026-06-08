import { Routes } from '@angular/router';

export const attributeRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./attribute.component').then(m => m.AttributeComponent),
  },
];
