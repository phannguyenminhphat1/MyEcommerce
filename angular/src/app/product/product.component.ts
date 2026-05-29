import { AuthService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ProductDto, ProductInListDto, ProductsService } from '@proxy/products';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoriesService } from '@proxy/product-categories';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ProductDetailComponent } from './product-detail.component';
import { NotificationService } from '../shared/services/notification.service';
import { MessageService } from 'primeng/api';
import { BadgeModule } from 'primeng/badge';
import { OverlayBadgeModule } from 'primeng/overlaybadge';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
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
  providers: [ProductsService, DialogService, NotificationService, MessageService],
})
export class ProductComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private productService = inject(ProductsService);
  private dialogService = inject(DialogService);
  private notificationService = inject(NotificationService);
  private ngUnsubcribe = new Subject<void>();
  private productCategoryService = inject(ProductCategoriesService);

  blockedPanel: boolean = false;
  items: ProductInListDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number = 0;

  public productCategories: any = [];
  public categoryId?: string;
  public keyword: string = '';

  public selectedItems: ProductInListDto[] = [];

  constructor() {}

  ngOnInit(): void {
    this.loadData();
    this.loadProductCategories();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.productService
      .getListFilter({
        maxResultCount: this.maxResultCount,
        keyword: this.keyword,
        categoryId: this.categoryId,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: PagedResultDto<ProductInListDto>) => {
          this.items = response.items ?? [];
          this.totalCount = response.totalCount ?? 0;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadProductCategories() {
    this.productCategoryService.getListAll().subscribe(response => {
      this.productCategories = response.map(element => ({
        value: element.id,
        label: element.name,
      }));
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

  login() {
    this.authService.navigateToLogin();
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
    const ref = this.dialogService.open(ProductDetailComponent, {
      header: 'Add Product',
      width: '50vw',
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Add product successfully');
      }
    });
  }

  showEditModal(id?: string) {
    if (!id) return;
    const ref = this.dialogService.open(ProductDetailComponent, {
      data: {
        id: id,
      },
      header: 'Edit Product',
      width: '50vw',
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Edit product successfully');
      }
    });
  }

  showDeleteModal(id?: string) {
    if (!id) return;
  }
}
