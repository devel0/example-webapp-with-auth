import { APP_TITLE, DEFAULT_SIZE_1_REM } from "../constants/general"
import { authApi } from "../axios.manager"
import { AxiosError } from "axios"
import { Box, Button } from "@mui/material"
import { ConfirmDialog, ConfirmDialogCloseResult, ConfirmDialogProps } from "../dialogs/ConfirmDialog"
import { DataGrid, GridColDef } from "@mui/x-data-grid"
import { EditUserDialog, NewUserDataSample } from "../dialogs/EditUserDialog"
import { EditUserRequestDto, UserListItemResponseDto } from "../../api"
import { GlobalState } from "../redux/states/GlobalState"
import { handleApiException, setSnack } from "../utils/utils"
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { useEffect, useState } from "react"

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
        try {
            const res = await authApi.apiAuthListUsersGet();
            setUsers(res.data)
        } catch (_ex) {
            const ex = _ex as AxiosError
            handleApiException(ex)
        }
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
        <Box m={DEFAULT_SIZE_1_REM}>
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
                        const res = await authApi.apiAuthListUsersGet(selectedUsername)

                        if (res.data.length > 0) {
                            const user = res.data[0]
                            setUserData({
                                existingUsername: user.userName!,                                
                                editEmail: user.email,                                
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
                                        await authApi.apiAuthDeleteUserPost({
                                            usernameToDelete: selectedUsername!
                                        })

                                        setSnack({
                                            msg: [`Delete successfully`],
                                            type: "success"
                                        })

                                        refreshList()
                                    }
                                    catch (_ex) {
                                        const ex = _ex as AxiosError
                                        handleApiException(ex, "Delete failed")
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
                initialState={{
                    sorting: {
                        sortModel: [
                            {
                                field: fn('userName'),
                                sort: 'asc'
                            }
                        ]
                    }
                }}
                onRowSelectionModelChange={(ids) => {
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