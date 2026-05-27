import { CommonModule } from '@angular/common';
import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-validation-message',
  standalone: true,
  templateUrl: './validation-message.component.html',
  imports: [CommonModule],
})
export class ValidationMessageComponent implements OnInit {
  @Input() entityForm!: FormGroup;
  @Input() fieldName!: string;
  @Input() validationMessages: any;
  constructor() {}

  ngOnInit(): void {}
}
