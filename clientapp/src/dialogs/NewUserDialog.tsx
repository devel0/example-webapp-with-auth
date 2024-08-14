import { Box, Button, Dialog, Grid, Table, TableBody, TableCell, TableRow, TextField, Typography, useTheme } from "@mui/material"
import DoneOutlineIcon from '@mui/icons-material/DoneOutline';
import CloseIcon from '@mui/icons-material/Close';
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { DEFAULT_FONTWEIGHT_BOLD, DEFAULT_SIZE_SMALL, DEFAULT_SIZE_XSMALL, ROLE } from "../constants/general"
import { useEffect, useState } from "react";
import { passwordIsValid } from "../utils/password-validator";
import { emailIsValid } from "../utils/email-validator";
import { usernameIsValid } from "../utils/username-validator";

export interface NewUserData {
    username: string,
    email: string,
    password: string,
    roles: ROLE[]
}

export const NewUserDataSample = () => {
    let res: NewUserData = {
        username: '',
        email: '',
        password: '',
        roles: []
    }
    return res
}

export const NewUserDialog = (props: {
    open: boolean,
    setOpen: React.Dispatch<React.SetStateAction<boolean>>,
    userData: NewUserData,
    setUserData: React.Dispatch<React.SetStateAction<NewUserData>>,
}) => {
    const { open, setOpen, userData, setUserData } = props
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const theme = useTheme()
    const [autoValidate, setAutoValidate] = useState(false)
    const [usernameValid, setUsernameValid] = useState<boolean | undefined>(undefined)
    const [emailValid, setEmailValid] = useState<boolean | undefined>(undefined)
    const [passwordValid, setPasswordValid] = useState<boolean | undefined>(undefined)

    useEffect(() => {
        if (autoValidate) validate()
        else { // set errors on initial
            setUsernameValid(undefined)
            setEmailValid(undefined)
            setPasswordValid(undefined)
        }
    }, [autoValidate, userData])

    useEffect(() => { // set autovalidate on close
        if (open === false) setAutoValidate(false)
    }, [open])

    const validate = () => {
        let res = true

        let q = usernameIsValid(userData.username).isValid
        setUsernameValid(q)
        if (q === false) res = false

        q = emailIsValid(userData.email).isValid
        setEmailValid(q)
        if (q === false) res = false

        q = passwordIsValid(userData.password).isValid
        setPasswordValid(q)
        if (q === false) res = false

        return res
    }

    const resetFormData = () => {
        setUserData({
            username: '',
            email: '',
            password: '',
            roles: []
        })
    }

    const formIsEmpty = () =>
        userData.username.trim().length === 0 &&
        userData.email.trim().length === 0 &&
        userData.password.trim().length === 0 &&
        userData.roles.length === 0

    return (
        <Dialog
            open={open}
            onClose={(e, reason) => {
                if (!formIsEmpty() && reason && reason === "backdropClick")
                    return; // modal dialog
                setOpen(false)
            }}>
            <Box
                sx={{ background: theme.palette.mode === 'light' ? 'white' : undefined }}
                p={DEFAULT_SIZE_SMALL}>
                <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    New user
                </Typography>
                <hr />
                <Table>
                    <TableBody>
                        <TableRow>
                            <TableCell valign="top">Username</TableCell>
                            <TableCell>
                                <TextField
                                    error={usernameValid === false}
                                    value={userData.username}
                                    onChange={e => setUserData({ ...userData, username: e.target.value })}
                                    helperText={usernameValid === false && usernameIsValid(userData.username).errors.map((errorMsg, errorMsgIdx) =>
                                        <Typography key={`username-err-${errorMsgIdx}`}>{errorMsg}</Typography>)}
                                />
                            </TableCell>
                        </TableRow>

                        <TableRow>
                            <TableCell valign="top">Email</TableCell>
                            <TableCell>
                                <TextField
                                    error={emailValid === false}
                                    value={userData.email}
                                    onChange={e => setUserData({ ...userData, email: e.target.value })}
                                    helperText={emailValid === false && emailIsValid(userData.email).errors.map((errorMsg, errorMsgIdx) =>
                                        <Typography key={`email-err-${errorMsgIdx}`}>{errorMsg}</Typography>)}
                                />
                            </TableCell>
                        </TableRow>

                        <TableRow>
                            <TableCell valign="top">Password</TableCell>
                            <TableCell>
                                <TextField
                                    error={passwordValid === false}
                                    type="password"
                                    value={userData.password}
                                    onChange={e => setUserData({ ...userData, password: e.target.value })}
                                    helperText={passwordValid === false && passwordIsValid(userData.password).errors.map((errorMsg, errorMsgIdx) =>
                                        <Typography key={`pass-err-${errorMsgIdx}`}>{errorMsg}</Typography>)}
                                    inputProps={{ // avoid browser autocomplete
                                        autoComplete: 'new-password',
                                        form: {
                                            autocomplete: 'off',
                                        },
                                    }}
                                />
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>

                <hr />

                <Box sx={{ display: 'flex', mt: DEFAULT_SIZE_SMALL }}>
                    <Box sx={{ flexGrow: 1 }} />
                    <Button
                        variant='outlined'
                        onClick={() => {
                            resetFormData()
                            setOpen(false)
                        }}
                        sx={{ mr: DEFAULT_SIZE_SMALL }}>
                        <CloseIcon sx={{ mr: DEFAULT_SIZE_XSMALL }} />
                        Cancel
                    </Button>
                    <Button
                        onClick={() => {
                            setAutoValidate(true)
                            if (validate())
                                setOpen(false)
                        }}
                        variant='outlined'>
                        <DoneOutlineIcon sx={{ mr: DEFAULT_SIZE_XSMALL }} />
                        Create
                    </Button>
                </Box>
            </Box>
        </Dialog>
    )

}