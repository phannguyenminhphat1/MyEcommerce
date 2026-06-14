import { PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { Subject, takeUntil } from 'rxjs';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { NotificationService } from '../shared/services/notification.service';
import { BadgeModule } from 'primeng/badge';
import { OverlayBadgeModule } from 'primeng/overlaybadge';
import { CommonModule } from '@angular/common';
import { CancelDialogService } from '../shared/services/cancel-dialog.service';
import { RoleDetailComponent } from './role-detail.component';
import { RoleInListDto, RolesService } from '@proxy/roles';
@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.scss'],
  imports: [
    CommonModule,
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    SelectModule,
    InputTextModule,
    FormsModule,
    ProgressSpinnerModule,
    DynamicDialogModule,
    BadgeModule,
    OverlayBadgeModule,
  ],
  providers: [RolesService, CancelDialogService],
})
export class RoleComponent implements OnInit, OnDestroy {
  private roleService = inject(RolesService);
  private dialogService = inject(DialogService);
  private notificationService = inject(NotificationService);
  private cancelDialogService = inject(CancelDialogService);
  private ngUnsubcribe = new Subject<void>();

  blockedPanel: boolean = false;
  items: RoleInListDto[] = [];
  public selectedItems: RoleInListDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number = 0;
  public keyword: string = '';

  constructor() {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.roleService
      .getListFilter({
        maxResultCount: this.maxResultCount,
        keyword: this.keyword,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: PagedResultDto<RoleInListDto>) => {
          this.items = response.items ?? [];
          this.totalCount = response.totalCount ?? 0;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
  }

  pageChanged(event: PaginatorState): void {
    const page = event.page ?? 0;
    const rows = event.rows ?? 10;
    this.skipCount = page * rows;
    this.maxResultCount = rows;
    this.loadData();
  }

  toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }

  showAddModal() {
    const ref = this.dialogService.open(RoleDetailComponent, {
      header: 'Add Role',
      width: '50vw',
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: RoleInListDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Add role successfully');
        this.selectedItems = [];
      }
    });
  }

  showEditModal(id?: string) {
    if (!id) return;
    const ref = this.dialogService.open(RoleDetailComponent, {
      data: {
        id: id,
      },
      header: 'Edit Role',
      width: '50vw',
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: RoleInListDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Edit role successfully');
        this.selectedItems = [];
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
    this.roleService
      .delete(id)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Delete role successfully');
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
    this.roleService
      .deleteMultiple(ids)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Delete role successfully');
          this.loadData();
          this.selectedItems = [];
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
}
