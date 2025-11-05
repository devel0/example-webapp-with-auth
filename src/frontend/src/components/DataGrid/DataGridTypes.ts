import { SortDirection } from "../../../api"

/** datagrid properties */
export interface DataGridProps<T> {
    columns: DataGridColumn<T>[]
    pageData: T[]
    onInit?: (dgApi: DataGridApi<T>) => void
    onColumnStateChanged?: (colIdx: number, col: DataGridColumn<T>, colState: DataGridColumnState<T>) => void
    /** if true a textbox for column filtering will included below the column header */
    filterTextBox?: boolean
}

/** datagrid api ref imperative methods */
export interface DataGridApi<T> {
    getColumnsState: () => DataGridColumnState<T>[] | null
    setColumnsState: (columnsState: DataGridColumnState<T>[]) => void
    /** set column width px rearranging others */
    setColumnWidth: (colIdx: number, width: number) => void
    setColumnSortDirection: (colIdx: number, sortDirection: SortDirection | null) => void
    setColumnSortDirectionByFieldName: (fn: string, sortDirection: SortDirection | null) => void
    setColumnFilter: (colIdx: number, filter: string | null) => void
}


/** generic supertype of column props */
export interface IDataGridColumn {
    flexWidth?: number
    width?: number
}

export enum FieldKind { generic, dateTimeOffset, numeric }

/** props of column */
export interface DataGridColumn<T> extends IDataGridColumn {
    header?: string
    /** initial flex width */
    flexWidth?: number
    /** initial fixed width */
    width?: number,
    /** initial collapsed */
    collapsed?: boolean
    fieldName?: string
    fieldKind?: FieldKind
    dbFunFilterPreprocess?: string
    getData: (x: T) => any
}

/** dynamic state of column */
export interface DataGridColumnState<T> {
    sortDirection: SortDirection | null,

    /** (date valueof) of sort change ( used to prioritize a sort vs other when multi ) */
    sortTimestamp: number | null

    filter: string | null

    filterCaseSensitive?: boolean

    customWidth?: number

    collapsed?: boolean
}
