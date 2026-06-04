import { inject, Injectable } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

@Injectable({
  providedIn: 'root',
})
export class CancelDialogService {
  private confirmationService = inject(ConfirmationService);

  delete(message: string, accept: () => void) {
    this.confirmationService.confirm({
      header: 'Confirmation',
      icon: 'pi pi-exclamation-circle',
      message,
      acceptLabel: 'Delete',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-secondary p-button-outlined',
      accept,
    });
  }
}
