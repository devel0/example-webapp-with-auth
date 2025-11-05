import { FormControl, IconButton, InputAdornment, InputLabel, MenuItem, Select, TextField } from "@mui/material"
import BackspaceOutlinedIcon from '@mui/icons-material/BackspaceOutlined';
import { DataGridColumn, DataGridColumnState, FieldKind } from "./DataGridTypes"
import { DateField, DatePicker } from "@mui/x-date-pickers"
import { useEffect, useId, useState } from "react"
import { Dayjs } from 'dayjs';
import { emptyString } from "../../utils/utils";
import styles from './DataGridFilter.module.scss'

enum DateOp {
    lessThan = "<",
    lessThanOrEquals = "<=",
    Equals = "==",
    greatThan = ">",
    greatThanOrEquals = ">="
}

export function DataGridFilter<T>(props: {
    col: DataGridColumn<T>,
    colIdx: number,
    columnsState: DataGridColumnState<T>[],
    columnFilter: string | null,
    setColumnFilter: (colIdx: number, filter: string | null) => void
}) {
    const { col, colIdx, columnsState, setColumnFilter } = props
    const [dtFrom, setDtFrom] = useState<Dayjs | null>(null);
    const [compareOp, setCompareOp] = useState<DateOp>(DateOp.Equals)
    const [numberFrom, setNumberFrom] = useState<number | null>(null)
    const [filterActive, setFilterActive] = useState(!emptyString(props.columnFilter))

    useEffect(() => {
        setFilterActive(!emptyString(props.columnFilter))
    }, [props.columnFilter])

    useEffect(() => {
        if (dtFrom == null)
            setColumnFilter(colIdx, null)
        else {
            try {
                setColumnFilter(colIdx, `${compareOp} "${dtFrom?.toISOString()}"`)
            } catch (err) {
                console.error(err)
            }
        }
    }, [compareOp, dtFrom])

    useEffect(() => {
        if (numberFrom == null)
            setColumnFilter(colIdx, null)
        else {
            try {
                setColumnFilter(colIdx, `${compareOp} ${numberFrom}`)
            } catch (err) {
                console.error(err)
            }
        }
    }, [compareOp, numberFrom])

    if (col.fieldKind === FieldKind.dateTimeOffset) {
        return <div style={{ display: 'flex', flexDirection: 'row', gap: '1rem' }}>
            <div>
                <Select
                    size='small'
                    value={compareOp}
                    onChange={x => {
                        setCompareOp(x.target.value as DateOp)

                    }}
                >
                    <MenuItem value={DateOp.lessThan}>{DateOp.lessThan}</MenuItem>
                    <MenuItem value={DateOp.lessThanOrEquals}>{DateOp.lessThanOrEquals}</MenuItem>
                    <MenuItem value={DateOp.Equals}>{DateOp.Equals}</MenuItem>
                    <MenuItem value={DateOp.greatThan}>{DateOp.greatThan}</MenuItem>
                    <MenuItem value={DateOp.greatThanOrEquals}>{DateOp.greatThanOrEquals}</MenuItem>
                </Select>
            </div>

            <DateField
                fullWidth
                size="small"
                value={dtFrom}
                onChange={x => setDtFrom(x)}
            />
        </div>
    }

    if (col.fieldKind === FieldKind.numeric) {
        return <div style={{ display: 'flex', flexDirection: 'row', gap: '1rem' }}>
            <div>
                <Select
                    size='small'
                    value={compareOp}
                    onChange={x => {
                        setCompareOp(x.target.value as DateOp)

                    }}
                >
                    <MenuItem value={DateOp.lessThan}>{DateOp.lessThan}</MenuItem>
                    <MenuItem value={DateOp.lessThanOrEquals}>{DateOp.lessThanOrEquals}</MenuItem>
                    <MenuItem value={DateOp.Equals}>{DateOp.Equals}</MenuItem>
                    <MenuItem value={DateOp.greatThan}>{DateOp.greatThan}</MenuItem>
                    <MenuItem value={DateOp.greatThanOrEquals}>{DateOp.greatThanOrEquals}</MenuItem>
                </Select>
            </div>

            <TextField
                fullWidth
                size="small"
                type="number"
                value={numberFrom ?? ''}
                onChange={x => {
                    if (emptyString(x.target.value))
                        setNumberFrom(null)
                    else
                        setNumberFrom(parseFloat(x.target.value))
                }}
            />
        </div>
    }

    return <div>
        <TextField
            fullWidth
            size='small'
            className={filterActive ? styles['filter'] : undefined}
            InputProps={{
                endAdornment: filterActive && (
                    <InputAdornment position="end">
                        <IconButton onClick={() => { setColumnFilter(colIdx, null) }} edge="end">
                            <BackspaceOutlinedIcon />
                        </IconButton>
                    </InputAdornment>
                ),
            }}
            value={columnsState[colIdx].filter ?? ''}
            onChange={x => { setColumnFilter(colIdx, x.target.value) }}
        />
    </div>
}