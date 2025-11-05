import { PaletteMode } from "@mui/material"
import { CurrentUserNfo } from "../../types/CurrentUserNfo"

export interface IGlobalPersistData {
  /** states if the storage is hydrated */
  hydrated: boolean

  currentUser: CurrentUserNfo | null
  /** states if current user got a first verify of the authorization */
  currentUserInitialized: boolean

  themePalette: PaletteMode
    
  refreshTokenExpiration: string | null
}