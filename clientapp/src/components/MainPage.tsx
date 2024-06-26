
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { Box, Button, Typography } from '@mui/material'
import ThemeChooser from './ThemeChooser'
import { useEffect } from 'react'
import { setSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'
import { genAdminApi } from '../axios.manager'
import { green, yellow } from '@mui/material/colors'

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
            {import.meta.env.DEV && <Typography color={yellow[400]}>Development environment</Typography>}<br />
            {import.meta.env.PROD && <Typography color={green[400]}>Production environment</Typography>}<br />
            master page<br />
            current user: {global.currentUser?.userName}<br />
            roles: {global.currentUser?.roles}<br />
            <ThemeChooser />
            <Button onClick={async () => {
                const api = genAdminApi()
                await api.apiLongRunning()
            }}>long running op</Button>
        </Box>
    )

}