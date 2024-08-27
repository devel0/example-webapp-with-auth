import { useLocation, useNavigate } from 'react-router-dom'
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import LogoutIcon from '@mui/icons-material/Logout';
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { APP_URL_Login, APP_URL_Users, DEFAULT_SIZE_SMALL, DEFAULT_SIZE_XSMALL, LOCAL_STORAGE_CURRENT_USER_NFO } from '../constants/general'
import { useEffect, useState } from 'react'
import { setLoggedOut, setSuccessfulLogin, setUrlWanted } from '../redux/slices/globalSlice'
import { Box, Button, CssBaseline, LinearProgress } from '@mui/material'
import ResponsiveAppBar, { AppBarItem } from './ResponsiveAppBar'
import { AboutDialog } from '../dialogs/AboutDialog';
import { CurrentUserNfo } from '../types/CurrentUserNfo';
import { authApi } from '../fetch.manager';
import { ResponseError } from '../../api';
import { HttpStatusCode } from 'axios';

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
        if (location.pathname !== APP_URL_Login && (!global.currentUserInitialized || !global.currentUser)) {

            authApi.apiAuthCurrentUserGet()
                .then(res => {
                    if (res.status === 'OK') {
                        const currentUser: CurrentUserNfo = {
                            userName: res.userName!,
                            email: res.email!,
                            roles: Array.from(res.roles ?? []),
                            permissions: Array.from(res.permissions ?? [])
                        }

                        dispatch(setSuccessfulLogin(currentUser))
                    }
                    else {
                        dispatch(setUrlWanted(location.pathname))
                        navigate(APP_URL_Login)
                    }
                })
                .catch(_err => {
                    const err = _err as ResponseError
                    if (err.response.status === HttpStatusCode.Unauthorized) {
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
            label: 'Profile',
            icon: <AccountCircleIcon />
        },
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

            <Box sx={{ minHeight: DEFAULT_SIZE_XSMALL }}>
                {global.generalNetwork && <LinearProgress />}
            </Box>

            {props.child}

            <AboutDialog open={aboutDialogOpen} setOpen={setAboutDialogOpen} />
        </Box>
    )
}
export default MainLayout
