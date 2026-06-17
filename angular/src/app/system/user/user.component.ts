import { PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { UserDto, UserInListDto, UsersService } from '@proxy/users';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { MessageConstants } from 'src/app/shared/constants/messages.const';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { UserDetailComponent } from './user-detail.component';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { InputNumberModule } from 'primeng/inputnumber';
import { EditorModule } from 'primeng/editor';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { ImageModule } from 'primeng/image';
import { ValidationMessageComponent } from 'src/app/shared/components/validation-message/validation-message.component';
import { CancelDialogService } from 'src/app/shared/services/cancel-dialog.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss'],
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
    CommonModule,
  ],
})
export class UserComponent implements OnInit, OnDestroy {
  private dialogService = inject(DialogService);
  private notificationService = inject(NotificationService);
  private cancelDialogService = inject(CancelDialogService);
  private userService = inject(UsersService);

  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number = 0;

  public items: UserInListDto[] = [];
  public selectedItems: UserInListDto[] = [];
  public keyword: string = '';

  constructor() {}

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  ngOnInit() {
    this.loadData();
  }

  loadData(selectionId = null) {
    this.toggleBlockUI(true);
    this.userService
      .getListWithFilter({
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
        keyword: this.keyword,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PagedResultDto<UserInListDto>) => {
          this.items = response.items as UserInListDto[];
          this.totalCount = response.totalCount as number;
          if (selectionId != null && this.items.length > 0) {
            this.selectedItems = this.items.filter(x => x.id == selectionId);
          }
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  showAddModal() {
    const ref = this.dialogService.open(UserDetailComponent, {
      header: 'Add user',
      width: '50vw',
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: UserDto) => {
      if (data) {
        this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
        this.selectedItems = [];
        this.loadData();
      }
    });
  }

  pageChanged(event: PaginatorState): void {
    const page = event.page ?? 0;
    const rows = event.rows ?? 10;
    this.skipCount = page * rows;
    this.maxResultCount = rows;
    this.loadData();
  }

  showEditModal(id?: string) {
    if (!id) return;
    const ref = this.dialogService.open(UserDetailComponent, {
      data: {
        id: id,
      },
      header: 'Edit user',
      width: '50vw',
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: UserDto) => {
      if (data) {
        this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
        this.selectedItems = [];
        this.loadData(data.id);
      }
    });
  }

  deleteItem(id?: string) {
    if (!id) return;
    this.cancelDialogService.delete('Are you sure you want to delete this record?', () => {
      this.deleteItemConfirmed(id);
    });
  }

  deleteItemConfirmed(id: string) {
    this.toggleBlockUI(true);
    this.userService
      .delete(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.loadData();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  deleteItems() {
    if (this.selectedItems.length == 0) return;
    var ids: string[] = [];
    this.selectedItems.forEach(element => {
      ids.push(element.id as string);
    });
    this.cancelDialogService.delete('Are you sure you want to delete those record?', () => {
      this.deleteItemsConfirmed(ids);
    });
  }

  deleteItemsConfirmed(ids: string[]) {
    this.toggleBlockUI(true);
    this.userService
      .deleteMultiple(ids)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.loadData();
          this.selectedItems = [];
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  // setPassword(id: string) {
  //   const ref = this.dialogService.open(SetPasswordComponent, {
  //     data: {
  //       id: id,
  //     },
  //     header: 'Đặt lại mật khẩu',
  //     width: '70%',
  //   });

  //   ref.onClose.subscribe((result: boolean) => {
  //     if (result) {
  //       this.notificationService.showSuccess(MessageConstants.CHANGE_PASSWORD_SUCCCESS_MSG);
  //       this.selectedItems = [];
  //       this.loadData();
  //     }
  //   });
  // }

  // assignRole(id: string) {
  //   const ref = this.dialogService.open(RoleAssignComponent, {
  //     data: {
  //       id: id,
  //     },
  //     header: 'Gán quyền',
  //     width: '70%',
  //   });

  //   ref.onClose.subscribe((result: boolean) => {
  //     if (result) {
  //       this.notificationService.showSuccess(MessageConstants.ROLE_ASSIGN_SUCCESS_MSG);
  //       this.loadData();
  //     }
  //   });
  // }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
