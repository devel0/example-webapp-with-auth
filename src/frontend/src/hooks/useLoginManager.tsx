import {
    APP_URL_Login, LOCAL_STORAGE_CURRENT_USER_NFO, LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE,
    RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC
} from "../constants/general"
import { authApi } from "../axios.manager"
import { CurrentUserNfo } from "../types/CurrentUserNfo"
import { GlobalState } from "../redux/states/GlobalState"
import { handleApiException } from "../utils/utils"
import { HttpStatusCode, AxiosError } from "axios"
import { setSuccessfulLogin, setUrlWanted } from "../redux/slices/globalSlice"
import { useAppSelector, useAppDispatch } from "../redux/hooks/hooks"
import { useEffect } from "react"
import { useNavigate } from "react-router-dom"

export const useLoginManager = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (global.currentUserInitialized) {
            const act = () => {
                const q = localStorage.getItem(LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE);
                if (q) {
                    const refreshTokenExpire = new Date(q);
                    console.log(`refresh token will expire at ${refreshTokenExpire}`)

                    const renewAt = new Date(refreshTokenExpire.getTime() - RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC * 1e3);
                    const now = new Date()
                    if (now.getTime() < renewAt.getTime()) {
                        console.log(`  renew at ${renewAt}`)
                        setTimeout(async () => {
                            console.log("  renewing refresh token");
                            try {
                                const res = await authApi.apiAuthRenewRefreshTokenGet();
                                if (res.data.refreshTokenNfo?.expiration) {
                                    localStorage.setItem(LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE, res.data.refreshTokenNfo.expiration);
                                    act()
                                }
                            } catch (ex_) {
                                const ex = ex_ as AxiosError
                                handleApiException(ex, "Renew refresh token")
                            }
                        }, renewAt.getTime() - now.getTime());
                    }
                }
            }

            act()
        }
    }, [global.currentUserInitialized])

    useEffect(() => {
        if (location.pathname !== APP_URL_Login() && (!global.currentUserInitialized || !global.currentUser)) {

            authApi.apiAuthCurrentUserGet()
                .then(res => {
                    if (res.status === HttpStatusCode.Ok) {
                        const currentUser: CurrentUserNfo = {
                            userName: res.data.userName!,
                            email: res.data.email!,
                            roles: Array.from(res.data.roles ?? []),
                            permissions: Array.from(res.data.permissions ?? [])
                        }

                        dispatch(setSuccessfulLogin(currentUser))
                    }
                    else {
                        dispatch(setUrlWanted(location.pathname))
                        navigate(APP_URL_Login())
                    }
                })
                .catch(_err => {
                    const err = _err as AxiosError

                    if (err.response?.status === HttpStatusCode.Unauthorized) {
                        if (document.location.pathname !== APP_URL_Login()) {
                            dispatch(setUrlWanted(location.pathname))
                            localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO)
                            document.location = APP_URL_Login()
                        }
                    }
                })
        }
    }, [location.pathname, global.currentUser, global.currentUserInitialized])
}