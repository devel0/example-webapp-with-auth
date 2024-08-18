import { Action, Dispatch } from "@reduxjs/toolkit"
import { AuthApi, Configuration, ErrorContext, FetchParams, MainApi, Middleware, ResponseContext } from "../api"
import { API_URL, APP_URL_Login, LOCAL_STORAGE_CURRENT_USER_NFO } from "./constants/general"
import { setGeneralNetwork, setSnack } from "./redux/slices/globalSlice";
import { HttpStatusCode } from "axios";
import { useAppDispatch } from "./redux/hooks/hooks";
import { store } from "./redux/stores/store";
import { SnackNfoType } from "./types/SnackNfo";

export class APIMiddleware implements Middleware {

    pre(context: ResponseContext): Promise<FetchParams | void> {
        store.dispatch(setGeneralNetwork(true))

        return Promise.resolve({ url: context.url, init: context.init });
    }

    post(context: ResponseContext): Promise<Response | void> {
        store.dispatch(setGeneralNetwork(false))

        if (context.response.status === HttpStatusCode.InternalServerError) {
            context.response.text().then(res => {
                const obj = JSON.parse(res)
                let title = ""
                let msg = ""
                if (obj.title || obj.detail) {
                    title = obj.title
                    msg = obj.detail
                }
                else {
                    msg = res
                }
                store.dispatch(setSnack({
                    title: title,
                    msg: msg,
                    type: SnackNfoType.error
                }))
            })
        }

        return Promise.resolve(context.response);
    }

    onError(context: ErrorContext): Promise<Response | void> {

        return Promise.resolve(context.response)
    }
}

export const authApi = new AuthApi(new Configuration({
    basePath: API_URL(),
    middleware: [new APIMiddleware()]
}))

export const mainApi = new MainApi(new Configuration({
    basePath: API_URL(),
    middleware: [new APIMiddleware()]
}))
