import { SortDirection } from "../../../../api/model/sortDirection"

/** generic supertype of column props */
export interface IDataGridColumn {
    header?: string
    /** initial flex width */
    flexWidth?: number
    /** initial fixed width */
    width?: number
    fieldKind?: FieldKind
    /** optional class for the data div */
    dataDivClass?: string
}

export enum FieldKind { generic, dateTimeOffset, numeric }

/** props of column */
export interface DataGridColumn<T> extends IDataGridColumn {
    /** initial collapsed */
    collapsed?: boolean
    fieldName?: string
    filterCaseSensitive?: boolean
    dbFunFilterPreprocess?: string
    getData: (x: T) => any
}

export enum CompareOp {
    lessThan = "<",
    lessThansOrEquals = "<=",
    Equals = "==",
    greatThan = ">",
    greatThanOrEquals = ">="
}

export interface FilterNfo {
    filter: string
    op: CompareOp
}

/** dynamic state of column */
export interface DataGridColumnState {
    sortDirection: SortDirection | null,

    /** (date valueof) of sort change ( used to prioritize a sort vs other when multi ) */
    sortTimestamp: number | null

    filter: FilterNfo | null

    customWidth?: number

    collapsed?: boolean

    columnIndex: number
}

export interface NeedLoadNfo {
    /** if set this dynamic filter will be used instead to recompute on load */
    precomputedDynFilter?: string
    columnsState: DataGridColumnState[]
    page: number
    pageSize: number
}

export interface NeedCountNfo {
    dynFilter: string
}

export interface ComputeDynFilerNfo {
    columnsState: DataGridColumnState[]
}