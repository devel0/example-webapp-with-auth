import styles from './DataGrid.module.scss'
import { DataGridApi, DataGridColumn, DataGridColumnState, DataGridProps } from './DataGridTypes'
import { forwardRef, RefObject, useEffect, useImperativeHandle, useRef, useState } from 'react'
import { from } from 'linq-to-typescript';
import { SortDirection } from '../../../api';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import ArrowDropUpIcon from '@mui/icons-material/ArrowDropUp';
import { TextField } from '@mui/material';
import { DataGridFilter } from './DataGridFilter';
import { DataGridColumnWidthHandler } from './DataGridColumnWidthHandler';
import { useResizeObserver } from 'usehooks-ts';

// https://oida.dev/typescript-react-generic-forward-refs/

// Redecalare forwardRef
declare module "react" {
    function forwardRef<T, P = {}>(
        render: (props: P, ref: React.Ref<T>) => React.ReactNode | null
    ): (props: P & React.RefAttributes<T>) => React.ReactNode | null;
}

function DataGridInner<T>(
    props: DataGridProps<T>,
    ref: React.ForwardedRef<DataGridApi<T>>) {

    const { pageData, columns } = props
    const [columnsState, setColumnsState] = useState<DataGridColumnState<T>[] | null>(null)
    const [refInit, setRefInit] = useState(false)
    const tableRef = useRef<HTMLTableElement>(null)
    const mainDivRef = useRef<HTMLDivElement>(null)

    const getColumnsState = () => columnsState

    const getColumnIdxByFieldName = (fn: string) => props.columns.findIndex(w => w.fieldName === fn)

    const setColumnSortDirection = (colIdx: number, sortDirection: SortDirection | null) => {
        if (columnsState == null) return

        columnsState[colIdx].sortDirection = sortDirection
        columnsState[colIdx].sortTimestamp = new Date().valueOf()

        setColumnsState([...columnsState])
    }

    const setColumnSortDirectionByFieldName = (fn: string, sortDirection: SortDirection | null) => {
        const colIdx = getColumnIdxByFieldName(fn)
        if (colIdx !== -1)
            setColumnSortDirection(colIdx, sortDirection)
    }

    const setColumnFilter = (colIdx: number, filter: string | null) => {
        if (columnsState == null) return

        const colState = columnsState[colIdx]
        const col = columns[colIdx]

        columnsState[colIdx].filter = filter

        setColumnsState([...columnsState])
        props.onColumnStateChanged?.(colIdx, col, colState)
    }

    const tableSize = useResizeObserver({
        ref: mainDivRef as RefObject<HTMLDivElement>,
        box: 'border-box'
    })

    const setColumnWidth = (colIdx: number, width: number) => {
        if (columnsState == null) return

        const widthRest = (columnsState[colIdx].customWidth ?? 0) - width

        columnsState[colIdx].customWidth = width

        let s = 0

        let visibleAfterThis = 0
        for (let idx = colIdx + 1; idx < columns.length; ++idx) {
            if (columns[idx].collapsed !== true)
                visibleAfterThis++
        }

        for (let idx = colIdx + 1; idx < columns.length; ++idx) {
            const col = columns[idx]

            columnsState[idx].customWidth! += Math.ceil(widthRest / visibleAfterThis)
        }

        console.log(`set sum width ${s}`)

        setColumnsState([...columnsState])
    }

    const dgApi: DataGridApi<T> = {
        getColumnsState,
        setColumnsState,
        setColumnWidth,
        setColumnSortDirection,
        setColumnSortDirectionByFieldName,
        setColumnFilter
    }

    useImperativeHandle(ref, () => dgApi, [props])

    const toggleColumnSort = (colIdx: number, additive: boolean) => {
        if (columnsState == null) return

        if (!additive) columnsState.forEach((w, wIdx) => {
            if (wIdx === colIdx) return
            w.sortDirection = null
            w.sortTimestamp = null
        })
        const colState = columnsState[colIdx]
        const col = columns[colIdx]

        if (colState.sortDirection == null)
            colState.sortDirection = 'Ascending'
        else if (colState.sortDirection == 'Ascending')
            colState.sortDirection = 'Descending'
        else
            colState.sortDirection = null

        colState.sortTimestamp = new Date().valueOf()

        setColumnsState([...columnsState])
        props.onColumnStateChanged?.(colIdx, col, colState)
    }

    // onInit
    useEffect(() => {
        if (tableSize.width != null) {

            if (columnsState != null) {
                if (refInit === false && props.onInit != null) {
                    setRefInit(true)
                    props.onInit(dgApi)
                }
            }

        }
    }, [
        columnsState,
        props.onInit, refInit
    ])

    // table init
    useEffect(() => {

        if (tableSize.width != null) {            
            const tableWidth = tableSize.width
            const fixedWidth = from(columns).sum(w => w.width ?? 0)
            const flexSum = from(columns).where(w => w.width == null).sum(w => w.flexWidth ?? 1)
            const flexAvailWidth = tableWidth - fixedWidth

            const newColumnsState = from(columns)
                .select((col, colIdx) => {
                    let colWidth = col.width ?? 0
                    if (col.collapsed !== true) {
                        if (col.width == null) {
                            colWidth = Math.ceil((col.flexWidth ?? 1) / flexSum * flexAvailWidth) - 8
                        }
                    }

                    if (columnsState != null) {
                        return {
                            ...columnsState[colIdx],
                            customWidth: colWidth
                        }
                    }
                    else
                        return {
                            customWidth: colWidth,
                            collapsed: col.collapsed
                        } as DataGridColumnState<T>
                })
                .toArray()

            setColumnsState(newColumnsState)
        }

    }, [
        tableSize.width,
        columns,        
    ])

    // update column widths when table width changes
    useEffect(() => {
        if (columnsState != null && tableSize.width != null) {
            // console.log(`tableSize: ${tableSize.width} x ${tableSize.height}`)
            let columnsFlexWidth: (number | null)[] = []

            const customWidthSum = from(columnsState).where(r => r.collapsed !== true).sum(r => r.customWidth ?? 0)

            for (let colIdx = 0; colIdx < columnsState.length; ++colIdx) {
                const columnState = columnsState[colIdx]

                if (columnState.collapsed !== true) {
                    if (columnState.customWidth != null)
                        columnsFlexWidth.push(columnState.customWidth / customWidthSum)
                    else
                        columnsFlexWidth.push(null)
                }
                else
                    columnsFlexWidth.push(null)
            }

            for (let colIdx = 0; colIdx < columnsState.length; ++colIdx) {
                const columnState = columnsState[colIdx]

                const colFlexWidth = columnsFlexWidth[colIdx]

                if (colFlexWidth != null) {
                    columnState.customWidth = Math.ceil(tableSize.width * colFlexWidth) - 8
                }

                // console.log(`column state [${colIdx}] : flex:${colFlexWidth} => width:${columnState.customWidth}`)
            }

            setColumnsState([...columnsState])
        }
    }, [tableSize.width])

    return <div
        className={styles['data-grid-div']}
        ref={mainDivRef}
    >

        <table
            className={styles['data-grid-table']}
            ref={tableRef}
        >

            {columnsState != null && <>

                <thead className={styles['data-grid-thead']}>
                    <tr className={styles['data-grid-thead-tr']}>
                        {/* TABLE COLUMN HEADERS */}
                        {props.columns.map((col, colIdx) =>
                            <td
                                className={styles['data-grid-th']}
                                key={`col-${colIdx}`}
                                width={`${columnsState[colIdx].customWidth}px`}
                            >
                                <div>
                                    <div
                                        onClick={(e) => { toggleColumnSort(colIdx, e.shiftKey) }}
                                        style={{ display: 'flex', flexDirection: 'row' }}
                                    >
                                        <b>{col.header}</b>
                                        <span style={{ flexGrow: 1 }} />

                                        {columnsState[colIdx].sortDirection != null &&
                                            columnsState[colIdx].sortDirection === 'Ascending' &&
                                            <ArrowDropUpIcon />}

                                        {columnsState[colIdx].sortDirection != null &&
                                            columnsState[colIdx].sortDirection === 'Descending' &&
                                            <ArrowDropDownIcon />}
                                    </div>

                                    {props.filterTextBox === true && <DataGridFilter
                                        col={col}
                                        colIdx={colIdx}
                                        columnsState={columnsState}
                                        columnFilter={columnsState[colIdx].filter}
                                        setColumnFilter={setColumnFilter}
                                    />}

                                </div>

                                <DataGridColumnWidthHandler
                                    onColumnWidthChange={deltaWidth => {
                                        setColumnWidth(colIdx, (columnsState[colIdx].customWidth ?? 0) + deltaWidth)
                                    }}
                                />
                            </td>)}
                    </tr>
                </thead>

                {/* TABLE ROWS */}
                <tbody>
                    {pageData.map((row, rowIdx) => <tr
                        className={styles['data-grid-tr']}
                        key={`row-${rowIdx}`}
                    >
                        {props.columns.map((col, colIdx) =>
                            <td
                                className={styles['data-grid-td']}
                                key={`body-td-${colIdx}`}
                                width={`${columnsState[colIdx].customWidth}px`}
                            >
                                {col.getData(row)}
                            </td>)}
                    </tr>)}
                </tbody>

            </>}

        </table>



    </div>
}

export const DataGrid = forwardRef(DataGridInner)
