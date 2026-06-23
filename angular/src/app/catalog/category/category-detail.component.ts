import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
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
import { ValidationMessageComponent } from '../../shared/components/validation-message/validation-message.component';
import { UtilityService } from '../../shared/services/utility.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ImageModule } from 'primeng/image';
import { ProductCategoriesService, ProductCategoryDto } from '@proxy/product-categories';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-detail',
  templateUrl: './category-detail.component.html',
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
  providers: [ProductCategoriesService],
})
export class CategoryDetailComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private productCategoriesService = inject(ProductCategoriesService);
  private utilService = inject(UtilityService);
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);
  private cd = inject(ChangeDetectorRef);
  private sanitizer = inject(DomSanitizer);

  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  form!: FormGroup;
  dataTypes: any[] = [];
  parentCategories: any[] = [];
  selectedEntity = {} as ProductCategoryDto;
  public coverPicture: any;

  constructor() {}

  get isCreateMode(): boolean {
    return this.utilService.isEmpty(this.config.data?.id) == true;
  }

  validationMessages = {
    code: [{ type: 'required', message: 'Code is required and must be unique' }],
    name: [
      { type: 'required', message: 'Name is required' },
      { type: 'maxlength', message: 'Name cannot exceed 255 characters' },
    ],
    slug: [{ type: 'required', message: 'A unique URL slug is required' }],
    parentId: [{ type: 'required', message: 'Please select a parent category' }],
    sortOrder: [{ type: 'required', message: 'Please enter the sort order' }],
    seoMetaDescription: [{ type: 'required', message: 'Please enter the seo description' }],
    coverPictureContent: [
      {
        type: 'required',
        message: 'Please select a cover picture',
      },
    ],
  };

  ngOnInit(): void {
    this.buildForm();
    this.initFormData();
  }

  generateSlug() {
    this.form.controls['slug'].setValue(
      this.utilService.MakeSeoTitle(this.form.get('name')?.value),
    );
  }

  initFormData() {
    this.toggleBlockUI(true);
    this.productCategoriesService
      .getListAll()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: response => {
          this.parentCategories = (response ?? [])
            .filter(item => item.id !== this.config.data?.id)
            .map(element => ({ value: element.id, label: element.name }));

          if (this.isCreateMode) {
            this.getNewSuggestionCode();
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
    this.productCategoriesService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: response => {
          this.selectedEntity = response;
          this.loadThumbnail(this.selectedEntity.coverPicture || '');
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  public saveChange() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toggleBlockUI(true);
    if (this.utilService.isEmpty(this.config.data?.id) == true) {
      this.productCategoriesService
        .create(this.form.value)
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
    } else {
      this.productCategoriesService
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

  private buildForm() {
    this.form = this.fb.group({
      name: new FormControl(
        this.selectedEntity.name || null,
        Validators.compose([Validators.required, Validators.maxLength(250)]),
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      parentId: new FormControl(this.selectedEntity.parentId || null),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      seoMetaDescription: new FormControl(
        this.selectedEntity.seoMetaDescription || null,
        Validators.required,
      ),
      visibility: new FormControl(this.selectedEntity.visibility ?? true),
      isActive: new FormControl(this.selectedEntity.isActive ?? true),
      coverPictureName: new FormControl(this.selectedEntity.coverPicture || null),
      coverPictureContent: new FormControl(this.coverPicture || null, Validators.required),
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  getNewSuggestionCode() {
    this.productCategoriesService
      .getSuggestNewCode()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: string) => {
          this.form.patchValue({
            code: response,
          });
        },
      });
  }

  onFileChange(event: any) {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.coverPicture = reader.result;
        this.form.patchValue({
          coverPictureName: file.name,
          coverPictureContent: reader.result,
        });

        this.cd.markForCheck();
      };
    }
  }

  loadThumbnail(fileName: string) {
    this.productCategoriesService
      .getCoverPicture(fileName)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: string) => {
          var fileExt = this.selectedEntity.coverPicture?.split('.').pop();
          this.coverPicture = this.sanitizer.bypassSecurityTrustResourceUrl(
            `data:image/${fileExt};base64, ${response}`,
          );
          this.form.patchValue({
            coverPictureContent: this.coverPicture.changingThisBreaksApplicationSecurity,
          });
        },
      });
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
