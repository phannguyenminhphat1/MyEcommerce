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
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { UtilityService } from './shared/services/utility.service';
import { NotificationService } from './shared/services/notification.service';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <router-outlet></router-outlet>
    <p-toast position="top-right"></p-toast>
    <p-confirmDialog></p-confirmDialog>
  `,
  imports: [
    LoaderBarComponent,
    SideMenuLayoutModule,
    ThemeSharedModule,
    DynamicLayoutComponent,
    InternetConnectionStatusComponent,
    AppLayout,
    RouterOutlet,
    ConfirmDialogModule,
    ToastModule,
  ],
  providers: [
    DialogService,
    MessageService,
    NotificationService,
    UtilityService,
    ConfirmationService,
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
