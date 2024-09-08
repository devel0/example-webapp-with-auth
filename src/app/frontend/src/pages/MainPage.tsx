
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { Box, Button } from '@mui/material'
import { useEffect } from 'react'
import { APP_TITLE, DEFAULT_SIZE_1_REM } from '../constants/general'
import { mainApi } from '../axios.manager'
import { handleApiException, setSnack } from '../utils/utils'
import { AxiosError } from 'axios'

export const MainPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    useEffect(() => {
        document.title = `${APP_TITLE} - Dashboard`

        // setSnack({
        //     msg: [ 'main page snack test' ],
        //     type: "info"
        // })
    }, [])

    return (
        <Box m={DEFAULT_SIZE_1_REM}>
            master page<br />
            current user: {global.currentUser?.userName}<br />
            roles: {global.currentUser?.roles}<br />

            <Button onClick={async () => {
                try {
                    await mainApi.apiMainLongRunningGet()
                } catch (_ex) {
                    const ex = _ex as AxiosError
                    handleApiException(ex)
                }
            }}>long running op</Button>

            <Button onClick={async () => {
                try {
                    await mainApi.apiMainTestExceptionGet()
                } catch (_ex) {
                    const ex = _ex as AxiosError
                    handleApiException(ex)
                }
            }}>test exception</Button>
        </Box>
    )

}