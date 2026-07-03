import { Component, inject, OnDestroy } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { RippleModule } from 'primeng/ripple';
import { AppFloatingConfigurator } from '../layout/component/app.floatingconfigurator';
import { AuthService } from '../shared/services/auth.service';
import { LoginRequestDto } from '../shared/models/login-request.dto';
import { Subject, takeUntil } from 'rxjs';
import { BlockUIModule } from 'primeng/blockui';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TokenStorageService } from '../shared/services/token.service';
import { LoginResponseDto } from '../shared/models/login-response.dto';
import { NotificationService } from '../shared/services/notification.service';
import { PanelModule } from 'primeng/panel';
import { ConfigStateService } from '@abp/ng.core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    PanelModule,
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    PasswordModule,
    FormsModule,
    RouterModule,
    RippleModule,
    AppFloatingConfigurator,
    ReactiveFormsModule,
    BlockUIModule,
    ProgressSpinnerModule,
  ],
  templateUrl: './login.html',
})
export class Login implements OnDestroy {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private tokenService = inject(TokenStorageService);
  private notificationService = inject(NotificationService);
  private configState = inject(ConfigStateService);
  private ngUnsubscribe = new Subject<void>();

  blockedPanel: boolean = false;
  btnDisabled = false;

  loginForm: FormGroup = this.fb.group({
    username: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  public login() {
    this.toggleBlockUI(true);
    var request: LoginRequestDto = {
      username: this.loginForm.controls['username'].value,
      password: this.loginForm.controls['password'].value,
    };
    this.authService
      .login(request)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: LoginResponseDto) => {
          this.tokenService.saveToken(res.access_token);
          this.tokenService.saveRefreshToken(res.refresh_token);
          this.toggleBlockUI(false);
          this.configState.refreshAppState().subscribe(() => {
            this.router.navigate(['/catalog/category']);
          });
        },
        error: ex => {
          this.notificationService.showError(
            ex.error?.error?.error_description || 'Login failed. Please try again.',
          );
          this.toggleBlockUI(false);
        },
      });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
      this.btnDisabled = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
        this.btnDisabled = false;
      }, 1000);
    }
  }
}
