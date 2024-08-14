import { Box, Button } from "@mui/material"
import { useAppDispatch, useAppSelector } from "../redux/hooks/hooks"
import { GlobalState } from "../redux/states/GlobalState"
import { useEffect, useState } from "react"
import { APP_TITLE, DEFAULT_SIZE_SMALL } from "../constants/general"
import { getAuthApi } from "../axios.manager"
import { UserListItemResponseDto } from "../../api"
import { DataGrid, GridColDef } from "@mui/x-data-grid"
import { NewUserData, NewUserDataSample, NewUserDialog } from "../dialogs/NewUserDialog"

export const UsersPage = () => {
    const global = useAppSelector<GlobalState>((state) => state.global)
    const dispatch = useAppDispatch()
    const [users, setUsers] = useState<UserListItemResponseDto[]>([])
    const [newUserDialogOpen, setNewUserDialogOpen] = useState(false)
    const [newUserData, setNewUserData] = useState<NewUserData>(NewUserDataSample())

    const fn = (x: keyof UserListItemResponseDto) => x
    useEffect(() => {
        document.title = `${APP_TITLE} - Users`

        const api = getAuthApi()
        api.apiAuthListUsersGet().then(res => {
            setUsers(res.data ?? [])
        })
    }, [])

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
                <Button onClick={() => setNewUserDialogOpen(true)}>
                    Create
                </Button>
            </Box>

            <DataGrid
                getRowId={r => r.userName ?? ''}
                rows={users}
                columns={cols}
            />

            <NewUserDialog
                open={newUserDialogOpen}
                setOpen={setNewUserDialogOpen}
                userData={newUserData}
                setUserData={setNewUserData} />
        </Box>
    )
}