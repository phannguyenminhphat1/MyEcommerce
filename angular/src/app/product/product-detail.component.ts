import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ProductDto, ProductsService } from '@proxy/products';
import { BlockUIModule } from 'primeng/blockui';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { SelectModule } from 'primeng/select';
import { FormBuilder, FormControl, FormGroup, FormsModule, Validators } from '@angular/forms';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ReactiveFormsModule } from '@angular/forms';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { EditorModule } from 'primeng/editor';
import { TextareaModule } from 'primeng/textarea';
import { ValidationMessageComponent } from '../shared/components/validation-message/validation-message.component';
import { productTypeOptions } from '@proxy/my-ecommerce/products';
import { ProductCategoriesService, ProductCategoryInListDto } from '@proxy/product-categories';
import { ManufacturerInListDto, ManufacturersService } from '@proxy/manufacturers';
import { UtilityService } from '../shared/services/utility.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
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
    ValidationMessageComponent,
  ],
  providers: [ProductsService, UtilityService],
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private productService = inject(ProductsService);
  private productCategoryService = inject(ProductCategoriesService);
  private manufacturerService = inject(ManufacturersService);
  private utilService = inject(UtilityService);
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);

  private ngUnsubcribe = new Subject<void>();
  blockedPanel: boolean = false;
  form!: FormGroup;
  selectedEntity = {} as ProductDto;
  manufacturers: any[] = [];
  productTypes: any[] = [];
  productCategories: any[] = [];
  btnDisabled = false;
  manufacturerId: string = '';

  constructor() {}

  validationMessages = {
    code: [{ type: 'required', message: 'Code is required and must be unique' }],

    name: [
      { type: 'required', message: 'Name is required' },
      { type: 'maxlength', message: 'Name cannot exceed 255 characters' },
    ],

    slug: [{ type: 'required', message: 'A unique URL slug is required' }],

    sku: [{ type: 'required', message: 'Product SKU is required' }],

    manufacturerId: [{ type: 'required', message: 'Please select a manufacturer' }],

    categoryId: [{ type: 'required', message: 'Please select a category' }],

    productType: [{ type: 'required', message: 'Please select a product type' }],

    sortOrder: [{ type: 'required', message: 'Please enter the sort order' }],

    sellPrice: [{ type: 'required', message: 'Please enter the selling price' }],

    thumbnailPicture: [{ type: 'required', message: 'Please enter the thumbnail picture' }],
  };

  ngOnInit(): void {
    this.buildForm();
    this.loadProductTypes();
    this.initFormData();
  }

  generateSlug() {
    this.form.controls['slug'].setValue(
      this.utilService.MakeSeoTitle(this.form.get('name')?.value),
    );
  }

  initFormData() {
    var productCategories = this.productCategoryService.getListAll();
    var manufacturers = this.manufacturerService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      productCategories,
      manufacturers,
    })
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: response => {
          var productCategories = response.productCategories as ProductCategoryInListDto[];
          var manufacturers = response.manufacturers as ManufacturerInListDto[];
          this.productCategories = productCategories.map(element => ({
            value: element.id,
            label: element.name,
          }));
          this.manufacturers = manufacturers.map(element => ({
            value: element.id,
            label: element.name,
          }));

          //Load edit data to form
          if (this.utilService.isEmpty(this.config.data?.id) == true) {
            this.toggleBlockUI(false);
          } else {
            this.loadFormDetails(this.config.data?.id);
          }
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productService
      .get(id)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: response => {
          this.selectedEntity = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadProductTypes() {
    productTypeOptions.forEach(element => {
      this.productTypes.push({
        value: element.value,
        label: element.key,
      });
    });
  }

  public saveChange() {
    this.toggleBlockUI(true);
    if (this.utilService.isEmpty(this.config.data?.id) == true) {
      this.productService
        .create(this.form.value)
        .pipe(takeUntil(this.ngUnsubcribe))
        .subscribe({
          next: response => {
            this.toggleBlockUI(false);
            this.ref.close(this.form.value);
          },
          error: () => {
            this.toggleBlockUI(false);
          },
        });
    } else {
      this.productService
        .update(this.config.data?.id, this.form.value)
        .pipe(takeUntil(this.ngUnsubcribe))
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

  private buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([Validators.required, Validators.maxLength(250)]),
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(
        this.selectedEntity.manufacturerId || null,
        Validators.required,
      ),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || true),
      isActive: new FormControl(this.selectedEntity.isActive || true),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
      description: new FormControl(this.selectedEntity.description || null),
      thumbnailPicture: new FormControl(this.selectedEntity.thumbnailPicture || null),
    });
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
