import { permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const catalogRoutes: Routes = [
  {
    path: 'product',
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'MyEcomAdminCatalog.Product',
    },
    loadComponent: () => import('./product/product.component').then(m => m.ProductComponent),
  },
  {
    path: 'attribute',
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'MyEcomAdminCatalog.Attribute',
    },
    loadComponent: () => import('./attribute/attribute.component').then(m => m.AttributeComponent),
  },
  {
    path: 'category',
    canActivate: [permissionGuard],
    data: {
      requiredPolicy: 'MyEcomAdminCatalog.ProductCategory',
    },
    loadComponent: () => import('./category/category.component').then(m => m.CategoryComponent),
  },
];
