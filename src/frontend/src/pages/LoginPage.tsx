import {
    APP_TITLE, APP_URL_Home, APP_URL_Login, DEFAULT_COLOR_TIPS,
    DEFAULT_SIZE_1_REM, LOCAL_STORAGE_CURRENT_USER_NFO,
    LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE
} from '../constants/general'
import { Box, Button, Container, CssBaseline, LinearProgress, TextField, Typography } from '@mui/material'
import { authApi } from '../axios.manager'
import { AxiosError } from 'axios'
import { CurrentUserNfo } from '../types/CurrentUserNfo'
import { GlobalState } from '../redux/states/GlobalState'
import { handleApiException, nullOrUndefined, setSnack } from '../utils/utils'
import { setSuccessfulLogin, setUrlWanted } from '../redux/slices/globalSlice'
import { useAppDispatch, useAppSelector } from '../redux/hooks/hooks'
import { useNavigate } from 'react-router'
import { useParams } from 'react-router-dom'
import AppLogo from '../images/app-icon.svg?react'
import React, { useEffect, useState } from 'react'

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
            if (global.urlWanted && global.urlWanted !== APP_URL_Login()) {
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
                handleApiException(ex, 'Reset password error')
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

                    if (response.data.refreshTokenExpiration)
                        localStorage.setItem(
                            LOCAL_STORAGE_REFRESH_TOKEN_EXPIRE,
                            response.data.refreshTokenExpiration
                        );
                }
                else
                    setSnack({
                        msg: ['login error'],
                        type: "warning"
                    })
            } catch (_ex) {
                const ex = _ex as AxiosError
                handleApiException(ex, 'Login error')
            }

        }
    }

    return (
        <Box sx={{ width: '100%' }}>
            <CssBaseline />

            <Box sx={{ alignSelf: 'center' }}>
                <Container component="main" maxWidth="xs" >
                    <Box sx={{
                        mt: DEFAULT_SIZE_1_REM,
                        display: 'flex',
                        justifyContent: 'center'
                    }}>
                        <AppLogo width={'50%'} height={'100%'} />
                    </Box>

                    <Box>
                        <Typography variant='h6' mt={DEFAULT_SIZE_1_REM} textAlign={'center'}>{APP_TITLE}</Typography>
                    </Box>

                    {resetPasswordToken && <Typography sx={{ color: DEFAULT_COLOR_TIPS }}>
                        To proceed in password reset enter your email and a new password
                    </Typography>}

                    <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
                        <TextField
                            margin="normal"
                            required
                            fullWidth
                            autoCapitalize='none'
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
                            autoCapitalize='none'
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
                                    handleApiException(ex, 'Reset password error')
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