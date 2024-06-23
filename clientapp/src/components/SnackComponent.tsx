import { Alert, Box, Snackbar } from '@mui/material'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { unsetSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'

export const SnackComponent = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()

    // function SlideTransition(props: SlideProps) {
    //     return <Slide {...props} direction="up" />
    // }

    // function FadeTransition(props: SlideProps) {
    //     return <Fade {...props} />
    // }

    return <Box>
        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackSuccessOpen} autoHideDuration={global.snackSuccessDuration}
            key={`succ${global.snackSuccessMsg}`}
            message={global.snackSuccessMsg}
            onClose={() => dispatch(unsetSnack(SnackNfoType.success))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.success))}
                severity="success"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackSuccessMsg}
            </Alert>
        </Snackbar>

        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackErrorOpen} autoHideDuration={global.snackErrorDuration}
            key={`err${global.snackErrorMsg}`}
            onClose={() => dispatch(unsetSnack(SnackNfoType.error))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.error))}
                severity="error"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackErrorMsg}
            </Alert>
        </Snackbar>

        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackWarningOpen} autoHideDuration={global.snackWarningDuration}
            key={`warn${global.snackWarningMsg}`}
            onClose={() => dispatch(unsetSnack(SnackNfoType.warning))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.warning))}
                severity="warning"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackWarningMsg}
            </Alert>
        </Snackbar>

        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackInfoOpen} autoHideDuration={global.snackInfoDuration}
            key={`nfo${global.snackInfoMsg}`}
            onClose={() => dispatch(unsetSnack(SnackNfoType.info))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.info))}
                severity="info"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackInfoMsg}
            </Alert>
        </Snackbar>
    </Box>

}