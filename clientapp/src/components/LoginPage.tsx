import {
    Box, Button, Container,
    CssBaseline,
    TextField
} from '@mui/material'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { useNavigate } from 'react-router'
import { GlobalState } from '../redux/states/GlobalState'
import { useEffect } from 'react'
import { setSnack, setSuccessfulLogin, setUrlWanted } from '../redux/slices/globalSlice'
import { APP_URL_Home, APP_URL_Login, LOCAL_STORAGE_CURRENT_USER_NFO } from '../constants/general'
import { CurrentUserNfo } from '../types/CurrentUserNfo'
import { SnackNfoType } from '../types/SnackNfo'
import { SnackComponent } from './SnackComponent'
import { getAuthApi } from '../axios.manager'

export const LoginPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()

    useEffect(() => {
        if (global.currentUser) {
            if (global.urlWanted && global.urlWanted !== APP_URL_Login) {
                const urlWanted = global.urlWanted

                dispatch(setUrlWanted(undefined))
                navigate(urlWanted)
            }
            else {
                navigate(APP_URL_Home)
            }
        }
    }, [global.currentUser])

    const handleSubmit = async (event: any) => {
        event.preventDefault()
        const data = new FormData(event.currentTarget)

        const authApi = getAuthApi()

        const response = await authApi.apiAuthLoginPost({
            email: String(data.get("email")),
            password: String(data.get("password")),
        });

        if (response?.data?.status === 'OK') {
            const currentUser: CurrentUserNfo = {
                userName: response.data.userName!,
                email: response.data.email!,
                roles: response.data.roles!
            }

            dispatch(setSuccessfulLogin(currentUser));

            localStorage.setItem(
                LOCAL_STORAGE_CURRENT_USER_NFO,
                JSON.stringify(currentUser)
            );
        }
        else
            dispatch(setSnack({
                msg: 'login error',
                type: SnackNfoType.warning                
            }))
    }

    return (
        <Box sx={{ width: '100%' }}>
            <CssBaseline/>

            <Box sx={{ alignSelf: 'center' }}>
                <Container component="main" maxWidth="xs" >

                    <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            id="email"
                            label="Email Address"
                            name="email"
                            autoComplete="email"
                            autoFocus
                        />
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            name="password"
                            label="Password"
                            type="password"
                            id="password"
                            autoComplete="current-password"
                        />

                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                        >
                            Sign In
                        </Button>
                    </Box >
                </Container>
            </Box>

            <SnackComponent />
        </Box>
    )

}