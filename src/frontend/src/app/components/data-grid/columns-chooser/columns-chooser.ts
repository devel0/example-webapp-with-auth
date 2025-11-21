import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions } from '@angular/material/dialog';
import { DataGrid } from '../data-grid';
import { BasicModule } from '../../../modules/basic/basic-module';

@Component({
  selector: 'app-columns-chooser',
  imports: [BasicModule, MatDialogContent, MatDialogActions],
  templateUrl: './columns-chooser.html',
  styleUrl: './columns-chooser.scss',
})
export class ColumnsChooser<T> {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ColumnsChooser<T>>) {
  }

  get dataGrid() : DataGrid<T> {
    return this.data.dataGrid
  }

  closeDialog()
  {
    this.dialogRef.close()
  }

}
