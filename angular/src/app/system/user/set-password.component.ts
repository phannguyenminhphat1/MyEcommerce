import { Component, OnInit, EventEmitter, OnDestroy, inject } from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
  ValidatorFn,
  AbstractControl,
  ValidationErrors,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { RoleDto } from '@proxy/roles';
import { UsersService } from '@proxy/users';
import { BlockUIModule } from 'primeng/blockui';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Subject, takeUntil } from 'rxjs';
import { ValidationMessageComponent } from 'src/app/shared/components/validation-message/validation-message.component';
import { KeyFilterModule } from 'primeng/keyfilter';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-set-password',
  templateUrl: 'set-password.component.html',
  standalone: true,
  imports: [
    PanelModule,
    TableModule,
    BlockUIModule,
    ButtonModule,
    SelectModule,
    InputTextModule,
    FormsModule,
    ProgressSpinnerModule,
    ReactiveFormsModule,
    InputNumberModule,
    ValidationMessageComponent,
    KeyFilterModule,
    CommonModule,
  ],
  providers: [UsersService],
})
export class SetPasswordComponent implements OnInit, OnDestroy {
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private userService = inject(UsersService);
  private fb = inject(FormBuilder);
  private ngUnsubscribe = new Subject<void>();

  // Default
  blockedPanel: boolean = false;
  form!: FormGroup;
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  public closeBtnName: string = '';
  selectedEntity = {} as RoleDto;

  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor() {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit() {
    this.buildForm();
    this.saveBtnName = 'Update';
    this.closeBtnName = 'Cancel';
  }

  // Validate
  noSpecial: RegExp = /^[^<>*!_~]+$/;
  validationMessages = {
    newPassword: [
      { type: 'required', message: 'Password is required' },
      {
        type: 'pattern',
        message:
          'Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character',
      },
    ],
    confirmNewPassword: [
      { type: 'required', message: 'Confirm password is required' },
      {
        type: 'notmatched',
        message: 'Confirm password does not match password',
      },
    ],
  };

  saveChange() {
    if (this.form.invalid) {
      return;
    }
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.userService
      .setPassword(this.config.data.id, this.form.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.toggleBlockUI(false);
        this.ref.close(this.form.value);
      });
  }

  buildForm() {
    this.form = this.fb.group(
      {
        newPassword: new FormControl(
          null,
          Validators.compose([
            Validators.required,
            Validators.pattern(
              '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$',
            ),
          ]),
        ),
        confirmNewPassword: new FormControl(null, Validators.required),
      },
      {
        validators: passwordMatchingValidatior,
      },
    );
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanel = false;
      }, 1000);
    }
  }
}

export const passwordMatchingValidatior: ValidatorFn = (
  control: AbstractControl,
): ValidationErrors | null => {
  const password = control.get('newPassword');
  const confirmPassword = control.get('confirmNewPassword');

  if (!password || !confirmPassword) {
    return null;
  }

  return password.value === confirmPassword.value ? null : { notmatched: true };
};
