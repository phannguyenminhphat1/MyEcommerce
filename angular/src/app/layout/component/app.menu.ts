import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AppMenuitem } from './app.menuitem';
import { PermissionService } from '@abp/ng.core';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, AppMenuitem, RouterModule],
  template: `<ul class="layout-menu">
    @for (item of model; track item.label) {
      @if (!item.separator) {
        <li app-menuitem [item]="item" [root]="true"></li>
      } @else {
        <li class="menu-separator"></li>
      }
    }
  </ul> `,
})
export class AppMenu {
  model: MenuItem[] = [];
  private permissionService = inject(PermissionService);

  ngOnInit() {
    this.model = [
      {
        label: 'Home',
        items: [{ label: 'Dashboard', icon: 'pi pi-fw pi-home', routerLink: ['/'] }],
      },
      {
        label: 'Catalogs',
        items: [
          {
            label: 'Products',
            icon: 'pi pi-fw pi-circle',
            routerLink: ['/catalog/product'],
            permission: 'MyEcomAdminCatalog.Product',
          },
          {
            label: 'Product Attributes',
            icon: 'pi pi-fw pi-circle',
            routerLink: ['/catalog/attribute'],
            permission: 'MyEcomAdminCatalog.Attribute',
          },
        ],
      },
      {
        label: 'Systems',
        items: [
          {
            label: 'Roles',
            icon: 'pi pi-fw pi-circle',
            routerLink: ['/system/role'],
            permission: 'AbpIdentity.Roles',
          },
          {
            label: 'Users',
            icon: 'pi pi-fw pi-circle',
            routerLink: ['/system/user'],
            permission: 'AbpIdentity.Users',
          },
        ],
      },
    ];
    this.applyPermissions();
  }

  private applyPermissions() {
    this.model.forEach(group => {
      if (!group.items?.length) {
        return;
      }

      group.items.forEach(item => {
        const permission = (item as any).permission;

        if (permission) {
          item.visible = this.permissionService.getGrantedPolicy(permission);
        }
      });

      group.visible = group.items.some(item => item.visible !== false);
    });
  }
}
