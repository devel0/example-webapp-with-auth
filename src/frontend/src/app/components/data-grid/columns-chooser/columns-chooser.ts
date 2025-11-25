import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent } from '@angular/material/dialog';
import { DataGrid } from '../data-grid';
import { BasicModule } from '../../../modules/basic/basic-module';

export interface ColumnChooserProps<T> {
  dataGrid: DataGrid<T>
}

@Component({
  selector: 'app-columns-chooser',
  imports: [BasicModule, MatDialogContent],
  templateUrl: './columns-chooser.html',
  styleUrl: './columns-chooser.scss',
})
export class ColumnsChooser<T> {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<ColumnsChooser<T>>) {
  }

  get dataGrid(): DataGrid<T> {
    const data: ColumnChooserProps<T> = this.data
    
    return this.data.dataGrid
  }

  closeDialog() {
    this.dialogRef.close()
  }

}
