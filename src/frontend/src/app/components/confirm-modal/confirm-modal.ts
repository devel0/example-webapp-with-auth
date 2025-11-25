import { Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { MatCard, MatCardTitle, MatCardContent, MatCardActions } from "@angular/material/card";
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BasicModule } from "../../modules/basic/basic-module";
import { A11yModule } from '@angular/cdk/a11y';

export interface ConfirmModalButton {
  buttonLabel: string,
  onClickResult: any,
  isDefault?: boolean
}

export interface ConfirmModalProps {
  title: string,
  msg: string,
  buttons: ConfirmModalButton[]
}

@Component({
  selector: 'app-confirm-modal',
  imports: [MatCard, MatCardTitle, MatCardContent, MatCardActions, BasicModule, A11yModule],
  templateUrl: './confirm-modal.html',
  styleUrl: './confirm-modal.scss',
})
export class ConfirmModal {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ConfirmModal>) {      
  }

  get modalProps(): ConfirmModalProps { return this.data }

  closeDialog(dialogResult?: any) {
    this.dialogRef.close(dialogResult)
  }

}
