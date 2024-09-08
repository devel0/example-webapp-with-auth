import { enqueueSnackbar } from "notistack"
import { SnackNfo } from "../types/SnackNfo"
import { DEFAULT_FONTWEIGHT_500, DEFAULT_FONTWEIGHT_900, DEFAULT_SIZE_1_25_REM, DEFAULT_SIZE_1_REM } from "../constants/general"
import { Box, Typography } from "@mui/material"
import { AxiosError } from "axios"

export const firstLetter = (str: string | undefined, capitalize: boolean = false) => {
    if (str && str.length > 0) {
        if (capitalize)
            return str[0].toUpperCase()

        return str[0]
    }
}

export const nullOrUndefined = (x: any) => x === null || x === undefined

export const handleApiException = async (ex: AxiosError, prefixMsg: string = 'Error') => {
    let msgs = [`${prefixMsg}: ${ex.response?.statusText}`]

    const detail = (ex.response?.data as any)?.detail

    if (detail)
        msgs.push(`Detail: ${detail}`)

    setSnack({
        title: prefixMsg,
        msg: msgs,
        type: "error"
    })
}

export function delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export const setSnack = (nfo: SnackNfo) => {

    enqueueSnackbar({
        message: <Box>
            {nfo.title && <Typography
                fontSize={DEFAULT_SIZE_1_25_REM}
                fontWeight={DEFAULT_FONTWEIGHT_900}>
                {nfo.title}
            </Typography>}

            {nfo.msg.map((msg, msgIdx) =>
                <Typography
                    fontSize={DEFAULT_SIZE_1_REM}
                    fontWeight={DEFAULT_FONTWEIGHT_500}
                    key={`snack-mex-${msgIdx}`}>
                    {msg}
                </Typography>
            )}
        </Box>,

        variant: nfo.type,

        preventDuplicate: true,

        autoHideDuration: nfo.durationMs === null ? null : (nfo.durationMs ?? 6000)
    })

}