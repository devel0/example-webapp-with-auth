import { APP_TITLE, DEFAULT_SIZE_1_REM } from '../constants/general'
import { authApi, mainApi } from '../axios.manager'
import { AxiosError } from 'axios'
import { Box, Button } from '@mui/material'
import { from } from 'linq-to-typescript'
import { handleApiException, setSnack } from '../utils/utils'
import { useEffect } from 'react'
import { useGlobalPersistService } from '../services/global-persist/Service'

export const MainPage = () => {
    const currentUser = useGlobalPersistService(x => x.currentUser)

    useEffect(() => {
        document.title = `${APP_TITLE} - Dashboard`

        // setSnack({
        //     msg: [ 'main page snack test' ],
        //     type: "info"
        // })
    }, [])

    // return <Box>dd</Box>

    return (
        <Box m={DEFAULT_SIZE_1_REM}>
            master page<br />
            current user: {currentUser?.userName}<br />
            roles: {from(currentUser?.roles ?? [])}<br />

            <Button onClick={async () => {
                try {
                    // await authApi.apiAuthRenewAccessTokenGet();

                    await mainApi.apiMainLongRunningGet()

                    setSnack({
                        msg: ['completed'],
                        type: "success"
                    })
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