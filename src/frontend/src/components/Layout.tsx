import { AboutDialog } from '../dialogs/AboutDialog';
import { APP_URL_Users, DEFAULT_SIZE_0_5_REM } from '../constants/general'
import { authApi } from '../axios.manager';
import { Box, LinearProgress } from '@mui/material'
import { GlobalState } from '../redux/states/GlobalState'
import { ReactNode, useState } from 'react'
import { setLoggedOut } from '../redux/slices/globalSlice'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { useLocation, useNavigate } from 'react-router-dom'
import { useLoginManager } from '../hooks/useLoginManager';
import { useMobileDetect } from '../hooks/useMobileDetect';
import LogoutIcon from '@mui/icons-material/Logout';
import ResponsiveAppBar, { AppBarItem } from './ResponsiveAppBar'

type Props = {
    child: ReactNode
}

const MainLayout = (props: Props) => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()
    const location = useLocation()
    const [aboutDialogOpen, setAboutDialogOpen] = useState(false)

    useLoginManager()

    useMobileDetect()

    const menuPages: AppBarItem[] = [
        {
            hidden: !global.currentUserCanManageUsers,
            label: 'Users',
            selected: location.pathname === APP_URL_Users,
            url: APP_URL_Users
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
