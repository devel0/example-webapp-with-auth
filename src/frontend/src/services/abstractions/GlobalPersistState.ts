// DATA

import { PaletteMode } from "@mui/material"
import { CurrentUserNfo } from "../../types/CurrentUserNfo"
import { THEME_INITIAL } from "../../constants/general"

interface GlobalPersistStateData {
  /** states if the storage is hydrated */
  hydrated: boolean

  currentUser: CurrentUserNfo | null
  /** states if current user got a first verify of the authorization */
  currentUserInitialized: boolean

  themePalette: PaletteMode
    
  refreshTokenExpiration: string | null
}

export const GlobalPersistStateInitial: GlobalPersistStateData = {
  hydrated: false,

  currentUser: null,
  currentUserInitialized: false,  

  themePalette: THEME_INITIAL,

  refreshTokenExpiration: null
}

// ACTIONS

interface GlobalPersistStateActions {
  setHydrated: () => void

  setCurrentUser: (currentUser: CurrentUserNfo | null) => void
  setRefreshTokenExpiration: (refreshTokenExpiration: string | null) => void
  setLogout: () => void  
  setThemePalette: (themePalette: PaletteMode) => void
  currentUserCanManageUsers: () => boolean | null

}

export interface GlobalPersistState extends GlobalPersistStateData, GlobalPersistStateActions { }
