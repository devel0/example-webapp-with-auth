
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { Box, Button } from '@mui/material'
import { useEffect } from 'react'
import { setSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'
import { APP_TITLE, DEFAULT_SIZE_SMALL } from '../constants/general'
import { mainApi } from '../fetch.manager'
import { handleApiException } from '../utils/utils'
import { ResponseError } from '../../api'

export const MainPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    useEffect(() => {
        document.title = `${APP_TITLE} - Dashboard`
        // dispatch(setSnack({
        //     msg: [ 'main page snack test' ],
        //     type: SnackNfoType.info
        // }))
    }, [])

    return (
        <Box m={DEFAULT_SIZE_SMALL}>
            master page<br />
            current user: {global.currentUser?.userName}<br />
            roles: {global.currentUser?.roles}<br />

            <Button onClick={async () => {
                try {
                    await mainApi.apiMainLongRunningGet()
                } catch (_ex) {
                    handleApiException(_ex as ResponseError)
                }
            }}>long running op</Button>
        </Box>
    )

}