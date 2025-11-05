import { APP_URL_Login, RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC } from "../constants/general"
import { authApi } from "../axios.manager"
import { CurrentUserNfo } from "../types/CurrentUserNfo"
import { handleApiException } from "../utils/utils"
import { HttpStatusCode, AxiosError } from "axios"
import { useEffect } from "react"
import { useGlobalPersistService } from "../services/global-persist/Service"
import { useGlobalService } from "../services/global/Service"
import { useNavigate } from "react-router-dom"
import { loginRedirectUrlFrom } from "../components/ProtectedRoutes"

export const useLoginManager = () => {
    const setRefreshTokenExpiration = useGlobalPersistService(x => x.setRefreshTokenExpiration)
    const setLogout = useGlobalPersistService(x => x.setLogout)
    const setCurrentUser = useGlobalPersistService(x => x.setCurrentUser)
    const currentUser = useGlobalPersistService(x => x.currentUser)
    const currentUserInitialized = useGlobalPersistService(x => x.currentUserInitialized)
    const hydrated = useGlobalPersistService(x => x.hydrated)
    const refreshTokenExpiration = useGlobalPersistService(x => x.refreshTokenExpiration)
    
    const navigate = useNavigate()

    useEffect(() => {
        if (hydrated) {

            const act = () => {
                const q = refreshTokenExpiration
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
                                    setRefreshTokenExpiration(res.data.refreshTokenNfo.expiration)
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
    }, [hydrated])

    useEffect(() => {
        if (location.pathname !== APP_URL_Login() && (!currentUserInitialized || currentUser == null)) {

            authApi.apiAuthCurrentUserGet()
                .then(res => {
                    if (res.status === HttpStatusCode.Ok) {
                        const currentUser: CurrentUserNfo = {
                            userName: res.data.userName!,
                            email: res.data.email!,
                            roles: Array.from(res.data.roles ?? []),
                            permissions: Array.from(res.data.permissions ?? [])
                        }

                        setCurrentUser(currentUser)
                    }
                    else {
                        navigate(APP_URL_Login(loginRedirectUrlFrom()))
                    }
                })
                .catch(_err => {
                    const err = _err as AxiosError

                    if (err.response?.status === HttpStatusCode.Unauthorized) {
                        if (document.location.pathname !== APP_URL_Login()) {
                            setLogout()

                            document.location = APP_URL_Login()
                        }
                    }
                })
        }
    }, [location.pathname, currentUser, currentUserInitialized])
}