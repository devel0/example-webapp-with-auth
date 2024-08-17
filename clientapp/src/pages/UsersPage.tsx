import { Box, Button } from "@mui/material"
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { useEffect, useState } from "react"
import { APP_TITLE, DEFAULT_SIZE_SMALL, ROLE } from "../constants/general"
import { UserListItemResponseDto } from "../../api"
import { DataGrid, GridColDef } from "@mui/x-data-grid"
import { EditUserData, EditUserDialog, NewUserDataSample } from "../dialogs/EditUserDialog"
import { SetUserRolesDialog } from "../dialogs/SetUserRolesDialog"
import { HttpStatusCode } from "axios"
import { authApi } from "../fetch.manager"

export const UsersPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const [users, setUsers] = useState<UserListItemResponseDto[]>([])
    const [editUserDialogOpen, setEditUserDialogOpen] = useState(false)
    const [userRolesDialogOpen, setUserRolesDialogOpen] = useState(false)
    const [selectedUsername, setSelectedUsername] = useState("")
    const [userData, setUserData] = useState<EditUserData>(NewUserDataSample())

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
            field: fn('lockoutEnd'),
            flex: 1
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

                <Button onClick={async () => {
                    const res = await authApi.apiAuthListUsersGet({
                        username: selectedUsername
                    })
                    if (res.length > 0) {
                        const user = res[0]
                        setUserData({
                            isNew: false,
                            username: user.userName!,
                            email: user.email!,
                            password: '', //PASSWORD_UNCHANGED,
                            roles: (user.roles ?? []) as ROLE[]
                        })
                        setEditUserDialogOpen(true)
                    }
                }}>
                    Edit
                </Button>

                <Button onClick={() => setUserRolesDialogOpen(true)}>
                    Roles
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
                }}
            />

            {userData && <EditUserDialog
                open={editUserDialogOpen}
                setOpen={setEditUserDialogOpen}
                userData={userData}
                setUserData={setUserData}
                refreshList={refreshList}
            />}

            <SetUserRolesDialog
                open={userRolesDialogOpen}
                setOpen={setUserRolesDialogOpen}
                username={selectedUsername}
                refreshList={refreshList}
            />

        </Box>
    )
}