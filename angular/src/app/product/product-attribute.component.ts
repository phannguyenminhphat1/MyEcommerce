import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ProductsService } from '@proxy/products';
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
import { DynamicDialogConfig } from 'primeng/dynamicdialog';
import { ImageModule } from 'primeng/image';
import { AttributeType } from '@proxy/my-ecommerce/product-attributes/attribute-type.enum';
import { ProductAttributeValueDto } from '@proxy/products/attributes/models';
import { NotificationService } from '../shared/services/notification.service';
import { ProductAttributeInListDto } from '@proxy/product-attributes/models';
import { ProductAttributesService } from '@proxy/product-attributes';
import { CancelDialogService } from '../shared/services/cancel-dialog.service';
import { TooltipModule } from 'primeng/tooltip';
import { DatePickerModule } from 'primeng/datepicker';

@Component({
  selector: 'app-product-attribute',
  templateUrl: './product-attribute.component.html',
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
    TooltipModule,
    DatePickerModule,
  ],
  providers: [ProductsService, ProductAttributesService],
})
export class ProductAttributeComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private productService = inject(ProductsService);
  private config = inject(DynamicDialogConfig);
  private notificationService = inject(NotificationService);
  private productAttributeService = inject(ProductAttributesService);
  private cancelDialogService = inject(CancelDialogService);

  private ngUnsubcribe = new Subject<void>();

  blockedPanel: boolean = false;
  btnDisabled = false;
  form!: FormGroup;
  attributes: any[] = [];
  fullAttributes: any[] = [];
  productAttributes: any[] = [];
  showDateTimeControl: boolean = false;
  showDecimalControl: boolean = false;
  showIntControl: boolean = false;
  showVarcharControl: boolean = false;
  showTextControl: boolean = false;

  constructor() {}

  ngOnInit(): void {
    this.buildForm();
    this.initFormData();
  }

  ngOnDestroy(): void {
    this.ngUnsubcribe.next();
    this.ngUnsubcribe.complete();
  }

  initFormData() {
    var attributes = this.productAttributeService.getListAll();
    this.toggleBlockUI(true);
    forkJoin({
      attributes,
    })
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: any) => {
          //Push data to dropdown
          this.fullAttributes = response.attributes;
          var attributes = response.attributes as ProductAttributeInListDto[];
          this.attributes = attributes.map(x => ({ value: x.id, label: x.label }));
          this.loadFormDetails(this.config.data?.id);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.productService
      .getListProductAttributeAll(id)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: (response: ProductAttributeValueDto[]) => {
          this.productAttributes = response;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  private buildForm() {
    this.form = this.fb.group({
      productId: new FormControl(this.config.data.id),
      attributeId: new FormControl(null, Validators.required),
      dateTimeValue: new FormControl(null),
      decimalValue: new FormControl(null),
      intValue: new FormControl(null),
      varcharValue: new FormControl(null),
      textValue: new FormControl(null),
    });
  }

  saveChange() {
    this.toggleBlockUI(true);
    this.productService
      .addProductAttribute(this.form.value)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: () => {
          this.toggleBlockUI(false);
          this.notificationService.showSuccess('Added product attribute successfully');
          this.loadFormDetails(this.config.data.id);
        },
        error: err => {
          this.notificationService.showError(err.error.error.message);
          this.toggleBlockUI(false);
        },
      });
  }

  selectAttribute(event: any) {
    var dataType = this.fullAttributes.filter(x => x.id == event.value)[0].dataType;
    this.showDateTimeControl = false;
    this.showDecimalControl = false;
    this.showIntControl = false;
    this.showTextControl = false;
    this.showVarcharControl = false;
    if (dataType == AttributeType.Date) {
      this.showDateTimeControl = true;
    } else if (dataType == AttributeType.Decimal) {
      this.showDecimalControl = true;
    } else if (dataType == AttributeType.Int) {
      this.showIntControl = true;
    } else if (dataType == AttributeType.Text) {
      this.showTextControl = true;
    } else if (dataType == AttributeType.Varchar) {
      this.showVarcharControl = true;
    }
  }

  removeItem(attribute: ProductAttributeValueDto) {
    var id = '';
    if (attribute.dataType == AttributeType.Date) {
      id = attribute.dateTimeId as string;
    } else if (attribute.dataType == AttributeType.Decimal) {
      id = attribute.decimalId as string;
    } else if (attribute.dataType == AttributeType.Int) {
      id = attribute.intId as string;
    } else if (attribute.dataType == AttributeType.Text) {
      id = attribute.textId as string;
    } else if (attribute.dataType == AttributeType.Varchar) {
      id = attribute.varcharId as string;
    }
    this.cancelDialogService.delete('Are you sure you want to delete this record?', () => {
      this.deleteItemsConfirmed(attribute, id);
    });
  }

  deleteItemsConfirmed(attribute: ProductAttributeValueDto, id: string) {
    this.toggleBlockUI(true);
    this.productService
      .removeProductAttribute(attribute.attributeId as string, id)
      .pipe(takeUntil(this.ngUnsubcribe))
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Deleted successfully');
          this.loadFormDetails(this.config.data?.id);
          this.toggleBlockUI(false);
        },
        error: err => {
          this.notificationService.showError(err.error.error.message);
          this.toggleBlockUI(false);
        },
      });
  }

  getDataTypeName(value: number) {
    return AttributeType[value];
  }

  getValueByType(attribute: ProductAttributeValueDto, value: number) {
    if (attribute.dataType == AttributeType.Date) {
      return attribute.dateTimeValue;
    } else if (attribute.dataType == AttributeType.Decimal) {
      return attribute.decimalValue;
    } else if (attribute.dataType == AttributeType.Int) {
      return attribute.intValue;
    } else if (attribute.dataType == AttributeType.Text) {
      return attribute.textValue;
    } else if (attribute.dataType == AttributeType.Varchar) {
      return attribute.varcharValue;
    }
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

  showEmptyControl(): boolean {
    return (
      !this.showDateTimeControl &&
      !this.showDecimalControl &&
      !this.showIntControl &&
      !this.showVarcharControl &&
      !this.showTextControl
    );
  }

  formatValue(attribute: ProductAttributeValueDto): string {
    switch (attribute.dataType) {
      case AttributeType.Date:
        if (!attribute.dateTimeValue) return '';
        return new Intl.DateTimeFormat('vi-VN', {
          dateStyle: 'short',
          timeStyle: 'short',
        }).format(new Date(attribute.dateTimeValue!));

      case AttributeType.Decimal:
        if (attribute.decimalValue == null) return '';

        return new Intl.NumberFormat('en-US', {
          minimumFractionDigits: 0,
          maximumFractionDigits: 2,
        }).format(attribute.decimalValue);

      case AttributeType.Int:
        if (attribute.intValue == null) return '';

        return new Intl.NumberFormat('en-US').format(attribute.intValue);

      case AttributeType.Text:
        return attribute.textValue ?? '';

      case AttributeType.Varchar:
        return attribute.varcharValue ?? '';

      default:
        return '';
    }
  }
}
