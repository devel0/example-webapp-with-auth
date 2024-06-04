import { useLocation, useNavigate } from 'react-router-dom'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { APP_URL_Login } from '../constants/general'
import { useEffect } from 'react'
import { setLoggedOut, setUrlWanted } from '../redux/slices/globalSlice'
import { Box, Button, CssBaseline, LinearProgress } from '@mui/material'
import { SnackComponent } from './SnackComponent'
import { genAuthApi } from '../axios.manager'

type Props = {
    child: JSX.Element
}

const MainLayout = (props: Props) => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()
    const location = useLocation()

    useEffect(() => {
        if (!global.currentUser && location.pathname !== APP_URL_Login) {
            dispatch(setUrlWanted(location.pathname))
            navigate(APP_URL_Login)
        }
    }, [location.pathname, global.currentUser])

    return (
        <Box>
            <CssBaseline />

            Toolbar

            <Button onClick={async () => {                
                const authApi = genAuthApi()
                await authApi.apiAuthLogoutGet()

                dispatch(setLoggedOut())
            }}>Logout</Button>

            {global.generalNetwork && <LinearProgress />}

            <SnackComponent />

            {props.child}
        </Box>
    )
}
export default MainLayout
