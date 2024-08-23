import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography, useTheme } from "@mui/material"
import { APP_TITLE, DEFAULT_FONTWEIGHT_BOLD, DEFAULT_SIZE_SMALL } from "../constants/general"
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
                <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
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
                    mt: DEFAULT_SIZE_SMALL
                }}>
                    <Box sx={{ flexGrow: 1 }} />
                    <Button
                        variant='outlined'
                        onClick={() => {
                            setOpen(false)
                        }}
                        sx={{ mr: DEFAULT_SIZE_SMALL }}>                        
                        Close
                    </Button>
                </Box>
            </DialogActions>

        </Dialog>
    )
}