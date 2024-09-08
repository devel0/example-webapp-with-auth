import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography, useTheme } from "@mui/material"
import DoDisturbIcon from '@mui/icons-material/DoDisturb';
import DoneOutlineIcon from '@mui/icons-material/DoneOutline';
import CloseIcon from '@mui/icons-material/Close';
import { APP_TITLE, DEFAULT_FONTWEIGHT_600, DEFAULT_SIZE_1_REM, DEFAULT_SIZE_0_5_REM } from "../constants/general"
import { green, yellow } from "@mui/material/colors"

export enum ConfirmDialogCloseResult {
    close,
    cancel,
    yes,
    no
}

export interface ConfirmDialogProps {
    open: boolean,
    setProps: (props: ConfirmDialogProps) => void,
    title?: string,
    msg?: string,
    onClose?: (action: ConfirmDialogCloseResult) => void,
    cancelButton?: boolean,
    yesButton?: boolean,
    noButton?: boolean,
    yesButtonText?: string,
    modal?: boolean,
    modalDisallowEscapeKey?: boolean    
}

export const ConfirmDialog = (props: ConfirmDialogProps) => {
    const theme = useTheme()
    const {
        open,
        setProps,
        title,
        msg,
        onClose,
        cancelButton,
        yesButton,
        noButton,
        yesButtonText,
        modal,
        modalDisallowEscapeKey
    } = props

    return (
        <Dialog
            open={open}
            onClose={(e, reason) => {
                if (modal === false || (reason !== 'backdropClick' && (modalDisallowEscapeKey !== true || reason !== 'escapeKeyDown')))
                    setProps({ ...props, open: false })
            }}>

            <DialogTitle>
                <Typography fontWeight={DEFAULT_FONTWEIGHT_600}>
                    {title ?? "Confirm dialog"}
                </Typography>
                <hr />
            </DialogTitle>

            <DialogContent sx={{
                background: theme.palette.mode === 'light' ? 'white' : undefined,
            }}>

                <Typography>
                    {msg ?? "Confirm to proceed ?"}
                </Typography>

            </DialogContent>

            <DialogActions>
                <Box sx={{
                    display: 'flex',
                    mt: DEFAULT_SIZE_1_REM
                }}>
                    <Box sx={{ flexGrow: 1 }} />

                    {cancelButton && <Button
                        variant='outlined'
                        onClick={() => {
                            onClose?.(ConfirmDialogCloseResult.cancel)
                            setProps({ ...props, open: false })
                        }}
                        sx={{ mr: DEFAULT_SIZE_1_REM }}>
                        <DoDisturbIcon sx={{ mr: DEFAULT_SIZE_0_5_REM }} />
                        Cancel
                    </Button>}

                    {noButton && <Button
                        variant='outlined'
                        onClick={async () => {
                            onClose?.(ConfirmDialogCloseResult.no)
                            setProps({ ...props, open: false })
                        }}
                        sx={{ mr: DEFAULT_SIZE_1_REM }}>
                        <CloseIcon sx={{ mr: DEFAULT_SIZE_0_5_REM }} />
                        No
                    </Button>}

                    {yesButton && <Button
                        onClick={async () => {
                            onClose?.(ConfirmDialogCloseResult.yes)
                            setProps({ ...props, open: false })
                        }}
                        variant='outlined'>
                        <DoneOutlineIcon sx={{ mr: DEFAULT_SIZE_0_5_REM }} />
                        {yesButtonText ?? "Yes"}
                    </Button>}
                </Box>
            </DialogActions>

        </Dialog >
    )
}