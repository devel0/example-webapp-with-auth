import { AxiosError, HttpStatusCode } from "axios"
import { Box, Button, styled, Typography } from "@mui/material"
import { closeSnackbar, enqueueSnackbar } from "notistack"
import { DEFAULT_FONTWEIGHT_500, DEFAULT_FONTWEIGHT_900, DEFAULT_SIZE_1_25_REM, DEFAULT_SIZE_1_REM } from "../constants/general"
import { from } from "linq-to-typescript";
import { LinkProps } from "react-router-dom"
import { SnackNfo } from "../types/SnackNfo"

export const LinkButton = styled(Button)<LinkProps>(() => ({
    // your link styles
    // width: '100%'
}))

export const firstLetter = (str: string | undefined, capitalize: boolean = false) => {
    if (str && str.length > 0) {
        if (capitalize)
            return str[0].toUpperCase()

        return str[0]
    }
}

export const nullOrUndefined = (x: any) => x === null || x === undefined

export const handleApiException = async (ex: AxiosError, prefixMsg: string = 'Error') => {
    let msgs = [`${ex.response?.statusText}`]

    const detail = (ex.response?.data as any)?.detail

    if (typeof detail === 'string') {
        const str: string = detail
        const ss = str.split('\n')
        for (let i = 0; i < ss.length; ++i) msgs.push(ss[i])
    }

    setSnack({
        title: prefixMsg,
        msg: msgs,
        type: ex.response?.status === HttpStatusCode.BadRequest ? "warning" : "error"
    })
}

export function delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export const setSnack = (nfo: SnackNfo) => {

    const id = enqueueSnackbar({
        message: <div onClick={() => {
            closeSnackbar(id)
        }}>
            {nfo.title && <Typography
                fontSize={DEFAULT_SIZE_1_25_REM}
                fontWeight={DEFAULT_FONTWEIGHT_900}>
                {nfo.title}
            </Typography>}

            {nfo.msg.length > 0 && <Box>
                <Typography
                    fontSize={DEFAULT_SIZE_1_REM}
                    fontWeight={DEFAULT_FONTWEIGHT_500}
                    key={`snack-mex-0`}>
                    {nfo.msg[0]}
                </Typography>

                {nfo.msg.length > 2 ?

                    <ul>
                        {from(nfo.msg).skip(1).toArray().map((msg, msgIdx) => <li key={`snack-mex-${msgIdx}`}>
                            <Typography
                                fontSize={DEFAULT_SIZE_1_REM}
                                fontWeight={DEFAULT_FONTWEIGHT_500}>
                                {msg}
                            </Typography>

                        </li>)}
                    </ul>

                    :

                    nfo.msg.length > 1 &&

                    <Typography
                        fontSize={DEFAULT_SIZE_1_REM}
                        fontWeight={DEFAULT_FONTWEIGHT_500}
                        key={`snack-mex-0`}>
                        {nfo.msg[1]}
                    </Typography>

                }
            </Box>

            }
        </div>,

        variant: nfo.type,

        preventDuplicate: true,

        autoHideDuration: nfo.durationMs === null ? null : (nfo.durationMs ?? 6000),

        hideIconVariant: nfo.type === 'error'

    })

}

export const computeIsMobile = () => {
    const w = window.innerWidth
    const isMobile = w <= 600

    return isMobile
}

export const generateUrl = (schema: string, values: { [key: string]: string | undefined }) => {
    let res = schema

    Object.keys(values).forEach(k => {
        const v = values[k]

        if (v !== undefined) {
            var rgx = new RegExp(`/:${k}[?]*`, "g")
            res = res.replace(rgx, `/${v}`)
        }
    })

    return res
}