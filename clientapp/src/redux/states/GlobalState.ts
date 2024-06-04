import { PaletteMode } from "@mui/material";
import { LOCAL_STORAGE_CURRENT_USER_NFO, LOCAL_STORAGE_THEME, THEME_INITIAL } from "../../constants/general";
import { CurrentUserNfo } from "../../types/CurrentUserNfo";

export interface GlobalState {
  theme: PaletteMode;
  urlWanted: string | undefined;
  generalNetwork: boolean;

  // this will serialized in the local storage for a quick page refresh
  currentUserInitialized: boolean;
  currentUser: CurrentUserNfo | undefined;

  // snacks

  snackSuccessOpen: boolean;
  snackSuccessMsg: string;
  snackSuccessDuration: number | null;

  snackErrorOpen: boolean;
  snackErrorMsg: string;
  snackErrorDuration: number | null;

  snackWarningOpen: boolean;
  snackWarningMsg: string;
  snackWarningDuration: number | null;

  snackInfoOpen: boolean;
  snackInfoMsg: string;
  snackInfoDuration: number | null;
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

export const GlobalInitialState: GlobalState = {
  theme: currentTheme,
  urlWanted: undefined,
  generalNetwork: false,

  currentUserInitialized: false,
  currentUser: initialCurrentUser,

  // snacks

  snackSuccessOpen: false,
  snackSuccessMsg: "",
  snackSuccessDuration: 6000,

  snackErrorOpen: false,
  snackErrorMsg: "",
  snackErrorDuration: 6000,

  snackWarningOpen: false,
  snackWarningMsg: "",
  snackWarningDuration: 6000,

  snackInfoOpen: false,
  snackInfoMsg: "",
  snackInfoDuration: 6000,

};
