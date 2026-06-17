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
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ImageModule } from 'primeng/image';
import { attributeTypeOptions } from '@proxy/my-ecommerce/product-attributes/attribute-type.enum';
import { RoleDto } from '@proxy/roles/models';
import { RolesService } from '@proxy/roles/roles.service';
import { ValidationMessageComponent } from 'src/app/shared/components/validation-message/validation-message.component';
import { UtilityService } from 'src/app/shared/services/utility.service';

@Component({
  selector: 'app-role-detail',
  templateUrl: './role-detail.component.html',
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
  providers: [RolesService],
})
export class RoleDetailComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private rolesService = inject(RolesService);
  private utilService = inject(UtilityService);
  private config = inject(DynamicDialogConfig);
  private ref = inject(DynamicDialogRef);

  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  btnDisabled = false;
  form!: FormGroup;
  dataTypes: any[] = [];
  selectedEntity = {} as RoleDto;

  constructor() {}

  validationMessages = {
    name: [{ type: 'required', message: 'Name is required' }],
    description: [{ type: 'required', message: 'Description is required' }],
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
      this.toggleBlockUI(false);
    } else {
      this.loadFormDetails(this.config.data?.id);
    }
  }

  loadFormDetails(id: string) {
    this.toggleBlockUI(true);
    this.rolesService
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
      this.rolesService
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
      this.rolesService
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
      name: new FormControl(this.selectedEntity.name || null, Validators.required),
      description: new FormControl(this.selectedEntity.description || null, Validators.required),
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
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
