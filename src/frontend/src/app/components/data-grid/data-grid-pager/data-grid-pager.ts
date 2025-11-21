import { Component, ContentChildren, Input, QueryList } from '@angular/core';
import { BasicModule } from "../../../modules/basic/basic-module";
import { DataGrid } from '../data-grid';

@Component({
  selector: 'app-data-grid-pager',
  imports: [BasicModule],
  templateUrl: './data-grid-pager.html',
  styleUrl: './data-grid-pager.scss',
})
export class DataGridPager<T> {

  @Input({ required: true }) dataGrid!: DataGrid<T>

  @ContentChildren('[toolbar-prefix]') toolbarPrefix!: QueryList<Component>
  @ContentChildren('[toolbar-suffix]') toolbarSuffix!: QueryList<Component>

  constructor() {
  }

}
