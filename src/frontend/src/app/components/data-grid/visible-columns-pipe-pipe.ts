import { Pipe, PipeTransform } from '@angular/core';
import { DataGridColumnState } from './types/data-grid-types';
import { Observable } from 'rxjs';

@Pipe({
  name: 'visibleColumnsPipe'
})
export class VisibleColumnsPipePipe implements PipeTransform {

  transform(columnsState: Observable<DataGridColumnState[] | null>): Observable<DataGridColumnState[] | null> {
    return columnsState
  }

}
