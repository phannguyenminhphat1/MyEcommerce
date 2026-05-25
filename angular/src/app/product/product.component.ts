import { AuthService, PagedResultDto } from '@abp/ng.core';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ProductInListDto, ProductsService } from '@proxy/products';
import { OAuthService } from 'angular-oauth2-oidc';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/product-categories';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
  imports: [
    PanelModule,
    TableModule,
    PaginatorModule,
    BlockUIModule,
    ButtonModule,
    SelectModule,
    InputTextModule,
    FormsModule,
  ],
  providers: [ProductsService],
})
export class ProductComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private ngUnsubcribe = new Subject<void>();
  private productService = inject(ProductsService);
  private productCategoryService = inject(ProductCategoriesService);

  blockedPanel: boolean = false;
  items: ProductInListDto[] = [];

  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number = 0;

  public productCategories: { value: string; name: string }[] = [];
  public categoryId?: string;
  public keyword: string = '';

  constructor() {}

  ngOnInit(): void {
    this.loadData();
    this.loadProductCategories();
  }

  loadData() {
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
        },
        error: () => {},
      });
  }

  loadProductCategories() {
    this.productCategoryService.getListAll().subscribe(response => {
      response.forEach(element => {
        this.productCategories.push({
          name: element.name ?? '',
          value: element.id ?? '',
        });
      });
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
}
