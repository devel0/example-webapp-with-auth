import { DataGridColumn, DataGridColumnState, FieldKind } from "./DataGridTypes"
import { DateField } from "@mui/x-date-pickers"
import { Dayjs } from 'dayjs';
import { emptyString } from "../../utils/utils";
import { IconButton, InputAdornment, InputProps, MenuItem, Select, TextField } from "@mui/material"
import { useEffect, useState } from "react"
import BackspaceOutlinedIcon from '@mui/icons-material/BackspaceOutlined';
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
    columnsState: DataGridColumnState[],
    setColumnsState: (x: DataGridColumnState[]) => void
}) {
    const { col, colIdx, columnsState, setColumnsState } = props
    const [dtFrom, setDtFrom] = useState<Dayjs | null>(null);
    const [compareOp, setCompareOp] = useState<DateOp>(DateOp.Equals)
    const [numberFrom, setNumberFrom] = useState<number | null>(null)
    const [filterActive, setFilterActive] = useState(false)//!emptyString(props.columnFilter))

    useEffect(() => {
        setFilterActive(!emptyString(columnsState[colIdx].filter))
    }, [colIdx, columnsState])

    useEffect(() => {
        if (dtFrom == null) {
            columnsState[colIdx].filter = null
            setColumnsState([...columnsState])
        }
        else {
            try {
                columnsState[colIdx].filter = `${compareOp} "${dtFrom?.toISOString()}"`
                setColumnsState([...columnsState])
            } catch (err) {
                console.error(err)
            }
        }
    }, [compareOp, dtFrom])

    useEffect(() => {
        if (numberFrom == null) {
            columnsState[colIdx].filter = null
            setColumnsState([...columnsState])
        }
        else {
            try {
                columnsState[colIdx].filter = `${compareOp} ${numberFrom}`
                setColumnsState([...columnsState])
            } catch (err) {
                console.error(err)
            }
        }
    }, [compareOp, numberFrom])

    const clearFilterProp = (clearAction: () => void): Partial<InputProps> => {
        return {
            endAdornment: filterActive && (
                <InputAdornment position="end">
                    <IconButton onClick={() => clearAction()} edge="end">
                        <BackspaceOutlinedIcon />
                    </IconButton>
                </InputAdornment>
            ),
        }
    }

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
                // className={filterActive ? styles['filter'] : undefined}
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
                className={filterActive ? styles['filter'] : undefined}
                InputProps={clearFilterProp(() => {
                    setNumberFrom(null)
                })}
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
            InputProps={clearFilterProp(() => {
                columnsState[colIdx].filter = null
                setColumnsState([...columnsState])
            })}
            value={columnsState[colIdx].filter ?? ''}
            onChange={x => {
                columnsState[colIdx].filter = x.target.value
                setColumnsState([...columnsState])
            }}
        />
    </div>
}