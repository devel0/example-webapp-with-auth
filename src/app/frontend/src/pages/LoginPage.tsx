import {
    Box, Button, colors, Container,
    CssBaseline,
    LinearProgress,
    TextField,
    Typography
} from '@mui/material'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { useNavigate } from 'react-router'
import { GlobalState } from '../redux/states/GlobalState'
import React, { useEffect, useState } from 'react'
import { setSuccessfulLogin, setUrlWanted } from '../redux/slices/globalSlice'
import { APP_TITLE, APP_URL_Home, APP_URL_Login, DEFAULT_COLOR_TIPS, DEFAULT_FONTSIZE_MEDIUM, DEFAULT_FONTSIZE_NORMAL, DEFAULT_FONTWEIGHT_BOLD, DEFAULT_SIZE_SMALL, DEFAULT_SIZE_XSMALL, LOCAL_STORAGE_CURRENT_USER_NFO } from '../constants/general'
import { CurrentUserNfo } from '../types/CurrentUserNfo'
import { SnackNfoType } from '../types/SnackNfo'
import AppLogo from '../images/app-icon.svg?react'
import { authApi } from '../axios.manager'
import { useParams } from 'react-router-dom'
import { delay, handleApiException, nullOrUndefined, setSnack } from '../utils/utils'
import { AutoFixOff } from '@mui/icons-material'
import { AxiosError } from 'axios'

export const LoginPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const navigate = useNavigate()
    const params = useParams()
    const [usernameOrEmailField, setUsernameOrEmailField] = useState("")
    const [passwordField, setPasswordField] = useState("")
    const [resetPasswordToken, setResetPasswordToken] = useState<string | undefined>(undefined)
    const [sendingPasswordResetProgress, setSendingPasswordResetProgress] = useState(false)

    useEffect(() => {
        if (params.from && params.from !== ':from') {
            dispatch(setUrlWanted(params.from))
        }
        if (params.token && params.token !== ":token") {
            setResetPasswordToken(params.token)
        }
    }, [params])

    useEffect(() => {
        if (global.currentUserInitialized && global.currentUser) {
            if (global.urlWanted && global.urlWanted !== APP_URL_Login) {
                const urlWanted = global.urlWanted

                dispatch(setUrlWanted(undefined))
                navigate(urlWanted)
            }
            else {
                navigate(APP_URL_Home)
            }
        }
    }, [global.currentUser, global.currentUserInitialized])

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault()
        const data = new FormData(event.currentTarget)

        const usernameOrEmail = String(data.get("username"));

        let loginAfterResetPassword = false

        if (resetPasswordToken) {
            try {
                const res = await authApi.apiAuthResetLostPasswordGet(
                    usernameOrEmailField,
                    resetPasswordToken,
                    passwordField
                )

                loginAfterResetPassword = true

            } catch (_ex) {
                const ex = _ex as AxiosError
                handleApiException(ex)
            }
        }

        if (nullOrUndefined(resetPasswordToken) || loginAfterResetPassword) {

            try {
                const response = await authApi.apiAuthLoginPost({
                    usernameOrEmail: usernameOrEmail,
                    password: String(data.get("password")),
                });

                if (response?.data.status === 'OK') {
                    const currentUser: CurrentUserNfo = {
                        userName: response.data.userName!,
                        email: response.data.email!,
                        roles: response.data.roles ?? [],
                        permissions: Array.from(response.data.permissions ?? [])
                    }

                    dispatch(setSuccessfulLogin(currentUser));

                    localStorage.setItem(
                        LOCAL_STORAGE_CURRENT_USER_NFO,
                        JSON.stringify(currentUser)
                    );
                }
                else
                    setSnack({
                        msg: ['login error'],
                        type: "warning"
                    })
            } catch (_ex) {
                const ex = _ex as AxiosError
                handleApiException(ex)
            }

        }
    }

    return (
        <Box sx={{ width: '100%' }}>
            <CssBaseline />

            <Box sx={{ alignSelf: 'center' }}>
                <Container component="main" maxWidth="xs" >
                    <Box sx={{
                        mt: DEFAULT_SIZE_SMALL,
                        display: 'flex',
                        justifyContent: 'center'
                    }}>
                        <AppLogo width={'50%'} height={'100%'} />
                    </Box>

                    <Box>
                        <Typography variant='h6' mt={DEFAULT_SIZE_SMALL} textAlign={'center'}>{APP_TITLE}</Typography>
                    </Box>

                    {resetPasswordToken && <Typography sx={{ color: DEFAULT_COLOR_TIPS }}>
                        To proceed in password reset enter your email and a new password
                    </Typography>}

                    <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            id="username"
                            name="username"
                            autoComplete="username"
                            label="Username or email"
                            onChange={e => setUsernameOrEmailField(e.target.value)}
                            autoFocus
                        />

                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            id="password"
                            name="password"
                            autoComplete="current-password"
                            label="Password"
                            onChange={e => setPasswordField(e.target.value)}
                            type="password"
                        />

                        <Button
                            type="submit"
                            fullWidth
                            variant="contained"
                            sx={{ mt: 3, mb: 2 }}
                        >
                            {resetPasswordToken ? "Reset password" : "Sign In"}
                        </Button>
                    </Box>

                    {nullOrUndefined(resetPasswordToken) && <Box>
                        <Box sx={{
                            display: 'flex'
                        }}>
                            <Box sx={{ flexGrow: 1 }} />
                            <Button onClick={async (e) => {
                                setSendingPasswordResetProgress(true)

                                try {
                                    const res = await authApi.apiAuthResetLostPasswordGet(usernameOrEmailField)

                                    setSnack({
                                        title: "Email confirmation sent",
                                        msg: ["An email with a password reset link was sent to your email."],
                                        type: "success",
                                        durationMs: 15000
                                    })
                                } catch (_ex) {
                                    const ex = _ex as AxiosError
                                    handleApiException(ex)
                                }

                                setSendingPasswordResetProgress(false)
                            }}>
                                Lost password ?
                            </Button>
                        </Box>
                        {sendingPasswordResetProgress && <LinearProgress />}
                    </Box>}

                </Container>
            </Box>
        </Box>
    )

}