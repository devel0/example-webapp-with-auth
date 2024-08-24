import { PaletteMode } from "@mui/material";
import { LOCAL_STORAGE_CURRENT_USER_NFO, LOCAL_STORAGE_THEME, THEME_INITIAL } from "../../constants/general";
import { CurrentUserNfo } from "../../types/CurrentUserNfo";
import { SnackNfo } from "../../types/SnackNfo";

export interface GlobalState {
  theme: PaletteMode;
  urlWanted: string | undefined;
  generalNetwork: boolean;

  // this will serialized in the local storage for a quick page refresh
  currentUserInitialized: boolean;
  currentUserCanManageUsers: boolean;
  currentUser: CurrentUserNfo | undefined;

  // snacks

  snackSuccess: SnackNfo;
  snackError: SnackNfo;
  snackWarning: SnackNfo;
  snackInfo: SnackNfo;
}

// retrieve authenticated user info from local storage
let initialCurrentUser: CurrentUserNfo | undefined = undefined;
{
  const qLocalStorage = localStorage.getItem(LOCAL_STORAGE_CURRENT_USER_NFO);
  if (qLocalStorage) initialCurrentUser = JSON.parse(qLocalStorage);
}

// retrieve theme from local storage
let currentTheme = THEME_INITIAL;
{
  const themeSaved = localStorage.getItem(LOCAL_STORAGE_THEME);
  if (themeSaved as PaletteMode) currentTheme = themeSaved as PaletteMode;
}

const defaultSnackNfo = () => {
  return {
    open: false,
    title: "",
    msg: [""],
    durationMs: 6000
  } as SnackNfo
}

export const GlobalInitialState: GlobalState = {
  theme: currentTheme,
  urlWanted: undefined,
  generalNetwork: false,

  currentUserInitialized: false,
  currentUserCanManageUsers: false,
  currentUser: initialCurrentUser,

  // snacks

  snackSuccess: defaultSnackNfo(),
  snackError: defaultSnackNfo(),
  snackWarning: defaultSnackNfo(),
  snackInfo: defaultSnackNfo()

};
