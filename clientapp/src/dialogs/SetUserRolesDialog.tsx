import { Box, Button, Checkbox, Dialog, Grid, Table, TableBody, TableCell, TableRow, TextField, Typography, useTheme } from "@mui/material"
import DoneOutlineIcon from '@mui/icons-material/DoneOutline';
import CloseIcon from '@mui/icons-material/Close';
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { ALL_ROLES, DEFAULT_FONTWEIGHT_BOLD, DEFAULT_SIZE_SMALL, DEFAULT_SIZE_XSMALL, ROLE, ROLE_user } from "../constants/general"
import { useEffect, useState } from "react";
import { HttpStatusCode } from "axios";
import { UserListItemResponseDto } from "../../api";
import { setSnack } from "../redux/slices/globalSlice";
import { SnackNfoType } from "../types/SnackNfo";
import { nullOrUndefined } from "../utils/utils";
import { authApi } from "../fetch.manager";

export const SetUserRolesDialog = (props: {
    open: boolean,
    setOpen: React.Dispatch<React.SetStateAction<boolean>>,
    username: string,
    refreshList: () => void
}) => {
    const { open, setOpen, username, refreshList } = props
    const [user, setUser] = useState<UserListItemResponseDto | undefined>(undefined)
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const theme = useTheme()

    useEffect(() => {
        if (open && username) {
            authApi.apiAuthListUsersGet({
                username: username
            }).then(res => {
                if (res.length > 0) {
                    setUser(res[0])
                }
            })
        }

    }, [open, username])

    return (
        <Dialog
            open={open}
            onClose={(e, reason) => {
                if (reason && (reason === "backdropClick" || reason === 'escapeKeyDown'))
                    return; // modal dialog
                setOpen(false)
            }}>
            {user && <Box
                sx={{ background: theme.palette.mode === 'light' ? 'white' : undefined }}
                p={DEFAULT_SIZE_SMALL}>
                <Typography fontWeight={DEFAULT_FONTWEIGHT_BOLD}>
                    User roles
                </Typography>
                <hr />
                <Table>
                    <TableBody>
                        <TableRow>
                            <TableCell valign="top">Username</TableCell>
                            <TableCell>{username}</TableCell>
                        </TableRow>

                        {ALL_ROLES.map((role, roleIdx) => <TableRow key={`role-${role}`}>
                            <TableCell valign="top">{role}</TableCell>
                            <TableCell>
                                <Checkbox
                                    checked={user?.roles?.indexOf(role) !== -1}
                                    onChange={e => {
                                        const roleIdx = (user.roles ?? []).indexOf(role)
                                        console.log(`roleIdx = ${roleIdx}`)

                                        if (e.target.checked === true) {
                                            if (roleIdx === -1) {
                                                if (nullOrUndefined(user.roles))
                                                    user.roles = []
                                                user.roles!.push(role)
                                                setUser({ ...user })
                                            }
                                        }
                                        else {
                                            if (roleIdx !== -1) {
                                                user.roles!.splice(roleIdx, 1)
                                                setUser({ ...user })
                                            }
                                        }
                                    }}
                                />
                            </TableCell>
                        </TableRow>)}
                    </TableBody>
                </Table>

                <hr />

                <Box sx={{ display: 'flex', mt: DEFAULT_SIZE_SMALL }}>
                    <Box sx={{ flexGrow: 1 }} />
                    <Button
                        variant='outlined'
                        onClick={() => {
                            setOpen(false)
                        }}
                        sx={{ mr: DEFAULT_SIZE_SMALL }}>
                        <CloseIcon sx={{ mr: DEFAULT_SIZE_XSMALL }} />
                        Cancel
                    </Button>
                    <Button
                        onClick={async () => {
                            const res = await authApi.apiAuthSetUserRolesPost({
                                setUserRolesRequestDto: {
                                    userName: user.userName,
                                    roles: user.roles
                                }
                            })
                            if (res.status === "OK") {
                                dispatch(setSnack({
                                    msg: 'User roles changed',
                                    type: SnackNfoType.success
                                }))
                                await refreshList()
                                setOpen(false)
                            }
                        }}
                        variant='outlined'>
                        <DoneOutlineIcon sx={{ mr: DEFAULT_SIZE_XSMALL }} />
                        Apply
                    </Button>
                </Box>
            </Box>}
        </Dialog>
    )

}