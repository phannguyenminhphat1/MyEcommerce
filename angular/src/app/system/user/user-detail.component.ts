import { Component, OnInit, EventEmitter, OnDestroy, inject } from '@angular/core';
import {
  Validators,
  FormControl,
  FormGroup,
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/shared/services/auth.service';
import { UtilityService } from 'src/app/shared/services/utility.service';
import { UserDto } from '@proxy/users';
import { UsersService } from '@proxy/users';
import { RoleDto, RolesService } from '@proxy/roles';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { InputNumberModule } from 'primeng/inputnumber';
import { EditorModule } from 'primeng/editor';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { ImageModule } from 'primeng/image';
import { ValidationMessageComponent } from 'src/app/shared/components/validation-message/validation-message.component';

@Component({
  selector: 'app-user-detail',
  templateUrl: 'user-detail.component.html',
  standalone: true,
  imports: [
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    SelectModule,
    InputTextModule,
    FormsModule,
    ProgressSpinnerModule,
    ReactiveFormsModule,
    InputNumberModule,
    EditorModule,
    CheckboxModule,
    TextareaModule,
    ImageModule,
    ValidationMessageComponent,
  ],
  providers: [RolesService, AuthService, UsersService],
})
export class UserDetailComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private roleService = inject(RolesService);
  private userService = inject(UsersService);
  private utilService = inject(UtilityService);
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);
  private ngUnsubscribe = new Subject<void>();

  blockedPanel: boolean = false;
  form!: FormGroup;
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  public roles: any[] = [];
  public countries: any[] = [];
  public provinces: any[] = [];
  selectedEntity = {} as UserDto;
  public avatarImage: any;
  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor() {}

  // Validate
  validationMessages = {
    name: [
      { type: 'required', message: 'Name is required' },
      { type: 'minlength', message: 'Name must be at least 2 characters' },
      { type: 'maxlength', message: 'Name cannot exceed 50 characters' },
    ],

    surname: [
      { type: 'required', message: 'Surname is required' },
      { type: 'minlength', message: 'Surname must be at least 2 characters' },
      { type: 'maxlength', message: 'Surname cannot exceed 50 characters' },
    ],

    userName: [
      { type: 'required', message: 'Username is required' },
      { type: 'minlength', message: 'Username must be at least 4 characters' },
      { type: 'maxlength', message: 'Username cannot exceed 32 characters' },
      { type: 'pattern', message: 'Username cannot contain spaces' },
    ],

    email: [
      { type: 'required', message: 'Email is required' },
      { type: 'email', message: 'Please enter a valid email address' },
      { type: 'pattern', message: 'Email cannot contain spaces' },
    ],

    password: [
      { type: 'required', message: 'Password is required' },
      {
        type: 'minlength',
        message: 'Password must be at least 8 characters long',
      },
      {
        type: 'pattern',
        message:
          'Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character',
      },
    ],

    phoneNumber: [
      { type: 'required', message: 'Phone number is required' },
      { type: 'minlength', message: 'Phone number must be at least 10 digits' },
      { type: 'maxlength', message: 'Phone number cannot exceed 15 digits' },
      {
        type: 'pattern',
        message: 'Phone number must contain digits only and cannot contain spaces',
      },
    ],
  };

  ngOnInit() {
    this.buildForm();
    var roles = this.roleService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      roles,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: any) => {
          var roles = repsonse.roles as RoleDto[];
          this.roles = roles.map(element => ({
            value: element.id,
            label: element.name,
          }));
          if (this.utilService.isEmpty(this.config.data?.id) == false) {
            this.loadFormDetails(this.config.data?.id);
          } else {
            this.setMode('create');
            this.toggleBlockUI(false);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  loadFormDetails(id: string) {
    this.userService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: UserDto) => {
          this.selectedEntity = response;
          this.buildForm();
          this.setMode('update');
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.saveData();
  }

  private saveData() {
    this.toggleBlockUI(true);
    if (this.utilService.isEmpty(this.config.data?.id)) {
      this.userService
        .create(this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.ref.close(this.form.value);
            this.toggleBlockUI(false);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.userService
        .update(this.config.data?.id, this.form.value)
        .pipe(takeUntil(this.ngUnsubscribe))
        .subscribe({
          next: () => {
            this.toggleBlockUI(false);
            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    }
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

  setMode(mode: string) {
    if (mode == 'update') {
      this.form.controls['userName'].clearValidators();
      this.form.controls['userName'].disable();
      this.form.controls['email'].clearValidators();
      this.form.controls['email'].disable();
      this.form.controls['password'].clearValidators();
      this.form.controls['password'].disable();
    } else if (mode == 'create') {
      this.form.controls['userName'].addValidators(Validators.required);
      this.form.controls['userName'].enable();
      this.form.controls['email'].addValidators(Validators.required);
      this.form.controls['email'].enable();
      this.form.controls['password'].addValidators(Validators.required);
      this.form.controls['password'].enable();
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: new FormControl(this.selectedEntity.name || null, [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
      ]),
      surname: new FormControl(this.selectedEntity.surname || null, [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
      ]),
      userName: new FormControl(this.selectedEntity.userName || null, [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(32),
        Validators.pattern(/^\S+$/),
      ]),
      email: new FormControl(this.selectedEntity.email || null, [
        Validators.required,
        Validators.email,
        Validators.pattern(/^\S+$/),
      ]),
      phoneNumber: new FormControl(this.selectedEntity.phoneNumber || null, [
        Validators.required,
        Validators.pattern(/^[0-9]{10,15}$/),
      ]),
      password: new FormControl(
        null,
        Validators.compose([
          Validators.required,
          Validators.pattern(
            '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$',
          ),
        ]),
      ),
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
