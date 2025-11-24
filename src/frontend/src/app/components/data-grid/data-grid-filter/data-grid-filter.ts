import { BasicModule } from "../../../modules/basic/basic-module";
import { nameof } from "../../../utils/utils";
import { CompareOp, DataGridColumnState, FieldKind, FilterNfo, IDataGridColumn } from '../types/data-grid-types';
import { AfterViewInit, ChangeDetectorRef, Component, EventEmitter, Input, NgZone, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import { BehaviorSubject, debounceTime, distinctUntilChanged, Subscribable, Subscription } from 'rxjs';
import { DisableAutocompleteDirective } from "../../../directives/disable-autocomplete";

@Component({
  selector: 'app-data-grid-filter',
  imports: [BasicModule, DisableAutocompleteDirective],
  templateUrl: './data-grid-filter.html',
  styleUrl: './data-grid-filter.scss',
})
export class DataGridFilter implements OnInit, AfterViewInit, OnChanges, OnDestroy {
  CompareOp = CompareOp
  FieldKind = FieldKind

  @Input() debounceMs: number = 1000
  @Input() column!: IDataGridColumn
  @Input() columnState!: DataGridColumnState | null
  @Output() filterChange = new EventEmitter<FilterNfo | null>()

  filterActive = false

  get operator() { return this.operatorSubject.value }
  set operator(value: CompareOp | null) {
    this.operatorSubject.next(value)
  }

  private _text: string | null = null
  get text() { return this._text }
  set text(value: string | null) {
    this._text = value

    this.filterActive = (value != null && value.length > 0)

    this.textSubject.next(value)
  }

  private operatorSubject
  private operatorSubscription

  private textSubject
  private textSubscription

  constructor(
  ) {
    this.operatorSubject = new BehaviorSubject<CompareOp | null>(this.columnState?.filter?.op ?? CompareOp.Equals)
    this.operatorSubscription = this.operatorSubject
      .subscribe(x => {

        this.filterChange.emit(
          this.text == null ?
            null :
            {
              filter: this.text,
              op: this.operator ?? CompareOp.Equals
            })
      });

    this.textSubject = new BehaviorSubject<string | null>(null)
    this.textSubscription = this.textSubject
      .pipe(debounceTime(this.debounceMs))
      .pipe(distinctUntilChanged())
      .subscribe(x => {

        this.filterChange.emit(
          this.text == null ?
            null :
            {
              filter: this.text,
              op: this.operator ?? CompareOp.Equals
            })
      });
  }

  ngOnInit() { }

  ngAfterViewInit() { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes[nameof<DataGridFilter>('columnState')]) {
      const val: DataGridColumnState | null = changes[nameof<DataGridFilter>('columnState')].currentValue
      this.text = val?.filter?.filter ?? ''
    }
  }

  ngOnDestroy() {
    this.textSubscription.unsubscribe()
    this.operatorSubscription.unsubscribe()
  }

  clearFilter() {
    this.text = ''
  }

}
