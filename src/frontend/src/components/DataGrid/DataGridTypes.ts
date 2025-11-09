import { SortDirection } from "../../../api"

/** generic supertype of column props */
export interface IDataGridColumn {
    header?: string
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
export interface DataGridColumnState {
    sortDirection: SortDirection | null,

    /** (date valueof) of sort change ( used to prioritize a sort vs other when multi ) */
    sortTimestamp: number | null

    filter: string | null

    filterCaseSensitive?: boolean

    customWidth?: number

    collapsed?: boolean
}
