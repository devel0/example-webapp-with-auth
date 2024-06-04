
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { Box, Button } from '@mui/material'
import ThemeChooser from './ThemeChooser'
import { useEffect } from 'react'
import { setSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'
import { genAuthApi, genAdminApi } from '../axios.manager'

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