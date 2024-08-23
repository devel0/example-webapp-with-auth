import { Box, Button } from "@mui/material"
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { useEffect, useState } from "react"
import { APP_TITLE, DEFAULT_SIZE_SMALL, ROLE } from "../constants/general"
import { EditUserRequestDto, UserListItemResponseDto } from "../../api"
import { DataGrid, GridColDef } from "@mui/x-data-grid"
import { EditUserDialog, NewUserDataSample } from "../dialogs/EditUserDialog"
import { HttpStatusCode } from "axios"
import { authApi } from "../fetch.manager"

export const UsersPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const [users, setUsers] = useState<UserListItemResponseDto[]>([])
    const [editUserDialogOpen, setEditUserDialogOpen] = useState(false)    
    const [selectedUsername, setSelectedUsername] = useState("")
    const [userData, setUserData] = useState<EditUserRequestDto>(NewUserDataSample())

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
                            existingUsername: user.userName!,
                            editUsername: null,
                            editEmail: user.email,
                            editLockoutEnd: null,
                            editPassword: null,
                            editRoles: user.roles,
                        })
                        setEditUserDialogOpen(true)
                    }
                }}>
                    Edit
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

        </Box>
    )
}