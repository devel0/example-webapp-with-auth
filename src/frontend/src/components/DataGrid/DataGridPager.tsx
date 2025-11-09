import { Box } from "@mui/material"
import { ForwardedRef, forwardRef, useImperativeHandle, useState } from "react"
import BackSvg from '../../images/back.svg?react'
import FirstSvg from '../../images/first.svg?react'
import ForwardSvg from '../../images/forward.svg?react'
import LastSvg from '../../images/last.svg?react'
import styles from './DataGridPager.module.scss'

interface DataGridPagerProps {
    /** current page (0 indexed) */
    page: number,

    /** change page state var */
    setPage: (x: number) => void,

    /** max rows for a page */
    pageSize: number,

    /** total number of rows */
    total: number
}

export interface DataGridPagerApi {
}

function DataGridPagerInner(
    props: DataGridPagerProps,
    ref: ForwardedRef<DataGridPagerApi>
) {
    const { page, pageSize, setPage, total } = props

    const dgApi: DataGridPagerApi = {

    }

    useImperativeHandle(ref, () => dgApi, [props])

    return <Box style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
        <button onClick={() => setPage(0)}>
            <FirstSvg />
        </button>
        <button onClick={() => setPage(Math.max(0, page - 1))}>
            <BackSvg />
        </button>
        <button onClick={() => setPage(page + 1)}>
            <ForwardSvg />
        </button>
        <button onClick={() => setPage(Math.max(0, Math.ceil(total / pageSize) - 1))}>
            <LastSvg />
        </button>
        <div className={styles['pager-page']}>
            <span>
                page: {page + 1} of {Math.ceil(total / pageSize)}
            </span>

            <span>
                ( {total} total rows )
            </span>
        </div>
    </Box>
}

export const DataGridPager = forwardRef(DataGridPagerInner)