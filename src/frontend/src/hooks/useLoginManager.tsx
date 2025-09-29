import { APP_URL_Login, RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC } from "../constants/general"
import { authApi } from "../axios.manager"
import { CurrentUserNfo } from "../types/CurrentUserNfo"
import { handleApiException } from "../utils/utils"
import { HttpStatusCode, AxiosError } from "axios"
import { useEffect } from "react"
import { useGlobalPersistService } from "../services/globalPersistService"
import { useGlobalService } from "../services/globalService"
import { useNavigate } from "react-router-dom"

export const useLoginManager = () => {
    const globalState = useGlobalService()
    const globalPersistState = useGlobalPersistService()
    const navigate = useNavigate()

    useEffect(() => {
        if (globalPersistState.hydrated) {

            const act = () => {
                const q = globalPersistState.refreshTokenExpiration
                if (q != null) {
                    const refreshTokenExpire = new Date(q);

                    const renewAt = new Date(refreshTokenExpire.getTime() - RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC * 1e3)
                    const now = new Date()
                    if (now.getTime() < renewAt.getTime()) {
                        console.log(`Renew refresh token at ${renewAt}`)
                        setTimeout(async () => {
                            try {
                                const res = await authApi.apiAuthRenewRefreshTokenPost()
                                if (res.data.refreshTokenNfo?.expiration) {
                                    globalPersistState.setRefreshTokenExpiration(res.data.refreshTokenNfo.expiration)
                                    act()
                                }
                            } catch (ex_) {
                                const ex = ex_ as AxiosError
                                handleApiException(ex, "Renew refresh token failed")
                            }
                        }, renewAt.getTime() - now.getTime());
                    }
                }
            }

            act()
        }
    }, [globalPersistState.hydrated])

    useEffect(() => {
        if (location.pathname !== APP_URL_Login() && (!globalPersistState.currentUserInitialized || globalPersistState.currentUser == null)) {

            authApi.apiAuthCurrentUserGet()
                .then(res => {
                    if (res.status === HttpStatusCode.Ok) {
                        const currentUser: CurrentUserNfo = {
                            userName: res.data.userName!,
                            email: res.data.email!,
                            roles: Array.from(res.data.roles ?? []),
                            permissions: Array.from(res.data.permissions ?? [])
                        }

                        globalPersistState.setCurrentUser(currentUser)
                    }
                    else {
                        globalState.setUrlWanted(location.pathname)

                        navigate(APP_URL_Login())
                    }
                })
                .catch(_err => {
                    const err = _err as AxiosError

                    if (err.response?.status === HttpStatusCode.Unauthorized) {
                        if (document.location.pathname !== APP_URL_Login()) {
                            globalState.setUrlWanted(location.pathname)

                            globalPersistState.setLogout()

                            document.location = APP_URL_Login()
                        }
                    }
                })
        }
    }, [location.pathname, globalPersistState.currentUser, globalPersistState.currentUserInitialized])
}