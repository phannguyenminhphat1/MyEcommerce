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
import { ACCESS_TOKEN, REFRESH_TOKEN } from '../shared/constants/keys.constant';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ButtonModule,
    CheckboxModule,
    InputTextModule,
    PasswordModule,
    FormsModule,
    RouterModule,
    RippleModule,
    AppFloatingConfigurator,
    ReactiveFormsModule,
  ],
  templateUrl: './login.html',
})
export class Login implements OnDestroy {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private ngUnsubcribe = new Subject<void>();

  loginForm: FormGroup = this.fb.group({
    username: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  public login() {
    var request: LoginRequestDto = {
      username: this.loginForm.controls['username'].value,
      password: this.loginForm.controls['password'].value,
    };
    this.authService
      .login(request)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe(response => {
        localStorage.setItem(ACCESS_TOKEN, response.access_token);
        localStorage.setItem(REFRESH_TOKEN, response.refresh_token);
        this.router.navigate(['']);
      });
  }

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
  }
}
