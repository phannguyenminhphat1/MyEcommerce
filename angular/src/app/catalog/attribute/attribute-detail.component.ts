import { Component, inject, OnDestroy, OnInit } from '@angular/core';
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
import { attributeTypeOptions } from '@proxy/my-ecommerce/product-attributes/attribute-type.enum';
import { ProductAttributeDto } from '@proxy/product-attributes/models';
import { ProductAttributesService } from '@proxy/product-attributes/product-attributes.service';

@Component({
  selector: 'app-attribute-detail',
  templateUrl: './attribute-detail.component.html',
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
  providers: [ProductAttributesService],
})
export class AttributeDetailComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private productAttributeService = inject(ProductAttributesService);
  private utilService = inject(UtilityService);
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);

  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  form!: FormGroup;
  dataTypes: any[] = [];
  selectedEntity = {} as ProductAttributeDto;

  constructor() {}

  validationMessages = {
    code: [{ type: 'required', message: 'Code is required and must be unique' }],
    label: [
      { type: 'required', message: 'Label is required' },
      { type: 'maxlength', message: 'Label cannot exceed 255 characters' },
    ],
    dataType: [{ type: 'required', message: 'Data type is required' }],
    sortOrder: [{ type: 'required', message: 'Please enter the sort order' }],
  };

  ngOnInit(): void {
    this.buildForm();
    this.loadProductAttributeTypes();
    this.initFormData();
  }

  initFormData() {
    this.toggleBlockUI(true);
    //Load edit data to form
    if (this.utilService.isEmpty(this.config.data?.id) == true) {
      this.getNewSuggestionCode();
      this.toggleBlockUI(false);
    } else {
      this.loadFormDetails(this.config.data?.id);
    }
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productAttributeService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
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

  loadProductAttributeTypes() {
    attributeTypeOptions.forEach(element => {
      this.dataTypes.push({
        value: element.value,
        label: element.key,
      });
    });
  }

  public saveChange() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.toggleBlockUI(true);
    if (this.utilService.isEmpty(this.config.data?.id) == true) {
      this.productAttributeService
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
      this.productAttributeService
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
      label: new FormControl(
        this.selectedEntity.label || null,
        Validators.compose([Validators.required, Validators.maxLength(250)]),
      ),
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      dataType: new FormControl(this.selectedEntity.dataType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      note: new FormControl(this.selectedEntity.note || null),
      visibility: new FormControl(this.selectedEntity.visibility ?? true),
      isActive: new FormControl(this.selectedEntity.isActive ?? true),
      isRequired: new FormControl(this.selectedEntity.isRequired ?? true),
      isUnique: new FormControl(this.selectedEntity.isUnique ?? false),
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  getNewSuggestionCode() {
    this.productAttributeService
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
