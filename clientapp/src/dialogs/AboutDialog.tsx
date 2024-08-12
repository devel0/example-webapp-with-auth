import { Box, Dialog, Typography } from "@mui/material"
import { useState } from "react"
import { APP_TITLE, DEFAULT_SIZE_LARGE, DEFAULT_SIZE_SMALL } from "../constants/general"
import { green, yellow } from "@mui/material/colors"

export const AboutDialog = (props: {
    open: boolean,
    setOpen: React.Dispatch<React.SetStateAction<boolean>>
}) => {
    const { open, setOpen } = props

    return (
        <Dialog
            open={open}
            onClose={() => setOpen(false)}>
            <Box m={DEFAULT_SIZE_SMALL}>
                About
                <hr />
                {APP_TITLE}<br />
                {import.meta.env.DEV && <Typography color={yellow[400]}>Development environment</Typography>}
                {import.meta.env.PROD && <Typography color={green[400]}>Production environment</Typography>}
                Git commit sha : {import.meta.env.VITE_GITCOMMIT}<br />
                Git commit date : {import.meta.env.VITE_GITCOMMITDATE}<br />
            </Box>
        </Dialog>
    )
}