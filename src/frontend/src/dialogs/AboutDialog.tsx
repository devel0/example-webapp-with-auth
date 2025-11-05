import { APP_TITLE, DEFAULT_FONTWEIGHT_600, DEFAULT_SIZE_1_REM } from "../constants/general"
import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography, useTheme } from "@mui/material"
import { green, yellow } from "@mui/material/colors"

export const AboutDialog = (props: {
    open: boolean,
    setOpen: React.Dispatch<React.SetStateAction<boolean>>
}) => {
    const theme = useTheme()
    const { open, setOpen } = props

    return (
        <Dialog
            open={open}
            onClose={() => setOpen(false)}>

            <DialogTitle>
                <Typography fontWeight={DEFAULT_FONTWEIGHT_600}>
                    About
                </Typography>
                <hr />
            </DialogTitle>

            <DialogContent sx={{
                background: theme.palette.mode === 'light' ? 'white' : undefined,
            }}>

                {APP_TITLE}<br />
                {import.meta.env.DEV && <Typography color={yellow[400]}>Development environment</Typography>}
                {import.meta.env.PROD && <Typography color={green[400]}>Production environment</Typography>}
                Git commit sha : {import.meta.env.VITE_GITCOMMIT}<br />
                Git commit date : {import.meta.env.VITE_GITCOMMITDATE}<br />

            </DialogContent>

            <DialogActions>
                <Box sx={{
                    display: 'flex',
                    mt: DEFAULT_SIZE_1_REM
                }}>
                    <Box sx={{ flexGrow: 1 }} />
                    <Button
                        variant='outlined'
                        onClick={() => {
                            setOpen(false)
                        }}
                        sx={{ mr: DEFAULT_SIZE_1_REM }}>                        
                        Close
                    </Button>
                </Box>
            </DialogActions>

        </Dialog>
    )
}