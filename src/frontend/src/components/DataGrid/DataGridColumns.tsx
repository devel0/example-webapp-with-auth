import { Box, Button, Checkbox, Dialog, DialogActions, DialogContent, DialogTitle, FormControlLabel } from "@mui/material"
import { DataGridColumnState, IDataGridColumn } from "./DataGridTypes"

interface DataGridColumnsProps {
    open: boolean
    setOpen: (x: boolean) => void
    columns: IDataGridColumn[]
    columnsState: DataGridColumnState[]
    setColumnVisibility: (colIdx: number, collapsed: boolean) => void    
}

export const DataGridColumns = (
    props: DataGridColumnsProps,
) => {
    const { open, setOpen, columns, columnsState, setColumnVisibility } = props

    return <Box>
        <Dialog
            open
            onClose={() => setOpen(false)}
        >

            <DialogTitle>Columns chooser</DialogTitle>

            <DialogContent>
                {columns.map((col, colIdx) => <div key={`item-${colIdx}`}>
                    <FormControlLabel
                        label={col.header}
                        control={
                            <Checkbox
                                checked={(columnsState[colIdx].collapsed ?? false) !== true}
                                onChange={(a, checked) => setColumnVisibility(colIdx, !checked)}
                            />
                        }
                    />
                </div>)}
            </DialogContent>

            <DialogActions>
                <Button onClick={() => setOpen(false)}>Close</Button>
            </DialogActions>

        </Dialog>
    </Box>
}
