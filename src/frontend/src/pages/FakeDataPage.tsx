import styles from './FakeDataPage.module.scss'
import { useEffect, useMemo, useRef, useState } from "react"
import { APP_TITLE } from "../constants/general"
import { Box, Button, setRef } from "@mui/material"
import { fakeDataApi } from "../axios.manager"
import { AxiosError, HttpStatusCode } from "axios"
import { getScrollbarWidth, handleApiException, pathBuilder } from "../utils/utils"
import { MSG_ERROR_LOAD } from "../constants/messages"
import { FakeData, GenericSort, SortModelItem } from "../../api"
import { useGlobalService } from '../services/global/Service'
import { DataGrid } from '../components/DataGrid/DataGrid'
import { useWindowSize } from 'usehooks-ts'
import { DataGridApi, DataGridColumn, DataGridColumnState, FieldKind } from '../components/DataGrid/DataGridTypes'
import { from } from 'linq-to-typescript'
import { buildGenericDynFilter, ColumnFilterNfo } from '../components/DataGrid/DataGridDynFilter'

export const FakeDataPage = () => {
    type TDATA = FakeData
    const [page, setPage] = useState(0)
    const [pageSize, setPageSize] = useState(25)
    const [pageData, setPageData] = useState<TDATA[]>([])
    const appBarHeight = useGlobalService(x => x.appBarHeight)
    const divRef = useRef<HTMLDivElement>(null)
    const [refTop, setRefTop] = useState<number | null>(null)
    // const [dgApi, setdgApi] = useState<DataGridApi<TDATA> | null>(null)
    const [columnsState, setColumnsState] = useState<DataGridColumnState<TDATA>[] | null>(null)
    const fnTDATA = pathBuilder<TDATA>()
    const dgApiRef = useRef<DataGridApi<TDATA> | null>(null)

    useEffect(() => {
        document.title = `${APP_TITLE} - FakeData`
    }, [])

    const columns: DataGridColumn<TDATA>[] = [
        {
            header: 'Id',
            fieldName: fnTDATA('id'),
            flexWidth: 2,
            // width: 350,
            getData: x => x.id,
            dbFunFilterPreprocess: 'DbFun.GuidString'
        },
        {
            header: 'FirstName',
            fieldName: fnTDATA('firstName'),
            getData: x => x.firstName,
            // initialSortDirection: 'Ascending'
        },
        {
            header: 'LastName',
            fieldName: fnTDATA('lastName'),
            getData: x => x.lastName
        },
        {
            header: 'Email',
            fieldName: fnTDATA('email'),
            getData: x => x.email
        },
        {
            header: 'Phone',
            fieldName: fnTDATA('phone'),
            getData: x => x.phone
        },
        {
            header: 'Group',
            fieldName: fnTDATA('groupNumber'),
            fieldKind: FieldKind.numeric,
            getData: x => x.groupNumber,
        },
        {
            header: 'Birth date',
            fieldName: fnTDATA('dateOfBirth'),
            fieldKind: FieldKind.dateTimeOffset,
            getData: x => new Date(x.dateOfBirth ?? '').toISOString()
        },
    ]

    useEffect(() => {
        const load = async () => {
            console.log('load server data')
            try {
                let sort: GenericSort | undefined = undefined
                let dynFilter: string | null = null

                if (columnsState != null) {
                    const qSort = from(columnsState)
                        .select((colState, colIdx) => { return { colIdx, col: columns[colIdx], colState, } })
                        .where(r => r.col.fieldName != null && r.colState.sortDirection != null)
                        .orderBy(w => w.colState.sortTimestamp)
                        .toArray()
                    if (qSort.length > 0) {
                        sort = {
                            columns: from(qSort).select(w => {
                                const res: SortModelItem = {
                                    columnName: w.col.fieldName,
                                    direction: w.colState.sortDirection ?? undefined,
                                }
                                return res
                            }).toArray()
                        }
                    }

                    const qFilter = from(columnsState)
                        .select((colState, colIdx) => { return { colIdx, col: columns[colIdx], colState, } })
                        .where(r => r.colState.filter != null)
                        .toArray()
                    if (qFilter.length > 0) {
                        // const columnFilters : ColumnFilterKeyValue[] =[]
                        dynFilter = buildGenericDynFilter({
                            columnFilters: qFilter.map(x => {
                                return {
                                    key: x.col.fieldName,
                                    value: x.colState.filter,
                                    filterCaseSensitive: x.colState.filterCaseSensitive,
                                    kind: x.col.fieldKind,
                                    preProcessField: x.col.dbFunFilterPreprocess
                                } as ColumnFilterNfo<FakeData>
                            })
                        })
                    }
                }

                if (dynFilter != null)
                    console.log(`dynFilter = ${dynFilter}`)

                const q = await fakeDataApi.apiFakeDataGetFakeDatasPost({
                    count: pageSize,
                    offset: page * pageSize,
                    sort,
                    dynFilter
                })

                if (q.status === HttpStatusCode.Ok) {
                    setPageData(q.data)
                }
            } catch (_ex) {
                const ex = _ex as AxiosError
                handleApiException(ex, MSG_ERROR_LOAD)
            }
        }

        load()
    }, [
        page, pageSize, columnsState
    ])

    useEffect(() => {
        if (divRef.current) {
            // console.log(`divRef offsetTop: ${divRef.current.offsetTop}`)
            setRefTop(divRef.current.offsetTop)
        }
    }, [divRef])

    const windowSize = useWindowSize()

    return <Box style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>

        <Box style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
            <span>
                page: {page}
            </span>
            <button onClick={() => setPage(Math.max(0, page - 1))}>
                prev
            </button>
            <button onClick={() => setPage(page + 1)}>
                next
            </button>
        </Box>

        <div ref={divRef} />

        {refTop != null && appBarHeight != null && <Box
            sx={{
                height: `${windowSize.height - refTop - 16}px`,
            }}>

            <div className={styles['table-demo-div']}>

                <DataGrid
                    // onApi={x => setdgApi(x)}
                    ref={dgApiRef}
                    filterTextBox
                    onInit={dgApi => {
                        console.log(`dg INIT`)
                        dgApi.setColumnSortDirectionByFieldName(fnTDATA('firstName'), 'Ascending')
                    }}
                    onColumnStateChanged={() => {
                        setColumnsState(dgApiRef.current?.getColumnsState() ?? null)
                    }}
                    columns={columns}
                    pageData={pageData}
                />

            </div>
        </Box>}

    </Box>
}
