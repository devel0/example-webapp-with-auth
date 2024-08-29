import { enqueueSnackbar } from "notistack"
import { SnackNfo } from "../types/SnackNfo"
import {
    DEFAULT_FONTSIZE_NORMAL, DEFAULT_FONTSIZE_NORMAL2, DEFAULT_FONTWEIGHT_SEMIBOLD,
    DEFAULT_FONTWEIGHT_XBOLD
} from "../constants/general"
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
                fontSize={DEFAULT_FONTSIZE_NORMAL2}
                fontWeight={DEFAULT_FONTWEIGHT_XBOLD}>
                {nfo.title}
            </Typography>}

            {nfo.msg.map((msg, msgIdx) =>
                <Typography
                    fontSize={DEFAULT_FONTSIZE_NORMAL}
                    fontWeight={DEFAULT_FONTWEIGHT_SEMIBOLD}
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