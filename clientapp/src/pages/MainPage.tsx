
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { Box, Button } from '@mui/material'
import { useEffect } from 'react'
import { setSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'
import { getMainApi } from '../axios.manager'
import { APP_TITLE } from '../constants/general'

export const MainPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    useEffect(() => {
        document.title = `${APP_TITLE} - Dashboard`
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
            
            <Button onClick={async () => {
                const api = getMainApi()
                await api.apiMainLongRunningGet()
            }}>long running op</Button>
        </Box>
    )

}