import { PaletteMode } from "@mui/material"
import { CurrentUserNfo } from "../../types/CurrentUserNfo"

export interface IGlobalPersistActions {
    setHydrated: () => void

    setCurrentUser: (currentUser: CurrentUserNfo | null) => void
    setRefreshTokenExpiration: (refreshTokenExpiration: string | null) => void
    setLogout: () => void
    setThemePalette: (themePalette: PaletteMode) => void
    currentUserCanManageUsers: () => boolean | null
}