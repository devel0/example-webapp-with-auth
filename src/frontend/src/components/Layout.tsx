import { AboutDialog } from '../dialogs/AboutDialog';
import { APP_URL_FakeDatas, APP_URL_Users, DEFAULT_SIZE_0_5_REM } from '../constants/general'
import { authApi } from '../axios.manager';
import { Box, LinearProgress } from '@mui/material'
import { ReactNode, useState } from 'react'
import { useGlobalPersistService } from '../services/global-persist/Service';
import { useGlobalService } from '../services/global/Service';
import { useLocation, useNavigate } from 'react-router-dom'
import { useLoginManager } from '../hooks/useLoginManager';
import { useMobileDetect } from '../hooks/useMobileDetect';
import LogoutIcon from '@mui/icons-material/Logout';
import ResponsiveAppBar, { AppBarItem } from './ResponsiveAppBar'
import { useAlive } from '../hooks/useAlive';

type Props = {
    child: ReactNode
}

const MainLayout = (props: Props) => {
    const generalNetwork = useGlobalService(x => x.generalNetwork)
    const globalPersistState = useGlobalPersistService()

    const navigate = useNavigate()
    const location = useLocation()
    const [aboutDialogOpen, setAboutDialogOpen] = useState(false)

    useLoginManager()

    useAlive()

    useMobileDetect()

    const menuPages: AppBarItem[] = [
        {
            label: 'Fakedatas',
            selected: location.pathname === APP_URL_FakeDatas,
            url: APP_URL_FakeDatas
        },
        {
            hidden: !globalPersistState.currentUserCanManageUsers(),
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
            label: `${globalPersistState.currentUser?.userName}`
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

                globalPersistState.setLogout()
            }
        }
    ]

    return (
        <Box sx={{ width: '100%' }}>
            {/* <CssBaseline /> */}

            <ResponsiveAppBar pages={menuPages} settings={menuSettings} />

            <Box sx={{ minHeight: DEFAULT_SIZE_0_5_REM }}>
                {generalNetwork && <LinearProgress />}
            </Box>

            {props.child}

            <AboutDialog open={aboutDialogOpen} setOpen={setAboutDialogOpen} />
        </Box>
    )
}
export default MainLayout
