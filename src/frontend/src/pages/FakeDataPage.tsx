import { APP_TITLE } from "../constants/general"
import { AxiosError, HttpStatusCode } from "axios"
import { Box, Button } from "@mui/material"
import { buildGenericDynFilter, ColumnFilterNfo } from '../components/DataGrid/DataGridDynFilter'
import { DataGrid, DataGridApi } from '../components/DataGrid/DataGrid'
import { DataGridColumn, DataGridColumnState, FieldKind } from '../components/DataGrid/DataGridTypes'
import { DataGridColumns } from '../components/DataGrid/DataGridColumns'
import { DataGridPager } from '../components/DataGrid/DataGridPager'
import { FakeData, GenericSort, SortModelItem } from "../../api"
import { fakeDataApi } from "../axios.manager"
import { from } from 'linq-to-typescript'
import { handleApiException, pathBuilder } from "../utils/utils"
import { MSG_ERROR_LOAD } from "../constants/messages"
import { RefObject, useEffect, useRef, useState } from "react"
import { useGlobalService } from '../services/global/Service'
import { useResizeObserver, useWindowSize } from 'usehooks-ts'
import styles from './FakeDataPage.module.scss'

type TDATA = FakeData
const fnTDATA = pathBuilder<TDATA>()

//
// DEFINE DATAGRID COLUMNS
//    
const columns: DataGridColumn<TDATA>[] = [
    {
        header: 'Id',
        fieldName: fnTDATA('id'),
        flexWidth: 1,
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
        getData: x => x.lastName,      
        filterCaseSensitive: false          
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
        flexWidth: 2,
        fieldName: fnTDATA('dateOfBirth'),
        fieldKind: FieldKind.dateTimeOffset,
        getData: x => new Date(x.dateOfBirth ?? '').toISOString()
    },
]

export const FakeDataPage = () => {
    const [columnsChooserOpen, setColumnsChooserOpen] = useState(false)
    const [columnsState, setColumnsState] = useState<DataGridColumnState[] | null>(null)
    const [dynFilter, setDynFilter] = useState<string | null>(null)
    const [page, setPage] = useState(0)
    const [pageData, setPageData] = useState<TDATA[]>([])
    const [pageSize, setPageSize] = useState(25)
    const [refTop, setRefTop] = useState<number | null>(null)
    const [total, setTotal] = useState<number | null>(null)
    const appBarHeight = useGlobalService(x => x.appBarHeight)
    const dgApiRef = useRef<DataGridApi<TDATA> | null>(null)
    const divRef = useRef<HTMLDivElement>(null)

    useEffect(() => {
        document.title = `${APP_TITLE} - FakeData`
    }, [])

    //
    // DEFINE PAGED/SORT/FILTER BACKEND LOADER
    //
    useEffect(() => {
        const load = async () => {
            // console.log(' load serverdata')

            try {
                let sort: GenericSort | undefined = undefined
                let dynFilterTmp: string | null = null

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
                        dynFilterTmp = buildGenericDynFilter({
                            columnFilters: qFilter.map(x => {
                                return {
                                    key: x.col.fieldName,
                                    value: x.colState.filter,
                                    filterCaseSensitive: x.col.filterCaseSensitive,
                                    kind: x.col.fieldKind,
                                    preProcessField: x.col.dbFunFilterPreprocess
                                } as ColumnFilterNfo<FakeData>
                            })
                        })
                    }
                }

                if (dynFilterTmp != null)
                    console.log(`dynFilter = ${dynFilterTmp}`)

                setDynFilter(dynFilterTmp)

                const q = await fakeDataApi.apiFakeDataGetFakeDatasPost({
                    count: pageSize,
                    offset: page * pageSize,
                    sort,
                    dynFilter: dynFilterTmp
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
        page, pageSize,
        columnsState
    ])

    //
    // DEFINE COUNTER BACKEND LOADER
    //
    useEffect(() => {
        const load = async () => {
            try {
                const q = await fakeDataApi.apiFakeDataCountFakeDatasPost({
                    dynFilter
                })
                if (q.status === HttpStatusCode.Ok)
                    setTotal(q.data)
            } catch (_ex) {
                const ex = _ex as AxiosError
                handleApiException(ex, MSG_ERROR_LOAD)
            }
        }

        load()
    }, [dynFilter])

    // DUMMY DIVREF TO COMPUTE TABLE FIXED HEIGHT
    const divRefSize = useResizeObserver({
        ref: divRef as RefObject<HTMLDivElement>,
        box: 'border-box'
    })

    useEffect(() => {
        if (divRef.current) {            
            setRefTop(divRef.current.offsetTop)
        }
    }, [divRef.current, divRefSize.width, divRefSize.height])

    const windowSize = useWindowSize()

    return <Box style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>

        {total != null && <DataGridPager
            customBefore={
                <Button
                    variant='outlined'
                    onClick={() => setColumnsChooserOpen(true)}
                >
                    Columns
                </Button>
            }
            page={page} setPage={setPage}
            pageSize={pageSize}
            total={total}
        />}

        <div ref={divRef} />

        {refTop != null && appBarHeight != null && <Box
            sx={{
                height: `${windowSize.height - refTop - 16}px`,
            }}>

            <div className={styles['table-demo-div']}>

                <DataGrid
                    ref={dgApiRef}
                    filterTextBox                    
                    onInit={dgApi => {
                        console.log(`dg INIT`)
                        // dgApi.setColumnSortDirectionByFieldName(fnTDATA('firstName'), 'Ascending')
                    }}                    
                    columns={columns}
                    columnsState={columnsState}
                    setColumnsState={x => setColumnsState(x)}
                    pageData={pageData}
                />

            </div>
        </Box>}

        {columnsState != null && columnsChooserOpen && <DataGridColumns
            open={columnsChooserOpen}
            setOpen={x => setColumnsChooserOpen(x)}
            columns={columns}
            columnsState={columnsState}
            setColumnVisibility={(colIdx, collapsed) => {
                columnsState[colIdx].collapsed = collapsed
                setColumnsState([...columnsState])
            }}
        />}
    </Box>
}
