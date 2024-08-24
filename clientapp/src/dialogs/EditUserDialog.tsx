import { Box, Button, Checkbox, Dialog, DialogActions, DialogContent, DialogTitle, Table, TableBody, TableCell, TableRow, TextField, Typography, useTheme } from "@mui/material"
import DoneOutlineIcon from '@mui/icons-material/DoneOutline';
import CloseIcon from '@mui/icons-material/Close';
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { DEFAULT_FONTWEIGHT_BOLD, DEFAULT_SIZE_SMALL, DEFAULT_SIZE_XSMALL } from "../constants/general"
import { useEffect, useState } from "react";
import { passwordIsValid } from "../utils/password-validator";
import { emailIsValid } from "../utils/email-validator";
import { usernameIsValid } from "../utils/username-validator";
import { setSnack } from "../redux/slices/globalSlice";
import { SnackNfoType } from "../types/SnackNfo";
import { handleApiException, nullOrUndefined } from "../utils/utils";
import { authApi } from "../fetch.manager";
import { AuthOptions, EditUserRequestDto, ResponseError } from "../../api";

export const NewUserDataSample = () => {
    let res: EditUserRequestDto = {
        existingUsername: null,
        editUsername: null,
        editEmail: null,
        editPassword: null,
        editLockoutEnd: null,
        editRoles: null,
        editDisabled: null,
    }
    return res
}

export const EditUserDialog = (props: {
    open: boolean,
    setOpen: React.Dispatch<React.SetStateAction<boolean>>,
    userData: EditUserRequestDto,
    setUserData: React.Dispatch<React.SetStateAction<EditUserRequestDto>>,
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
    const [authOptions, setAuthOptions] = useState<AuthOptions | undefined>(undefined)
    const [allRoles, setAllRoles] = useState<string[] | undefined>(undefined)

    useEffect(() => {
        if (global.currentUser) {
            authApi.apiAuthAuthOptionsGet().then(res => {
                setAuthOptions(res)
            })

            authApi.apiAuthListRolesGet().then(res => {
                setAllRoles(res)
            })
        }
    }, [global.currentUser])

    useEffect(() => {
        if (authOptions) {
            if (autoValidate) validate()
            else { // set errors on initial
                setUsernameValid(undefined)
                setEmailValid(undefined)
                setPasswordValid(undefined)
            }
        }
    }, [authOptions, autoValidate, userData])

    useEffect(() => { // set autovalidate on close
        if (open === false) setAutoValidate(false)
    }, [open])

    const validate = () => {
        if (authOptions === undefined) return false

        let res = true
        let q = false

        if (nullOrUndefined(userData.existingUsername)) {
            q = usernameIsValid(authOptions, userData.editUsername).isValid
            setUsernameValid(q)
            if (q === false) res = false

            if (nullOrUndefined(userData.editEmail)) {
                setEmailValid(false)
                res = false
            }

            if (nullOrUndefined(userData.editPassword)) {
                setPasswordValid(false)
                res = false
            }
        }

        if (userData.editEmail !== null && userData.editEmail !== undefined) {
            q = emailIsValid(userData.editEmail).isValid
            setEmailValid(q)
            if (q === false) res = false
        }

        if (userData.editPassword !== null && userData.editPassword !== undefined) {
            q = passwordIsValid(authOptions, userData.editPassword).isValid
            setPasswordValid(q)
            if (q === false) res = false
        }

        return res
    }

    const resetFormData = () => {
        setUserData(NewUserDataSample())
    }

    return authOptions && allRoles && (
        <Dialog
            open={open}
            onClose={(e, reason) => {
                if (reason && (reason === "backdropClick" || reason === 'escapeKeyDown'))
                    return; // modal dialog
                setOpen(false)
            }}>

            <DialogTitle>
                <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    {nullOrUndefined(userData.existingUsername) ? 'New user' : `Edit user ${userData.existingUsername}`}
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
                                {userData.existingUsername ?
                                    <Typography>
                                        {userData.existingUsername}
                                    </Typography>
                                    :
                                    <TextField
                                        error={usernameValid === false}
                                        value={userData.editUsername ?? ''}
                                        onChange={e => setUserData({ ...userData, editUsername: e.target.value })}
                                        helperText={usernameValid === false && usernameIsValid(authOptions, userData.editUsername).errors.map((errorMsg, errorMsgIdx) =>
                                            <Typography key={`username-err-${errorMsgIdx}`}>{errorMsg}</Typography>)}
                                    />}
                            </TableCell>
                        </TableRow>

                        <TableRow>
                            <TableCell valign="top">Disabled</TableCell>
                            <TableCell>
                                <Checkbox
                                    checked={userData?.editDisabled === true}
                                    onChange={e => setUserData({ ...userData, editDisabled: e.target.checked === true })}
                                />
                            </TableCell>
                        </TableRow>

                        <TableRow>
                            <TableCell valign="top">Email</TableCell>
                            <TableCell>
                                <TextField
                                    error={emailValid === false}
                                    value={userData.editEmail ?? ''}
                                    onChange={e => setUserData({ ...userData, editEmail: e.target.value })}
                                    helperText={emailValid === false && emailIsValid(userData.editEmail ?? '').errors.map((errorMsg, errorMsgIdx) =>
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
                                    value={userData.editPassword ?? ''}
                                    onChange={e => setUserData({ ...userData, editPassword: e.target.value })}
                                    placeholder="Type to change password"
                                    helperText={passwordValid === false && passwordIsValid(authOptions, userData.editPassword ?? '').errors.map((errorMsg, errorMsgIdx) =>
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

                        {allRoles.map((role, roleIdx) => <TableRow key={`role-${role}`}>
                            <TableCell valign="top">{role}</TableCell>
                            <TableCell>
                                <Checkbox
                                    checked={(userData?.editRoles ?? []).indexOf(role) !== -1}
                                    onChange={e => {
                                        const roleIdx = (userData.editRoles ?? []).indexOf(role)
                                        console.log(`roleIdx = ${roleIdx}`)

                                        if (e.target.checked === true) {
                                            if (roleIdx === -1) {
                                                if (nullOrUndefined(userData.editRoles))
                                                    userData.editRoles = []
                                                userData.editRoles!.push(role)
                                                setUserData({ ...userData })
                                            }
                                        }
                                        else {
                                            if (roleIdx !== -1) {
                                                userData.editRoles!.splice(roleIdx, 1)
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
                                        editUserRequestDto: userData
                                    })

                                    dispatch(setSnack({
                                        msg: [`User ${userData.existingUsername ?? userData.editUsername} ${nullOrUndefined(userData.existingUsername) ? 'created' : 'changes applied'}`],
                                        type: SnackNfoType.success
                                    }))

                                    await refreshList()
                                    setOpen(false)
                                }
                                catch (_ex) {
                                    handleApiException(_ex as ResponseError, 'Edit user error')
                                }
                            }
                        }}
                        variant='outlined'>
                        <DoneOutlineIcon sx={{ mr: DEFAULT_SIZE_XSMALL }} />
                        {userData.existingUsername === undefined ? 'Create' : 'Apply'}
                    </Button>
                </Box>
            </DialogActions>

        </Dialog>
    )

}