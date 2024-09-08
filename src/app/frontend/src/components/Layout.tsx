import { useLocation, useNavigate } from 'react-router-dom'
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import LogoutIcon from '@mui/icons-material/Logout';
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { APP_URL_Login, APP_URL_Users, DEFAULT_SIZE_1_REM, DEFAULT_SIZE_0_5_REM, LOCAL_STORAGE_CURRENT_USER_NFO, LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE, RENEW_REFRESH_TOKEN_BEFORE_EXPIRE_SEC } from '../constants/general'
import { useEffect, useState } from 'react'
import { setLoggedOut, setSuccessfulLogin, setUrlWanted } from '../redux/slices/globalSlice'
import { Box, Button, CssBaseline, LinearProgress } from '@mui/material'
import ResponsiveAppBar, { AppBarItem } from './ResponsiveAppBar'
import { AboutDialog } from '../dialogs/AboutDialog';
import { CurrentUserNfo } from '../types/CurrentUserNfo';
import { authApi } from '../axios.manager';
import { AxiosError, HttpStatusCode } from 'axios';
import { useInterval } from 'usehooks-ts';
import { handleApiException } from '../utils/utils';

type Props = {
    child: JSX.Element
}

const MainLayout = (props: Props) => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()
    const location = useLocation()
    const [aboutDialogOpen, setAboutDialogOpen] = useState(false)

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
        if (location.pathname !== APP_URL_Login && (!global.currentUserInitialized || !global.currentUser)) {

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
                        navigate(APP_URL_Login)
                    }
                })
                .catch(_err => {
                    const err = _err as AxiosError

                    if (err.response?.status === HttpStatusCode.Unauthorized) {
                        if (document.location.pathname !== APP_URL_Login) {
                            dispatch(setUrlWanted(location.pathname))
                            localStorage.removeItem(LOCAL_STORAGE_CURRENT_USER_NFO)
                            document.location = APP_URL_Login
                        }
                    }
                })
        }
    }, [location.pathname, global.currentUser, global.currentUserInitialized])

    const menuPages: AppBarItem[] = [
        {
            hidden: !global.currentUserCanManageUsers,
            label: 'Users',
            onClick: () => navigate(APP_URL_Users)
        },
        {
            label: 'About',
            onClick: () => setAboutDialogOpen(true)
        },
    ]

    const menuSettings: AppBarItem[] = [

        {
            label: `${global.currentUser?.userName}`
        },

        // {
        //     label: 'Profile',
        //     icon: <AccountCircleIcon />
        // },

        {
            label: 'Logout',
            icon: <LogoutIcon />,
            onClick: async () => {
                await authApi.apiAuthLogoutGet()

                dispatch(setLoggedOut())
            }
        }
    ]

    return (
        <Box sx={{ width: '100%' }}>
            {/* <CssBaseline /> */}

            <ResponsiveAppBar pages={menuPages} settings={menuSettings} />

            <Box sx={{ minHeight: DEFAULT_SIZE_0_5_REM }}>
                {global.generalNetwork && <LinearProgress />}
            </Box>

            {props.child}

            <AboutDialog open={aboutDialogOpen} setOpen={setAboutDialogOpen} />
        </Box>
    )
}
export default MainLayout
