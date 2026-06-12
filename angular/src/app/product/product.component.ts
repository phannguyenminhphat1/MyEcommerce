import { PagedResultDto } from '@abp/ng.core';
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
import { BadgeModule } from 'primeng/badge';
import { OverlayBadgeModule } from 'primeng/overlaybadge';
import { CommonModule } from '@angular/common';
import { ProductType } from '@proxy/my-ecommerce/products';
import { CancelDialogService } from '../shared/services/cancel-dialog.service';
import { ProductAttributeComponent } from './product-attribute.component';
import { TooltipModule } from 'primeng/tooltip';
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
    TooltipModule,
  ],
  providers: [ProductsService, CancelDialogService],
})
export class ProductComponent implements OnInit, OnDestroy {
  private productService = inject(ProductsService);
  private dialogService = inject(DialogService);
  private notificationService = inject(NotificationService);
  private cancelDialogService = inject(CancelDialogService);
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

  getProductTypeName(value: number) {
    return ProductType[value];
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
        this.selectedItems = [];
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
    this.productService
      .delete(id)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Delete successfully');
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
    this.productService
      .deleteMultiple(ids)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Delete successfully');
          this.loadData();
          this.selectedItems = [];
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  manageProductAttribute(id: string) {
    if (!id) return;
    const ref = this.dialogService.open(ProductAttributeComponent, {
      data: {
        id: id,
      },
      header: 'Manage Product Attributes',
      width: '50vw',
      contentStyle: {
        height: '600px',
      },
      closable: true,
      modal: true,
    });
    ref?.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Update product attributes successfully');
        this.selectedItems = [];
      }
    });
  }
}
