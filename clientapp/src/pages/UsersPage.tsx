import { Box, Button } from "@mui/material"
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { useEffect, useState } from "react"
import { APP_TITLE, DEFAULT_SIZE_SMALL, ROLE } from "../constants/general"
import { EditUserRequestDto, ResponseError, UserListItemResponseDto } from "../../api"
import { DataGrid, GridColDef } from "@mui/x-data-grid"
import { EditUserDialog, NewUserDataSample } from "../dialogs/EditUserDialog"
import { HttpStatusCode } from "axios"
import { authApi } from "../fetch.manager"
import { ConfirmDialog, ConfirmDialogCloseResult, ConfirmDialogProps } from "../dialogs/ConfirmDialog"
import { setSnack } from "../redux/slices/globalSlice"
import { SnackNfoType } from "../types/SnackNfo"
import { handleApiException } from "../utils/utils"

export const UsersPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const [users, setUsers] = useState<UserListItemResponseDto[]>([])
    const [editUserDialogOpen, setEditUserDialogOpen] = useState(false)
    const [selectedUsername, setSelectedUsername] = useState<string | undefined>(undefined)
    const [userData, setUserData] = useState<EditUserRequestDto>(NewUserDataSample())
    const [confirmDialogProps, setConfirmDialogProps] = useState<ConfirmDialogProps>({
        open: false,
        setProps: (props) => setConfirmDialogProps(props)
    })

    useEffect(() => {
        document.title = `${APP_TITLE} - Users`
        refreshList()
    }, [])

    const refreshList = async () => {
        const res = await authApi.apiAuthListUsersGet();
        setUsers(res)
    }

    const fn = (x: keyof UserListItemResponseDto) => x

    const cols: GridColDef<UserListItemResponseDto>[] = [
        {
            field: fn('userName'),
            flex: 1
        },
        {
            field: fn('email'),
            flex: 1
        },
        {
            field: fn('roles'),
            flex: 1
        },
        {
            field: fn('disabled'),
            flex: 1,
            renderCell: params => {
                const row: UserListItemResponseDto = params.row

                return row.disabled === true ? '✓' : ''
            }
        },
    ]

    return (
        <Box m={DEFAULT_SIZE_SMALL}>
            <Box sx={{
                display: 'flex'
            }}>
                <Button onClick={() => {
                    setUserData(NewUserDataSample())
                    setEditUserDialogOpen(true)
                }}>
                    Create
                </Button>

                <Button
                    disabled={selectedUsername === undefined}
                    onClick={async () => {
                        const res = await authApi.apiAuthListUsersGet({
                            username: selectedUsername
                        })
                        if (res.length > 0) {
                            const user = res[0]
                            setUserData({
                                existingUsername: user.userName!,
                                editUsername: null,
                                editEmail: user.email,
                                editLockoutEnd: null,
                                editPassword: null,
                                editRoles: user.roles,
                                editDisabled: user.disabled
                            })
                            setEditUserDialogOpen(true)
                        }
                    }}>
                    Edit
                </Button>

                <Button
                    disabled={selectedUsername === undefined || selectedUsername === null}
                    onClick={() => {
                        setConfirmDialogProps({
                            ...confirmDialogProps,
                            title: `Delete user ${selectedUsername}`,
                            open: true,
                            onClose: async (reason) => {
                                if (reason === ConfirmDialogCloseResult.yes) {
                                    try {
                                        const res = await authApi.apiAuthDeleteUserPost({
                                            deleteUserRequestDto: {
                                                usernameToDelete: selectedUsername!
                                            }
                                        })

                                        dispatch(setSnack({
                                            msg: [ `Delete successfully` ],
                                            type: SnackNfoType.success
                                        }))

                                        refreshList()
                                    }
                                    catch (_ex) {
                                        handleApiException(_ex as ResponseError, "Delete failed")
                                    }
                                }
                            },
                            yesButton: true,
                            noButton: true,
                            modal: true
                        })
                    }}>
                    Delete
                </Button>
            </Box>

            <DataGrid
                getRowId={r => r.userName ?? ''}
                rows={users}
                columns={cols}
                onRowSelectionModelChange={(ids, detail) => {
                    if (ids.length > 0) {
                        setSelectedUsername(ids[0].toString())
                    }
                    else
                        setSelectedUsername(undefined)
                }}
            />

            {userData && <EditUserDialog
                open={editUserDialogOpen}
                setOpen={setEditUserDialogOpen}
                userData={userData}
                setUserData={setUserData}
                refreshList={refreshList}
            />}

            {confirmDialogProps && <ConfirmDialog {...confirmDialogProps} />}

        </Box>
    )
}