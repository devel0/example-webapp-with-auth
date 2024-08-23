import { Alert, Box, Snackbar, Typography } from '@mui/material'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { GlobalState } from '../redux/states/GlobalState'
import { unsetSnack } from '../redux/slices/globalSlice'
import { SnackNfoType } from '../types/SnackNfo'
import { DEFAULT_FONTWEIGHT_BOLD } from '../constants/general'

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
            open={global.snackSuccess.open} autoHideDuration={global.snackSuccess.duration}
            key={`snack-succ`}
            // message={global.snackSuccess.msg}
            onClose={() => dispatch(unsetSnack(SnackNfoType.success))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.success))}
                severity="success"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackSuccess.title && <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    {global.snackSuccess.title}
                </Typography>}
                {global.snackSuccess.msg.map((msg, msgIdx) =>
                    <Typography key={`snack-err-${msgIdx}`}>{msg}</Typography>
                )}                
            </Alert>
        </Snackbar>

        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackError.open} autoHideDuration={global.snackError.duration}
            key={`snack-err`}
            // message={global.snackError.msg}
            onClose={() => dispatch(unsetSnack(SnackNfoType.error))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.error))}
                severity="error"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackError.title && <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    {global.snackError.title}
                </Typography>}
                {global.snackError.msg.map((msg, msgIdx) =>
                    <Typography key={`snack-err-${msgIdx}`}>{msg}</Typography>
                )}
            </Alert>
        </Snackbar>

        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackWarning.open} autoHideDuration={global.snackWarning.duration}
            key={`snack-wrn`}
            // message={global.snackWarning.msg}
            onClose={() => dispatch(unsetSnack(SnackNfoType.warning))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.warning))}
                severity="warning"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackWarning.title && <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    {global.snackWarning.title}
                </Typography>}
                {global.snackWarning.msg.map((msg, msgIdx) =>
                    <Typography key={`snack-err-${msgIdx}`}>{msg}</Typography>
                )}
            </Alert>
        </Snackbar>

        <Snackbar
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            // TransitionComponent={SlideTransition}
            open={global.snackInfo.open} autoHideDuration={global.snackInfo.duration}
            key={`snack-nfo`}
            // message={global.snackInfo.msg}
            onClose={() => dispatch(unsetSnack(SnackNfoType.info))}>
            <Alert
                onClose={() => dispatch(unsetSnack(SnackNfoType.info))}
                severity="info"
                sx={{ width: '100%', mt: 4 }}>
                {global.snackInfo.title && <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    {global.snackInfo.title}
                </Typography>}
                {global.snackInfo.msg.map((msg, msgIdx) =>
                    <Typography key={`snack-err-${msgIdx}`}>{msg}</Typography>
                )}                
            </Alert>
        </Snackbar>
    </Box>

}