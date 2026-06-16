import { Component, EventEmitter, inject, OnDestroy, OnInit } from '@angular/core';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { Subject, takeUntil } from 'rxjs';
import { SelectModule } from 'primeng/select';
import { FormBuilder, FormControl, FormGroup, FormsModule, Validators } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ReactiveFormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { EditorModule } from 'primeng/editor';
import { TextareaModule } from 'primeng/textarea';
import { ValidationMessageComponent } from '../shared/components/validation-message/validation-message.component';
import { UtilityService } from '../shared/services/utility.service';
import { ImageModule } from 'primeng/image';
import { RolesService } from '@proxy/roles/roles.service';
import { CommonModule } from '@angular/common';
import {
  GetPermissionListResultDto,
  PermissionGrantInfoDto,
  PermissionGroupDto,
  UpdatePermissionDto,
  UpdatePermissionsDto,
} from '@proxy/volo/abp/permission-management';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-permission-grant',
  templateUrl: './permission-grant.component.html',
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
    CommonModule,
  ],
  providers: [RolesService],
})
export class PermissionGrantComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private rolesService = inject(RolesService);
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);

  private ngUnsubcribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  form!: FormGroup;
  public saveBtnName: string = '';
  public closeBtnName: string = '';
  public groups: PermissionGroupDto[] = [];
  public permissions: PermissionGrantInfoDto[] = [];
  public selectedPermissions: string[] = [];
  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor() {}

  ngOnInit(): void {
    this.buildForm();
    this.loadDetail(this.config.data.name, 'R');
    this.saveBtnName = 'Update';
    this.closeBtnName = 'Cancel';
  }

  loadDetail(providerKey: string, providerName: string) {
    this.toggleBlockUI(true);
    this.rolesService
      .getPermissions(providerName, providerKey)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: GetPermissionListResultDto) => {
          this.groups = response.groups as PermissionGroupDto[];
          this.groups.forEach(element => {
            (element.permissions as PermissionGroupDto[]).forEach(permission => {
              this.permissions.push(permission);
            });
          });
          this.buildForm();
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
    var permissions: UpdatePermissionDto[] = [];
    for (let index = 0; index < this.permissions.length; index++) {
      const isGranted =
        this.selectedPermissions.filter(x => x == this.permissions[index].name).length > 0;
      permissions.push({ name: this.permissions[index].name, isGranted: isGranted });
    }
    var updateValues: UpdatePermissionsDto = {
      permissions: permissions,
    };
    this.rolesService
      .updatePermissions('R', this.config.data.name, updateValues)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe(() => {
        this.toggleBlockUI(false);
        this.ref.close(this.form.value);
      });
  }

  private buildForm() {
    this.form = this.fb.group({});
    //Fill value
    for (let index = 0; index < this.groups.length; index++) {
      const group = this.groups[index];
      for (
        let jIndex = 0;
        jIndex < (group.permissions as PermissionGrantInfoDto[]).length;
        jIndex++
      ) {
        const permission = (group.permissions as PermissionGrantInfoDto[])[jIndex];
        if (permission.isGranted) {
          this.selectedPermissions.push(permission.name as string);
        }
      }
    }
  }

  close() {
    this.ref.close();
  }

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
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
