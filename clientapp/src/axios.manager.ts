import axios, { AxiosError, HttpStatusCode } from "axios"
import { API_URL } from "./constants"
import { Action, Dispatch } from "@reduxjs/toolkit"
import { setGeneralNetwork, setSnack } from "./redux/slices/globalSlice"
import { SnackNfoType } from "./types/SnackNfo"
import { APP_URL_Login, LOCAL_STORAGE_CURRENT_USER_NFO } from "./constants/general"
import { AuthApi, Configuration, AdminApi } from "../api"

let dbgCnt = 0

export const ConfigAxios = (dispatch: Dispatch<Action>) => {

    axios.defaults.withCredentials = true

    axios.interceptors.request.use(
        async (config) => {            
            dispatch(setGeneralNetwork(true))

            return config
        },

        (error) => {
            Promise.reject(error)
        }
    )

    axios.interceptors.response.use(
        (response) => {
            // console.log(`response interceptor ${response.request.responseURL} [${dbgCnt--}]`)

            if (dbgCnt <= 0)
                dispatch(setGeneralNetwork(false))

            return response
        },

        (error: AxiosError) => {
            if (error?.response?.status === HttpStatusCode.InternalServerError) {
                dispatch(setSnack({
                    msg: String(error.response.data),
                    type: SnackNfoType.error
                }))
            }
            else if (error?.response?.status === HttpStatusCode.Unauthorized) {
                if (document.location.pathname !== APP_URL_Login) {
                    localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO)
                    document.location = APP_URL_Login
                }

                if (error.response.headers.length > 0) {
                    // console.log('refreshing token')
                }
            }
        }
    )

}

export const genAdminApi = () => {
    const config = new Configuration()
    return new AdminApi(config, API_URL())
}

export const genAuthApi = () => {
    const config = new Configuration()
    return new AuthApi(config, API_URL())
}
