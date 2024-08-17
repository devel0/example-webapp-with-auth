import { Box, Button, Card, Checkbox, Dialog, DialogActions, DialogContent, DialogTitle, Grid, Modal, Table, TableBody, TableCell, TableRow, TextField, Typography, useTheme } from "@mui/material"
import DoneOutlineIcon from '@mui/icons-material/DoneOutline';
import CloseIcon from '@mui/icons-material/Close';
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { ALL_ROLES, API_URL, DEFAULT_FONTWEIGHT_BOLD, DEFAULT_SIZE_SMALL, DEFAULT_SIZE_XSMALL, ROLE } from "../constants/general"
import { useEffect, useState } from "react";
import { passwordIsValid } from "../utils/password-validator";
import { emailIsValid } from "../utils/email-validator";
import { usernameIsValid } from "../utils/username-validator";
import { HttpStatusCode } from "axios";
import { setSnack } from "../redux/slices/globalSlice";
import { SnackNfoType } from "../types/SnackNfo";
import { nullOrUndefined } from "../utils/utils";
import { DraggablePaperComponent } from "../utils/draggable-dialog";
import { createApi } from "@reduxjs/toolkit/query";
import { ApiException } from "../types/ApiException";
import { authApi } from "../fetch.manager";
import { ResponseError } from "../../api";

export interface EditUserData {
    isNew: boolean,
    username: string,
    email: string,
    password: string,
    roles: ROLE[]
}

// export const PASSWORD_UNCHANGED = "********"

export const NewUserDataSample = () => {
    let res: EditUserData = {
        isNew: true,
        username: '',
        email: '',
        password: '',
        roles: []
    }
    return res
}

export const EditUserDialog = (props: {
    open: boolean,
    setOpen: React.Dispatch<React.SetStateAction<boolean>>,
    userData: EditUserData,
    setUserData: React.Dispatch<React.SetStateAction<EditUserData>>,
    refreshList: () => void
}) => {
    const { open, setOpen, userData, setUserData, refreshList } = props
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

        if (userData.isNew === true || userData.password.trim().length > 0) {
            q = passwordIsValid(userData.password).isValid
            setPasswordValid(q)
            if (q === false) res = false
        }

        return res
    }

    const resetFormData = () => {
        setUserData(NewUserDataSample())
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
                if (!formIsEmpty() && reason && (reason === "backdropClick" || reason === 'escapeKeyDown'))
                    return; // modal dialog
                setOpen(false)
            }}>

            <DialogTitle>
                <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    {userData.isNew === true ? 'New user' : `Edit user ${userData.username}`}
                </Typography>
                <hr />
            </DialogTitle>

            <DialogContent sx={{
                background: theme.palette.mode === 'light' ? 'white' : undefined,
            }}>

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
                                    placeholder="Type to change password"
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

                        {ALL_ROLES.map((role, roleIdx) => <TableRow key={`role-${role}`}>
                            <TableCell valign="top">{role}</TableCell>
                            <TableCell>
                                <Checkbox
                                    checked={userData?.roles?.indexOf(role as ROLE) !== -1}
                                    onChange={e => {
                                        const roleIdx = (userData.roles ?? []).indexOf(role as ROLE)
                                        console.log(`roleIdx = ${roleIdx}`)

                                        if (e.target.checked === true) {
                                            if (roleIdx === -1) {
                                                if (nullOrUndefined(userData.roles))
                                                    userData.roles = []
                                                userData.roles!.push(role as ROLE)
                                                setUserData({ ...userData })
                                            }
                                        }
                                        else {
                                            if (roleIdx !== -1) {
                                                userData.roles!.splice(roleIdx, 1)
                                                setUserData({ ...userData })
                                            }
                                        }
                                    }}
                                />
                            </TableCell>
                        </TableRow>)}
                    </TableBody>
                </Table>
            </DialogContent>

            <DialogActions>
                <Box sx={{
                    display: 'flex',
                    mt: DEFAULT_SIZE_SMALL
                }}>
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
                        onClick={async () => {
                            setAutoValidate(true)
                            if (validate()) {

                                try {
                                    await authApi.apiAuthEditUserPost({
                                        editUserRequestDto: {
                                            createNew: userData.isNew,
                                            userName: userData.username,
                                            email: userData.email,
                                            changePassword: userData.password,
                                            roles: userData.roles
                                        }
                                    })

                                    dispatch(setSnack({
                                        msg: `User ${userData.username} ${userData.isNew ? 'created' : 'changes applied'}`,
                                        type: SnackNfoType.success
                                    }))

                                    await refreshList()
                                    setOpen(false)
                                }
                                catch (_ex) {
                                    const ex = _ex as ResponseError
                                    if (ex.response.status === HttpStatusCode.NotFound) {
                                        dispatch(setSnack({
                                            msg: 'NOT FOUND!!!',
                                            type: SnackNfoType.error
                                        }))
                                    }
                                    console.log(ex);
                                }
                            }
                        }
                        }
                        variant='outlined'>
                        <DoneOutlineIcon sx={{ mr: DEFAULT_SIZE_XSMALL }} />
                        {userData.isNew === true ? 'Create' : 'Apply'}
                    </Button>
                </Box>
            </DialogActions>

        </Dialog >
    )

}