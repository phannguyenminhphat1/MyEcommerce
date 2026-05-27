import { Component, inject, OnInit } from '@angular/core';
import {
  InternetConnectionStatusComponent,
  LoaderBarComponent,
  ThemeSharedModule,
} from '@abp/ng.theme.shared';
import { DynamicLayoutComponent } from '@abp/ng.core';
import { AppLayout } from './layout/component/app.layout';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './shared/services/auth.service';
import { LOGIN_URL } from './shared/constants/urls.constant';
import { SideMenuLayoutModule } from '@abp/ng.theme.lepton-x/layouts';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <router-outlet></router-outlet>
  `,
  imports: [
    LoaderBarComponent,
    SideMenuLayoutModule,
    ThemeSharedModule,
    DynamicLayoutComponent,
    InternetConnectionStatusComponent,
    AppLayout,
    RouterOutlet,
  ],
})
export class AppComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void {
    if (this.authService.isAuthenticated() == false) {
      this.router.navigate([LOGIN_URL]);
    }
  }
}
