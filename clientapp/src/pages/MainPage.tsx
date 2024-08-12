
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { Box, Button, Typography } from '@mui/material'
import ThemeChooser from '../components/ThemeChooser'
import { useEffect } from 'react'
import { setSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'
import { green, orange, yellow } from '@mui/material/colors'
import { getMainApi } from '../axios.manager'

export const MainPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    useEffect(() => {
        dispatch(setSnack({
            msg: 'main page snack test',
            type: SnackNfoType.info
        }))
    }, [])

    return (
        <Box>
            {import.meta.env.DEV && <Typography color={yellow[400]}>Development environment</Typography>}
            {import.meta.env.PROD && <Typography color={green[400]}>Production environment</Typography>}
            {import.meta.env.VITE_GITCOMMIT && <Typography color={orange[400]}>Git commit {import.meta.env.VITE_GITCOMMIT}</Typography>}<br />
            master page<br />
            current user: {global.currentUser?.userName}<br />
            roles: {global.currentUser?.roles}<br />
            <ThemeChooser />
            <Button onClick={async () => {
                const api = getMainApi()
                await api.apiMainLongRunningGet()
            }}>long running op</Button>
        </Box>
    )

}