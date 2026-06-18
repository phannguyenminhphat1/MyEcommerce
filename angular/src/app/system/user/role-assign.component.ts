import { Component, OnInit, EventEmitter, OnDestroy, inject } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RoleDto, RolesService } from '@proxy/roles';
import { UserDto } from '@proxy/users';
import { UsersService } from '@proxy/users';
import { BlockUIModule } from 'primeng/blockui';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { PanelModule } from 'primeng/panel';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { ValidationMessageComponent } from 'src/app/shared/components/validation-message/validation-message.component';
import { PickListModule } from 'primeng/picklist';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-assign',
  templateUrl: 'role-assign.component.html',
  standalone: true,
  imports: [
    PanelModule,
    TableModule,
    BlockUIModule,
    ProgressSpinnerModule,
    ReactiveFormsModule,
    ValidationMessageComponent,
    PickListModule,
    ButtonModule,
    CommonModule,
    FormsModule,
  ],
  providers: [RolesService, UsersService],
})
export class RoleAssignComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  private ref = inject(DynamicDialogRef);
  private config = inject(DynamicDialogConfig);
  private userService = inject(UsersService);
  private roleService = inject(RolesService);

  // Default
  public blockedPanel: boolean = false;
  public title: string = '';
  public btnDisabled = false;
  public saveBtnName: string = '';
  public closeBtnName: string = '';
  public availableRoles: string[] = [];
  public seletedRoles: string[] = [];
  formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

  constructor() {}

  ngOnInit() {
    var roles = this.roleService.getListAll();
    forkJoin({
      roles,
    })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: any) => {
          var roles = repsonse.roles as RoleDto[];
          this.availableRoles = roles.map(element => element.name as string);
          this.loadDetail(this.config.data.id);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
    this.saveBtnName = 'Update';
    this.closeBtnName = 'Cancel';
  }

  loadRoles() {
    this.toggleBlockUI(true);
    this.roleService
      .getListAll()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: RoleDto[]) => {
          this.availableRoles = response.map(element => element.name as string);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadDetail(id: any) {
    this.toggleBlockUI(true);
    this.userService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: UserDto) => {
          this.seletedRoles = response.roles as string[];
          this.availableRoles = this.availableRoles.filter(x => !this.seletedRoles.includes(x));
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
    this.userService
      .assignRoles(this.config.data.id, this.seletedRoles)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.toggleBlockUI(false);
        this.ref.close();
      });
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

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
}
