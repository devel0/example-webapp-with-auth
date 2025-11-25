import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { BasicModule } from '../../modules/basic/basic-module';
import { BehaviorSubject } from 'rxjs';
import { ComputeDynFilerNfo, DataGridColumn, DataGridColumnState, FieldKind, FilterNfo, NeedCountNfo, NeedLoadNfo } from './types/data-grid-types';
import { DataGridFilter } from "./data-grid-filter/data-grid-filter";
import { from } from 'linq-to-typescript';
import { SizeNfo, TrackResize } from "../../directives/track-resize";
import { DataGridColumnHandle } from "./data-grid-column-handle/data-grid-column-handle";
import { MatDialog } from '@angular/material/dialog';
import { ColumnChooserProps, ColumnsChooser } from './columns-chooser/columns-chooser';
import { VisibleColumnsPipePipe } from "./visible-columns-pipe-pipe";
import { SkipIfPipe } from "../utils/skip-if-pipe";
import { emptyString } from '../../utils/utils';

@Component({
  selector: 'app-data-grid',
  imports: [BasicModule, DataGridFilter, TrackResize, DataGridColumnHandle, SkipIfPipe],
  templateUrl: './data-grid.html',
  styleUrl: './data-grid.scss'
})
export class DataGrid<T> implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('tableRef') tableRef!: ElementRef<HTMLTableElement>

  FieldKind = FieldKind

  @Input({ alias: 'loadData', required: true }) loadData!: (nfo: NeedLoadNfo) => Promise<T[] | null>
  @Input({ alias: 'countData', required: true }) countData!: (nfo: NeedCountNfo) => Promise<number | null>
  @Input({ alias: 'computeDynFilter', required: true }) computeDynFilter!: (nfo: ComputeDynFilerNfo) => string
  @Input({ alias: 'columns', required: true }) columns: DataGridColumn<T>[] = []
  @Input('filterTextBox') filterTextBox: boolean = false
  @Input('getRowId') getRowId!: (row: T) => string

  @Output() rowClicked = new EventEmitter<T>()
  @Output() rowDoubleClicked = new EventEmitter<T>()

  private selectedRowIds = new BehaviorSubject<Set<string>>(new Set<string>())
  selectedRowIds$ = this.selectedRowIds.asObservable()

  private dynFilter = new BehaviorSubject<string | null>(null)
  dynFilter$ = this.dynFilter.asObservable()

  private page = new BehaviorSubject<number>(0)
  page$ = this.page.asObservable()

  private pageSize = new BehaviorSubject<number>(25)
  pageSize$ = this.pageSize.asObservable()

  private totals = new BehaviorSubject<number>(0)
  totals$ = this.totals.asObservable()

  private totalPages = new BehaviorSubject<number>(0)
  totalPages$ = this.totalPages.asObservable()

  private pageData = new BehaviorSubject<T[]>([])
  pageData$ = this.pageData.asObservable()

  private columnsState = new BehaviorSubject<DataGridColumnState[] | null>(null)
  columnsState$ = this.columnsState.asObservable()

  private singleClickTimeoutMs = 250
  private singleClickTimeoutRefs: number[] = []

  get currentColumnState() { return this.columnsState.value }

  tableSize: SizeNfo | null = null
  initialLoadExecuted = false

  constructor(
    public dialog: MatDialog
  ) {
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
  }

  ngOnDestroy() {
  }

  onTableSizeChanged(size: SizeNfo) {
    const widthChanged = size.width != (this.tableSize?.width ?? 0)
    this.tableSize = size
    if (widthChanged)
      this.onTableResize()
  }

  onColumnWidthDeltaChange(colIdx: number, deltaWidth: number) {
    if (this.columnsState.value != null)
      this.setColumnWidth(colIdx, (this.columnsState.value[colIdx].customWidth ?? 0) + deltaWidth)
  }

  setColumnWidth(colIdx: number, width: number) {
    if (this.columnsState.value == null) return

    const newColumnsState = [...this.columnsState.value]

    const widthRest = (newColumnsState[colIdx].customWidth ?? 0) - width

    newColumnsState[colIdx].customWidth = width

    let s = 0

    let visibleAfterThis = 0
    for (let idx = colIdx + 1; idx < this.columns.length; ++idx) {
      if (this.columns[idx].collapsed !== true)
        visibleAfterThis++
    }

    for (let idx = colIdx + 1; idx < this.columns.length; ++idx) {
      const col = this.columns[idx]

      newColumnsState[idx].customWidth! += Math.ceil(widthRest / visibleAfterThis)
    }

    this.columnsState.next(newColumnsState)
  }

  isRowSelected(row: T) {
    if (this.getRowId != null) {
      return this.selectedRowIds.value.has(this.getRowId(row))
    }
    return false
  }

  async OnRowClicked(row: T) {
    this.singleClickTimeoutRefs.push(setTimeout(() => {
      this.toggleRowSelect(row)
      this.rowClicked.emit(row)
    }, this.singleClickTimeoutMs))
  }

  async OnRowDoubleClicked(row: T) {
    this.singleClickTimeoutRefs.forEach(w => clearTimeout(w))
    this.singleClickTimeoutRefs = []
    this.rowDoubleClicked.emit(row)
  }

  async onFilterChanged(colIdx: number, filterNfo: FilterNfo | null) {
    if (this.columnsState.value == null) return

    const newColumnsState = [...this.columnsState.value]
    newColumnsState[colIdx] = { ...this.columnsState.value[colIdx] }

    const colState = newColumnsState[colIdx]
    const col = this.columns[colIdx]
    let filterChangedFromPrevious =
      (colState.filter == null && !emptyString(filterNfo?.filter))
      ||
      (colState.filter != null && colState.filter.filter !== (filterNfo?.filter ?? ''))
    colState.filter = filterNfo

    this.columnsState.next(newColumnsState)

    if (filterChangedFromPrevious) {
      await this.execCountAndLoad()
    }
  }

  setColumnVisibility(colIdx: number, collapsed: boolean) {
    if (this.columnsState.value == null) return
    const newColumnsState = [...this.columnsState.value]
    newColumnsState[colIdx] = { ...this.columnsState.value[colIdx] }
    const colState = newColumnsState[colIdx]
    colState.collapsed = collapsed
    this.columnsState.next(newColumnsState)
  }

  async toggleColumnSort(colIdx: number, additive: boolean) {
    if (this.columnsState.value == null) return
    const newColumnsState = [...this.columnsState.value]
    newColumnsState[colIdx] = { ...this.columnsState.value[colIdx] }

    if (!additive) this.columnsState.value.forEach((w, wIdx) => {
      if (wIdx === colIdx) return
      w.sortDirection = null
      w.sortTimestamp = null
    })
    const colState = newColumnsState[colIdx]

    if (colState.sortDirection == null)
      colState.sortDirection = 'Ascending'
    else if (colState.sortDirection == 'Ascending')
      colState.sortDirection = 'Descending'
    else
      colState.sortDirection = null

    colState.sortTimestamp = new Date().valueOf()

    this.columnsState.next(newColumnsState)

    this.execLoad()
  }

  async firstPage() {
    if (this.page.value !== 0) {
      this.page.next(0)
      this.execLoad()
    }
  }

  async prevPage() {
    if (this.page.value > 0) {
      this.page.next(this.page.value - 1)
      this.execLoad()
    }
  }

  async nextPage() {
    if (this.page.value < this.totalPages.value - 1) {
      this.page.next(this.page.value + 1)
      this.execLoad()
    }
  }

  async lastPage() {
    if (this.page.value !== this.totalPages.value - 1) {
      this.page.next(this.totalPages.value - 1)
      this.execLoad()
    }
  }

  showColumnsChooser() {
    this.dialog.open<ColumnsChooser<T>, ColumnChooserProps<T>>(ColumnsChooser, {
      data: {
        dataGrid: this
      }
    })
  }

  private async onTableResize() {
    if (this.tableSize != null) {

      const tableWidth = this.tableSize.width
      const fixedWidth = from(this.columns)
        .select((w, colIdx) => {
          if (this.columnsState.value != null) {
            const colState = this.columnsState.value[colIdx]
            if (colState.collapsed === true) return 0
            if (colState.customWidth != null) return colState.customWidth
          }
          return w.width ?? 0
        })
        .sum(w => w)
      const flexSum = from(this.columns).where(w => w.width == null).sum(w => w.flexWidth ?? 1)
      const flexAvailWidth = tableWidth - fixedWidth

      const newColumnsState = from(this.columns)
        .select((col, colIdx) => {
          let colWidth = col.width ?? 0
          if (col.collapsed !== true) {
            if (col.width == null) {
              colWidth = Math.ceil((col.flexWidth ?? 1) / flexSum * flexAvailWidth) - 8
            }
          }

          if (this.columnsState != null && this.columnsState.value) {
            const colState = this.columnsState.value[colIdx]

            return {
              ...(colState),
              customWidth: Math.ceil((colState.customWidth ?? 0) / fixedWidth * (this.tableSize?.width ?? 0)) - 8
            }
          }
          else {

            return {
              columnIndex: colIdx,
              customWidth: colWidth,
              collapsed: col.collapsed
            } as DataGridColumnState
          }
        })
        .toArray()

      // console.debug(`col widths ${from(newColumnsState).select(w => w.customWidth ?? 0).toArray().join(',')}`)

      this.columnsState.next(newColumnsState)

      if (!this.initialLoadExecuted) {
        this.initialLoadExecuted = true

        console.log(`initial load => exec count and load`)
        await this.execCountAndLoad()
      }
    }
  }

  /** compute dynfilter and update {@link dynFilter} */
  private execComputeDynFilter() {
    const _dynFilter = this.computeDynFilter({ columnsState: this.columnsState.value ?? [] })
    this.dynFilter.next(_dynFilter)
  }

  /** count, load  and update {@link totals}, {@link page}, {@link totalPages} and {@link pageData} */
  private async execCountAndLoad() {
    this.execComputeDynFilter()
    await this.execCount()
    this.execLoad()
  }

  /** count and update {@link totals}, {@link page}, {@link totalPages} */
  private async execCount() {
    const _totals = await this.countData({ dynFilter: this.dynFilter.value ?? '' }) ?? 0
    this.totals.next(_totals)
    this.totalPages.next(Math.ceil(_totals / this.pageSize.value))
    if (this.page.value > this.totalPages.value - 1)
      this.page.next(Math.max(0, this.totalPages.value - 1))
  }

  /** reload current page  */
  async reload() {
    await this.execLoad()
  }

  /** load and update {@link pageData} */
  private async execLoad() {
    const data = await this.loadData({
      precomputedDynFilter: this.dynFilter.value ?? '',
      columnsState: this.columnsState.value ?? [],
      page: this.page.value,
      pageSize: this.pageSize.value
    })
    this.pageData.next(data ?? [])
  }

  private toggleRowSelect(row: T) {
    if (this.getRowId == null) {
      console.debug(`cannot select row if getRowId is null`)
      return
    }

    const rowId = this.getRowId(row)
    const newSelectedRows = new Set<string>(this.selectedRowIds.value)
    if (newSelectedRows.has(rowId))
      newSelectedRows.delete(rowId)
    else
      newSelectedRows.add(rowId)
    this.selectedRowIds.next(newSelectedRows)
  }

}
