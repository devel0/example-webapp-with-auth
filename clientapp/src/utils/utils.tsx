import { ResponseError } from "../../api"
import { setSnack } from "../redux/slices/globalSlice"
import { store } from "../redux/stores/store"
import { SnackNfoType } from "../types/SnackNfo"

export const firstLetter = (str: string | undefined, capitalize: boolean = false) => {
    if (str && str.length > 0) {
        if (capitalize)
            return str[0].toUpperCase()

        return str[0]
    }
}

export const nullOrUndefined = (x: any) => x === null || x === undefined

export const handleApiException = async (ex: ResponseError, prefixMsg: string = 'Error') => {
    let msg = [`${prefixMsg}: ${ex.response.statusText}`]

    try {
        const json = await ex.response.json()

        if (json.detail)
            msg.push(`Detail: ${json.detail}`)
    }
    catch { }

    store.dispatch(setSnack({
        msg: msg,
        type: SnackNfoType.error
    }))
}

export function delay(ms: number) {
    return new Promise( resolve => setTimeout(resolve, ms) );
}