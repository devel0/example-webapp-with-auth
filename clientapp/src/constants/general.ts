//------------------------------------------------------------------
// API
//------------------------------------------------------------------

export const API_URL = () => `https://${import.meta.env.VITE_SERVERNAME}`

//------------------------------------------------------------------
// APP
//------------------------------------------------------------------

export const APP_TITLE = "WebAppTest";
export const APP_LOGO_TEXT: string | undefined = "WEBAPP";

import { PaletteMode } from "@mui/material";

export const APP_URL_BASE = "/app";

export const APP_URL_Home = `${APP_URL_BASE}`
export const APP_URL_Login = `${APP_URL_BASE}/login`

export const LOCAL_STORAGE_CURRENT_USER_NFO = "currentUserNfo";
export const LOCAL_STORAGE_THEME = "theme";

//------------------------------------------------------------------
// GUI
//------------------------------------------------------------------

export const CSS_VAR_TOOLBAR_TEXT_COLOR = "--toolbar-text-color";
export const CSS_VAR_APP_BUTTON_FG = "--app-button-fg";
export const CSS_VAR_APP_BUTTON_BG = "--app-button-bg";
export const CSS_VAR_APP_CONTROL_BG = "--app-control-bg";
export const light_bg_paper = "#c0c0c0";

export const THEME_DARK = "dark";
export const THEME_LIGHT = "light";

export const THEME_INITIAL: PaletteMode = THEME_DARK;

export const DEFAULT_SIZE_XSMALL = '0.5rem'
export const DEFAULT_SIZE_SMALL = '1rem'
export const DEFAULT_SIZE_LARGE = '2rem'
export const DEFAULT_SIZE_XLARGE = '4rem'